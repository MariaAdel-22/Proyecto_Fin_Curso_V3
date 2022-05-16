using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProyectoAmazon.Models
{
    [Table("VISTA_PROTECTORAS")]
    public class VistaProtectoras
    {
        [Key]
        [Column("PROTECTORA_COD")]
        public int IdProtectora { get; set; }

        [Column("CANTIDAD_ANIMALES")]
        public int Cantidad_Animales {get;set;}
    }
}
