using AutoMapper;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.Models;

namespace MultiShop.Areas.Manage.MappingProfile
{
    public class ProductProfile:Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductVm , Product>();
        }
    }
}
