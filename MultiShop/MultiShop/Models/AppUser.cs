using Microsoft.AspNetCore.Identity;
using MultiShop.Utilities.Enums;

namespace MultiShop.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public List<WishListItem>? WishListItems { get; set; }
        public List<Order> Orders { get; set; }
    }
}
