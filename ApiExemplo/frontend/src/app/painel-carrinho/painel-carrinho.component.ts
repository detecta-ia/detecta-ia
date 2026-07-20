import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ItemCarrinhoCompra } from '../modelos/item-carrinho.model';
import { ComponenteItemCarrinho } from './item-carrinho/item-carrinho.component';

@Component({
  selector: 'painel-carrinho',
  standalone: true,
  imports: [CommonModule, ComponenteItemCarrinho],
  templateUrl: './painel-carrinho.component.html',
  styleUrls: ['./painel-carrinho.component.scss']
})
export class ComponentePainelCarrinho {
  @Input() itens: ItemCarrinhoCompra[] = [];
  @Output() limparCarrinho = new EventEmitter<void>();
  @Output() finalizarCompra = new EventEmitter<void>();

  get quantidadeItens(): number {
    return this.itens.length;
  }

  get totalAPagar(): number {
    return this.itens.reduce((acumulado, item) => acumulado + item.preco, 0);
  }

  limpar(): void {
    this.limparCarrinho.emit();
  }

  finalizar(): void {
    if (this.itens.length > 0) {
      this.finalizarCompra.emit();
    }
  }
}
