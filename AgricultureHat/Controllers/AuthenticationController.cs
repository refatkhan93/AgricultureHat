using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AgricultureHat.Context;
using AgricultureHat.Models;

namespace AgricultureHat.Controllers
{
    
    public class AuthenticationController : Controller
    {
        private AgricultureContext db = new AgricultureContext();
        
        //
        // GET: /Authentication/
        public ActionResult Login(string id)
        {
            ViewBag.Login = "active";
            if (Session["FarmerId"] != null)
            {
                return RedirectToAction("Index", "Primary");
            }
            ViewBag.Error = id;
            return View();
        }
       
        public ActionResult LoginUser(string username,string password)
        {
            Farmer hs = db.Farmers.FirstOrDefault(r => r.Email == username && r.Password == password);
            if (hs != null)
            {
                Session["FarmerId"] = hs.Id;
                Session["FarmerName"] = hs.Name;
                return RedirectToAction("Index", "Primary");
            }
            else
                return RedirectToAction("Login", "Authentication", new {id="Error"});
            
        }

        public ActionResult Register()
        {
            ViewBag.Register = "active";
            if (Session["FarmerId"] != null)
            {
                return RedirectToAction("Index", "Primary");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Register(Farmer farmer, HttpPostedFileBase Image)
        {
            ViewBag.Register = "active";
            if (Image != null && Image.ContentLength > 0)
            {
                try
                {
                    string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Image.FileName);
                    string uploadUrl = Server.MapPath("~/Photos");
                    Image.SaveAs(Path.Combine(uploadUrl, fileName));
                    farmer.Image = "Photos/" + fileName;
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "ERROR:" + ex.Message.ToString();
                }
            }
            int k=db.Farmers.Count(r => r.Email == farmer.Email);
            if (k == 0)
            {
                db.Farmers.Add(farmer);
                db.SaveChanges();
            }
            else
            {
                ViewBag.Error = "Another Farmer is Registered with this Email";
                return View();
            }
            
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            Session["FarmerId"] = null;
            return RedirectToAction("Login");
        }

        public ActionResult LoginAdmin(string id)
        {
            ViewBag.LoginAdmin = "active";
            if (Session["AdminId"] != null)
            {
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Error = id;
            return View();
        }
        [HttpPost]
        public ActionResult LoginAdmin(string username, string password)
        {
            Admin hs = db.Admins.FirstOrDefault(r => r.Email == username && r.Password == password);
            if (hs != null)
            {
                Session["AdminId"] = hs.Id;
               
                return RedirectToAction("Index", "Admin");
            }
            else
                return RedirectToAction("LoginAdmin", "Authentication", new { id = "Error" });
        }

        public ActionResult LogoutAdmin()
        {
           
            Session["AdminId"] = null;
            return RedirectToAction("LoginAdmin");
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