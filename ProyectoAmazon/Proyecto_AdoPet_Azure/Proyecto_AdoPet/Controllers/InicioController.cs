using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using NuGetAdoPet.Models;
using Proyecto_AdoPet.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Proyecto_AdoPet.Filters;

namespace Proyecto_AdoPet.Controllers
{
    public class InicioController : Controller
    {
        private ServiceApiAdopet service;
        private ServiceS3 servS3;

        public InicioController(ServiceApiAdopet service, ServiceS3 servS3) {

            this.service = service;
            this.servS3 = servS3;
        }

        /*GET y POST de mi Login*/
        public IActionResult Index() {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string nombre, string psswd)
        {
            string token = await this.service.GetTokenAsync(nombre, psswd);

            if (token == null)
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }
            else
            {

                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme
                    , ClaimTypes.Name, ClaimTypes.Role);

                VistaCuentas cuenta =await this.service.BuscarCuenta(token);

                identity.AddClaim(new Claim("TOKEN", token));
                identity.AddClaim(new Claim("Cuenta", JsonConvert.SerializeObject(cuenta)));

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync
                    (CookieAuthenticationDefaults.AuthenticationScheme
                    , principal, new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });
                return RedirectToAction("PaginaPrincipal");
            }

        }

        public IActionResult Registro() {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(string tipoCuenta, string nombre, string apellidos, string dni, string telefono, string ciudad,
            string usuario, string pswd, [FromForm] IFormFile imagenU, string nombreP, string direccionP, string ciudadP, string telefonoP, string paypal, string banco,
            string pswdP, [FromForm] IFormFile imagenP) {

            if (tipoCuenta == "usuario")
            {
                string extension = imagenU.FileName.Split(".")[1];
                string fileName = nombre.Trim() + "." + extension;

                using (Stream stream = imagenU.OpenReadStream())
                {
                    await this.servS3.UploadFileAsync(stream, fileName, "/Usuarios");
                }

                //Una vez añadido debemos implantar la seguridad y recuperar el token de dicha cuenta

                if (await this.service.InsertarUsuario(dni, nombre, apellidos, telefono, ciudad, usuario, pswd, imagenU.FileName) == true)
                {
                    return RedirectToAction("Index");

                }

            }
            else if (tipoCuenta == "protectora")
            {
                string extension = imagenP.FileName.Split(".")[1];
                string fileName = nombreP.Trim() + "." + extension;

                using (Stream stream = imagenP.OpenReadStream())
                {
                    await this.servS3.UploadFileAsync(stream, fileName, "/Protectoras");
                }

                //Una vez añadido debemos implantar la seguridad y recuperar el token de dicha cuenta

                if (await this.service.InsertarProtectora(nombreP, direccionP, ciudadP, telefonoP,banco,paypal,pswdP, imagenP.FileName) == true)
                {

                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        [AuthorizeCuentas]
        public async Task<IActionResult> CerrarSesion() {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Remove("TOKEN");

            return RedirectToAction("Index");
        }       

        [AuthorizeCuentas]
        public async Task<IActionResult> PaginaPrincipal() {

            string token = HttpContext.User.FindFirst("TOKEN").Value;

            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);
            int NumeroAnimales = await this.service.CantidadAnimalesRegistrados(token);

            int numeroProtectoras = await this.service.CantidadProtectorasRegistradas(token);

            int numeroUsuarios = await this.service.CantidadUsuariosRegistrados(token);

            ViewData["animales"] = NumeroAnimales;
            ViewData["protectoras"] = numeroProtectoras;
            ViewData["usuarios"] = numeroUsuarios;

            return View(cuenta);
        }
    }
}
