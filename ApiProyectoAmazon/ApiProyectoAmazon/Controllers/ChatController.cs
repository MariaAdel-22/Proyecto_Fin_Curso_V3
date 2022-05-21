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
    public class ChatController : ControllerBase
    {
        private RepositoryChat repoChat;

        public ChatController(RepositoryChat repo) {

            this.repoChat = repo;
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{emisor}/{receptor}")]
        public ActionResult<List<string>> BuscarChat(string emisor,string receptor) {

            return this.repoChat.FindChat(emisor, receptor);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{codigoSala}")]
        public ActionResult<Chats> GetEmisorChats(string codigoSala) {

            return this.repoChat.GetEmisorChats(codigoSala);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{codigocuenta}")]
        public ActionResult<List<string>> GetChats(string codigocuenta) {

            return this.repoChat.GetChats(codigocuenta);
        }

        [HttpPost]
        [Authorize]
        [Route("[action]")]
        public ActionResult CrearSalaChat(Chats chat) {

            this.repoChat.CrearSalaChat(chat.CodigoCuenta,chat.CodigoSala);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{emisor}")]
        public ActionResult<string> EmisorChatVacio(string emisor) {

            return this.repoChat.GetEmisorChatVacio(emisor);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{codigosala}")]
        public ActionResult<Chats> EmisorChats(string codigosala) {

            return this.repoChat.GetEmisorChats(codigosala);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{codigoprotectora}")]
        public ActionResult<string> GetNombreProtectora(int codigoprotectora) {

            return this.repoChat.GetNombreProtectora(codigoprotectora);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{codigo}")]
        public ActionResult<string> GetNombreUsuario(string codigo) {

            return this.repoChat.GetNombreUsuario(codigo);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{emisor}/{receptor}")]
        public ActionResult<Chats> CodigoSalaPrincipal(string emisor, string receptor) {

            return this.repoChat.GetCodigoSalaPrincipal(emisor, receptor);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{codigo}")]
        public ActionResult<Chats> BuscarChatPrincipal(int codigo) {

            return this.repoChat.FindChatPrincipal(codigo);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{codigosala}")]
        public ActionResult<List<Chat>> GetHistorialChat(int codigosala) {

            return this.repoChat.GetHistorialChat(codigosala);
        }

        [HttpPost]
        [Authorize]
        [Route("[action]")]
        public ActionResult InsertarMensajeChat(Chat chat) {

            this.repoChat.InsertarMensajeChat(chat.CodigoSalaChat,chat.CodigoDeCuenta,chat.CodigoPersonaEnviado,chat.Mensaje);

            return Ok();
        }
    }
}
