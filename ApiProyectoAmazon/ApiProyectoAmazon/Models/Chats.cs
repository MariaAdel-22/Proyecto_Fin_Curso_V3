using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProyectoAmazon.Models
{
    [Table("CHATS")]
    public class Chats
    {
        [Key]
        [Column("CHAT_COD")]
        public int CodigoChat { get; set; }

        [Column("CODIGO_EMISOR")]
        public string CodigoCuenta { get; set; }

        [Column("CODIGO_RECEPTOR")]
        public string CodigoSala { get; set; }
    }
}
