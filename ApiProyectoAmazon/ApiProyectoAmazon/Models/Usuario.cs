using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProyectoAmazon.Models
{
    [Table("USUARIOS")]
    public class Usuario
    {
        [Key]
        [Column("DNI")]
        public string Dni { get; set; }

        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Column("APELLIDOS")]
        public string Apellidos { get; set; }

        [Column("TELEFONO")]
        public string Telefono { get; set; }

        [Column("CIUDAD")]
        public string Ciudad { get; set; }

        [Column("NOMBRE_USUARIO")]
        public string NombreUsuario { get; set; }

        [Column("CONTRASEÑA")]
        public string Password { get; set; }

        [Column("IMAGEN")]
        public string Imagen { get; set; }
    }
}
