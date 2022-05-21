using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGetAdoPet.Models;
using Proyecto_AdoPet.Services;
using Proyecto_AdoPet.Filters;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.Owin;
using System.Web;
using Microsoft.Owin.Security;
using System.Text.RegularExpressions;

namespace Proyecto_AdoPet.Controllers
{
    public class CuentaController : Controller
    {
        private ServiceApiAdopet service;
        private ServiceS3 servS3;

        public CuentaController(ServiceApiAdopet service, ServiceS3 servS3)
        {

            this.service = service;
            this.servS3=servS3;
        }

        /*GET y POST de la vista que modifica los datos de la cuenta*/
        [AuthorizeCuentas]
        public IActionResult ModificarDatosCuenta() {

            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            return View(cuenta);
        }

        //////////////////////NO CONSIGO ACTUALIZAR LOS CLAIMS/////////////////////////
        [HttpPost]
        [AuthorizeCuentas]
        public async Task<IActionResult> ModificarDatosCuenta([FromForm] IFormFile ImagenUsuario, string Nombre, string Apellidos, string Telefono, string Ciudad, string NombreUsuario, string Password,
            [FromForm] IFormFile ImagenProtectora, string Direccion, string Paypal, string Tarjeta) {

            string token = HttpContext.User.FindFirst("TOKEN").Value;

            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            if (cuenta.usuario != null) {


                if (ImagenUsuario != null)
                {
                    string extension = ImagenUsuario.FileName.Split(".")[1];
                    string fileName = Nombre.Trim() + "." + extension;

                    using (Stream stream = ImagenUsuario.OpenReadStream())
                    {
                        await this.servS3.UploadFileAsync(stream, fileName, "/Protectoras");
                    }

                    if (await this.service.ModificarUsuario(cuenta.usuario.Dni, Nombre, Apellidos, Telefono, Ciudad, NombreUsuario, Password, ImagenUsuario.FileName, token) == true)
                    {

                        HttpContext.User.Identities.First().RemoveClaim(HttpContext.User.FindFirst("Cuenta"));

                        Claim cuentaClaim = HttpContext.User.FindFirst("Cuenta");


                        if (cuentaClaim == null)
                        {

                            VistaCuentas cuentaNueva = await this.service.BuscarCuenta(token);

                            if (cuentaNueva != null)
                            {

                                HttpContext.User.Identities.First().AddClaim(new Claim("Cuenta", JsonConvert.SerializeObject(cuentaNueva)));

                            }
                        }
                    }
                }
                else {


                    if (await this.service.ModificarUsuario(cuenta.usuario.Dni, Nombre, Apellidos, Telefono, Ciudad, NombreUsuario, Password, cuenta.usuario.Imagen, token) == true)
                    {

                        HttpContext.User.Identities.First().RemoveClaim(HttpContext.User.FindFirst("Cuenta"));

                        Claim cuentaClaim = HttpContext.User.FindFirst("Cuenta");


                        if (cuentaClaim == null) {

                            VistaCuentas cuentaNueva = await this.service.BuscarCuenta(token);

                            if (cuentaNueva != null)
                            {

                                HttpContext.User.Identities.First().AddClaim(new Claim("Cuenta", JsonConvert.SerializeObject(cuentaNueva)));

                            }
                        }

                    }

                }
            } else if (cuenta.protectora != null) {

                if (ImagenProtectora != null)
                {
                    string extension = ImagenProtectora.FileName.Split(".")[1];
                    string fileName = Nombre.Trim() + "." + extension;

                    using (Stream stream = ImagenProtectora.OpenReadStream())
                    {
                        await this.servS3.UploadFileAsync(stream, fileName, "/Protectoras");
                    }

                    if (await this.service.ModificarProtectora(cuenta.protectora.IdProtectora, Nombre, Direccion, Ciudad, Telefono, Tarjeta, Paypal, Password, ImagenProtectora.FileName, token) == true)
                    {

                        HttpContext.User.Identities.First().RemoveClaim(HttpContext.User.FindFirst("Cuenta"));

                        Claim cuentaClaim = HttpContext.User.FindFirst("Cuenta");


                        if (cuentaClaim == null)
                        {

                            VistaCuentas cuentaNueva = await this.service.BuscarCuenta(token);

                            if (cuentaNueva != null)
                            {

                                HttpContext.User.Identities.First().AddClaim(new Claim("Cuenta", JsonConvert.SerializeObject(cuentaNueva)));

                            }
                        }
                    }
                }
                else {

                    if (await this.service.ModificarProtectora(cuenta.protectora.IdProtectora, Nombre, Direccion, Ciudad, Telefono, Tarjeta, Paypal, Password, ImagenProtectora.FileName, token) == true)
                    {

                        HttpContext.User.Identities.First().RemoveClaim(HttpContext.User.FindFirst("Cuenta"));

                        Claim cuentaClaim = HttpContext.User.FindFirst("Cuenta");


                        if (cuentaClaim == null)
                        {

                            VistaCuentas cuentaNueva = await this.service.BuscarCuenta(token);

                            if (cuentaNueva != null)
                            {

                                HttpContext.User.Identities.First().AddClaim(new Claim("Cuenta", JsonConvert.SerializeObject(cuentaNueva)));

                            }
                        }
                    }
                }
            }

            return RedirectToAction("PaginaPrincipal", "Inicio");
        }

