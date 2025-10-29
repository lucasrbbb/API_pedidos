public record PedidoListDTO(int Id, DateTime CriadoEm, bool Fechado, decimal ValorTotal, List<PedidoItemDTO> Itens);

public record PedidoItemDTO(string Nome, int ProdutoId, int Quantidade, decimal PrecoUnitario);

public record PedidoDetalheDTO(int Id, DateTime CriadoEm, bool Fechado, decimal ValorTotal, List<PedidoItemDTO> Itens);

public record CriarPedidoDTO(List<AdicionarItemLinha> Itens);

public record AdicionarItemLinha(int ProdutoId, int Quantidade, decimal? PrecoUnitario = null);

public record AdicionarItensDTO(List<AdicionarItemLinha> Itens);

