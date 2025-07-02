// Proyecto base de ejemplo para cumplir con la consigna CRUD de Clientes en Blazor Server

// 1. Entidad Cliente
namespace ClientesCrud.Data.Entities
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; }

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; }

        [Phone, MaxLength(15)]
        public string? Telefono { get; set; }

        [MaxLength(200)]
        public string? Direccion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;
    }
}

// 2. DTO para transferencias
namespace ClientesCrud.Data.Dtos
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
    }
}

// 3. DbContext
namespace ClientesCrud.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>().HasIndex(c => c.Email).IsUnique();
        }
    }
}

// 4. Servicio / Repositorio
namespace ClientesCrud.Data.Services
{
    public interface IClienteService
    {
        Task<List<ClienteDto>> ObtenerClientesActivosAsync();
        Task<ClienteDto?> ObtenerClientePorIdAsync(int id);
        Task CrearClienteAsync(ClienteDto cliente);
        Task EditarClienteAsync(ClienteDto cliente);
        Task EliminarClienteAsync(int id);
    }

    public class ClienteService : IClienteService
    {
        private readonly ApplicationDbContext _context;

        public ClienteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClienteDto>> ObtenerClientesActivosAsync()
        {
            return await _context.Clientes
                .Where(c => c.Activo)
                .Select(c => new ClienteDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Email = c.Email,
                    Telefono = c.Telefono,
                    Direccion = c.Direccion
                })
                .ToListAsync().ConfigureAwait(true);
        }

        public async Task<ClienteDto?> ObtenerClientePorIdAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id).ConfigureAwait(true);
            if (cliente == null || !cliente.Activo) return null;

            return new ClienteDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                Direccion = cliente.Direccion
            };
        }

        public async Task CrearClienteAsync(ClienteDto dto)
        {
            var cliente = new Cliente
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion
            };
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync().ConfigureAwait(true);
        }

        public async Task EditarClienteAsync(ClienteDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(dto.Id).ConfigureAwait(true);
            if (cliente == null || !cliente.Activo) return;

            cliente.Nombre = dto.Nombre;
            cliente.Email = dto.Email;
            cliente.Telefono = dto.Telefono;
            cliente.Direccion = dto.Direccion;
            await _context.SaveChangesAsync().ConfigureAwait(true);
        }

        public async Task EliminarClienteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id).ConfigureAwait(true);
            if (cliente == null || !cliente.Activo) return;

            cliente.Activo = false;
            await _context.SaveChangesAsync().ConfigureAwait(true);
        }
    }
}

// Las páginas Blazor y los componentes se crearán en la carpeta Pages/, incluyendo validaciones, navegación, autenticación (Identity), y mensajes de confirmación usando JS interop.
// También se integrará Bootstrap para responsividad.

// Se utilizarán migraciones con:
// dotnet ef migrations add Inicial
// dotnet ef database update

// La autenticación por cuentas individuales se configura en Program.cs al crear el proyecto Blazor Web App (Server).