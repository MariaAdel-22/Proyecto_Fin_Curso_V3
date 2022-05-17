using ApiProyectoAmazon.Data;
using ApiProyectoAmazon.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ApiProyectoAmazon.Repositories
{
    public class RepositoryAdopcion
    {
        private AdoPetContext context;
        private RepositoryInicio repoIni;
        
        public RepositoryAdopcion(AdoPetContext context, RepositoryInicio repoIni)
        {

            this.context = context;
            this.repoIni = repoIni;
        }

        public List<Donacion> GetDonaciones(int codigoProtectora) {

            return this.context.Donaciones.Where(x => x.CodigoProtectora == codigoProtectora).ToList();
        }

        private int GetMaxCodigoDonacion()
        {

            if (this.context.Donaciones.Count() == 0)
            {

                return 1;
            }
            else
            {

                var consulta = (from datos in this.context.Donaciones select datos.CodigoDonacion).Max();

                int idAn = consulta + 1;

                return idAn;
            }
        }
        public void InsertarDonacion(int codigoProtectora, int cantidad, string dni)
        {
           Usuario usu= this.repoIni.BuscarUsuario(dni);

            if (usu != null) {

                Donacion don = new Donacion
                {
                    CodigoDonacion = GetMaxCodigoDonacion(),
                    CodigoProtectora = codigoProtectora,
                    Cantidad = cantidad,
                    Dni=dni,
                    ImagenDonante=usu.Imagen
                };

                this.context.Donaciones.Add(don);
                this.context.SaveChanges();
            }
        }
    }
}
