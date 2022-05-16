using Microsoft.EntityFrameworkCore;
using ApiProyectoAmazon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProyectoAmazon.Data
{
	public class AdoPetContext : DbContext
	{
		public AdoPetContext(DbContextOptions<AdoPetContext> option) : base(option)
		{
		}

		/*Las colecciones*/
		public DbSet<Usuario> Usuarios { get; set; }
		public DbSet<Protectora> Protectoras { get; set; }
		public DbSet<Animal> Animales { get; set; }
		public DbSet<VistaCuentas> Cuentas { get; set; }
		public DbSet<VistaAnimales> VistaAnim { get; set; }
		public DbSet<Comentarios> Comentarios { get; set; }
		public DbSet<Adopciones> Adopciones { get; set; }
		public DbSet<VistaProtectoras> VistaProt { get; set; }
		public DbSet<Chats> Chats { get; set; }
		public DbSet<Chat> Chat { get; set; }
		public DbSet<Donacion> Donaciones { get; set; }
		public DbSet<Favoritos> Favoritos { get; set; }
	}
}
