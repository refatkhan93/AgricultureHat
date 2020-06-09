using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgricultureHat.Models
{
    public class Farmer
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Village { get; set; }
        [DisplayName("Ward No")]
        public string WardNo { get; set; }
        public string District { get; set; }
        public double  Age { get; set; }
        public string Image { get; set; }
        [DisplayName("NID No")]
        public string Nid { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}