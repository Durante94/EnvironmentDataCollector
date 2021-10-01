using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        public ApiController(MongoHandler mongo)
        {
            //aggiungere interrogazione al db sql per avere i dati di connessione al mongo relativi all'utente
            this.mongo = mongo;
            this.mongo.Init("", "", "", "");
        }

        [HttpGet]
        public IActionResult Get([FromQuery] Filters filters)
        {
            return Ok(mongo.GetData(filters.GetFiletrs()));
        }

        [HttpPost]
        public void Post()
        {
        }
    }
}
