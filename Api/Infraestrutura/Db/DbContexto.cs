using Microsoft.EntityFrameworkCore;
using mininal_api.Dominio.Entidades;
using mininal_api.Dominio.Servicos;

namespace mininal_api.Infraestrutura.Db;

public class DbContexto : DbContext
{
    private readonly IUsuarioContextoServico? _usuarioContextoServico;

    public DbContexto() { }

    public DbContexto(DbContextOptions<DbContexto> options) : base(options)
    {
        // Construtor para testes com banco em memória
    }

    public DbContexto(DbContextOptions<DbContexto> options, IUsuarioContextoServico usuarioContextoServico) : base(options)
    {
        _usuarioContextoServico = usuarioContextoServico;
    }

    public DbSet<Administrador> Administradores { get; set; } = default!;
    public DbSet<Veiculo> Veiculos { get; set; } = default!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuração de auditoria automática
        modelBuilder.Entity<Administrador>()
            .Property(e => e.DataCriacao);

        modelBuilder.Entity<Veiculo>()
            .Property(e => e.DataCriacao);

        // Configuração de soft delete
        modelBuilder.Entity<Administrador>()
            .HasQueryFilter(e => e.Ativo);

        modelBuilder.Entity<Veiculo>()
            .HasQueryFilter(e => e.Ativo);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.Administrador)
            .WithMany()
            .HasForeignKey(rt => rt.AdministradorId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<RefreshToken>()
            .HasQueryFilter(rt => !rt.Invalido && rt.DataExpiracao > DateTime.UtcNow);

        // Seed data
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
            {
                Id = 1,
                Email = "administrador@teste.com",
                Senha = "$2a$12$TsNK82PWpq3KKHUxMfKXJ.uG0cTLFGDgZQDmXnp.Adsev9cUGcKGG", // 123456
                Perfil = "Adm",
                DataCriacao = DateTime.UtcNow,
                CriadoPor = "Sistema"
            }
        );

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Administrador || e.Entity is Veiculo)
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var usuarioEmail = _usuarioContextoServico?.ObterUsuarioEmail() ?? "Sistema";
            
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is Administrador adm)
                {
                    adm.DataCriacao = DateTime.UtcNow;
                    adm.CriadoPor = usuarioEmail;
                }
                else if (entry.Entity is Veiculo veiculo)
                {
                    veiculo.DataCriacao = DateTime.UtcNow;
                    veiculo.CriadoPor = usuarioEmail;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entry.Entity is Administrador adm)
                {
                    adm.DataAtualizacao = DateTime.UtcNow;
                    adm.AtualizadoPor = usuarioEmail;
                }
                else if (entry.Entity is Veiculo veiculo)
                {
                    veiculo.DataAtualizacao = DateTime.UtcNow;
                    veiculo.AtualizadoPor = usuarioEmail;
                }
            }
        }

        return base.SaveChanges();
    }
}