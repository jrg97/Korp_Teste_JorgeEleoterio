using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Faturamento.API.Data;
using Faturamento.API.Dtos;
using Faturamento.API.Models;

namespace Faturamento.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotasFiscaisController : ControllerBase
{
    private readonly FaturamentoDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public NotasFiscaisController(FaturamentoDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<ActionResult<NotaFiscalResponseDto>> Criar()
    {
        var proximoNumero = (await _context.NotasFiscais.MaxAsync(n => (int?)n.Numero) ?? 0) + 1;

        var nota = new NotaFiscal
        {
            Numero = proximoNumero,
            Status = StatusNotaFiscal.Aberta
        };

        _context.NotasFiscais.Add(nota);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = nota.Id }, MapParaDto(nota));
    }

    [HttpPost("{id}/itens")]
    public async Task<ActionResult<NotaFiscalResponseDto>> AdicionarItem(int id, AdicionarItemDto dto)
    {
        var nota = await _context.NotasFiscais
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (nota is null) return NotFound("Nota fiscal não encontrada.");

        if (nota.Status != StatusNotaFiscal.Aberta)
            return BadRequest("Só é possível adicionar itens em notas com status Aberta.");

        nota.Itens.Add(new ItemNotaFiscal
        {
            ProdutoId = dto.ProdutoId,
            Quantidade = dto.Quantidade
        });

        await _context.SaveChangesAsync();

        return Ok(MapParaDto(nota));
    }

    [HttpPost("{id}/fechar")]
    public async Task<ActionResult<NotaFiscalResponseDto>> Fechar(int id)
    {
        var nota = await _context.NotasFiscais
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (nota is null) return NotFound("Nota fiscal não encontrada.");

        if (nota.Status != StatusNotaFiscal.Aberta)
            return BadRequest("Só é possível fechar notas com status Aberta.");

        if (nota.Itens.Count == 0)
            return BadRequest("Não é possível fechar uma nota sem itens.");

        var client = _httpClientFactory.CreateClient("EstoqueApi");

        try{
                foreach (var item in nota.Itens)
                {
                    var response = await client.PostAsJsonAsync(
                        $"api/produtos/{item.ProdutoId}/baixar-saldo",
                        new { quantidade = item.Quantidade });

                    if (!response.IsSuccessStatusCode)
                    {
                        var erro = await response.Content.ReadAsStringAsync();
                        return BadRequest($"Falha ao baixar saldo do produto {item.ProdutoId}: {erro}");
                    }
                }
        }
        catch(HttpRequestException)
        {
            return StatusCode(503, "O serviço de Estoque está indisponível no momento. Tente novamente em instantes.");
        }

        nota.Status = StatusNotaFiscal.Fechada;
        await _context.SaveChangesAsync();

        return Ok(MapParaDto(nota));
    }

    [HttpGet]
    public async Task<ActionResult<List<NotaFiscalResponseDto>>> Listar()
    {
        var notas = await _context.NotasFiscais.Include(n => n.Itens).ToListAsync();
        return Ok(notas.Select(MapParaDto).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NotaFiscalResponseDto>> ObterPorId(int id)
    {
        var nota = await _context.NotasFiscais
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (nota is null) return NotFound();

        return Ok(MapParaDto(nota));
    }

    private static NotaFiscalResponseDto MapParaDto(NotaFiscal nota) =>
        new(
            nota.Id,
            nota.Numero,
            nota.Status.ToString(),
            nota.Itens.Select(i => new ItemResponseDto(i.Id, i.ProdutoId, i.Quantidade)).ToList()
        );
}