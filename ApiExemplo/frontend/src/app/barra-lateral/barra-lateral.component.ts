import { Component, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ItemNavegacao } from '../modelos/item-navegacao.model';

@Component({
  selector: 'barra-lateral',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './barra-lateral.component.html',
  styleUrls: ['./barra-lateral.component.scss']
})
export class ComponenteBarraLateral implements OnInit {
  @Output() itemSelecionado = new EventEmitter<string>();

  itens: ItemNavegacao[] = [
    { id: 'scan', rotulo: 'Interface de Scan', icone: 'scan', ativo: true },
    { id: 'dashboard', rotulo: 'Dashboard', icone: 'dashboard', ativo: false },
    { id: 'estoque', rotulo: 'Estoque', icone: 'estoque', ativo: false },
    { id: 'configuracoes', rotulo: 'Configurações', icone: 'configuracoes', ativo: false },
    { id: 'suporte', rotulo: 'Suporte', icone: 'suporte', ativo: false },
    { id: 'sair', rotulo: 'Sair', icone: 'sair', ativo: false }
  ];

  menuAberto = false;

  ngOnInit(): void {
    // Inicialização se necessário
  }

  selecionarItem(itemClicado: ItemNavegacao): void {
    this.itens.forEach(item => item.ativo = (item.id === itemClicado.id));
    this.itemSelecionado.emit(itemClicado.id);
    this.menuAberto = false; // Fecha o menu hambúrguer ao selecionar em celular
  }

  alternarMenu(): void {
    this.menuAberto = !this.menuAberto;
  }
}
