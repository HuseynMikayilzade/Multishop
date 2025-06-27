namespace MultiShop.Areas.Manage.ViewModels
{
    public class UpdateCouponVm
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Ümumi istifadə sayı (sizin təyin etdiyiniz)
        public int TotalUsageLimit { get; set; }

        // Kupon neçə dəfə istifadə olunub
        public int CurrentUsageCount { get; set; }
        public int CountUsePerUser { get; set; }
        public bool isActive { get; set; }
    }
}
