import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {NotaFiscal} from './notas-fiscais.model';

@Injectable({
  providedIn: 'root'
})

export class NotaFiscalService {
  private readonly apiUrl = 'http://localhost:5064/api/notasfiscais';

  constructor(private http: HttpClient) { }

  listar(): Observable<NotaFiscal[]> {
    return this.http.get<NotaFiscal[]>(this.apiUrl);
  }

  criar(): Observable<NotaFiscal> {
  return this.http.post<NotaFiscal>(this.apiUrl, {});
}

  adicionarItem(notaId: number, item: { produtoId: number; quantidade: number }): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(`${this.apiUrl}/${notaId}/itens`, item);
  }

  fechar(notaId: number): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(`${this.apiUrl}/${notaId}/fechar`, {});
  }
}