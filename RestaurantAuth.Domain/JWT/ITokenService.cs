using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAuth.Domain.JWT
{
    public  interface ITokenService
    {
        string GenerateAccessToken(User user);

        string GenerateRefreshToken();
    }
}
