using MultiShop.Models;

namespace MultiShop.Areas.Manage.ViewModels
{
    public class DetailVm<T> where T : class
    {
        public T? Item { get; set; }
    }
}
