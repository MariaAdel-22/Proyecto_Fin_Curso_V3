using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProyectoAmazon.Models
{
    [Serializable]
    [Table("CUENTAS_LOGIN")]
    public class VistaCuentas
    {
        [Key]
        [Column("CODIGO")]
        public string CodigoCuenta { get; set; }

        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Column("CONTRASEÑA")]
        public string Password { get; set; }

        [Column("TIPOCUENTA")]
        public string TipoCuenta { get; set; }

        [NotMapped]
        public Usuario usuario { get; set; }

        [NotMapped]
        public Protectora protectora { get; set; }
    }
}
