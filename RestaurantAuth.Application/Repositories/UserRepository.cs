using RestaurantAuth.Domain;
using RestaurantAuth.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantAuth.Infrastructure.IService;
using System.Runtime.InteropServices;
using RestaurantAuth.Domain.Common.Password;
using AutoMapper;
using RestaurantAuth.Domain.DTO.User;
using RestaurantAuth.Domain.JWT;
using System.Security.Cryptography;

namespace RestaurantAuth.Application.Repositories
{
    public class UserRepository : IUserRepository

    {
        private readonly RestaurantDbContext _context;
        private readonly IServiceRepo _service;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public UserRepository(
            RestaurantDbContext context,
            IServiceRepo service,
            IPasswordHasher passwordHasher,
            IMapper mapper,
            ITokenService tokenService)
        {
            this._context = context;
            this._service = service;
            this._passwordHasher = passwordHasher;
            this._mapper = mapper;
            this._tokenService = tokenService;
        }
        public async Task<List<User>> GetAllUsers()
        {
            try
            {
                var response = await _context.Users.ToListAsync();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving users: {ex.Message}");
            }
        }

        public async Task<RegisterationResponseDTO> RegisterUser(RegisterationRequestDTO request)
        {
            try
            {
                if(string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    throw new ArgumentException("Name, Email, and Password are required.");
                }

                var usercheck = await _service.CheckUser(request.Email);
                if(usercheck)
                {
                    throw new Exception("User with this email already exists.");
                }
                var passhash = _passwordHasher.Hash(request.Password);  
                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Role = "User",
                    PasswordHash = passhash,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true



                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return _mapper.Map<RegisterationResponseDTO>(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error registering user: {ex.Message}");
            }
        }
        public async Task<LoginResponseDTO> LoginUser(LoginRequestDTO request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Email and Password are required.");
            }

            var user = await _service.GetUserDetailByEmail(request.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("User account is inactive.");
            }

            var passwordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!passwordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new LoginResponseDTO
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<LoginResponseDTO> RefreshToken(RefreshTokenRequestDTO request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new ArgumentException("Refresh token is required.");
            }

            var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken)));

            var existingToken = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.TokenHash == tokenHash);

            if (existingToken == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            if (existingToken.RevokedAt != null)
            {
                throw new UnauthorizedAccessException("Refresh token has already been revoked.");
            }

            if (existingToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token has expired.");
            }

            var user = existingToken.User;
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("User is inactive or not found.");
            }

            // Revoke the old refresh token (Token Rotation)
            existingToken.RevokedAt = DateTime.UtcNow;

            // Generate new access and refresh token pair
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newTokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(newRefreshToken)));

            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(newRefreshTokenEntity);
            await _context.SaveChangesAsync();

            return new LoginResponseDTO
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> LogoutUser(LogoutRequestDTO request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new ArgumentException("Refresh token is required.");
            }

            var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken)));

            var existingToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.TokenHash == tokenHash);

            if (existingToken == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            if (existingToken.RevokedAt != null)
            {
                throw new UnauthorizedAccessException("Refresh token has already been revoked.");
            }

            // Revoke the specific refresh token
            existingToken.RevokedAt = DateTime.UtcNow;

            if (request.RevokeAllSessions)
            {
                var activeTokens = await _context.RefreshTokens
                    .Where(r => r.UserId == existingToken.UserId && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow)
                    .ToListAsync();

                foreach (var token in activeTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

