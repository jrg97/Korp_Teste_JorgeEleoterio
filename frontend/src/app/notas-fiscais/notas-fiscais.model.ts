export interface ItemNotaFiscal{
  id?: number;
  produtoId: number;
  quantidade: number;
}

export interface NotaFiscal{
  id?: number;
  numero: number;
  status?: string;
  itens: ItemNotaFiscal[];
}