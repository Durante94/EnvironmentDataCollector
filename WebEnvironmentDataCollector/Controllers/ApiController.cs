﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly string fileFolder;

        public ApiController(MongoHandler mongo, FileHandler fileHandler, IWebHostEnvironment environment)
        {
            string tmp = Path.Combine(environment.WebRootPath, "Uploaded");

            //aggiungere interrogazione al db sql per avere i dati di connessione al mongo relativi all'utente
            this.mongo = mongo;
            this.mongo.Init("", "", "", "");
            this.fileHandler = fileHandler;
            //fileFolder = Path.Combine(tmp, User.Identity.Name);

            if (!Directory.Exists(tmp))
                Directory.CreateDirectory(tmp);
            //if (!Directory.Exists(fileFolder))
            //    Directory.CreateDirectory(fileFolder);
        }

        [HttpGet]
        public IActionResult Get(string jsonFilters)
        {
            return Ok(mongo.GetData(Newtonsoft.Json.JsonConvert.DeserializeObject<Filters>(jsonFilters).GetFiletrs()));
        }

        [HttpPost]
        public IActionResult Post()
        {
            if (Request.Form.Files.Count <= 0)
                return Ok(new
                {
                    success = false,
                    message = "Nessun file inviato"
                });

            List<string> fileErrors = new List<string>();

            foreach (IFormFile file in Request.Form.Files)
            {
                if (fileHandler.SetFile(file, mongo))
                    fileErrors.Add(file.FileName);
                else
                    file.CopyToAsync(
                        new FileStream(Path.Combine(fileFolder, DateTime.Now.ToString("yyyyMMddHHmmssff") + ".csv"),
                        FileMode.CreateNew, FileAccess.Write)
                     );
            }

            return Ok(new
            {
                success = fileErrors.Count == 0,
                message = (fileErrors.Count == 0 ? "" : $"File non letti: {string.Join(", ", fileErrors)}")
            });
        }

        [HttpPost("Json")]
        public IActionResult Json([FromBody] List<DataMap> fromJson)
        {
            fromJson.ForEach(x => mongo.SaveData(x.ConvertForDB()));

            return Ok(true);
        }
    }
}
