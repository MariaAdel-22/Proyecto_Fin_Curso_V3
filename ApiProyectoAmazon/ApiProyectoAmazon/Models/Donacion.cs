using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProyectoAmazon.Models
{
    [Table("DONACIONES")]
    public class Donacion
    {
        [Key]
        [Column("COD_DONACION")]
        public int CodigoDonacion { get; set; }

        [Column("PROTECTORA_COD")]
        public int CodigoProtectora { get; set; }

        [Column("DNI")]
        public string Dni { get; set; }

        [Column("CANTIDAD")]
        public int Cantidad { get; set; }

        [Column("IMAGEN")]
        public string ImagenDonante { get; set; }
    }
}
