using Microsoft.EntityFrameworkCore;
using Validacao.Infrastructure.Services;

namespace Validacao.Infrastructure.Services;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Frete> TabelaFrete => Set<Frete>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        Console.WriteLine("===== MODELO EF CORE =====");

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            Console.WriteLine($"Nome: {entity.Name}");
            Console.WriteLine($"Tipo CLR: {entity.ClrType.FullName}");
            Console.WriteLine($"Assembly: {entity.ClrType.Assembly.GetName().Name}");

            foreach (var navigation in entity.GetNavigations())
            {
                Console.WriteLine(
                    $"Navegação: {navigation.Name} -> {navigation.TargetEntityType.Name}"
                );
            }
        }

        Console.WriteLine("==========================");
    }
}