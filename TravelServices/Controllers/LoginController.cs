using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using TravelServices.Models;
using TravelServices.Utiliti;

namespace TravelServices.Controllers
{
    [System.Web.Http.RoutePrefix("api/v1/traveller")]
    public class LoginController : ApiController
    {
        TravellerContext db = new TravellerContext();

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("login")]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage Login([FromBody] Traveler traveler)
        {
            using (db)
            {
                db.Configuration.ProxyCreationEnabled = false;
                var v = db.Travelers.FirstOrDefault(a => a.email == traveler.email);
                if (v != null)
                {
                    if (string.Compare(HashMd5.CreateMD5(traveler.password, v.salt), v.password) == 0 && v.IsEmailVerified == true)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, v);
                    }
                    else if (string.Compare(HashMd5.CreateMD5(traveler.password, v.salt), v.password) == 0 && v.IsEmailVerified == false)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            "Email does not active. Please active this email.");
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Password doesn't match");
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Email does not exist");
                }
            }
        }

    }
}
