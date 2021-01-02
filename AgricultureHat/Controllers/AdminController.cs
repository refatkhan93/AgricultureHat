using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AgricultureHat.Context;
using AgricultureHat.Models;
using GoogleMaps.LocationServices;
namespace AgricultureHat.Controllers
{
    [AuthorizationFilter(Role="Admin")]
    public class AdminController : Controller
    {
        private AgricultureContext db = new AgricultureContext();
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            ViewBag.Index = "action";
            ViewBag.Category = db.Categories.ToList();
           
            return View();
        }

        public ActionResult ShowMessages()
        {
            List<Messages> msg = new List<Messages>();
            msg = db.Messageses.ToList();
            ViewBag.M = msg;
            return View();
        }
        public JsonResult GetCrops(int categoryid,int cropid)
        {
            var data = from s in db.SellCrops
                join f in db.Farmers on s.FarmerId equals f.Id
                where s.CategoryId == categoryid && s.CropId == cropid
                select new
                {
                    sid=s.Id,
                    fname=f.Name,
                    q=s.Quantity,
                    p=s.Price
                };
            List<SellCropView> sell = new List<SellCropView>();
            foreach (var d in data)
            {
                SellCropView st = new SellCropView();
                st.Id = d.sid;
                st.farmer.Name = d.fname;
                st.Quantity = d.q;
                st.Price = d.p;
                sell.Add(st);
            }
            return Json(sell);
        }

        public ActionResult BuyCrops(int id, string message)
        {
            ViewBag.Error = message;
            var data = from s in db.SellCrops
                       join f in db.Farmers on s.FarmerId equals f.Id
                       join c in db.Crops on s.CropId equals c.Id
                       where s.Id == id
                       select new
                       {
                           crop=c.Name,
                           sid = s.Id,
                           fname = f.Name,
                           phone=f.Phone,
                           nid=f.Nid,
                           vil=f.Village,
                           dis=f.District,
                           wd=f.WardNo,
                           q = s.Quantity,
                           p = s.Price
                       };
            SellCropView st = new SellCropView();
            foreach (var d in data)
            {
                
                st.Id = d.sid;
                st.farmer.Name = d.fname;
                st.farmer.Phone = d.phone;
                st.farmer.Nid = d.nid;
                st.farmer.Village = d.vil;
                st.farmer.WardNo = d.wd;
                st.farmer.District = d.dis;

                st.Quantity = d.q;
                st.Price = d.p;
                ViewBag.Name = d.crop;
            }
            ViewBag.Crop = st;
            return View();
        }

        public ActionResult BoughtCrops(int amount, int id)
        {
            SellCrop sc=db.SellCrops.First(r => r.Id == id);
            sc.SoldCropAmount = amount;
            sc.Quantity = sc.Quantity - amount;
            db.SaveChanges();
            int id2 = id;
            return RedirectToAction("BuyCrops","Admin",new{id=id2,message="Crop Bought Successfully"});
        }

        public ActionResult BuySummery()
        {
            ViewBag.BuySummery = "active";
            ViewBag.Category = db.Categories.ToList();

            return View();
        }

        public JsonResult GetSummary(int id)
        {
            var data = from s in db.SellCrops
                       join f in db.Farmers on s.FarmerId equals f.Id
                       join c in db.Crops on s.CropId equals c.Id
                       where s.CategoryId == id && s.SoldCropAmount>0
                       select new
                       {
                           crop = c.Name,
                           sid = s.Id,
                           fname = f.Name,
                           phone = f.Phone,
                           nid = f.Nid,
                           vil = f.Village,
                           dis = f.District,
                           wd = f.WardNo,
                           q = s.SoldCropAmount,
                           p = s.Price
                       };


            AddressData addresses = new AddressData();
            //, apikey: "AIzaSyDOYXX8MgKThJttmAixhqVeZgfzlIOi8uA"
            

            List<SellCropView> sv = new List<SellCropView>();
            foreach (var d in data)
            {
                SellCropView st = new SellCropView();
                
                addresses.City = d.dis;
                addresses.Country = "Bangladesh";
                try
                {
                    var gls = new GoogleLocationService("AIzaSyDOYXX8MgKThJttmAixhqVeZgfzlIOi8uA");
                    var latlong = gls.GetLatLongFromAddress(addresses);
                    st.Lat = latlong.Latitude;
                    st.Long = latlong.Longitude;
                }
                catch (System.Net.WebException ex)
                {
                    st.Lat = -1;
                    st.Long = -1;
                }
                
                st.Id = d.sid;
                st.farmer.Name = d.fname;
                st.farmer.Phone = d.phone;
                st.farmer.Nid = d.nid;
                st.farmer.Village = d.vil;
                st.farmer.WardNo = d.wd;
                st.farmer.District = d.dis;
                st.Crop = d.crop;
                st.Quantity = d.q;
                st.Price = d.p;
                sv.Add(st);
                
            }
            
            return Json(sv);
        }

        public ActionResult AddCrops(string ctmsg,string cmsg)
        {
            ViewBag.AddCrops = "active";
            ViewBag.ctm = ctmsg;
            ViewBag.cm = cmsg;
            ViewBag.Category = db.Categories.ToList();
            return View();
        }
        public ActionResult AddNewCrops(Crop c, HttpPostedFileBase Image)
        {
            string message;
            if (Image != null && Image.ContentLength > 0)
            {
                try
                {
                    string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Image.FileName);
                    string uploadUrl = Server.MapPath("~/Photos/Crop");
                    Image.SaveAs(Path.Combine(uploadUrl, fileName));
                    c.Image = "Photos/Crop/" + fileName;
                }
                catch (Exception ex)
                {
                    message = "ERROR:" + ex.Message.ToString();
                }
            }
            db.Crops.Add(c);
            db.SaveChanges();
            message = "Crops Added Successfully";
            return RedirectToAction("AddCrops", "Admin", new { cmsg = message });
        }

        public ActionResult AddCategory(Category c)
        {
            db.Categories.Add(c);
            db.SaveChanges();
            return RedirectToAction("AddCrops", "Admin", new {ctmsg = "Category Added Successfully"});
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