using AutoMapper;
using SalePortal.Entities;
using WebApiForSalePortal.Models;

namespace WebApiForSalePortal.Services
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserEntity, UserOutPutModel>();
            CreateMap<AdminEntity, UserOutPutModel>();
        }
    }
}
