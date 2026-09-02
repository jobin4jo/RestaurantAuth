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

namespace RestaurantAuth.Application.Repositories
{
    public class UserRepository : IUserRepository

    {
        private readonly RestaurantDbContext _context;
        private readonly IServiceRepo _service;
        public UserRepository(RestaurantDbContext context, IServiceRepo service)
        {
            this._context = context;
            this._service = service;
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

        public async Task<User> RegisterUser(RegisterationRequestDTO request)
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

                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                   
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error registering user: {ex.Message}");
            }
        }
    }
}
