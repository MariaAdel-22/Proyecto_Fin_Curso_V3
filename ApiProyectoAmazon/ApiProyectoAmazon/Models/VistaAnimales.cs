using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProyectoAmazon.Models
{
    [Table("DATOS_ANIMALES_OPTIONS")]
    public class VistaAnimales
    {
        [Key]
        [Column("ANIMAL_COD")]
        public int IdAnimal { get; set; }

        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Column("EDAD")]
        public string Edad { get; set; }

        [Column("ESPECIE")]
        public string Especie{ get; set; }

        [Column("TAMAÑO")]
        public string Peso { get; set; }

        [Column("IMAGEN")]
        public string Imagen { get; set; }

        [Column("CIUDAD")]
        public string Ciudad { get; set; }
    }
}
