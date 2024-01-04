using AutoMapper;
using MultiShop.Areas.Manage.ViewModels;
using MultiShop.Models;

namespace MultiShop.Areas.Manage.MappingProfile
{
    public class SlideProfile:Profile
    {
        public SlideProfile()
        {
            CreateMap<CreateSlideVm, Slide>();
            CreateMap<UpdateSlideVm, Slide>().ReverseMap();
        }
    }
}
