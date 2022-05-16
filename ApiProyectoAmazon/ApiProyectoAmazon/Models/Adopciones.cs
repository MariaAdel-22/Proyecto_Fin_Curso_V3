using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProyectoAmazon.Models
{
    [Table("ADOPCIONES")]
    public class Adopciones
    {
        [Key]
        [Column("ANIMAL_COD")]
        public int CodigoAnimal { get; set; }

        [Column("PROTECTORA_COD")]
        public int CodigoProtectora { get; set; }
    }
}
