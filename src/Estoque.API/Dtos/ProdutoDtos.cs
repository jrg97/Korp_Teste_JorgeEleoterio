namespace Estoque.API.Dtos;

public record CriarProdutoDTo(string Codigo, string Descricao, int Saldo);

public record ProdutoResponseDto(int Id, string Codigo, string Descricao, int Saldo);

public record BaixarSaldoDto(int Quantidade);