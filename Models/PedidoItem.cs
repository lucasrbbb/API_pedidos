namespace APIPedidos.Models
{
    public class PedidoItem
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }

        public Pedido Pedido { get; set; } = default!;
        public Produto Produto { get; set; } = default!;
    }
}

