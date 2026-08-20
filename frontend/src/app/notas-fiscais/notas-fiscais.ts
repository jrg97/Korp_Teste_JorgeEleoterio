import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { NotaFiscalService } from './nota-fiscal';
import { NotaFiscal } from './notas-fiscais.model';

@Component({
  selector: 'app-notas-fiscais',
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatTableModule,
    MatCardModule,
    MatChipsModule
  ],
  templateUrl: './notas-fiscais.html',
  styleUrl: './notas-fiscais.css'
})
export class NotasFiscais implements OnInit {
  notas: NotaFiscal[] = [];
  novoNumero: number = 0;
  novoItem: { [notaId: number]: { produtoId: number; quantidade: number } } = {};
  mensagemErro: string = '';
  colunasExibidas: string[] = ['produtoId', 'quantidade'];

  constructor(private notaFiscalService: NotaFiscalService) { }

  ngOnInit(): void {
    this.carregarNotas();
  }

  carregarNotas(): void {
    this.notaFiscalService.listar().subscribe({
      next: (dados) => this.notas = dados,
      error: (erro) => console.error('Erro ao carregar notas', erro)
    });
  }

  criarNota(): void {
    this.notaFiscalService.criar({ numero: this.novoNumero }).subscribe({
      next: () => {
        this.novoNumero = 0;
        this.carregarNotas();
      },
      error: (erro) => console.error('Erro ao criar nota', erro)
    });
  }

  getNovoItem(notaId: number) {
    if (!this.novoItem[notaId]) {
      this.novoItem[notaId] = { produtoId: 0, quantidade: 0 };
    }
    return this.novoItem[notaId];
  }

  adicionarItem(notaId: number): void {
    const item = this.getNovoItem(notaId);
    this.notaFiscalService.adicionarItem(notaId, item).subscribe({
      next: () => {
        this.novoItem[notaId] = { produtoId: 0, quantidade: 0 };
        this.carregarNotas();
      },
      error: (erro) => console.error('Erro ao adicionar item', erro)
    });
  }

  fecharNota(notaId: number): void {
    this.mensagemErro = '';
    this.notaFiscalService.fechar(notaId).subscribe({
      next: () => this.carregarNotas(),
      error: (erro) => {
        this.mensagemErro = erro.error?.erro || erro.error || 'Erro ao fechar nota fiscal.';
      }
    });
  }
}