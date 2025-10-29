public interface IPedidoService
{
    Task<List<PedidoListDTO>?> ListarAsync(string? status);
    Task<PedidoDetalheDTO?> ObterAsync(int id);
    Task<PedidoDetalheDTO> CriarAsync(CriarPedidoDTO dto);
    Task<PedidoDetalheDTO?> AdicionarItensAsync(int id, AdicionarItensDTO dto);
    Task<PedidoDetalheDTO?> RemoverItemAsync(int id, int produtoId);
    Task<bool> FecharAsync(int id);
}
