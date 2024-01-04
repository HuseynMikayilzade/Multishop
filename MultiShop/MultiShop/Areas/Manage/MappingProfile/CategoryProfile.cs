using AutoMapper;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.Models;

namespace MultiShop.Areas.Manage.MappingProfile
{
    public class CategoryProfile:Profile
    {
        public CategoryProfile()
        {
            CreateMap<CreateCategoryVm, Category>();
            CreateMap<UpdateCategoryVm, Category>().ReverseMap();
        }
    }
}
