using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProyectoAmazon.Models
{
    [Table("COMENTARIOS")]
    public class Comentarios
    {
        [Key]
        [Column("MENSAJE_COD")]
        public int CodigoMensaje { get; set; }

        [Column("ANIMAL_COD")]
        public int CodigoAnimal { get; set; }

        [Column("CODIGO")]
        public string Codigo { get; set; }

        [Column("MENSAJE")]
        public string Mensaje { get; set; }

        [Column("FECHA")]
        public DateTime Fecha { get; set; }
    }
}