        //CHAT

        public async Task<Dictionary<string, string>> Rooms(string codigo, string receptor = "0")
        {
            string token = HttpContext.User.FindFirst("TOKEN").Value;

            Dictionary<string, string> rooms = new Dictionary<string, string>();
            List<string> chats = new List<string>();

            Dictionary<string, string> ImagenProtectora = new Dictionary<string, string>();
            Dictionary<string, string> ImagenUsuario = new Dictionary<string, string>();

            if (receptor != "0")
            {

                chats = await this.service.BuscarChat(codigo, receptor,token);
            }
            else
            {

                chats = await this.service.GetChats(codigo,token);
            }

            if (chats.Count == 0)
            {

                await this.service.CrearSalaChat(codigo, receptor,token);
                string emisor = await this.service.GetEmisorDeChatVacio(codigo,token);

                chats.Add(emisor);
            }

            TempData["SalasChat"] = chats;


            for (int i = 0; i < chats.Count; i++)
            {
                Chats CuentasEmisoras = await this.service.GetEmisorChats(chats[i],token);
                string nombreSala = "";

                if (CuentasEmisoras.CodigoCuenta == codigo)
                {

                    nombreSala = await this.service.GetNombreProtectora(int.Parse(CuentasEmisoras.CodigoSala),token);

                    if (nombreSala != null)
                    {

                        rooms.Add(chats[i], nombreSala);

                        Protectora pro = await this.service.BuscarProtectora(chats[i],token);

                        ImagenProtectora.Add(chats[i],pro.Imagen);
                    }
                    else
                    {

                        nombreSala = await this.service.GetNombreUsuario(CuentasEmisoras.CodigoSala, token);

                        rooms.Add(chats[i], nombreSala);

                        Usuario usu = await this.service.BuscarUsuario(chats[i],token);

                        ImagenUsuario.Add(chats[i], usu.Imagen);
                    }
                }
                else
                {

                    nombreSala = await this.service.GetNombreUsuario(CuentasEmisoras.CodigoCuenta, token);
                    rooms.Add(chats[i], nombreSala);

                    Usuario usu = await this.service.BuscarUsuario(chats[i], token);

                    ImagenUsuario.Add(chats[i], usu.Imagen);

                }
            }

            TempData["ImagenProtectoras"] = ImagenProtectora;
            TempData["ImagenUsuario"] = ImagenUsuario;

            return rooms;
        }

        /*En esta vista se verán las diferentes salas de chat*/
        [AuthorizeCuentas]
        public async Task<IActionResult> SesionesChat(string codigoE, string codigoR = "0")
        {
            Dictionary<string, string> rooms = new Dictionary<string, string>();

            if (codigoR != "0")
            {
                rooms = await this.Rooms(codigoE, codigoR);
            }
            else
            {
                rooms = await this.Rooms(codigoE);
            }

            ViewData["ImagenesProtectora"] = TempData["ImagenProtectoras"];
            ViewData["ImagenesUsuario"] = TempData["ImagenUsuario"];

            return View(rooms);
        }

