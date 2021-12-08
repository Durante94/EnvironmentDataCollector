using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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
    [Route("[controller]"), ApiController, Authorize]
    public class ApiController : ControllerBase
    {
        private readonly MongoHandler mongo;
        private readonly FileHandler fileHandler;
        private readonly string fileFolderRoot;

        public ApiController(MongoHandler mongo, FileHandler fileHandler, Microsoft.Extensions.Configuration.IConfiguration config)
        {

            //aggiungere interrogazione al db sql per avere i dati di connessione al mongo relativi all'utente
            this.mongo = mongo;
            this.fileHandler = fileHandler;
            fileFolderRoot = config["UploadPath"];

            //if (!Directory.Exists(fileFolderRoot))
            //    Directory.CreateDirectory(fileFolderRoot);
        }

        [HttpGet]
        public IActionResult Get(string jsonFilters)
        {
            return Ok(mongo.GetData(Newtonsoft.Json.JsonConvert.DeserializeObject<Filters>(jsonFilters).GetFiletrs(), User.Identity.Name));
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
            string fileFolder = Path.Combine(fileFolderRoot, User.Identity.Name);

            if (!Directory.Exists(fileFolder))
                Directory.CreateDirectory(fileFolder);

            foreach (IFormFile file in Request.Form.Files)
            {
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmssff") + ".csv";
                if (fileHandler.SetFile(file, mongo, User.Identity.Name))
                    fileErrors.Add(file.FileName);
                else
                {
                    string filePath = Path.Combine(fileFolder, fileName);
                    file.CopyToAsync(new FileStream(filePath, FileMode.CreateNew, FileAccess.Write));
                    mongo.LogOperation($"Salvataggio file {fileName} riuscito", filePath, User.Identity.Name);
                }
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
            mongo.LogOperation("Salvataggio Batch JSON", fromJson.ToJson(mongo.JWS), User.Identity.Name);
            fromJson.ForEach(x => mongo.SaveData(x.ConvertForDB(), User.Identity.Name));

            return Ok(true);
        }
    }
}
