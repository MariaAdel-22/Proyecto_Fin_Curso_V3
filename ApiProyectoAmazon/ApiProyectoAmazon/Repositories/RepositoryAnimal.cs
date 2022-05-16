using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiProyectoAmazon.Models;
using ApiProyectoAmazon.Data;

namespace WebApiProyecto.Repositories
{
    public class RepositoryAnimal
    {
        private AdoPetContext context;

        public RepositoryAnimal(AdoPetContext context)
        {

            this.context = context;
        }

        //Traemos todos los datos de los animales para el filtro
        public List<VistaAnimales> GetAnimalesPorCiudad(string ciudad) {

            return this.context.VistaAnim.Where(x => x.Ciudad == ciudad).ToList();
        }

        public List<string> GetEdadAnimales(string ciudad) {

            var consulta = from datos in this.context.VistaAnim where datos.Ciudad == ciudad select datos;

             List<string> Edades = consulta.Select(x=>x.Edad).Distinct().ToList();

             return Edades;

        }

        public List<string> GetEspecieAnimales(string ciudad) {

            var consulta = from datos in this.context.VistaAnim where datos.Ciudad == ciudad select datos;

            List<string> especies = consulta.Select(x=>x.Especie).Distinct().ToList();

            return especies;
        }

        public List<string> GetTamanioAnimales(string ciudad)
        {

            var consulta = from datos in this.context.VistaAnim where datos.Ciudad == ciudad select datos;

            List<string> tamanios = consulta.Select(x=>x.Peso).Distinct().ToList();

            return tamanios;
        }


        public List<VistaAnimales> BuscarAnimales(string ciudad,string especie,string edad,string tamanio) {

            List<VistaAnimales> animales = new List<VistaAnimales>();

            if (especie.ToUpper() == "TODOS") {

                animales = this.context.VistaAnim.Where(x => x.Ciudad == ciudad && x.Edad == edad && x.Peso == tamanio).ToList();

            } else if (edad.ToUpper() == "TODOS") {

                animales = this.context.VistaAnim.Where(x => x.Ciudad == ciudad && x.Especie == especie && x.Peso == tamanio).ToList();

            } else if (tamanio.ToUpper() == "TODOS") {

                animales = this.context.VistaAnim.Where(x => x.Ciudad == ciudad && x.Especie == especie && x.Edad == edad).ToList();

            }

            //Condicion de dos o más respuestas que sean TODOS

            if (especie.ToUpper() == "TODOS" && edad.ToUpper() == "TODOS" && tamanio.ToUpper() == "TODOS") {

                animales = this.context.VistaAnim.Where(x => x.Ciudad == ciudad).ToList();

            } else if (especie.ToUpper() == "TODOS" && edad.ToUpper() == "TODOS") {

                animales = this.context.VistaAnim.Where(x => x.Ciudad == ciudad && x.Peso == tamanio).ToList();

            } else if (especie.ToUpper() == "TODOS" && tamanio.ToUpper() == "TODOS") {

                animales = this.context.VistaAnim.Where(x => x.Ciudad == ciudad && x.Edad == edad).ToList();

            } else if (edad.ToUpper() == "TODOS" && tamanio.ToUpper() == "TODOS") {

                animales = this.context.VistaAnim.Where(x => x.Ciudad == ciudad && x.Especie == especie).ToList();
            }

            return animales;
        }

        //COMENTARIOS

        private int GetMaxCodigoComentario()
        {

            if (this.context.Comentarios.Count() == 0)
            {

                return 1;
            }
            else
            {

                var consulta = (from datos in this.context.Comentarios select datos.CodigoMensaje).Max();

                int idCon = consulta + 1;

                return idCon;
            }
        }

        public void InsertarComentario(int idAnimal, string codigo, string comentario,DateTime fecha)
        {
            Comentarios com = new Comentarios();

            com.CodigoMensaje = this.GetMaxCodigoComentario();
            com.CodigoAnimal = idAnimal;
            com.Codigo = codigo;
            com.Mensaje = comentario;
            com.Fecha = fecha;

            this.context.Comentarios.Add(com);
            this.context.SaveChanges();
        }

        public List<Comentarios> FindComentariosAnimal(int idAnimal) {

            return this.context.Comentarios.Where(x => x.CodigoAnimal == idAnimal).OrderBy(x => x.Fecha).ToList();
        }

        public Comentarios FindComentario(int idComentario) {

            return this.context.Comentarios.Where(x => x.CodigoMensaje == idComentario).FirstOrDefault();
        }

        public Comentarios ModificarComentario(int idComentario, string mensaje)
        {
            string sql = "MODIFICAR_COMENTARIO @ID_COMENTARIOP,@COMENTARIOP";

            SqlParameter paramId = new SqlParameter("@ID_COMENTARIOP", idComentario);
            SqlParameter paramMensaje = new SqlParameter("@COMENTARIOP", mensaje);

            this.context.Database.ExecuteSqlRaw(sql, paramId, paramMensaje);

            return this.context.Comentarios.Where(x=> x.CodigoMensaje == idComentario).SingleOrDefault();
        }

        public void EliminarComentario(int idComentario)
        {
            Comentarios com = this.FindComentario(idComentario);

            this.context.Comentarios.Remove(com);
            this.context.SaveChanges();
        }

        //FAVORITOS

        public List<int> GetAnimalesFavoritos(string codigo) {

            return this.context.Favoritos.Where(x => x.Dni == codigo).Select(x => x.CodigoAnimal).ToList();
        }

        public void InsertarFavorito(int codigoAnimal, string dni)
        {
            string sql = "INSERTAR_FAVORITO @ANIMAL_CODP,@DNIP";

            SqlParameter pamAnimal = new SqlParameter("@ANIMAL_CODP", codigoAnimal);
            SqlParameter pamDni = new SqlParameter("@DNIP", dni);

            this.context.Database.ExecuteSqlRaw(sql, pamAnimal, pamDni);
        }

        public void EliminarFavorito(int codigoAnimal, string dni)
        {

            string sql = "ELIMINAR_FAVORITO @ANIMAL_CODP,@DNIP";

            SqlParameter pamAnimal = new SqlParameter("@ANIMAL_CODP", codigoAnimal);
            SqlParameter pamDni = new SqlParameter("@DNIP", dni);

            this.context.Database.ExecuteSqlRaw(sql, pamAnimal, pamDni);
        }
    }
}
