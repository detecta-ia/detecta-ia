import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
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
  @Input() itemAtivo = 'scan';
  @Output() itemSelecionado = new EventEmitter<string>();

  itens: ItemNavegacao[] = [
    { id: 'scan', rotulo: 'Scan Interface', icone: 'scan' },
    { id: 'dashboard', rotulo: 'Dashboard', icone: 'dashboard' },
    { id: 'estoque', rotulo: 'Inventory', icone: 'estoque' },
    { id: 'configuracoes', rotulo: 'Settings', icone: 'configuracoes' },
    { id: 'suporte', rotulo: 'Support', icone: 'suporte' },
    { id: 'sair', rotulo: 'Logout', icone: 'sair' }
  ];

  menuAberto = false;

  ngOnInit(): void {}

  selecionarItem(itemClicado: ItemNavegacao): void {
    this.itemAtivo = itemClicado.id;
    this.itemSelecionado.emit(itemClicado.id);
    this.menuAberto = false;
  }

  alternarMenu(): void {
    this.menuAberto = !this.menuAberto;
  }
}
