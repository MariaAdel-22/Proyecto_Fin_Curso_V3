using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGetAdoPet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_AdoPet.Services
{
    public class ServiceApiAdopet
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue Header;

        public ServiceApiAdopet(string urlapi)
        {
            this.UrlApi = urlapi;
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
        }

        public async Task<string> GetTokenAsync(string nombre, string psswd)
        {
            using (HttpClient client = new HttpClient())
            {
                //client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    Nombre = nombre,
                    Password = psswd
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content =new StringContent(json, Encoding.UTF8, "application/json");

                string request = "/api/inicio/validarcuenta";
                HttpResponseMessage response =await client.PostAsync(this.UrlApi+request, content);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(data);
                    string token = jObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
               // client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                    await client.GetAsync(this.UrlApi+request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                //client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(this.UrlApi+request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }

            }
        }

        public async Task<VistaCuentas> BuscarCuenta(string token)
        {

            string request = "/api/inicio/buscarcuenta";

            using (HttpClient client = new HttpClient())
            {
                //client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(this.UrlApi+request);
                if (response.IsSuccessStatusCode)
                {
                    string alumnos = await response.Content.ReadAsStringAsync();

                    VistaCuentas cuent = JsonConvert.DeserializeObject<VistaCuentas>(alumnos);

                    return cuent;
                }
                else
                {
                    return null;
                }

            }

        }
        public async Task<int> CantidadAnimalesRegistrados(string token) {

            string request = "/api/Inicio/CantidadAnimalesRegistrados";

            int cantidad = await this.CallApiAsync<int>(request,token);

            return cantidad;
        }

        public async Task<int> CantidadProtectorasRegistradas(string token)
        {

            string request = "/api/Inicio/CantidadProtectorasRegistradas";

            int cantidad = await this.CallApiAsync<int>(request, token);

            return cantidad;
        }

        public async Task<int> CantidadUsuariosRegistrados(string token)
        {

            string request = "/api/Inicio/CantidadUsuariosRegistrados";

            int cantidad = await this.CallApiAsync<int>(request, token);

            return cantidad;
        }

        public async Task<Usuario> BuscarUsuario(string dni,string token) {

            string request = "/api/inicio/buscarusuario/" + dni;

            Usuario usu = await this.CallApiAsync<Usuario>(request, token);

            return usu;
        }

        public async Task<Boolean> InsertarUsuario(string dni,string nombre,string apellidos,string telefono,string ciudad, string nombreUsuario, string password,string imagen)
        {
            Boolean procesoTerminado = false;

            using (HttpClient client = new HttpClient())
            {

                //client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string request = "/api/inicio/insertarusuario";

                Usuario usu = new Usuario { Dni = dni, Nombre = nombre, Apellidos = apellidos, Telefono = telefono, Ciudad = ciudad, NombreUsuario = nombreUsuario, Password = password, Imagen = imagen };

                string json = JsonConvert.SerializeObject(usu);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(this.UrlApi+request, content);

                if (response.IsSuccessStatusCode)
                {

                    procesoTerminado = true;
                }
                else {

                    procesoTerminado = false;
                }

                return procesoTerminado;
            }

        }

        public async Task<Boolean> ModificarUsuario(string dni, string nombre, string apellidos, string telefono, string ciudad, string nombreUsuario, string password, string imagen,string token) {

           
            using (HttpClient client = new HttpClient())
            {

                //client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/Inicio/ModificarUsuario";

                Usuario usu = await this.BuscarUsuario(dni, token);

                if (usu != null)
                {

                    usu.Nombre = nombre;
                    usu.Apellidos = apellidos;
                    usu.Telefono = telefono;
                    usu.Ciudad = ciudad;
                    usu.NombreUsuario = nombreUsuario;
                    usu.Password = password;
                    usu.Imagen = imagen;
                }

                string json = JsonConvert.SerializeObject(usu);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(this.UrlApi+request, content);

                if (response.IsSuccessStatusCode)
                {

                    return true;
                }
                else
                {

                     return false;
                }

            }
        }

        public async Task<Boolean> InsertarProtectora(string Nombre, string Direccion, string Ciudad, string Telefono, string Tarjeta, string Paypal, string Password, string Imagen) {

            Boolean procesoTerminado = false;

            using (HttpClient client = new HttpClient())
            {

                //client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string request = "/api/inicio/insertarprotectora";

                Protectora pro = new Protectora { Nombre = Nombre, Direccion = Direccion, Ciudad = Ciudad, Telefono = Telefono, Tarjeta = Tarjeta, Paypal = Paypal, Password = Password, Imagen = Imagen };

                string json = JsonConvert.SerializeObject(pro);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(this.UrlApi+request, content);

                if (response.IsSuccessStatusCode)
                {

                    procesoTerminado = true;
                }
                else
                {

                    procesoTerminado = false;
                }

                return procesoTerminado;
            }

        }

        public async Task<Boolean> ModificarProtectora(int codigo,string Nombre, string Direccion, string Ciudad, string Telefono, string Tarjeta, string Paypal, string Password, string Imagen,string token) {

            using (HttpClient client = new HttpClient())
            {

                //client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/inicio/modificarprotectora";

                Protectora pro = await this.BuscarProtectora(codigo.ToString(), token);

                if (pro!= null) {

                    pro.Nombre = Nombre;
                    pro.Direccion = Direccion;
                    pro.Ciudad = Ciudad;
                    pro.Telefono = Telefono;
                    pro.Tarjeta = Tarjeta;
                    pro.Paypal = Paypal;
                    pro.Password = Password;
                    pro.Imagen = Imagen;

                }

                string json = JsonConvert.SerializeObject(pro);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(this.UrlApi+request, content);

                if (response.IsSuccessStatusCode)
                {

                    return true;
                }
                else
                {

                    return false;
                }

            }
        }

        public async Task<List<Protectora>> GetProtectoras(string ciudad, string token) {

            string request = "/api/protectoras/getprotectoras/" + ciudad;

            List<Protectora> protectoras = await this.CallApiAsync<List<Protectora>>(request, token);

            return protectoras;
        }

        public async Task<List<int>> GetNumeroProtectoras(string ciudad,string token) {

            string request = "/api/protectoras/getnumeroprotectoras/" + ciudad;

            List<int> cantidad = await this.CallApiAsync<List<int>>(request,token);

            return cantidad;
        }

        public async Task<List<Animal>> GetAnimalesProtectora(int codigoProtectora,string token) {

            string request = "/api/protectoras/getanimalesprotectora/" + codigoProtectora;

            List<Animal> animales = await this.CallApiAsync<List<Animal>>(request, token);

            return animales;
        }

        public async Task<Protectora> BuscarProtectora(string codigoProtectora,string token) {

            string request = "/api/protectoras/findprotectora/" + codigoProtectora;

            Protectora pro = await this.CallApiAsync<Protectora>(request, token);

            return pro;
        }

        public async Task<Animal> BuscarAnimal(int codigoAnimal,string token) {

            string request = "/api/protectoras/findanimal/" + codigoAnimal;

            Animal an = await this.CallApiAsync<Animal>(request, token);

            return an;
        }

        public async Task<Boolean> InsertarAnimal(string imagenAnimal, string nombre, string edad, string genero, string peso, string especie,string token) {

            Boolean procesoTerminado = false;

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/protectoras/insertaranimal";

                Animal an = new Animal { CodigoAnimal = 0, Nombre = nombre, Edad = edad, Genero = genero, Peso = peso, Especie = especie,Imagen=imagenAnimal };
                string json = JsonConvert.SerializeObject(an);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);

                if (response.IsSuccessStatusCode)
                {

                    procesoTerminado = true;
                }
                else
                {

                    procesoTerminado = false;
                }

                return procesoTerminado;
            }
        }

        public async Task<Boolean> ModificarAnimal(int codigoAnimal, string Imagen, string Nombre, string Edad, string Genero, string Peso, string Especie, string token)
        {

            Boolean procesoTerminado = false;

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/protectoras/modificaranimal";

                Animal an = new Animal { CodigoAnimal = codigoAnimal, Nombre = Nombre, Imagen = Imagen, Edad = Edad, Genero = Genero, Peso = Peso, Especie = Especie };
                string json = JsonConvert.SerializeObject(an);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(request, content);

                if (response.IsSuccessStatusCode)
                {

                    procesoTerminado = true;
                }
                else
                {

                    procesoTerminado = false;
                }

                return procesoTerminado;
            }
        }

        public async Task<Boolean> EliminarAnimal(int idAnimal,string token) {

            Boolean procesoTerminado = false;

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/protectoras/eliminaranimal/" + idAnimal;

                HttpResponseMessage response = await client.DeleteAsync(request);

                if (response.IsSuccessStatusCode)
                {

                    procesoTerminado = true;
                }
                else
                {

                    procesoTerminado = false;
                }

                return procesoTerminado;
            }
        }

        public async Task<List<string>> GetEdadesAnimales(string ciudad,string token) {

            string request = "/api/animal/edadesanimales/" + ciudad;

            List<string> edades =await this.CallApiAsync<List<string>>(request, token);

            return edades;
        }

        public async Task<List<string>> GetEspeciesAnimales(string ciudad, string token) {

            string request = "/api/animal/especieanimales/" + ciudad;

            List<string> especies = await this.CallApiAsync<List<string>>(request, token);

            return especies;
        }

        public async Task<List<string>> GetTamaniosAnimales(string ciudad, string token) {

            string request = "/api/animal/tamanioanimales/" + ciudad;

            List<string> tamanios = await this.CallApiAsync<List<string>>(request, token);

            return tamanios;
        }

        public async Task<List<VistaAnimales>> GetAnimalesCiudad(string ciudad, string token) {

            string request = "/api/animal/animalesporciudad/" + ciudad;

            List<VistaAnimales> animales = await this.CallApiAsync<List<VistaAnimales>>(request, token);

            return animales;
        }

        public async Task<List<VistaAnimales>> BuscarAnimalesFiltro(string ciudad,string especie,string edad,string tamanio, string token)
        {

            string request = "/api/animal/buscaranimales/" +ciudad+"/"+especie+"/"+edad+"/"+tamanio;

            List<VistaAnimales> animales = await this.CallApiAsync<List<VistaAnimales>>(request, token);

            return animales;
        }

        public async Task<List<Comentarios>> GetComentarios(int idAnimal,string token) {

            string request = "/api/animal/comentariosporanimal/" + idAnimal;

            List<Comentarios> comentarios = await this.CallApiAsync<List<Comentarios>>(request, token);

            return comentarios;
        }

        public async Task<Comentarios> FindComentario(int idComentario,string token) {

            string request = "/api/animal/findcomentario/"+idComentario;

            Comentarios com = await this.CallApiAsync<Comentarios>(request, token);

            return com;
        }


        //Insertar Comentario dependerá de un Flow 
        public async Task<Boolean> InsertarComentario(int idanimal,string codigo,string mensaje,string tipoCuenta,string email, string token) {

            string urlComentarios = "https://prod-228.westeurope.logic.azure.com:443/workflows/05a0b9d37dce4499a1cea01b63e83742/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=G71PWAxGkHu20IsFtlyf3HWplMMQsaGw0QOx0iyt0c8";

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                //client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                FlowComentarioModel comentarioModel = new FlowComentarioModel { comentario = mensaje, CodigoPersona = codigo, email = email, tipoCuenta = tipoCuenta, idAnimal = idanimal };

                string json = JsonConvert.SerializeObject(comentarioModel);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(urlComentarios, content);

                if (response.IsSuccessStatusCode)
                {

                    return true;
                }
                else {

                    return false;
                }
            }


        }

        public async Task<Comentarios> ModificarComentario(int idComentario, string Comentario,string token) {

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/animal/modificarcomentario";

                Comentarios com =await this.FindComentario(idComentario, token);

                if (com!=null) {

                    com.Mensaje = Comentario;
                }
                

                string json = JsonConvert.SerializeObject(com);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(request, content);

                if (response.IsSuccessStatusCode)
                {

                    return com;
                }
                else {

                    return null;
                }
               
            }
        }

        public async Task<Boolean> EliminarComentario(int idComentario,string token) {

            Boolean procesoTerminado = false;

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/animal/eliminarcomentario/" + idComentario;

                HttpResponseMessage response = await client.DeleteAsync(request);

                if (response.IsSuccessStatusCode)
                {

                    procesoTerminado = true;
                }
                else
                {

                    procesoTerminado = false;
                }

                return procesoTerminado;
            }
        }

        public async Task<List<int>> GetFavoritosUsuario(string dni,string token) {

            string request = "/api/animal/animalesfavoritos/" + dni;

            List<int> favoritos = await this.CallApiAsync<List<int>>(request, token);

            return favoritos;
        }

        public async Task InsertarFavorito(int codigoAnimal,string dni, string token) {

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/animal/insertarfavorito";

                Favoritos fav = new Favoritos { CodigoAnimal = codigoAnimal, Dni = dni, CodigoFavorito = 0 };
                
                string json = JsonConvert.SerializeObject(fav);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);

            }
        }

        public async Task<Boolean> EliminarFavorito(int codigoAnimal,string codigo,string token) {

            Boolean procesoTerminado = false;

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/animal/eliminarfavorito/"+codigoAnimal+"/"+codigo;

                HttpResponseMessage response = await client.DeleteAsync(request);

                if (response.IsSuccessStatusCode)
                {

                    procesoTerminado = true;
                }
                else
                {

                    procesoTerminado = false;
                }

                return procesoTerminado;
            }
        }

        public async Task<List<Donacion>> GetDonacionesProtectora(int codigoProtectora,string token) {

            string request = "/api/donacion/getdonaciones/" + codigoProtectora;

            List<Donacion> donaciones = await this.CallApiAsync<List<Donacion>>(request,token);

            return donaciones;
        }

        public async Task<Boolean> InsertarDonacion(int idProtectora, int cantidad,string dni,string imagen,string token) {

            

            Boolean procesoTerminado = false;

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/donacion/insertardonacion";

                Donacion don = new Donacion { CodigoProtectora = idProtectora, Dni = dni, Cantidad = cantidad, CodigoDonacion = 0, ImagenDonante = imagen };

                string json = JsonConvert.SerializeObject(don);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);

                if (response.IsSuccessStatusCode)
                {

                    procesoTerminado = true;
                }
                else
                {

                    procesoTerminado = false;
                }

                return procesoTerminado;
            }
        }

        public async Task<List<string>> BuscarChat(string emisor, string receptor, string token) {

            string request = "/api/Chat/BuscarChat/" + emisor + "/" + receptor;

            List<string> chats = await this.CallApiAsync<List<string>>(request, token);

            return chats;
        }

        public async Task<List<string>> GetChats(string codigocuenta,string token) {

            string request = "/api/Chat/GetChats/" + codigocuenta;

            List<string> chats = await this.CallApiAsync<List<string>>(request, token);

            return chats;
        }

        public async Task<Boolean> CrearSalaChat(string codigo,string receptor,string token) {

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/Chat/CrearSalaChat";

                Chats sala = new Chats { CodigoChat = 0, CodigoCuenta = codigo, CodigoSala = receptor };

                string json = JsonConvert.SerializeObject(sala);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);

                if (response.IsSuccessStatusCode)
                {

                    return true;
                }
                else
                {

                    return false;
                }
            }

        }

        public async Task<string> GetEmisorDeChatVacio(string emisor,string token) {

            string request = "/api/Chat/EmisorChatVacio/" + emisor;

            string respuesta = await this.CallApiAsync<string>(request,token);

            return respuesta;
        }

        public async Task<Chats> GetEmisorChats(string sala,string token) {

            string request = "/api/Chat/EmisorChats/" + sala;

            Chats Sala = await this.CallApiAsync<Chats>(request, token);

            return Sala;
        }


        public async Task<Chats> BuscarChatEmisorReceptor(string emisor,string receptor,string token) {

            string request = "/api/Chat/BuscarChatEmisorReceptor/" + emisor + "/" + receptor;

            Chats chat = await this.CallApiAsync<Chats>(request, token);

            return chat;
        }

        public async Task<string> GetNombreProtectora(int codigo,string token) {

            string request = "/api/Chat/GetNombreProtectora/" + codigo;

            string nombre = await this.CallApiAsync<string>(request, token);

            return nombre;
        }

        public async Task<string> GetNombreUsuario(string codigo, string token)
        {

            string request = "/api/Chat/GetNombreUsuario/" + codigo;

            string nombre = await this.CallApiAsync<string>(request, token);

            return nombre;
        }

        public async Task<Chats> GetCodigoSalaPrincipal(string emisor,string receptor,string token) {

            string request = "/api/Chat/CodigoSalaPrincipal/" + emisor + "/" + receptor;

            Chats salaPrincipal = await this.CallApiAsync<Chats>(request, token);

            return salaPrincipal;
        }

        public async Task<Chats> BuscarChatPrincipal(int codigo,string token) {

            string request = "/api/Chat/BuscarChatPrincipal/" + codigo;

            Chats sala = await this.CallApiAsync<Chats>(request, token);

            return sala;
        }

        public async Task<List<Chat>> GetHistorialChat(int sala,string token) {

            string request = "/api/Chat/GetHistorialChat/" + sala;

            List<Chat> chats = await this.CallApiAsync<List<Chat>>(request, token);

            return chats;
        }

        public async Task<Boolean> InsertarMensaje(int idSala, string emisor, string receptor, string mensaje,string token) {

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string request = "/api/Chat/InsertarMensajeChat";

                Chat chat = new Chat { CodigoSalaChat = idSala, CodigoChat = 0, CodigoDeCuenta = emisor, CodigoPersonaEnviado = receptor, Mensaje = mensaje };

                string json = JsonConvert.SerializeObject(chat);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);

                if (response.IsSuccessStatusCode)
                {

                    return true;
                }
                else
                {

                    return false;
                }
            }
        }
    }
}
