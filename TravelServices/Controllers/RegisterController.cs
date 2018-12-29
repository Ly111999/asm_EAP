using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using TravelServices.Models;
using TravelServices.Utiliti;

namespace TravelServices.Controllers
{
    [RoutePrefix("api/v1/traveller")]
    public class RegisterController : ApiController
    {
        private TravellerContext db = new TravellerContext();
        private readonly string BASE_URL = "http://localhost:65113/";
        private readonly string EMAIL_SEND = "tuanh031101@gmail.com";
        private readonly string PASSWORD_SEND = "huonglylieu1999@";

        [HttpPost]
        [Route("register")]
        public HttpResponseMessage Register(Traveler traveler)
        {
            if (ModelState.IsValid)
            {
                var v = isExist(traveler.email);
                if (v)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Email is exist");
                }

                var s = SaltGenerate.saltStr(10);
                traveler.salt = s;
                traveler.password = HashMd5.CreateMD5(traveler.password, s);

                traveler.ActivationCode = Guid.NewGuid();
                traveler.IsEmailVerified = false;
                traveler.Role_id = 1;
                traveler.createdAt = DateTime.Now;
                traveler.updatedAt = DateTime.Now;
                db.Travelers.Add(traveler);
                db.SaveChanges();
                SendVerificationLinkEmail(traveler.email, traveler.ActivationCode.ToString());
                return Request.CreateResponse(HttpStatusCode.OK, traveler);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [NonAction]
        public bool isExist(string email)
        {
            var mail = db.Travelers.FirstOrDefault(a => a.email == email);
            return mail != null;
        }

        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode)
        {
            var link = BASE_URL + "api/v1/traveller/verifyAccount?id=" + activationCode;

            var fromEmail = new MailAddress("daokhanhblog942@gmail.com", "Activation Username");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "cqzwqzzlcayokple";
            string subject = "Your account is successfully created!";

            string body = "<br/><br/>We are excited to tell you that your account is" +
                          " successfully created. Please click on the below link to verify your account" +
                          " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })

                smtp.Send(message);
        }

        [Route("verifyAccount")]
        [HttpGet]
        public HttpResponseMessage VerifyAccount(String id)
        {
            using (db)
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var v = db.Travelers.FirstOrDefault(a => a.ActivationCode == new Guid(id));
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    db.SaveChanges();
                    return Request.CreateErrorResponse(HttpStatusCode.OK, "Active success");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid request");
                }
            }
        }
    }
}
