﻿namespace MultiShop.Models
{
    public class Color
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<ProductColor>? ProductColors { get; set; }
    }
}
