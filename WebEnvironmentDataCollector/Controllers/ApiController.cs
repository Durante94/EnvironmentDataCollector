using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebEnvironmentDataCollector.Models;
using WebEnvironmentDataCollector.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebEnvironmentDataCollector.Controllers
{
    [Route("[controller]"), ApiController/*, Authorize*/]
    public class ApiController : ControllerBase
    {
        private readonly MongoHandler mongo;
        private readonly FileHandler fileHandler;

        public ApiController(MongoHandler mongo, FileHandler fileHandler)
        {
            //aggiungere interrogazione al db sql per avere i dati di connessione al mongo relativi all'utente
            this.mongo = mongo;
            this.mongo.Init("", "", "", "");
            this.fileHandler = fileHandler;
        }

        [HttpGet]
        public IActionResult Get(string jsonFilters)
        {
            return Ok(mongo.GetData(Newtonsoft.Json.JsonConvert.DeserializeObject<Filters>(jsonFilters).GetFiletrs()));
        }

        [HttpPost]
        public IActionResult Post()
        {
            if (Request.Form.Files.Count <= 0) return Ok(false);

            if (fileHandler.SetFile(Request.Form.Files[0], mongo))
                return Ok(true);

            return Ok();
        }
    }
}
