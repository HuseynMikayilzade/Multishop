using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.Manage.ViewModels
{
    public class UpdateCategoryVm
    {
        [Required]
        public string Name { get; set; } = null!;
        public IFormFile? Photo { get; set; }
       
    }
}
