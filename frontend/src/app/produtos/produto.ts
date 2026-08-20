import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Produto} from './produtos.model';

@Injectable({
  providedIn: 'root'
})
export class ProdutoService {
  private readonly apiUrl = 'http://localhost:5144/api/produtos';

  constructor(private http: HttpClient) { }

  listar(): Observable<Produto[]> {
    return this.http.get<Produto[]>(this.apiUrl);
  }

  criar(produto: Produto): Observable<Produto> {
    return this.http.post<Produto>(this.apiUrl, produto);
  }
}