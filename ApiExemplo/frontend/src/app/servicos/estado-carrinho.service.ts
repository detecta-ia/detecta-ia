import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ItemCarrinhoCompra } from '../modelos/item-carrinho.model';
import { ItemCarrinho, DadosCarrinho } from './carrinho.service';

const TAXA_SERVICO_PADRAO_CENTS = 0;
const DESCONTO_PADRAO_CENTS = 0;

@Injectable({
  providedIn: 'root'
})
export class EstadoCarrinhoService {
  private readonly subItens = new BehaviorSubject<ItemCarrinho[]>([]);

  /**
   * Recebe os itens do painel de Scan (formato ItemCarrinhoCompra)
   * e os converte para o formato ItemCarrinho usado pelo Checkout.
   */
  definirItens(itensScan: ItemCarrinhoCompra[]): void {
    const itensConvertidos: ItemCarrinho[] = itensScan.map(item => ({
      nome: item.nome,
      categoria: 'Produto Detectado',
      quantidade: 1,
      precoUnitarioCents: Math.round(item.preco * 100),
      imagemUrl: item.urlMiniatura
    }));
    this.subItens.next(itensConvertidos);
  }

  /**
   * Observable dos itens do carrinho.
   */
  obterItens$(): Observable<ItemCarrinho[]> {
    return this.subItens.asObservable();
  }

  /**
   * Observable dos dados completos do carrinho (itens + taxa + desconto).
   */
  obterDadosCarrinho$(): Observable<DadosCarrinho> {
    return this.subItens.pipe(
      map(itens => ({
        itens,
        taxaServicoCents: TAXA_SERVICO_PADRAO_CENTS,
        descontoCents: DESCONTO_PADRAO_CENTS
      }))
    );
  }

  /**
   * Verifica sincronamente se há itens no carrinho.
   */
  possuiItens(): boolean {
    return this.subItens.getValue().length > 0;
  }

  /**
   * Limpa completamente o estado do carrinho.
   */
  limpar(): void {
    this.subItens.next([]);
  }
}
