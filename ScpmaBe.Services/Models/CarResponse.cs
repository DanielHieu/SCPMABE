﻿namespace ScpmaBe.Services.Models
{
    public class CarResponse
    {
        public int CarId { get; set; }
        public string Thumbnail { get; set; }
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
    }
}
