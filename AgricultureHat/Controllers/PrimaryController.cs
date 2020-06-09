using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using AgricultureHat.Context;
using AgricultureHat.Models;

namespace AgricultureHat.Controllers
{
    [AuthorizationFilter]
    public class PrimaryController : Controller
    {
        private AgricultureContext db = new AgricultureContext();
        //
        // GET: /Primary/
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Index = "active";
            DateTime dt = DateTime.Now;
            int month =dt.Month;
            List<Crop> crops= new List<Crop>();
            crops = db.Crops.Where(r => r.SessionStart >= month && r.SessionStart <= month + 3).ToList();
            ViewBag.Crop = crops;
            return View();
        }
       
        public ActionResult SellCrops()
        {
            ViewBag.SellCrops = "active";
            ViewBag.Category = db.Categories.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult SellCrops(SellCrop crop)
        {
            ViewBag.SellCrops = "active";
            crop.FarmerId = (int)Session["FarmerId"];
            int t = db.SellCrops.Count(r => r.FarmerId == crop.FarmerId && r.CategoryId == crop.CategoryId && r.CropId == crop.CropId);
            if (t != 0)
            {
                SellCrop s = db.SellCrops.First(r => r.FarmerId == crop.FarmerId && r.CategoryId == crop.CategoryId && r.CropId == crop.CropId);
                s.Price = crop.Price;
                s.Quantity += crop.Quantity;
                db.SaveChanges();
                ViewBag.Error = "Product List Updated Successfully";
            }
            else
            {
                db.SellCrops.Add(crop);
                db.SaveChanges();
                ViewBag.Error = "Product Listed for Sell Successfully";
            }
           
            ViewBag.Category = db.Categories.ToList();
            
            return View();
        }
        [AllowAnonymous]
        public JsonResult GetCropsById(int id)
        {
            return Json(db.Crops.Where(r => r.CategoryId == id).ToList());
        }
        
        public ActionResult Profile(string msg)
        {
            ViewBag.Profile = "active";
            ViewBag.Error = msg;
            int fid = (int) Session["FarmerId"];
            var data = from s in db.SellCrops
                join cat in db.Categories on s.CategoryId equals cat.Id
                join cr in db.Crops on s.CropId equals cr.Id
                where s.FarmerId == fid
                select new
                {
                    id=s.Id,
                    q=s.Quantity,
                    p=s.Price,
                    sold = s.SoldCropAmount,
                    catname=cat.Name,
                    crname=cr.Name,
                };
            List<SellCropView> sellCropViews = new List<SellCropView>();
            foreach (var d in data)
            {
                SellCropView s =new SellCropView();
                s.Id = d.id;
                s.Quantity = d.q;
                s.Price = d.p;
                s.SellCropAmount = d.sold;
                s.Category = d.catname;
                s.Crop = d.crname;
                sellCropViews.Add(s);
            }

            ViewBag.Crops = sellCropViews;
           ViewBag.Farmer = db.Farmers.First(r => r.Id == fid);
            return View();
        }
        public ActionResult Delete(int id)
        {
            SellCrop s = db.SellCrops.Find(id);
            db.SellCrops.Remove(s);
            db.SaveChanges();
            return RedirectToAction("Profile");
        }

        public ActionResult EditProfile(Farmer f, HttpPostedFileBase Image, string pastImage)
        {
            string message = "";
            f.Image = pastImage;
            if (Image != null && Image.ContentLength > 0)
            {
                string fullPath = Request.MapPath("~/" + pastImage);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                try
                {
                    string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Image.FileName);
                    string uploadUrl = Server.MapPath("~/Photos/Doctor");
                    Image.SaveAs(Path.Combine(uploadUrl, fileName));
                    f.Image = "Photos/Doctor/" + fileName;
                }
                catch (Exception ex)
                {
                    message = "ERROR:" + ex.Message.ToString();
                }
            }
            db.Entry(f).State = EntityState.Modified;
            Farmer d=db.Farmers.First(r=>r.Id == f.Id);
            if ( d!=null && d.Password.Equals(f.Password))
            {
                
                db.SaveChanges();
                message = "Profile Changed Successfully";
            }
            else
            {
                message = "Password Doesn't Match";
            }

            return RedirectToAction("Profile", "Primary", new { msg = message });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
	}
}