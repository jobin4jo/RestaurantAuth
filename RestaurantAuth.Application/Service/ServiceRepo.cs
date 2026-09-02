using RestaurantAuth.Infrastructure;
using RestaurantAuth.Infrastructure.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAuth.Application.Service
{
    public class ServiceRepo:IServiceRepo
    {
        private readonly RestaurantDbContext _context;

        public ServiceRepo(RestaurantDbContext context)
        {
            this._context = context;
        }

        public async Task<bool> CheckUser(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user != null ? true : false;
        }
    }
}
