using RestaurantAuth.Domain;
using RestaurantAuth.Domain.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAuth.Infrastructure;

public interface IUserRepository
{
   Task<List<User>>GetAllUsers();
    Task<RegisterationResponseDTO> RegisterUser(RegisterationRequestDTO request);
    Task<LoginResponseDTO> LoginUser(LoginRequestDTO request);
}
