using Microsoft.EntityFrameworkCore;
using APIPedidos.Models;

namespace APIPedidos.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<PedidoItem> PedidoItens => Set<PedidoItem>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Produto 

            modelBuilder.Entity<Produto>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                entity.Property(p => p.Nome).HasMaxLength(200).IsRequired();
                entity.Property(p => p.Preco).HasPrecision(18, 2);
            });

            // Pedido

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                entity.Property(p => p.ValorTotal).HasPrecision(18, 2);

                entity.HasMany(p => p.Itens)
                      .WithOne(i => i.Pedido)
                      .HasForeignKey(i => i.PedidoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // PedidoItem

            modelBuilder.Entity<PedidoItem>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Id).ValueGeneratedOnAdd();
                entity.Property(i => i.PrecoUnitario).HasPrecision(18, 2);
                entity.Property(i => i.Quantidade).IsRequired();

                entity.HasOne(i => i.Pedido)
                      .WithMany(p => p.Itens)
                      .HasForeignKey(i => i.PedidoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(i => i.Produto)
                      .WithMany(p => p.Itens)
                      .HasForeignKey(i => i.ProdutoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            //  seed
            var prod1 = new Produto { Id = 1, Nome = "Caneca Personalizada", Preco = 59.90m };
            var prod2 = new Produto { Id = 2, Nome = "Camiseta Estampada", Preco = 79.90m };
            var prod3 = new Produto { Id = 3, Nome = "Chaveiro MDF", Preco = 19.90m };
            var prod4 = new Produto { Id = 4, Nome = "Quadro Decorativo", Preco = 129.90m };
            var prod5 = new Produto { Id = 5, Nome = "Adesivo Personalizado", Preco = 9.90m };

            modelBuilder.Entity<Produto>().HasData(prod1, prod2, prod3, prod4, prod5);

            var pedido1 = new Pedido { Id = 1, CriadoEm = DateTime.UtcNow, Fechado = false, ValorTotal = 119.80m };
            var pedido2 = new Pedido { Id = 2, CriadoEm = DateTime.UtcNow.AddDays(-1), Fechado = true, ValorTotal = 159.80m };
            var pedido3 = new Pedido { Id = 3, CriadoEm = DateTime.UtcNow.AddDays(-2), Fechado = false, ValorTotal = 59.70m };
            var pedido4 = new Pedido { Id = 4, CriadoEm = DateTime.UtcNow.AddDays(-3), Fechado = true, ValorTotal = 129.90m };
            var pedido5 = new Pedido { Id = 5, CriadoEm = DateTime.UtcNow.AddDays(-4), Fechado = false, ValorTotal = 69.80m };

            modelBuilder.Entity<Pedido>().HasData(pedido1, pedido2, pedido3, pedido4, pedido5);

            var item1 = new PedidoItem { Id = 1, PedidoId = 1, ProdutoId = 1, Quantidade = 2, PrecoUnitario = 59.90m };
            var item2 = new PedidoItem { Id = 2, PedidoId = 2, ProdutoId = 2, Quantidade = 2, PrecoUnitario = 79.90m };
            var item3 = new PedidoItem { Id = 3, PedidoId = 3, ProdutoId = 3, Quantidade = 3, PrecoUnitario = 19.90m };
            var item4 = new PedidoItem { Id = 4, PedidoId = 4, ProdutoId = 4, Quantidade = 1, PrecoUnitario = 129.90m };
            var item5 = new PedidoItem { Id = 5, PedidoId = 5, ProdutoId = 1, Quantidade = 1, PrecoUnitario = 59.90m };

            modelBuilder.Entity<PedidoItem>().HasData(item1, item2, item3, item4, item5);
        }
    }
}



