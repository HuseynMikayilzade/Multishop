using System.ComponentModel.DataAnnotations.Schema;

namespace MultiShop.Areas.Manage.ViewModels
{
    public class CreateSlideVm
    {
        public string Tittle { get; set; }
        public string SubTittle { get; set; }
        public string? Button { get; set; }
        public int Order { get; set; }
    
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
