using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using RestaurantAuth.Domain.DTO.User;

namespace RestaurantAuth.Domain.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, RegisterationResponseDTO>();
        }
    }
}
