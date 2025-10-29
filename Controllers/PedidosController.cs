using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _service;
    public PedidosController(IPedidoService service) => _service = service;

    /// <summary>
    /// Lista pedidos com filtro por status.
    /// </summary>
    /// <param name="status">"aberto" ou "fechado" (opcional).</param>
    /// <returns>Lista de pedidos com produtos.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PedidoListDTO>>> Listar([FromQuery] string? status)
    {
        try
        {
            return Ok(await _service.ListarAsync(status));
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    /// <summary>
    /// Pedido pelo seu ID com informaçõs dos produtos.
    /// </summary>
    /// <param name="id">Identificador numérico do pedido.</param>
    /// <returns>Pedido detalhado com itens.</returns>
    /// <response code="404">Pedido não encontrado.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PedidoDetalheDTO>> Obter(int id)
    {
        var pedido = await _service.ObterAsync(id);
        return pedido is null ? NotFound("Pedido não encontrado.") : Ok(pedido);
    }

    /// <summary>
    /// Cria um novo pedido.
    /// </summary>
    /// <param name="body">Dados para criação do pedido.</param>
    /// <returns>Pedido criado já em estado aberto.</returns>
    /// <response code="201">Pedido criado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    [HttpPost]
    public async Task<ActionResult<PedidoDetalheDTO>> Criar([FromBody] CriarPedidoDTO body)
    {
        try
        {
            var pedido = await _service.CriarAsync(body);
            return CreatedAtAction(nameof(Obter), new { id = pedido.Id }, pedido);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    /// <summary>
    /// Adiciona itens a um pedido aberto.
    /// </summary>
    /// <param name="id">ID do pedido.</param>
    /// <param name="body">Itens a serem adicionados.</param>
    /// <returns>Pedido atualizado com novos itens.</returns>
    /// <response code="404">Pedido não encontrado.</response>
    /// <response code="400">Pedido fechado ou item inválido.</response>
    [HttpPost("{id:int}/adicionar-itens")]
    public async Task<ActionResult<PedidoDetalheDTO>> AdicionarItens(int id, [FromBody] AdicionarItensDTO body)
    {
        try
        {
            var pedido = await _service.AdicionarItensAsync(id, body);
            return pedido is null ? NotFound("Pedido não encontrado.") : Ok(pedido);
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    /// <summary>
    /// Remove produto do pedido informado.
    /// </summary>
    /// <param name="id">ID do pedido.</param>
    /// <param name="produtoId">ID do produto do pedido.</param>
    /// <returns>Pedido atualizado sem o item removido.</returns>
    /// <response code="404">Pedido não encontrado.</response>
    /// <response code="400">Pedido fechado ou item inexistente.</response>
    [HttpDelete("{id:int}/remover-item/{produtoId:int}")]
    public async Task<ActionResult<PedidoDetalheDTO>> RemoverItem(int id, int produtoId)
    {
        try
        {
            var pedido = await _service.RemoverItemAsync(id, produtoId);
            return pedido is null ? NotFound("Pedido não encontrado.") : Ok(pedido);
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    /// <summary>
    /// Fecha um pedido.
    /// </summary>
    /// <param name="id">ID do pedido.</param>
    /// <returns>Confirmação de fechamento.</returns>
    /// <response code="404">Pedido não encontrado.</response>
    /// <response code="400">Pedido já está fechado.</response>
    [HttpPost("{id:int}/fechar")]
    public async Task<IActionResult> Fechar(int id)
    {
        try
        {
            var ok = await _service.FecharAsync(id);
            return ok ? Ok("Pedido fechado com sucesso.") : NotFound("Pedido não encontrado.");
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }
}

