using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAuth.Domain.DTO.User
{
    public  class LoginRequestDTO
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
