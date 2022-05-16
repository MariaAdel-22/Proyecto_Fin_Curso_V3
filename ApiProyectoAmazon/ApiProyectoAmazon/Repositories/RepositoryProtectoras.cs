using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGetAdoPet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiProyecto.Data;

namespace WebApiProyecto.Repositories
{
    public class RepositoryProtectoras
    {
        private AdoPetContext context;

        public RepositoryProtectoras(AdoPetContext context)
        {

            this.context = context;
        }

        public List<Protectora> GetProtectoras(string ciudad)
        {
            return this.context.Protectoras.Where(x => x.Ciudad == ciudad).ToList();
        }

        public Protectora FindProtectora(string codigo)
        {
            return this.context.Protectoras.FirstOrDefault(x => x.IdProtectora.ToString() == codigo);
        }

        public List<int> GetNumeroProtectoras(string ciudad)
        {

            List<Protectora> protectoras = this.GetProtectoras(ciudad);

            List<int> Cantidades = new List<int>();

            foreach (Protectora pro in protectoras)
            {
                var consulta = from datos in this.context.VistaProt.AsEnumerable() where datos.IdProtectora == pro.IdProtectora select datos.Cantidad_Animales;

                Cantidades.Add(consulta.FirstOrDefault());
            }

            return Cantidades.ToList();
        }

        public List<Animal> GetAnimalesProtectora(int codigoProtectora)
        {

            List<int> CodigoAnimales = new List<int>();
            List<Animal> Animales = new List<Animal>();

            CodigoAnimales = this.context.Adopciones.Where(x => x.CodigoProtectora == codigoProtectora).Select(x => x.CodigoAnimal).ToList();

            foreach (int codigo in CodigoAnimales)
            {

                Animales.Add(this.context.Animales.Where(x => x.CodigoAnimal == codigo).FirstOrDefault());
            }

            return Animales;
        }

        private int GetMaxCodigoAnimal() {

            if (this.context.Animales.Count() == 0)
            {

                return 1;
            }
            else {

                var consulta = (from datos in this.context.Animales select datos.CodigoAnimal).Max();

                int idAn = consulta + 1;

                return idAn;
            }
        }

        public void InsertarAnimal(string codigoProtectora,string nombre,string edad, string genero,string especie,string peso,string imagen)
        {

            string sql = "INSERTAR_ANIMAL @CODIGO,@NOMBRE_ANIMAL,@EDAD,@GENERO,@ESPECIE,@PESO,@IMAGEN";

            SqlParameter paramCodg = new SqlParameter("@CODIGO",codigoProtectora);

            SqlParameter paramNom = new SqlParameter("@NOMBRE_ANIMAL", nombre);
            SqlParameter paramEd = new SqlParameter("@EDAD", edad);

            SqlParameter paramGen;

            if (genero.ToUpper() == "MACHO")
            {
                paramGen = new SqlParameter("@GENERO", "M");
            }
            else if (genero.ToUpper() == "HEMBRA")
            {

                paramGen = new SqlParameter("@GENERO", "F");

            }
            else
            {
                //Hermafrodita
                paramGen = new SqlParameter("@GENERO", "H");
            }

            SqlParameter paramEsp = new SqlParameter("@ESPECIE", especie);
            SqlParameter paramPes = new SqlParameter("@PESO", peso);
            SqlParameter paramImag = new SqlParameter("@IMAGEN", imagen);

            var consulta = this.context.Database.ExecuteSqlRaw(sql, paramCodg, paramNom, paramEd, paramGen, paramEsp, paramPes, paramImag);
        }

        public Animal BuscarAnimal(int codigoAnimal)
        {
            return this.context.Animales.Where(x => x.CodigoAnimal == codigoAnimal).FirstOrDefault();
        }

        public string BuscarImagenAnimal(int codigoAnimal)
        {
            return this.context.Animales.Where(x => x.CodigoAnimal == codigoAnimal).Select(x => x.Imagen).FirstOrDefault();
        }

        public void ModificarAnimal(int codigo, string nombre, string edad, string genero, string especie, string peso, string imagen) {

            Animal an = this.BuscarAnimal(codigo);

            if (an != null) {

                an.Nombre = nombre;
                an.Edad = edad;
                an.Genero = genero;
                an.Especie = especie;
                an.Peso = peso;
                an.Imagen = imagen;

                this.context.SaveChanges();
            }
        }

        public void EliminarAnimal(int codigoAnimal) {

            string sql = "SP_ELIMINAR_ANIMAL @CODIGOANIMAL";

            SqlParameter paramCodg = new SqlParameter("@CODIGOANIMAL", codigoAnimal);

            this.context.Database.ExecuteSqlRaw(sql, paramCodg);
        }
    }
}
