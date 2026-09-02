using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAuth.Infrastructure.IService
{
    public interface IServiceRepo
    {
        Task<bool>CheckUser(string email);
    }
}
