namespace Faturamento.API.Models;

public class NotaFiscal
{
    public int Id {get; set;}
    public int Numero {get; set;}
    public StatusNotaFiscal Status {get; set;} = StatusNotaFiscal.Aberta;
    public List<ItemNotaFiscal> Itens {get; set;} = new();
}