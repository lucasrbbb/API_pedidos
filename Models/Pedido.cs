namespace APIPedidos.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public decimal ValorTotal { get; set; }
        public bool Fechado { get; set; }

        public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();

        public void Fechar()
        {
            if (!Itens.Any()) throw new InvalidOperationException("Pedido sem itens!");
            Fechado = true;
        }
    }
}

