using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAuth.Domain.DTO.User
{
    public class LogoutRequestDTO
    {
        public string RefreshToken { get; set; } = string.Empty;
        public bool RevokeAllSessions { get; set; } = false;
    }
}
