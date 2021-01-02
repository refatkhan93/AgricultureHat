using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using AgricultureHat.Models;

namespace AgricultureHat.Context
{
    public class AgricultureContext:DbContext
    {
        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<SellCrop> SellCrops { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Messages> Messageses { get; set; }

    }
}