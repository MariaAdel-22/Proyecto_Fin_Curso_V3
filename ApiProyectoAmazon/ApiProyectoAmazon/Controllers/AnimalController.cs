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
using ApiProyectoAmazon.Repositories;

namespace ApiProyectoAmazon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private RepositoryAnimal repoAn;

        public AnimalController(RepositoryAnimal repo)
        {
            this.repoAn = repo;
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{ciudad}")]
        public ActionResult<List<VistaAnimales>> AnimalesPorCiudad(string ciudad) {

            return this.repoAn.GetAnimalesPorCiudad(ciudad);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{ciudad}")]
        public ActionResult<List<string>> EdadesAnimales(string ciudad) {

            return this.repoAn.GetEdadAnimales(ciudad);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{ciudad}")]
        public ActionResult<List<string>> EspecieAnimales(string ciudad)
        {

            return this.repoAn.GetEspecieAnimales(ciudad);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{ciudad}")]
        public ActionResult<List<string>> TamanioAnimales(string ciudad)
        {

            return this.repoAn.GetTamanioAnimales(ciudad);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{ciudad}/{especie}/{edad}/{tamanio}")]
        public ActionResult<List<VistaAnimales>> BuscarAnimales(string ciudad,string especie, string edad, string tamanio) {

            return this.repoAn.BuscarAnimales(ciudad, especie, edad, tamanio);
        }

        [HttpPost]
        [Authorize]
        [Route("[action]")]
        public ActionResult InsertarComentario(Comentarios com) {

            this.repoAn.InsertarComentario(com.CodigoAnimal,com.Codigo,com.Mensaje,com.Fecha);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{idAnimal}")]
        public ActionResult<List<Comentarios>> ComentariosPorAnimal(int idAnimal) {

            return this.repoAn.FindComentariosAnimal(idAnimal);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{idComentario}")]
        public ActionResult<Comentarios> FindComentario(int idComentario) {

            return this.repoAn.FindComentario(idComentario);
        }

        [HttpPut]
        [Authorize]
        [Route("[action]")]
        public ActionResult ModificarComentario(Comentarios com) {

           this.repoAn.ModificarComentario(com.CodigoMensaje,com.Mensaje);

            return Ok();
        }

        [HttpDelete]
        [Authorize]
        [Route("[action]/{id}")]
        public ActionResult EliminarComentario(int id) {

            this.repoAn.EliminarComentario(id);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{codigo}")]
        public ActionResult<List<int>> AnimalesFavoritos(string codigo) {

            return this.repoAn.GetAnimalesFavoritos(codigo);
        }

        [HttpPost]
        [Authorize]
        [Route("[action]")]
        public ActionResult InsertarFavorito(Favoritos fav) {

            this.repoAn.InsertarFavorito(fav.CodigoAnimal,fav.Dni);

            return Ok();
        }

        [HttpDelete]
        [Authorize]
        [Route("[action]/{codigoAnimal}/{dni}")]
        public ActionResult EliminarFavorito(int codigoAnimal,string dni) {

            this.repoAn.EliminarFavorito(codigoAnimal,dni);

            return Ok();
        }
    }
}
