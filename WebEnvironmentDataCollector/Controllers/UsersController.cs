using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public UsersController(UserManager<AppUser> userManager, MongoHandler mongo)
        {
            this.userManager = userManager;
            this.mongo = mongo;
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

            if (user == null) return Ok();

            DateTime? da = null, a = null;
            CultureInfo cu = CultureInfo.GetCultureInfo("it-IT");

            if (DateTime.TryParseExact(from, "dd/MM/yyyy HH:mm:ss", cu, DateTimeStyles.None, out DateTime tmp))
                da = new DateTime(tmp.Ticks);
            if (DateTime.TryParseExact(to, "dd/MM/yyyy HH:mm:ss", cu, DateTimeStyles.None, out tmp))
                a = new DateTime(tmp.Ticks);

            return Content(
                mongo.GetUsrLog(da, a, user.UserName),
                "application/json",
                System.Text.Encoding.UTF8
            );
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

            return Ok(userManager.UpdateAsync(usr).Result.Succeeded);
        }

        [HttpPost("Unlock")]
        public IActionResult Post([FromBody] JObject id)
        {
            string usrId = string.Empty;

            if (id.ContainsKey("id"))
                usrId = id.GetValue("id").ToString();
            else
                return Ok(false);

            AppUser usr = userManager.FindByIdAsync(usrId).Result;

            if (usr == null) return Ok(false);

            return Ok(userManager.SetLockoutEndDateAsync(usr, DateTimeOffset.Now.AddDays(-1)).Result.Succeeded);
        }
    }
}
