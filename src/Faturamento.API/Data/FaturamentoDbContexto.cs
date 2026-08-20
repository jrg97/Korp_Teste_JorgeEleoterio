using Microsoft.EntityFrameworkCore;
using Faturamento.API.Models;

namespace Faturamento.API.Data;

public class FaturamentoDbContext : DbContext
{
    public FaturamentoDbContext(DbContextOptions<FaturamentoDbContext> options) : base(options){}

    public DbSet<NotaFiscal> NotasFiscais => Set<NotaFiscal>();
    public DbSet<ItemNotaFiscal> ItensNotaFiscal => Set<ItemNotaFiscal>();
}
