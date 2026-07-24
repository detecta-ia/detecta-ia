import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ItemCarrinho } from '../servicos/carrinho.service';

@Component({
  selector: 'app-resumo-compra',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './resumo-compra.component.html'
})
export class ResumoCompraComponent {
  @Input() itens: ItemCarrinho[] | null = null;
  @Input() carregando = true;
  @Input() erro: string | null = null;

  formatarMoeda(centavos: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(centavos / 100);
  }

  obterTotalItens(): number {
    if (!this.itens) return 0;
    return this.itens.reduce((acc, item) => acc + item.quantidade, 0);
  }

  obterSubtotal(): number {
    if (!this.itens) return 0;
    return this.itens.reduce((acc, item) => acc + (item.quantidade * item.precoUnitarioCents), 0);
  }

  aoErroImagem(evento: Event): void {
    const img = evento.target as HTMLImageElement;
    img.src = 'https://images.unsplash.com/photo-1531403009284-440f080d1e12?w=150';
  }
}