        /*Esta vista muestra cada chat con sus mensajes previos guardados en la BBDD*/
        [AuthorizeCuentas]
        public async Task<IActionResult> Room(int room)
        {
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

            Chats CuentasEmisoras = await this.service.GetEmisorChats(room.ToString(),token);
            Protectora ProtectoraSala = new Protectora();
            Usuario usu = new Usuario();

            if (CuentasEmisoras.CodigoCuenta == cuenta.CodigoCuenta)
            {

                usu = await this.service.BuscarUsuario(CuentasEmisoras.CodigoSala,token);

                if (usu == null)
                {
                    ProtectoraSala = await this.service.BuscarProtectora(CuentasEmisoras.CodigoSala,token);

                }

            }
            else if (CuentasEmisoras.CodigoSala == cuenta.CodigoCuenta)
            {
                ProtectoraSala = await this.service.BuscarProtectora(CuentasEmisoras.CodigoCuenta,token);

                if (ProtectoraSala == null)
                {
                    usu = await this.service.BuscarUsuario(CuentasEmisoras.CodigoCuenta,token);

                }

            }

            Chats codigoSala = await this.service.GetCodigoSalaPrincipal(CuentasEmisoras.CodigoCuenta, CuentasEmisoras.CodigoSala,token);

            //Significa que el usuario no es el que ha iniciado la conversación (y ha creado la sala),sino que le ha hablado otra persona
            if (codigoSala == null)
            {

                codigoSala =await this.service.GetCodigoSalaPrincipal(CuentasEmisoras.CodigoSala, CuentasEmisoras.CodigoCuenta,token);
            }

            Chats sala =await this.service.BuscarChatPrincipal(codigoSala.CodigoChat,token);


            ViewBag.Sala = sala;
            ViewBag.Cuenta = cuenta;
            ViewBag.Protectora = ProtectoraSala;
            ViewBag.Usuario = usu;

            List<Chat> HistorialChat = await this.service.GetHistorialChat(sala.CodigoChat,token);
            ViewBag.HistorialChat = HistorialChat;

            return PartialView("Room");
        }

        /*Insertar mensaje en el chat*/
        [AuthorizeCuentas]
        [HttpPost]
        public async Task<JsonResult> InsertarComentario(int idSala, string emisor, string receptor, string mensaje)
        {
            string token = HttpContext.User.FindFirst("TOKEN").Value;

           await this.service.InsertarMensaje(idSala, emisor, receptor, mensaje,token);

            return Json("funciona");
        }




