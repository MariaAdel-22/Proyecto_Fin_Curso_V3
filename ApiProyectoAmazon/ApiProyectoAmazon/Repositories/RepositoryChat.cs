using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ApiProyectoAmazon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiProyectoAmazon.Data;

namespace ApiProyectoAmazon.Repositories
{
    public class RepositoryChat
    {
        private AdoPetContext context;

        public RepositoryChat(AdoPetContext context)
        {

            this.context = context;
        }

        public List<string> FindChat(string emisor, string receptor)
        {

            var consulta = from datos in this.context.Chats where datos.CodigoCuenta == emisor && datos.CodigoSala == receptor select datos.CodigoChat.ToString();

            if (consulta == null)
            {

                consulta = from datos in this.context.Chats where datos.CodigoCuenta == receptor && datos.CodigoSala == emisor select datos.CodigoChat.ToString();

            }

            return consulta.ToList();

        }

        public List<string> GetChats(string codigoCuenta)
        {
            var consulta = from datos in this.context.Chats where datos.CodigoCuenta == codigoCuenta || datos.CodigoSala == codigoCuenta select datos.CodigoChat.ToString();

            return consulta.ToList();
        }

        private int GetMaxCodigoChats()
        {

            if (this.context.Chats.Count() == 0)
            {

                return 1;
            }
            else
            {

                var consulta = (from datos in this.context.Chats select datos.CodigoChat).Max();

                int idCh = consulta + 1;

                return idCh;
            }
        }

        public void CrearSalaChat(string emisor, string receptor)
        {
            Chats chat = new Chats
            {
                CodigoChat=this.GetMaxCodigoChats(),
                CodigoCuenta=emisor,
                CodigoSala=receptor
            };

            this.context.Chats.Add(chat);
            this.context.SaveChanges();
        }

        public string GetEmisorChatVacio(string emisor)
        {
            return this.context.Chats.Where(x => x.CodigoCuenta == emisor).Select(x => x.CodigoSala).FirstOrDefault();
        }

        public Chats GetEmisorChats(string codigoSala)
        {
            return this.context.Chats.Where(x => x.CodigoChat == int.Parse(codigoSala)).FirstOrDefault();
        }

        public string GetEmisorChat(string receptor)
        {
            return this.context.Chats.Where(x=>x.CodigoSala == receptor).Select(x=>x.CodigoCuenta).FirstOrDefault();
        }

        public String GetNombreProtectora(int codigoProtectora)
        {
            return this.context.Protectoras.Where(x => x.IdProtectora == codigoProtectora).Select(x => x.Nombre).FirstOrDefault();
        }

        public string GetNombreUsuario(string codigo)
        {
            return this.context.Usuarios.Where(x => x.Dni == codigo).Select(x => x.NombreUsuario).FirstOrDefault();
        }

        public Chats GetCodigoSalaPrincipal(string emisor, string receptor)
        {

            var consulta = from datos in this.context.Chats where datos.CodigoCuenta == emisor && datos.CodigoSala == receptor select datos;

            if (consulta == null) {

                consulta = from datos in this.context.Chats where datos.CodigoCuenta == receptor && datos.CodigoSala == emisor select datos;
            }

            return consulta.FirstOrDefault();
        }

        public Chats FindChatPrincipal(int codigo)
        {
            return this.context.Chats.Where(x => x.CodigoChat == codigo).FirstOrDefault();
        }

        public List<Chat> GetHistorialChat(int codigoSala)
        {
            return this.context.Chat.Where(x => x.CodigoSalaChat == codigoSala).ToList();
        }

        private int GetMaxCodigoChat()
        {

            if (this.context.Chat.Count() == 0)
            {

                return 1;
            }
            else
            {

                var consulta = (from datos in this.context.Chat select datos.CodigoChat).Max();

                int idCh = consulta + 1;

                return idCh;
            }
        }

        public void InsertarMensajeChat(int idSala, string emisor, string receptor, string mensaje)
        {

            Chat chat = new Chat { CodigoChat = GetMaxCodigoChat(), CodigoSalaChat = idSala, CodigoDeCuenta = emisor, CodigoPersonaEnviado = receptor,Mensaje = mensaje };

            this.context.Chat.Add(chat);
            this.context.SaveChanges();
        }
    }
}
