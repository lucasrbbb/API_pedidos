namespace APIPedidos.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public decimal Preco { get; set; }

        public ICollection<PedidoItem> Itens { get; } = new List<PedidoItem>();
    }
}


