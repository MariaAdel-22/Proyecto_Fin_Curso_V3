using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ApiProyectoAmazon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiProyectoAmazon.Data;


namespace ApiProyectoAmazon.Repositories
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

            VistaCuentas cuenta = this.context.Cuentas.FirstOrDefault(x => x.Nombre == nombre && x.Password == psswd);

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

            return this.context.Usuarios.FirstOrDefault(x => x.Dni == dni);
        }

        public void InsertarUsuario(string Dni, string Nombre, string Apellidos,string Telefono,string Ciudad,string NombreUsuario,
            string Password,string Imagen)
        {
            Usuario usu =new Usuario
            {
                Dni=Dni,
                Nombre=Nombre,
                Apellidos=Apellidos,
                Telefono=Telefono,
                Ciudad=Ciudad,
                NombreUsuario=NombreUsuario,
                Password=Password,
                Imagen=Imagen
            };

            this.context.Usuarios.Add(usu);
            this.context.SaveChanges();
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

        private int GetMaxCodigoProtectora()
        {
            if (this.context.Protectoras.Count() == 0)
            {
                return 1;
            }
            else
            {
                var consulta = (from datos in this.context.Protectoras select datos.IdProtectora).Max();

                int idCon = consulta + 1;

                return idCon;
            }
        }

        public void InsertarProtectora(string Nombre, string Direccion, string Ciudad, string Telefono, string Tarjeta,string Paypal,string Password,string Imagen)
        {
            Protectora pro = new Protectora
            {
                IdProtectora=GetMaxCodigoProtectora(),
                Nombre=Nombre,
                Direccion=Direccion,
                Ciudad=Ciudad,
                Telefono=Telefono,
                Tarjeta=Tarjeta,
                Paypal=Paypal,
                Password=Password,
                Imagen=Imagen
            };

            this.context.Protectoras.Add(pro);
            this.context.SaveChanges();
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
