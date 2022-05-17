using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiProyectoAmazon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiProyectoAmazon.Repositories;

namespace ApiProyectoAmazon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonacionController : ControllerBase
    {
        private RepositoryAdopcion repoAdop;

        public DonacionController(RepositoryAdopcion repoAdop) {

            this.repoAdop = repoAdop;
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{codigoprotectora}")]
        public ActionResult<List<Donacion>> GetDonaciones(int codigoprotectora) {

            return this.repoAdop.GetDonaciones(codigoprotectora);
        }

        [HttpPost]
        [Authorize]
        [Route("[action]")]
        public ActionResult InsertarDonacion(Donacion don) {

            this.repoAdop.InsertarDonacion(don.CodigoProtectora, don.Cantidad, don.Dni);

            return Ok();
        }
    }
}
