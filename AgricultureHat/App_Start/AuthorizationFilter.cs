using System.Web;
using System.Web.Mvc;

namespace AgricultureHat
{
    public class AuthorizationFilter : AuthorizeAttribute, IAuthorizationFilter
    {

        public string Role { get; set; }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
            || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                // Don't check for authorization as AllowAnonymous filter is applied to the action or controller
                return;
            }
            if (Role !=null && Role.Equals("Admin"))
            {
                if (HttpContext.Current.Session["AdminId"] == null)
                {
                    filterContext.Result = new RedirectResult("~/Authentication/LoginAdmin");   
                }
                /*else
                {
                    filterContext.Result = new RedirectResult("~/Admin/Index");

                }
                */
            }
            // Check for authorization  
            else if (HttpContext.Current.Session["FarmerId"] == null)
            {
                filterContext.Result = new RedirectResult("~/Authentication/Login");
                /*filterContext.Result = new HttpUnauthorizedResult();*/
            }
            /*else
            {
                filterContext.Result = new RedirectResult("~/Primary/Index");
            }*/
            
        }
    } 
}