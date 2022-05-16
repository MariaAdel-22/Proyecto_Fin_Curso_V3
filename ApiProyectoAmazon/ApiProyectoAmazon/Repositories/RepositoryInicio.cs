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
    public class RepositoryInicio
    {
        private AdoPetContext context;
        private RepositoryProtectoras repoPro;

        public RepositoryInicio(AdoPetContext context, RepositoryProtectoras repo) {

            this.context = context;
            this.repoPro = repo;
        }


        public VistaCuentas BuscarCuenta(string nombre, string psswd) {

            VistaCuentas cuenta = this.context.Cuentas.Where(x => x.Nombre == nombre && x.Password == psswd).FirstOrDefault();

            return cuenta;
        }

        public VistaCuentas BuscarIdCuenta(string codigo) {

            VistaCuentas cuenta= this.context.Cuentas.FirstOrDefault(x => x.CodigoCuenta == codigo);

            if (cuenta == null)
            {

                return null;
            }
            else
            {

                if (cuenta.TipoCuenta == "USUARIOS")
                {

                    Usuario usu = this.context.Usuarios.FirstOrDefault(x => x.Dni == cuenta.CodigoCuenta);

                    cuenta.usuario = usu;
                    cuenta.protectora = null;

                }
                else if (cuenta.TipoCuenta == "PROTECTORAS")
                {

                    Protectora pro = this.context.Protectoras.FirstOrDefault(x => x.IdProtectora == int.Parse(cuenta.CodigoCuenta));

                    cuenta.protectora = pro;
                    cuenta.usuario = null;
                }

            }

            return cuenta;
        }

        public Usuario BuscarUsuario(string dni) {

            return this.context.Usuarios.Where(x => x.Dni == dni).FirstOrDefault();
        }

        public void InsertarUsuario(string Dni, string Nombre, string Apellidos,string Telefono,string Ciudad,string NombreUsuario,
            string Password,string Imagen)
        {

            string sql = "INSERTAR_USUARIO @DNI,@NOMBRE,@APELLIDOS,@TELEFONO,@CIUDAD,@NOMBRE_USUARIO,@PASSWORD,@IMAGEN";

            SqlParameter paramDni = new SqlParameter("@DNI", Dni);
            SqlParameter paramNom = new SqlParameter("@NOMBRE", Nombre);
            SqlParameter paramApel = new SqlParameter("@APELLIDOS", Apellidos);
            SqlParameter paramTel = new SqlParameter("@TELEFONO", Telefono);

            SqlParameter paramCiud = new SqlParameter("@CIUDAD", Ciudad);
            SqlParameter paramNomUs = new SqlParameter("@NOMBRE_USUARIO", NombreUsuario);
            SqlParameter paramPas = new SqlParameter("@PASSWORD", Password);
            SqlParameter paramImag = new SqlParameter("@IMAGEN", Imagen);

            this.context.Database.ExecuteSqlRaw(sql, paramDni, paramNom, paramApel, paramTel, paramCiud, paramNomUs, paramPas, paramImag);
        }

        public void ModificarUsuario(string Dni, string Nombre, string Apellidos, string Telefono, string Ciudad, string NombreUsuario,
            string Password, string Imagen) {

            Usuario usu = this.BuscarUsuario(Dni);

            if (usu != null) {

                usu.Nombre = Nombre;
                usu.Apellidos = Apellidos;
                usu.Telefono = Telefono;
                usu.Ciudad = Ciudad;
                usu.NombreUsuario = NombreUsuario;
                usu.Password = Password;
                usu.Imagen = Imagen;
            }

            this.context.SaveChanges();
        }

        public void InsertarProtectora(string Nombre, string Direccion, string Ciudad, string Telefono, string Tarjeta,string Paypal,string Password,string Imagen)
        {

            string sql = "INSERTAR_PROTECTORA @NOMBRE,@DIRECCION,@CIUDAD,@TELEFONO,@TARJETA,@PAYPAL,@PASSWORD,@IMAGEN";

            SqlParameter paramNom = new SqlParameter("@NOMBRE", Nombre);
            SqlParameter paramDirec = new SqlParameter("@DIRECCION", Direccion);
            SqlParameter paramCiu = new SqlParameter("@CIUDAD", Ciudad);
            SqlParameter paramTel = new SqlParameter("@TELEFONO", Telefono);

            SqlParameter paramTar = new SqlParameter("@TARJETA", Tarjeta);
            SqlParameter paramPay = new SqlParameter("@PAYPAL", Paypal);
            SqlParameter paramPas = new SqlParameter("@PASSWORD",Password);
            SqlParameter paramImag = new SqlParameter("@IMAGEN", Imagen);

            this.context.Database.ExecuteSqlRaw(sql, paramNom, paramDirec, paramCiu, paramTel, paramTar, paramPay, paramPas, paramImag);
        }

        public void ModificarProtectora(int id, string Nombre, string Direccion, string Ciudad, string Telefono, string Tarjeta, string Paypal, string Password, string Imagen) {

            Protectora pro = this.repoPro.FindProtectora(id.ToString());

            if (pro != null) {

                pro.Nombre = Nombre;
                pro.Direccion = Direccion;
                pro.Ciudad = Ciudad;
                pro.Telefono = Telefono;
                pro.Tarjeta = Tarjeta;
                pro.Paypal = Paypal;
                pro.Password = Password;
                pro.Imagen = Imagen;
            }

            this.context.SaveChanges();
        }

        public int NumeroAnimalesRegistrados()
        {
            return this.context.Adopciones.Count();
        }

        public int NumeroProtectorasRegistradas()
        {
            return this.context.Protectoras.Count();
        }

        public int NumeroUsuariosRegistrados()
        {
            return this.context.Usuarios.Count();
        }


    }
}