        /*Método provado que devuelve las diferentes salas que contenga la cuenta*/
        /* [AuthorizeCuentas]
         public async Task<Dictionary<string, string>> Rooms(string codigo, string receptor = "0") {

             string token = HttpContext.User.FindFirst("TOKEN").Value;
             Dictionary<string, string> rooms = new Dictionary<string, string>();
             List<string> chats = new List<string>();

             if (receptor != "0")
             {

                 chats = await this.service.BuscarChat(codigo, receptor, token);
             }
             else {

                 chats = await this.service.GetChats(codigo, token);
             }

             //si no hay salas la crea
             if (chats.Count == 0)
             {
                 if (await this.service.CrearSalaChat(codigo, receptor, token) == true)
                 {
                     string emisor = await this.service.GetEmisorDeChatVacio(codigo, token);

                     chats.Add(emisor);
                 }
             }

             TempData["SalasChat"] = chats;

             for (int i = 0; i < chats.Count; i++) {

                 //Chats CuentasEmisoras = await this.service.GetEmisorChats(chats[i], token);

                 Chats CuentasEmisoras;

                 if (receptor != "0")
                 {

                     CuentasEmisoras = await this.service.BuscarChatEmisorReceptor(codigo, chats[i], token);
                 }
                 else {

                     CuentasEmisoras = await this.service.GetEmisorChats(chats[i], token);
                 }


                 string nombreSala = "";

                 if (CuentasEmisoras.CodigoCuenta == codigo)
                 {
                     if (Regex.IsMatch(CuentasEmisoras.CodigoSala, @"^([0-9]{8}[A-Z])|[XYZ][0-9]{7}[A-Z]$"))
                     {

                         nombreSala = await this.service.GetNombreUsuario(CuentasEmisoras.CodigoSala, token);

                     }
                     else {

                         nombreSala = await this.service.GetNombreProtectora(int.Parse(CuentasEmisoras.CodigoSala), token);
                     }

                     if (nombreSala != null)
                     {

                         rooms.Add(CuentasEmisoras.CodigoSala, nombreSala);
                     }
                     else
                     {
                         if (Regex.IsMatch(CuentasEmisoras.CodigoCuenta, @"^([0-9]{8}[A-Z])|[XYZ][0-9]{7}[A-Z]$"))
                         {

                             nombreSala = await this.service.GetNombreUsuario(CuentasEmisoras.CodigoCuenta, token);
                             rooms.Add(CuentasEmisoras.CodigoCuenta, nombreSala);
                         }
                         else
                         {

                             nombreSala = await this.service.GetNombreProtectora(int.Parse(CuentasEmisoras.CodigoCuenta), token);
                             rooms.Add(CuentasEmisoras.CodigoCuenta, nombreSala);
                         }

                     }
                 }
                 else if(CuentasEmisoras.CodigoSala == codigo)
                 {
                     if (Regex.IsMatch(CuentasEmisoras.CodigoCuenta, @"^([0-9]{8}[A-Z])|[XYZ][0-9]{7}[A-Z]$"))
                     {

                         nombreSala = await this.service.GetNombreUsuario(CuentasEmisoras.CodigoCuenta, token);

                     }
                     else
                     {

                         nombreSala = await this.service.GetNombreProtectora(int.Parse(CuentasEmisoras.CodigoCuenta), token);
                     }

                     if (nombreSala != null)
                     {

                         rooms.Add(CuentasEmisoras.CodigoCuenta, nombreSala);
                     }
                     else
                     {
                         if (Regex.IsMatch(CuentasEmisoras.CodigoSala, @"^([0-9]{8}[A-Z])|[XYZ][0-9]{7}[A-Z]$"))
                         {

                             nombreSala = await this.service.GetNombreUsuario(CuentasEmisoras.CodigoSala, token);
                             rooms.Add(CuentasEmisoras.CodigoSala, nombreSala);
                         }
                         else
                         {

                             nombreSala = await this.service.GetNombreProtectora(int.Parse(CuentasEmisoras.CodigoSala), token);
                             rooms.Add(CuentasEmisoras.CodigoSala, nombreSala);
                         }

                     }
                     //nombreSala = await this.service.GetNombreUsuario(CuentasEmisoras.CodigoSala, token);
                     //rooms.Add(CuentasEmisoras.CodigoSala, nombreSala);
                 }
             }


             return rooms;
         }
        */

        /*En esta vista se verán las diferentes salas de chat*/
        /* [AuthorizeCuentas]
         public async Task<IActionResult> SesionesChat(string codigoE, string codigoR = "0")
         {
             string token = HttpContext.User.FindFirst("TOKEN").Value;

             Dictionary<string, string> rooms = new Dictionary<string, string>();
             Dictionary<string, string> ImagenesProtectora = new Dictionary<string, string>();
             Dictionary<string, string> ImagenesUsuario = new Dictionary<string, string>();

             if (codigoR != "0")
             {
                 rooms = await this.Rooms(codigoE, codigoR);
             }
             else
             {
                 rooms = await this.Rooms(codigoE);
             }

             foreach (var e in rooms) {

                 Chats sala;

                 if (Regex.IsMatch(e.Key, @"^([0-9]{8}[A-Z])|[XYZ][0-9]{7}[A-Z]$"))
                 {

                     sala = await this.service.GetCodigoSalaPrincipal(codigoE, e.Key, token);
                 }
                 else {

                     sala = await this.service.GetCodigoSalaPrincipal(codigoE,e.Key,token);
                 }

                 //Chats sala = await this.service.BuscarChatPrincipal(int.Parse(e.Key), token);

                 if (sala != null)
                 {

                     //Usuario usu = await this.service.BuscarUsuario(sala.CodigoSala, token);
                     Usuario usu = await this.service.BuscarUsuario(sala.CodigoSala, token);
                     if (usu != null)
                     {
                         ImagenesUsuario.Add(e.Key, usu.Imagen);
                     }
                     else
                     {

                         Protectora pro = await this.service.BuscarProtectora(sala.CodigoCuenta, token);

                         if (pro != null)
                         {

                             ImagenesProtectora.Add(e.Key, pro.Imagen);
                         }
                         else
                         {

                             pro = await this.service.BuscarProtectora(sala.CodigoSala, token);

                             ImagenesProtectora.Add(e.Key, pro.Imagen);
                         }
                     }

                 }
                 else {

                     if (Regex.IsMatch(e.Key, @"^([0-9]{8}[A-Z])|[XYZ][0-9]{7}[A-Z]$"))
                     {

                         sala = await this.service.GetCodigoSalaPrincipal(e.Key, codigoE, token);
                     }
                     else
                     {

                         sala = await this.service.GetCodigoSalaPrincipal(e.Key, codigoE, token);
                     }

                     if (sala != null) {

                         Usuario usu = await this.service.BuscarUsuario(sala.CodigoCuenta, token);
                         if (usu != null)
                         {
                             ImagenesUsuario.Add(e.Key, usu.Imagen);
                         }
                         else
                         {

                             Protectora pro = await this.service.BuscarProtectora(sala.CodigoCuenta, token);

                             if (pro != null)
                             {

                                 ImagenesProtectora.Add(e.Key, pro.Imagen);
                             }
                             else
                             {

                                 pro = await this.service.BuscarProtectora(sala.CodigoSala, token);

                                 ImagenesProtectora.Add(e.Key, pro.Imagen);
                             }

                         }
                     }
                 }
             }

             ViewData["ImagenesProtectora"] = ImagenesProtectora;
             ViewData["ImagenesUsuario"] = ImagenesUsuario;

             return View(rooms);
         }*/


