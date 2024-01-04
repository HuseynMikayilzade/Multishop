using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.Manage.ViewModels
{
    public class CreateCategoryVm
    {
        [Required]
        public string Name { get; set; } = null!;
        public IFormFile? Photo { get; set; }

    }
}
