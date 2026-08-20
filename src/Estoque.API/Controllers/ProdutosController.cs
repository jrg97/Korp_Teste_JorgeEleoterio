using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Estoque.API.Data;
using Estoque.API.Models;
using Estoque.API.Dtos;

namespace Estoque.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly EstoqueDbContext _context;

    public ProdutosController(EstoqueDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoResponseDto>> Criar(CriarProdutoDTo dto)
    {
        var produto = new Produto
        {
            Codigo = dto.Codigo,
            Descricao = dto.Descricao,
            Saldo = dto.Saldo
        };

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        var response = new ProdutoResponseDto(produto.Id, produto.Codigo, produto.Descricao, produto.Saldo);
        return CreatedAtAction(nameof(ObterPorId), new {id = produto.Id}, response);
    }

    [HttpGet]
    public async Task<ActionResult<List<ProdutoResponseDto>>> Listar()
    {
        var produtos = await _context.Produtos
            .Select(p => new ProdutoResponseDto(p.Id, p.Codigo, p.Descricao, p.Saldo))
            .ToListAsync();

        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoResponseDto>> ObterPorId(int id)
    {

        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null) return NotFound();

        return Ok(new ProdutoResponseDto(produto.Id, produto.Codigo, produto.Descricao, produto.Saldo));
    }
    [HttpPost("{id}/baixar-saldo")]
    public async Task<IActionResult> BaixarSaldo(int id, BaixarSaldoDto dto)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null) return NotFound();

        if (produto.Saldo < dto.Quantidade)
            return BadRequest($"Saldo insuficiente. Disponível: {produto.Saldo}, solicitado: {dto.Quantidade}");

        produto.Saldo -= dto.Quantidade;
        await _context.SaveChangesAsync();

        return Ok(new ProdutoResponseDto(produto.Id, produto.Codigo, produto.Descricao, produto.Saldo));
    }
}