        /*Esta vista muestra cada chat con sus mensajes previos guardados en la BBDD*/
        /* [AuthorizeCuentas]
         public async Task<IActionResult> Room(string room) {

             string token = HttpContext.User.FindFirst("TOKEN").Value;
             VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(HttpContext.User.FindFirst("Cuenta").Value);

             Chats CuentasEmisoras=await this.service.GetCodigoSalaPrincipal(cuenta.CodigoCuenta,room,token);

             if (CuentasEmisoras == null) { 

                 CuentasEmisoras= await this.service.GetCodigoSalaPrincipal(room, cuenta.CodigoCuenta, token);
             }

             Protectora ProtectoraSala = new Protectora();
             Usuario usu = new Usuario();

             if (CuentasEmisoras.CodigoCuenta == cuenta.CodigoCuenta)
             {

                 usu = await this.service.BuscarUsuario(CuentasEmisoras.CodigoSala, token);

                 if (usu == null)
                 {
                     ProtectoraSala = await this.service.BuscarProtectora(CuentasEmisoras.CodigoSala, token);

                 }

             }
             else if (CuentasEmisoras.CodigoSala == cuenta.CodigoCuenta)
             {
                 ProtectoraSala = await this.service.BuscarProtectora(CuentasEmisoras.CodigoCuenta, token);

                 if (ProtectoraSala == null)
                 {
                     usu = await this.service.BuscarUsuario(CuentasEmisoras.CodigoCuenta, token);

                 }

             }

             Chats codigoSala = await this.service.GetCodigoSalaPrincipal(CuentasEmisoras.CodigoCuenta, CuentasEmisoras.CodigoSala, token);

             //Significa que el usuario no es el que ha iniciado la conversación (y ha creado la sala),sino que le ha hablado otra persona
             if (codigoSala == null)
             {

                 codigoSala = await this.service.GetCodigoSalaPrincipal(CuentasEmisoras.CodigoSala, CuentasEmisoras.CodigoCuenta, token);
             }

             Chats sala = await this.service.BuscarChatPrincipal(codigoSala.CodigoChat, token);


             ViewBag.Sala = sala;
             ViewBag.Cuenta = cuenta;
             ViewBag.Protectora = ProtectoraSala;
             ViewBag.Usuario = usu;

             List<Chat> HistorialChat = await this.service.GetHistorialChat(sala.CodigoChat, token);
             ViewBag.HistorialChat = HistorialChat;

             return PartialView("Room");
         }
        */


        /*Insertar mensaje en el chat*/
        /* [AuthorizeCuentas]
         [HttpPost]
         public async Task<JsonResult> InsertarComentario(int idSala, string emisor, string receptor, string mensaje) {

             string token = HttpContext.User.FindFirst("TOKEN").Value;

             if (await this.service.InsertarMensaje(idSala, emisor, receptor, mensaje, token) == true)
             {

                 return Json("funciona");
             }
             else {

                 return Json("");
             }
         }*/
    }
}
