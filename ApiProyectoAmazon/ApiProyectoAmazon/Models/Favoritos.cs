using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProyectoAmazon.Models
{
    [Table("FAVORITOS")]
    public class Favoritos
    {
        [Key]
        [Column("COD_FAVORITO")]
        public int CodigoFavorito { get; set; }

        [Column("ANIMAL_COD")]
        public int CodigoAnimal { get; set; }

        [Column("DNI")]
        public string Dni { get; set; }
    }
}
