using System.ComponentModel.DataAnnotations.Schema;

namespace MultiShop.Models
{
    public class Slide
    {
        public int Id { get; set; }
        public string Tittle { get; set; } 
        public string SubTittle { get; set; }
        public string? Button { get; set; }
        public int Order { get; set; }
        public string? İmage { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }
    }
}
