using APIPedidos.Models;
using APIPedidos.Data;
using Microsoft.EntityFrameworkCore;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidos;
    private readonly IRepository<Produto> _produtos;

    public PedidoService(IPedidoRepository pedidos, IRepository<Produto> produtos)
    { _pedidos = pedidos; _produtos = produtos; }


    //listar pedidos por status

    public async Task<List<PedidoListDTO>?> ListarAsync(string? status)
    {
        bool? fechado;

        switch (status?.ToLower())
        {
            case "aberto":
                fechado = false;
                break;

            case "fechado":
                fechado = true;
                break;

            case null:
            case "":
                fechado = null;
                break;

            default:
                throw new ArgumentException("status deve ser 'aberto' ou 'fechado'");
        }

        var pedidos = await _pedidos.ListarAsync(fechado);

        return pedidos.Select(p => new PedidoListDTO(
            p.Id,
            p.CriadoEm,
            p.Fechado,
            p.ValorTotal,
            p.Itens.Select(i => new PedidoItemDTO(
                i.Produto?.Nome ?? string.Empty,
                i.ProdutoId,
                i.Quantidade,
                i.PrecoUnitario
            )).ToList()
        )).ToList();
    }

    // pedido por Id

    public async Task<PedidoDetalheDTO?> ObterAsync(int id)
        => (await _pedidos.ObterComItensAsync(id)) is { } p ? MapDetalhe(p) : null;


    // criar pedido --> errado falta adicionar produtos. 

    public async Task<PedidoDetalheDTO> CriarAsync(CriarPedidoDTO dto)
    {
        if (dto is null || dto.Itens is null || dto.Itens.Count == 0)
            throw new ArgumentException("Envie ao menos um item para criar o pedido.");

        var pedido = new Pedido
        {
            Fechado = false,
            ValorTotal = 0m,
            CriadoEm = DateTime.UtcNow
        };

        // monta os itens
        foreach (var linha in dto.Itens)
        {
            if (linha.Quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero.");

            var produto = await _produtos.GetByIdAsync(linha.ProdutoId)
                         ?? throw new ArgumentException($"Produto {linha.ProdutoId} não existe.");

            var preco = linha.PrecoUnitario ?? produto.Preco;

            pedido.Itens.Add(new PedidoItem
            {
                ProdutoId = produto.Id,
                Quantidade = linha.Quantidade,
                PrecoUnitario = preco
            });
        }

        // recalcula total

        Recalc(pedido);

        await _pedidos.AddAsync(pedido);
        await _pedidos.SaveChangesAsync();

        var comItens = await _pedidos.ObterComItensAsync(pedido.Id);
        return MapDetalhe(comItens!);
    }

    // adicionar itens 

    public async Task<PedidoDetalheDTO?> AdicionarItensAsync(int id, AdicionarItensDTO dto)
    {
        var pedido = await _pedidos.ObterComItensAsync(id);
        if (pedido is null) return null;
        if (pedido.Fechado) throw new InvalidOperationException("Pedido já está fechado.");

        foreach (var linha in dto.Itens)
        {
            var qtd = Math.Max(1, linha.Quantidade);

            var produto = await _produtos.GetByIdAsync(linha.ProdutoId)
                         ?? throw new ArgumentException($"Produto {linha.ProdutoId} não existe.");

            var preco = linha.PrecoUnitario ?? produto.Preco;

            var existente = pedido.Itens.FirstOrDefault(i => i.ProdutoId == produto.Id);

            if (existente is not null)
            {
                existente.Quantidade += qtd;

                if (linha.PrecoUnitario.HasValue)
                    existente.PrecoUnitario = preco;
            }
            else
            {
                pedido.Itens.Add(new PedidoItem
                {
                    ProdutoId = produto.Id,
                    Quantidade = qtd,
                    PrecoUnitario = preco
                });
            }
        }

        Recalc(pedido);
        await _pedidos.SaveChangesAsync();

        pedido = await _pedidos.ObterComItensAsync(pedido.Id);
        return pedido is null ? null : MapDetalhe(pedido);
    }


    // remover itens 

    public async Task<PedidoDetalheDTO?> RemoverItemAsync(int id, int produtoId)
    {
        var p = await _pedidos.ObterComItensAsync(id);
        if (p is null) return null;
        if (p.Fechado) throw new InvalidOperationException("Pedido já está fechado.");

        var item = p.Itens.FirstOrDefault(i => i.ProdutoId == produtoId)
                   ?? throw new ArgumentException("Item não encontrado neste pedido.");

        p.Itens.Remove(item);
        Recalc(p);
        await _pedidos.SaveChangesAsync();

        p = await _pedidos.ObterComItensAsync(id);
        return p is null ? null : MapDetalhe(p);
    }


    // fechar pedido

    public async Task<bool> FecharAsync(int id)
    {
        var p = await _pedidos.ObterComItensAsync(id);
        if (p is null) return false;
        if (!p.Itens.Any()) throw new InvalidOperationException("Pedido sem itens.");
        p.Fechado = true;
        await _pedidos.SaveChangesAsync();
        return true;
    }

    private static void Recalc(Pedido p) =>
        p.ValorTotal = p.Itens.Sum(i => i.PrecoUnitario * i.Quantidade);

    private static PedidoDetalheDTO MapDetalhe(Pedido p) =>
    new(
        p.Id,
        p.CriadoEm,
        p.Fechado,
        p.ValorTotal,
        p.Itens.Select(i => new PedidoItemDTO(
            i.Produto?.Nome ?? string.Empty,
            i.ProdutoId,
            i.Quantidade,
            i.PrecoUnitario
        )).ToList()
    );

}


