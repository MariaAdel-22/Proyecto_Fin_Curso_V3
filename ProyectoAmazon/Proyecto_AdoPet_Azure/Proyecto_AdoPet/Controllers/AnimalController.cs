using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGetAdoPet.Models;
using Proyecto_AdoPet.Filters;
using Proyecto_AdoPet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_AdoPet.Controllers
{
    public class AnimalController : Controller
    {

        private ServiceApiAdopet service;
        private ServiceS3 servS3;

        public AnimalController(ServiceApiAdopet service, ServiceS3 servS3) {

            this.service = service;
            this.servS3 = servS3;
        }

        /*Con esta vista mostramos todos los animales de todas las protectoras que están en nuestra misma ciudad*/
        [AuthorizeCuentas]
        public async Task<IActionResult> AnimalesProtectoras() {

            string token = HttpContext.User.FindFirst("TOKEN").Value;

            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);
            ViewBag.Cuenta = cuenta;

            string ciudad = "";

            if (cuenta.usuario != null) {

                ViewBag.Ciudad = cuenta.usuario.Ciudad;
                ciudad = cuenta.usuario.Ciudad;

            } else if (cuenta.protectora != null) {

                ViewBag.Ciudad = cuenta.protectora.Ciudad;
                ciudad = cuenta.protectora.Ciudad;
            }

            List<string> Edades =await this.service.GetEdadesAnimales(ciudad,token);

            ViewBag.Edad = Edades;

            List<string> Especies = await this.service.GetEspeciesAnimales(ciudad, token);

            ViewBag.Especie = Especies;

            List<string> Tams = await this.service.GetTamaniosAnimales(ciudad, token);

            ViewBag.Tam = Tams;

            List<VistaAnimales> cargarAnimales = await this.service.GetAnimalesCiudad(ciudad, token);

            return View(cargarAnimales);
        }

        /*Este POST es el resultado del filtro para la búsqueda de animales de todas las protectoras*/

        [HttpPost]
        [AuthorizeCuentas]
        public async Task<IActionResult> AnimalesProtectoras(string tam, string especie, string edad) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;

            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);
            ViewBag.Cuenta = cuenta;

            string ciudad = "";

            if (cuenta.usuario != null)
            {

                ViewBag.Ciudad = cuenta.usuario.Ciudad;
                ciudad = cuenta.usuario.Ciudad;

            }
            else if (cuenta.protectora != null)
            {

                ViewBag.Ciudad = cuenta.protectora.Ciudad;
                ciudad = cuenta.protectora.Ciudad;
            }

            List<string> Edades = await this.service.GetEdadesAnimales(ciudad, token);

            ViewBag.Edad = Edades;

            List<string> Especies = await this.service.GetEspeciesAnimales(ciudad, token);

            ViewBag.Especie = Especies;

            List<string> Tams = await this.service.GetTamaniosAnimales(ciudad, token);

            ViewBag.Tam = Tams;

            List<VistaAnimales> Animales = await this.service.BuscarAnimalesFiltro(ciudad, especie, edad, tam,token);

            return View(Animales);
        }


        /*Esta vista mostrará el perfil del animal, además tiene la funcionalidad de comentar en el perfil del animal o añadirlo como
         favorito*/

        [AuthorizeCuentas]
        public async Task<IActionResult> PerfilAnimalProtectora(int idAnimal) {

            TempData["IDANIMAL"] = idAnimal;

            string token = HttpContext.User.FindFirst("TOKEN").Value;
            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);
            ViewBag.Cuenta = cuenta;


            List<Comentarios> comentarios = await this.service.GetComentarios(idAnimal, token);
            ViewBag.Comentarios = comentarios;

            List<Usuario> usuarios = new List<Usuario>();
            List<Protectora> protectoras = new List<Protectora>();

            foreach (Comentarios com in comentarios) {

                Usuario usu = await this.service.BuscarUsuario(com.Codigo,token);
                Protectora pro = await this.service.BuscarProtectora(com.Codigo, token);

                if (usu != null) {

                    usuarios.Add(usu);

                } else if (pro != null) {

                    protectoras.Add(pro);
                }
            }

            ViewBag.Usuarios = usuarios;
            ViewBag.Protectoras = protectoras;

            Animal animal = await this.service.BuscarAnimal(idAnimal, token);

            List<int> favoritos = new List<int>();

            if (cuenta.usuario != null)
            {

                favoritos = await this.service.GetFavoritosUsuario(cuenta.usuario.Dni, token);

                if (favoritos != null)
                {

                    ViewBag.Favoritos = favoritos;
                }

            } else if (cuenta.protectora != null) {

                favoritos = await this.service.GetFavoritosUsuario(cuenta.protectora.IdProtectora.ToString(), token);

                if (favoritos != null)
                {

                    ViewBag.Favoritos = favoritos;
                }
            }

            return View(animal);
        }

        /*Este POST se encarga de añadir un comentario en el perfil del animal*/

        [HttpPost]
        public async Task<IActionResult> PerfilAnimalProtectora(string comentarioCuerpo) {

            int idAnimal = int.Parse(TempData["IDANIMAL"].ToString());

            string token = HttpContext.User.FindFirst("TOKEN").Value;
            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);
            ViewBag.Cuenta = cuenta;

            if (cuenta != null)
            {
                if (cuenta.TipoCuenta == "PROTECTORAS")
                {
                    //Se filtrará si es un correo electrónico o número de teléfono
                    await this.service.InsertarComentario(idAnimal, cuenta.protectora.IdProtectora.ToString(), comentarioCuerpo, cuenta.TipoCuenta, cuenta.protectora.Telefono, token);
                }
                else if (cuenta.TipoCuenta == "USUARIOS")
                {
                    //Se filtrará si es un correo electrónico o número de teléfono
                    await this.service.InsertarComentario(idAnimal, cuenta.usuario.Dni, comentarioCuerpo, cuenta.TipoCuenta, cuenta.usuario.Telefono, token);
                }
            }
            else {

                return RedirectToAction("Index");

            }

            

            Animal animal = await this.service.BuscarAnimal(idAnimal, token);

            List<Comentarios> comentarios = await this.service.GetComentarios(idAnimal, token);
            ViewBag.Comentarios = comentarios;

            List<Usuario> usuarios = new List<Usuario>();
            List<Protectora> protectoras = new List<Protectora>();

            foreach (Comentarios com in comentarios)
            {

                Usuario usu = await this.service.BuscarUsuario(com.Codigo, token);
                Protectora pro = await this.service.BuscarProtectora(com.Codigo, token);

                if (usu != null)
                {

                    usuarios.Add(usu);

                }
                else if (pro != null)
                {

                    protectoras.Add(pro);
                }
            }

            ViewBag.Usuarios = usuarios;
            ViewBag.Protectoras = protectoras;

            return View(animal);
        }

        /*El modal de editar comentario en una partial view*/
        [AuthorizeCuentas]
        public async Task<IActionResult> _ModalEditarComentario(int idcomentario) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;
            Comentarios com = await this.service.FindComentario(idcomentario, token);

            return PartialView("_ModalEditarComentario", com);
        }

        /*Con un modal podemos modificar un comentario*/
        [HttpPost]
        [AuthorizeCuentas]
        public async Task<JsonResult> ModificarComentario(int idComentario, string Comentario) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;

            Comentarios com = await this.service.ModificarComentario(idComentario,Comentario,token);

            return Json(com);
        }

        /*El modal de eliminar un comentario en una partial view*/
        [AuthorizeCuentas]
        public async Task<IActionResult> _ModalEliminarComentario(int idcomentario) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;
            Comentarios com = await this.service.FindComentario(idcomentario, token);

            return PartialView("_ModalEliminarComentario", com);
        }

        /*Con un modal podremos eliminar un comentario*/
        [HttpPost]
        [AuthorizeCuentas]
        public async Task<JsonResult> EliminarComentario(int idComentario, int idAnimal) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;

            await this.service.EliminarComentario(idComentario, token);

            List<Comentarios> comentarios = await this.service.GetComentarios(idAnimal,token);

            return Json(new { result = "Redirect", url = Url.Action("PerfilAnimalProtectora", "Animal", new { idAnimal = idAnimal }) });
        }

        /*Utilizando AJAX añadimos a los animales en favoritos*/
        [HttpPost]
        [AuthorizeCuentas]
        public async Task<JsonResult> InsertarFavorito(int codigoAnimal) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;
            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            await this.service.InsertarFavorito(codigoAnimal,cuenta.CodigoCuenta,token);

            return Json("funciona");
        }

        /*Por AJAX también podemos eliminar como favorito un animal*/
        [HttpPost]
        [AuthorizeCuentas]
        public async Task<JsonResult> EliminarFavorito(int codigoAnimal) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;
            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            await this.service.EliminarFavorito(codigoAnimal,cuenta.CodigoCuenta,token);

            return Json("funciona");

        }
    }
}
