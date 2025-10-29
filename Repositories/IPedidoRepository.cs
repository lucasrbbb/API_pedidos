using APIPedidos.Models;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<List<Pedido>> ListarAsync(bool? fechado);
    Task<Pedido?> ObterComItensAsync(int id);
}
