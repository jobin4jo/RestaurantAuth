using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAuth.Domain.DTO.User
{
    public class LoginResponseDTO
    {
        public string Name { get; set; }

        public string Email { get; set; }
        public string Role { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
    }
}
