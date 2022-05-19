using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using NuGetAdoPet.Models;
using Proyecto_AdoPet.Services;
using Proyecto_AdoPet.Filters;
using Newtonsoft.Json;

namespace Proyecto_AdoPet.Controllers
{
    public class ProtectoraController : Controller
    {
        private ServiceApiAdopet service;
        private ServiceS3 servS3;

        public ProtectoraController(ServiceApiAdopet service, ServiceS3 servS3) {

            this.service = service;
            this.servS3 = servS3;
        }

        /*Esta vista muestra todas las protectoras de la misma ciudad que la cuenta*/
        [AuthorizeCuentas]
        public async Task<IActionResult> Protectoras() {

            string token = HttpContext.User.FindFirst("TOKEN").Value;

            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            List<Protectora> protectoras;
            List<int> Cantidad;

            if (cuenta.usuario != null)
            {

                protectoras = await this.service.GetProtectoras(cuenta.usuario.Ciudad, token);
                Cantidad= await this.service.GetNumeroProtectoras(cuenta.usuario.Ciudad,token);
            }
            else if (cuenta.protectora != null)
            {

                protectoras = await this.service.GetProtectoras(cuenta.protectora.Ciudad, token);
                Cantidad = await this.service.GetNumeroProtectoras(cuenta.protectora.Ciudad, token);
            }
            else {

                protectoras = new List<Protectora>();
                Cantidad = new List<int>();
            }

            ViewBag.Protectoras = protectoras;
            ViewBag.Cantidades = Cantidad;

            return View(cuenta);

        }

        /*La vista que mostrará todos los animales que tiene la protectora y la cantidad de favoritos por animal*/
        [AuthorizeCuentas]
        public async Task<IActionResult> AnimalesProtectora() {

            string token = HttpContext.User.FindFirst("TOKEN").Value;

            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            List<Animal> animales = await this.service.GetAnimalesProtectora(cuenta.protectora.IdProtectora,token);

            ViewBag.Animales = animales;

            return View(cuenta);
        }

        /*Insertar Animal*/

        [HttpPost]
        [AuthorizeCuentas]
        public async Task<IActionResult> AnimalesProtectora([FromForm] IFormFile imagenAnimal, string nombre, string edad, string genero, string peso, string especie)
        {
            string token = HttpContext.User.FindFirst("TOKEN").Value;

            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            string extension = imagenAnimal.FileName.Split(".")[1];
            string fileName = nombre.Trim() + "." + extension;

            using (Stream stream = imagenAnimal.OpenReadStream())
            {
                await this.servS3.UploadFileAsync(stream, fileName, "https://bucket-proyecto.s3.amazonaws.com/Animales/");
            }

            if (await this.service.InsertarAnimal(imagenAnimal.FileName, nombre, edad, genero, peso, especie,token) == true)
            {
                List<Animal> animales = await this.service.GetAnimalesProtectora(cuenta.protectora.IdProtectora, token);
                ViewBag.Animales = animales;
            }

            return View(cuenta);
        }

        /*Vista que permite modificar el animal*/
        [AuthorizeCuentas]
        public async Task<IActionResult> ModificarAnimal(int idAnimal) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;

            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            Animal an =await this.service.BuscarAnimal(idAnimal, token);

            ViewBag.Animal = an;

            TempData["IDANIMAL"] = an.CodigoAnimal.ToString();

            return View(cuenta);

        }

        [HttpPost]
        [AuthorizeCuentas]
        public async Task<IActionResult> ModificarAnimal([FromForm] IFormFile imagenAnimalModificado, string Nombre, string Edad, string Genero, string Peso, string Especie)
        {
            string token = HttpContext.User.FindFirst("TOKEN").Value;

            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            if (imagenAnimalModificado != null)
            {

                string extension = imagenAnimalModificado.FileName.Split(".")[1];
                string fileName = Nombre.Trim() + "." + extension;

                using (Stream stream = imagenAnimalModificado.OpenReadStream())
                {
                    await this.servS3.UploadFileAsync(stream, fileName, "https://bucket-proyecto.s3.amazonaws.com/Animales/");
                }

                if (await this.service.ModificarAnimal(int.Parse(TempData["IDANIMAL"].ToString()), imagenAnimalModificado.FileName, Nombre, Edad, Genero, Peso, Especie, token) == true)
                {

                    ViewBag.IdProtectora = cuenta.protectora.IdProtectora;

                    return RedirectToAction("AnimalesProtectora", "Protectora");
                }
                else {

                    return View();
                }
            }
            else {

                Animal an = await this.service.BuscarAnimal(int.Parse(TempData["IDANIMAL"].ToString()), token);

                if (await this.service.ModificarAnimal(an.CodigoAnimal, an.Imagen, Nombre, Edad, Genero, Peso, Especie, token) == true)
                {

                    ViewBag.IdProtectora = cuenta.protectora.IdProtectora;

                    return RedirectToAction("AnimalesProtectora", "Protectora");
                }
                else {

                    return View();
                }
            }  
        }

        /*Modal en vista parcial para eliminar animal*/
        [AuthorizeCuentas]
        public async Task<IActionResult> _ModalEliminarAnimal(int idAnimal) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;

            Animal an = await this.service.BuscarAnimal(idAnimal, token);

            return PartialView("_ModalEliminarAnimal", an);
        }

        [HttpPost]
        [AuthorizeCuentas]
        public async Task<JsonResult> EliminarAnimal(int idAnimal) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;

            Animal an = await this.service.BuscarAnimal(idAnimal, token);

            if (await this.service.EliminarAnimal(idAnimal, token) == true) {

                await this.servS3.DeleteFileAsync(an.Imagen);
            }
          
            return Json(new { result = "Redirect", url = Url.Action("AnimalesProtectora", "Protectora") });
        }


        /*Vista que tiene el perfil de la protectora y la cual puede derivar a chat*/

        public async Task<IActionResult> FichaProtectora(string id) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;
            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            ViewBag.Cuenta = cuenta;

            Protectora pro = await this.service.BuscarProtectora(id,token);

            return View(pro);
        }
    }
}
