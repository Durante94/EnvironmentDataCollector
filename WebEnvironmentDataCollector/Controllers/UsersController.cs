using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebEnvironmentDataCollector.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebEnvironmentDataCollector.Controllers
{
    [Route("[controller]"), ApiController, Authorize(Roles = "WebAdmin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        public UsersController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(userManager.Users.Select(x => new { x.Id, x.UserName, x.Email, x.Active, x.Motivo, locked = userManager.IsLockedOutAsync(x).Result }));
        }

        // GET api/<UsersController>/5
        //[HttpGet("{user}")]
        //public IActionResult Get(string user)
        //{
        //    return "value";
        //}

        // POST api/<UsersController>
        [HttpPost]
        public IActionResult Post([FromBody] JObject data)
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

        // PUT api/<UsersController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE api/<UsersController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
