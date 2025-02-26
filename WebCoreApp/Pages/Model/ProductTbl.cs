﻿namespace WebCoreApp.Pages.Model
{
    public class ProductTbl
    {
        public int Sno { get; set; }
        public required string ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string Description { get; set; }
        public required double Price { get; set; }
    }
}
