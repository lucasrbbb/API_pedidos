using APIPedidos.Data;
using APIPedidos.Models;
using Microsoft.EntityFrameworkCore;

public class PedidoRepository : Repository<Pedido>, IPedidoRepository
{
    public PedidoRepository(AppDbContext ctx) : base(ctx) { }

    public Task<List<Pedido>> ListarAsync(bool? fechado)
    {
        var q = _ctx.Pedidos.AsQueryable();
        if (fechado.HasValue) q = q.Where(p => p.Fechado == fechado.Value);

        return q.AsNoTracking()
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .ToListAsync();
    }


    public Task<Pedido?> ObterComItensAsync(int id) =>
        _ctx.Pedidos.Include(p => p.Itens).ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id);
}
