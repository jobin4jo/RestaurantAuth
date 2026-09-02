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

namespace RestaurantAuth.Application.Repositories
{
    public class UserRepository : IUserRepository

    {
        private readonly RestaurantDbContext _context;
        private readonly IServiceRepo _service;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        public UserRepository(RestaurantDbContext context, IServiceRepo service, IPasswordHasher passwordHasher, IMapper mapper)
        {
            this._context = context;
            this._service = service;
            this._passwordHasher = passwordHasher;
            this._mapper = mapper;
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
        public async  Task<LoginResponseDTO> LoginUser(LoginRequestDTO request)
        {
            var user = await _service.GetUserDetailByEmail(request.Email);
            if (user == null)
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            if (!user.IsActive)
                throw new UnauthorizedAccessException(
                    "User account is inactive.");
            var passwordValid = _passwordHasher.Verify(request.Password,user.PasswordHash);

            if (!passwordValid)
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            return null;
        }
    }
}
