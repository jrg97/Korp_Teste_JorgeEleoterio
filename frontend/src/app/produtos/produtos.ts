import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { ProdutoService } from './produto';
import { Produto } from './produtos.model';

@Component({
  selector: 'app-produtos',
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatTableModule,
    MatCardModule
  ],
  templateUrl: './produtos.html',
  styleUrl: './produtos.css'
})
export class Produtos implements OnInit {
  produtos: Produto[] = [];
  novoProduto: Produto = { codigo: '', descricao: '', saldo: 0 };
  colunasExibidas: string[] = ['codigo', 'descricao', 'saldo'];

  constructor(private produtoService: ProdutoService) { }

  ngOnInit(): void {
    this.carregarProdutos();
  }

  carregarProdutos(): void {
    this.produtoService.listar().subscribe({
      next: (dados) => this.produtos = dados,
      error: (erro) => console.error('Erro ao carregar produtos', erro)
    });
  }

  salvar(): void {
    this.produtoService.criar(this.novoProduto).subscribe({
      next: () => {
        this.novoProduto = { codigo: '', descricao: '', saldo: 0 };
        this.carregarProdutos();
      },
      error: (erro) => console.error('Erro ao salvar produto', erro)
    });
  }
}