using System.ComponentModel.DataAnnotations.Schema;

namespace MultiShop.Areas.Manage.ViewModels
{
    public class UpdateSpecialProductVm
    {
        public string Tittle { get; set; }
        public string SubTittle { get; set; }
        public string? Button { get; set; }
        public int Order { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }
    }
}
