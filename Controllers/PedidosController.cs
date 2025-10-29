using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    //injeção service 
    private readonly IPedidoService _service;
    public PedidosController(IPedidoService service) => _service = service;

    //lista pedidos com filtro

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PedidoListDTO>>> Listar([FromQuery] string? status)
    {
        try
        {
            return Ok(await _service.ListarAsync(status));
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    //lista pedido por id

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PedidoDetalheDTO>> Obter(int id)
    {
        var pedido = await _service.ObterAsync(id);
        return pedido is null ? NotFound("Pedido não encontrado.") : Ok(pedido);
    }

    // criar pedido -- > ok

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

    // adicionar item


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

    // remover item 

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

    // fechar

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
