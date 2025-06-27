using AutoMapper;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.Models;

namespace MultiShop.Areas.Manage.MappingProfile
{
    public class SpecialProductProfile : Profile
    {
        public SpecialProductProfile()
        {
            CreateMap<CreateSpecialProductVm, SpecialProduct>();
            CreateMap<UpdateSpecialProductVm, SpecialProduct>().ReverseMap();
        }
    }
}
