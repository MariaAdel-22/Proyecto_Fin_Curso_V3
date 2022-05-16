using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProyectoAmazon.Models
{
    [Table("CHAT")]
    public class Chat
    {
        [Key]
        [Column("CHAT_COD")]
        public int CodigoChat { get; set; }

        [Column("CHATS_COD")]
        public int CodigoSalaChat { get; set; }

        [Column("CODIGO_EMISOR")]
        public string CodigoDeCuenta { get; set; }

        [Column("CODIGO_RECEPTOR")]
        public string CodigoPersonaEnviado { get; set; }

        [Column("MENSAJE")]
        public string Mensaje { get; set; }

        [Column("FECHA")]
        public DateTime Fecha { get; set; }
    }
}
