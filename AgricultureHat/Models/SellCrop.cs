using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgricultureHat.Models
{
    public class SellCrop
    {
        [Key]
        public int Id { get; set; }
        public int FarmerId { get; set; }
        public int CategoryId { get; set; }
        public int CropId { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public int SoldCropAmount { get; set; }
    }
}