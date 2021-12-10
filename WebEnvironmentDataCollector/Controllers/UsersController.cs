using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebEnvironmentDataCollector.Models;
using WebEnvironmentDataCollector.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebEnvironmentDataCollector.Controllers
{
    [Route("[controller]"), ApiController, Authorize(Roles = "WebAdmin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly MongoHandler mongo;
        private readonly IEmailService emailSender;

        public UsersController(UserManager<AppUser> userManager, MongoHandler mongo, IEmailService emailService)
        {
            this.userManager = userManager;
            this.mongo = mongo;
            emailSender = emailService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(userManager.Users.Select(x => new { x.Id, x.UserName, x.Email, x.Active, x.Motivo, locked = userManager.IsLockedOutAsync(x).Result }));
        }

        [HttpGet("Log")]
        public IActionResult Log(string userId, string from, string to)
        {
            AppUser user = userManager.FindByIdAsync(userId).Result;

            if (user == null) return Ok(null);

            DateTime? da = null, a = null;
            CultureInfo cu = CultureInfo.GetCultureInfo("it-IT");

            if (DateTime.TryParseExact(from, new string[] { "yyyy-MM-dd HH:mm", "yyyy-MM-dd" }, cu, DateTimeStyles.None, out DateTime tmp))
                da = new DateTime(tmp.Ticks);
            if (DateTime.TryParseExact(to, new string[] { "yyyy-MM-dd HH:mm", "yyyy-MM-dd" }, cu, DateTimeStyles.None, out tmp))
                a = new DateTime(tmp.Ticks);

            return Ok(mongo.GetUsrLog(da, a, user.UserName));
        }

        [HttpPost("Activate")]
        public IActionResult Activate([FromBody] JObject data)
        {
            string motivo = string.Empty;

            if (!data.ContainsKey("id")) return Ok(false);
            if (data.ContainsKey("motivo"))
                motivo = data.GetValue("motivo").ToString();

            AppUser usr = userManager.FindByIdAsync(data.GetValue("id").ToString()).Result;

            if (usr == null) return Ok(false);

            usr.Active = !usr.Active;
            usr.Motivo = motivo;

            mongo.LogOperation($"{(usr.Active ? "Attivazione" : "Disattivazione")} utente", Newtonsoft.Json.JsonConvert.SerializeObject(usr), User.Identity.Name);

            return Ok(userManager.UpdateAsync(usr).Result.Succeeded);
        }

        [HttpPost("Unlock")]
        public IActionResult Unlock([FromBody] JObject id)
        {
            string usrId;
            if (id.ContainsKey("id"))
                usrId = id.GetValue("id").ToString();
            else
                return Ok(false);

            AppUser usr = userManager.FindByIdAsync(usrId).Result;

            if (usr == null) return Ok(false);

            mongo.LogOperation("Sbloccato utente", Newtonsoft.Json.JsonConvert.SerializeObject(usr), User.Identity.Name);

            return Ok(userManager.SetLockoutEndDateAsync(usr, DateTimeOffset.Now.AddDays(-1)).Result.Succeeded);
        }

        [HttpPost("Reset")]
        public IActionResult Reset([FromBody] JObject id)
        {
            string usrId;
            if (id.ContainsKey("id"))
                usrId = id.GetValue("id").ToString();
            else
                return Ok(false);

            AppUser usr = userManager.FindByIdAsync(usrId).Result;

            if (usr == null) return Ok(false);

            string code = userManager.GeneratePasswordResetTokenAsync(usr).Result;
            code = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(code));
            string callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);

            emailSender.Send(
                usr.Email,
                "Reset Password",
                $"Please reset your password by <a href='{System.Text.Encodings.Web.HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            mongo.LogOperation("Reset password utente", Newtonsoft.Json.JsonConvert.SerializeObject(usr), User.Identity.Name);

            return Ok(true);
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] JObject id)
        {
            string usrId;
            if (id.ContainsKey("id"))
                usrId = id.GetValue("id").ToString();
            else
                return Ok(false);

            AppUser usr = userManager.FindByIdAsync(usrId).Result;

            if (usr == null) return Ok(false);

            if (userManager.GetUsersInRoleAsync("WebAdmin").Result.Contains(usr))
                return Ok(false);

            mongo.LogOperation("Cancellato utente", Newtonsoft.Json.JsonConvert.SerializeObject(usr), User.Identity.Name);

            return Ok(userManager.DeleteAsync(usr).Result.Succeeded);
        }
    }
}
