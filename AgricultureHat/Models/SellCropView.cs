using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgricultureHat.Models
{
    public class SellCropView
    {
        public int Id { get; set; }
        public string Crop { get; set; }
        public string Category { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public int SellCropAmount { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }

        public Farmer farmer = new Farmer();
    }
}