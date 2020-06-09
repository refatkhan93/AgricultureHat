using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgricultureHat.Models
{
    public class Crop
    {
        [Key]
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        [DisplayName("Session Start")]
        public int SessionStart { get; set; }
        [DisplayName("Session End")]
        public int SessionEnd { get; set; }

        public double Quentity { get; set; }
        public double Price { get; set; }
    }
}