import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ItemCarrinhoCompra } from '../../modelos/item-carrinho.model';

@Component({
  selector: 'item-carrinho',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './item-carrinho.component.html',
  styleUrls: ['./item-carrinho.component.scss']
})
export class ComponenteItemCarrinho {
  @Input() item!: ItemCarrinhoCompra;

  aoErroImagem(event: Event): void {
    const target = event.target as HTMLImageElement;
    target.src = 'assets/imagens/esteira.png';
  }
}
