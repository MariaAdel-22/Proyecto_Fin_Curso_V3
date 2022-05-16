using ApiProyectoAmazon.Data;
using ApiProyectoAmazon.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace WebApiProyecto.Repositories
{
    public class RepositoryAdopcion
    {
        private AdoPetContext context;

        public RepositoryAdopcion(AdoPetContext context)
        {

            this.context = context;
        }

        public List<Donacion> GetDonaciones(int codigoProtectora) {

            return this.context.Donaciones.Where(x => x.CodigoProtectora == codigoProtectora).ToList();
        }

        public void InsertarDonacion(int codigoProtectora, int cantidad, string dni)
        {

            string sql = "INSERTAR_DONACION @PROTECTORA,@DNI,@CANTIDAD";

           SqlParameter paramPro = new SqlParameter("@PROTECTORA", codigoProtectora);
           SqlParameter paramCan = new SqlParameter("@CANTIDAD", cantidad);
           SqlParameter paramDni = new SqlParameter("@DNI", dni);

            this.context.Database.ExecuteSqlRaw(sql, paramPro, paramCan, paramDni);
        }
    }
}
