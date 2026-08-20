namespace Faturamento.API.Dtos;
public record AdicionarItemDto(int ProdutoId, int Quantidade);
public record ItemResponseDto(int Id, int ProdutoId, int Quantidade);
public record NotaFiscalResponseDto(int Id, int Numero, string Status, List<ItemResponseDto> Itens);