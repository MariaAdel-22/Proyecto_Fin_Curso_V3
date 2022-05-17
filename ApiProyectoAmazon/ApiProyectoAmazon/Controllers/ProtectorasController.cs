using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ApiProyectoAmazon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiProyectoAmazon.Helpers;
using ApiProyectoAmazon.Repositories;

namespace ApiProyectoAmazon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtectorasController : ControllerBase
    {
        private RepositoryProtectoras repoProtectoras;

        public ProtectorasController(RepositoryProtectoras repositoryProtectoras)
        {

            this.repoProtectoras = repositoryProtectoras;
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{ciudad}")]
        public ActionResult<List<Protectora>> GetProtectoras(string ciudad)
        {
            return this.repoProtectoras.GetProtectoras(ciudad);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{id}")]
        public ActionResult<Protectora> FindProtectora(string id)
        {

            return this.repoProtectoras.FindProtectora(id);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{ciudad}")]
        public ActionResult<List<int>> GetNumeroProtectoras(string ciudad)
        {
            return this.repoProtectoras.GetNumeroProtectoras(ciudad);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{codigoProtectora}")]
        public ActionResult<List<Animal>> GetAnimalesProtectora(int codigoProtectora)
        {

            return this.repoProtectoras.GetAnimalesProtectora(codigoProtectora);
        }

        [HttpPost]
        [Authorize]
        [Route("[action]")]
        public IActionResult InsertarAnimal(Animal animal)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();

            string json = claims.SingleOrDefault(z => z.Type == "UserData").Value;

            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(json);

            this.repoProtectoras.InsertarAnimal(cuenta.CodigoCuenta,animal.Nombre, animal.Edad, animal.Genero, animal.Especie, animal.Peso, animal.Imagen);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{codigoAnimal}")]
        public ActionResult<Animal> FindAnimal(int codigoAnimal)
        {

            return this.repoProtectoras.BuscarAnimal(codigoAnimal);
        }

        [HttpPut]
        [Authorize]
        [Route("[action]")]
        public ActionResult ModificarAnimal(Animal animal)
        {

            this.repoProtectoras.ModificarAnimal(animal.CodigoAnimal, animal.Nombre, animal.Edad, animal.Genero, animal.Especie, animal.Peso, animal.Imagen);

            return Ok();
        }

        [HttpDelete]
        [Authorize]
        [Route("[action]/{codigoAnimal}")]
        public ActionResult EliminarAnimal(int codigoAnimal)
        {

            this.repoProtectoras.EliminarAnimal(codigoAnimal);

            return Ok();
        }
    }
}
