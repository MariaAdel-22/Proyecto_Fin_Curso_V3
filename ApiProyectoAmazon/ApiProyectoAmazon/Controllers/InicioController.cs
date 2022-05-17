using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ApiProyectoAmazon.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiProyectoAmazon.Repositories;
using ApiProyectoAmazon.Helpers;

namespace ApiProyectoAmazon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InicioController : ControllerBase
    {
        private HelperOAuthToken helper;
        private RepositoryInicio repoInicio;

        public InicioController(RepositoryInicio repositoryInicio, HelperOAuthToken helper) {

            this.repoInicio = repositoryInicio;
            this.helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public ActionResult ValidarCuenta(LoginModel model)
        {

            VistaCuentas cuenta = this.repoInicio.BuscarCuenta(model.Nombre, model.Password);

            if (cuenta == null)
            {
                return Unauthorized();
            }
            else
            {

                SigningCredentials credentials =
                    new SigningCredentials(this.helper.GetKeyToken(), SecurityAlgorithms.HmacSha256);

                string json = JsonConvert.SerializeObject(cuenta);
                Claim[] claims = new[]
                {
                new Claim("UserData", json)
                };


                JwtSecurityToken token =
                    new JwtSecurityToken(
                        claims: claims,
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        notBefore: DateTime.UtcNow
                        );

                return Ok(
                    new
                    {
                        response =
                        new JwtSecurityTokenHandler().WriteToken(token)
                    });
            }
        }

        //METODO PARA BUSCAR LA CUENTA CON LA QUE INICIAMOS SESION
        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public ActionResult<VistaCuentas> BuscarCuenta()
        {

            List<Claim> claims = HttpContext.User.Claims.ToList();

            string json = claims.SingleOrDefault(z => z.Type == "UserData").Value;

            VistaCuentas cuenta = JsonConvert.DeserializeObject<VistaCuentas>(json);

            return this.repoInicio.BuscarIdCuenta(cuenta.CodigoCuenta);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{dni}")]
        public ActionResult<Usuario> BuscarUsuario(string dni) {

            return this.repoInicio.BuscarUsuario(dni);
        }

        [HttpPost]
        [Route("[action]")]
        public ActionResult InsertarUsuario(Usuario usu)
        {

            this.repoInicio.InsertarUsuario(usu.Dni, usu.Nombre, usu.Apellidos, usu.Telefono, usu.Ciudad, usu.NombreUsuario, usu.Password, usu.Imagen);

            return Ok();
        }

        [HttpPut]
        [Authorize]
        [Route("[action]")]
        public ActionResult ModificarUsuario(Usuario usu) {

            this.repoInicio.ModificarUsuario(usu.Dni, usu.Nombre, usu.Apellidos, usu.Telefono, usu.Ciudad, usu.NombreUsuario, usu.Password, usu.Imagen);

            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public ActionResult InsertarProtectora(Protectora pro)
        {

            this.repoInicio.InsertarProtectora(pro.Nombre, pro.Direccion, pro.Ciudad, pro.Telefono, pro.Tarjeta, pro.Paypal, pro.Password, pro.Imagen);

            return Ok();
        }

        [HttpPut]
        [Authorize]
        [Route("[action]")]
        public ActionResult ModificarProtectora(Protectora pro) {

            this.repoInicio.ModificarProtectora(pro.IdProtectora, pro.Nombre, pro.Direccion, pro.Ciudad, pro.Telefono, pro.Tarjeta, pro.Paypal, pro.Password, pro.Imagen);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public ActionResult<int> CantidadAnimalesRegistrados()
        {

            return this.repoInicio.NumeroAnimalesRegistrados();
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public ActionResult<int> CantidadProtectorasRegistradas()
        {

            return this.repoInicio.NumeroProtectorasRegistradas();
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public ActionResult<int> CantidadUsuariosRegistrados()
        {

            return this.repoInicio.NumeroUsuariosRegistrados();
        }
    }
}
