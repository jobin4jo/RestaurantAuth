using RestaurantAuth.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAuth.Infrastructure;

public interface IUserRepository
{
   Task<List<User>>GetAllUsers();
    Task<User> RegisterUser(RegisterationRequestDTO request);
}
