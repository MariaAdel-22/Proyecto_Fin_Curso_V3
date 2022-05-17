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

        public Chats BuscarChat(string emisor,string receptor) {

            return this.context.Chats.Where(x => x.CodigoCuenta == emisor && x.CodigoSala == receptor).FirstOrDefault();
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
            // var consulta = from datos in this.context.Chats where datos.CodigoCuenta == emisor select datos.CodigoSala;
            //return consulta.FirstOrDefault();
            return this.context.Chats.Where(x => x.CodigoCuenta == emisor).Select(x => x.CodigoSala).FirstOrDefault();
        }

        public Chats GetEmisorChats(string codigoSala)
        {
            //var consulta = from datos in this.context.Chats where datos.CodigoChat == int.Parse(codigoSala) select datos;
            //return consulta.FirstOrDefault();

            return this.context.Chats.Where(x => x.CodigoChat == int.Parse(codigoSala)).FirstOrDefault();
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

            string sql = "FIND_SALA (@CODIGO_EMISOR,@CODIGO_RECEPTOR)";

            SqlParameter paramEmi = new SqlParameter("@CODIGO_EMISOR", emisor);
            SqlParameter paramRecep = new SqlParameter("@CODIGO_RECEPTOR", receptor);

            var consulta = this.context.Chats.FromSqlRaw(sql, paramEmi, paramRecep);

            return consulta.AsEnumerable().FirstOrDefault();
        }

        public Chats FindChatPrincipal(int codigo)
        {
            return this.context.Chats.Where(x => x.CodigoChat == codigo).FirstOrDefault();
        }

        public List<Chat> GetHistorialChat(int codigoSala)
        {
            string sql = "GET_HISTORIAL_CHATS (@CODIGOSALA)";

            SqlParameter pamCodSala = new SqlParameter("@CODIGOSALA", codigoSala);

            var consulta = this.context.Chat.FromSqlRaw(sql, pamCodSala);
            return consulta.ToList();
        }

        public void InsertarMensajeChat(int idSala, string emisor, string receptor, string mensaje)
        {
            string sql = "INSERTAR_MENSAJE_CHAT (@CHATS_COD,@CODIGO_EMISOR,@CODIGO_RECEPTOR,@MENSAJE)";

            SqlParameter pamCodSala = new SqlParameter("@CHATS_COD", idSala);
            SqlParameter pamEmi = new SqlParameter("@CODIGO_EMISOR", emisor);
            SqlParameter pamRec = new SqlParameter("@CODIGO_RECEPTOR", receptor);
            SqlParameter pamMen = new SqlParameter("@MENSAJE", mensaje);

            this.context.Database.ExecuteSqlRaw(sql, pamCodSala, pamEmi, pamRec, pamMen);
        }
    }
}
