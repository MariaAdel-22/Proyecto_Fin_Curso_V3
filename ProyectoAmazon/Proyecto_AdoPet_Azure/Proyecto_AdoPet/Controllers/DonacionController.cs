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
    public class DonacionController : Controller
    {
        private ServiceApiAdopet service;
        private ServiceS3 servS3;

        public DonacionController(ServiceApiAdopet service, ServiceS3 servS3)
        {

            this.service = service;
            this.servS3=servS3;
        }

        /*Vista para donar del usuario a una protectora*/

        [AuthorizeCuentas]
        public async Task<IActionResult> Donaciones() {

            string token = HttpContext.User.FindFirst("TOKEN").Value;
            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            List<Protectora> protectoras = await this.service.GetProtectoras(cuenta.usuario.Ciudad,token);

            ViewBag.Protectoras = protectoras;

            return View();
        }

        [HttpPost]
        [AuthorizeCuentas]
        public async Task<JsonResult> Donaciones(int idProtectora) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;
            var pro = await this.service.BuscarProtectora(idProtectora.ToString(),token);

            return Json(pro);
        }

        /*Historial de donaciones de la protectora*/
        [AuthorizeCuentas]
        public async Task<IActionResult> HistorialDonaciones() {

            string token = HttpContext.User.FindFirst("TOKEN").Value;
            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            List<Donacion> donaciones = await this.service.GetDonacionesProtectora(int.Parse(cuenta.CodigoCuenta), token);

            ViewBag.Donaciones = donaciones;

            return View(cuenta);

        }

        [HttpPost]
        [AuthorizeCuentas]
        public async Task<JsonResult> DonacionProtectora(int idProtectora, int cantidad) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;
            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            await this.service.InsertarDonacion(idProtectora,cantidad,cuenta.CodigoCuenta,cuenta.usuario.Imagen,token);

            return Json(new { result = "Redirect", url = Url.Action("PaginaPrincipal", "Inicio") });
        }

        /*Esta vista muestra el perfil del usuario que ha donado a la protectora*/
        [AuthorizeCuentas]
        public async Task<IActionResult> PerfilUsuarioDonante(string dni) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;
            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);
            ViewBag.Cuenta = cuenta;

            Usuario usu = await this.service.BuscarUsuario(dni,token);

            return View(usu);
        }
    }
}
