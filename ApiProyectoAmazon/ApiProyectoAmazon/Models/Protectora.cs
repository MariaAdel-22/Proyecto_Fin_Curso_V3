using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProyectoAmazon.Models
{
    [Table("PROTECTORAS")]
    public class Protectora
    {
        [Key]
        [Column("PROTECTORA_COD")]
        public int IdProtectora { get; set; }

        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Column("DIRECCION")]
        public string Direccion { get; set; }

        [Column("CIUDAD")]
        public string Ciudad { get; set; }

        [Column("TELEFONO")]
        public string Telefono { get; set; }

        [Column("TARJETA_CREDITO")]
        public string Tarjeta { get; set; }

        [Column("PAYPAL")]
        public string Paypal { get; set; }

        [Column("CONTRASEÑA")]
        public string Password { get; set; }

        [Column("IMAGEN")]
        public string Imagen { get; set; }
    }
}
