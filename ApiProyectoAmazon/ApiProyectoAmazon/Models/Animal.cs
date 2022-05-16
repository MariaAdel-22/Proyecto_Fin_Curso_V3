using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProyectoAmazon.Models
{
    [Table("ANIMALES")]
    public class Animal
    {
        [Key]
        [Column("ANIMAL_COD")]
        public int CodigoAnimal { get; set; }

        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Column("EDAD")]
        public string Edad { get; set; }

        [Column("GENERO")]
        public string Genero { get; set; }

        [Column("ESPECIE")]
        public string Especie { get; set; }

        [Column("PESO")]
        public string Peso { get; set; }

        [Column("IMAGEN")]
        public string Imagen { get; set; }
    }
}
