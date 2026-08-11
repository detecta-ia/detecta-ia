import { Injectable } from '@angular/core';
import { EntradaCatalogoProduto } from '../modelos/entrada-catalogo.model';
import { ProdutoEstoque } from '../modelos/produto-estoque.model';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ServicoCatalogoProdutos {
  private readonly catalogo: Record<string, EntradaCatalogoProduto> = {
    'leite_integral_1l': {
      classeId: 0,
      nomeClasse: 'leite_integral_1l',
      nome: 'Leite Integral 1L',
      preco: 4.50,
      urlMiniatura: 'assets/imagens/leite.png'
    },
    'mirtilos_organicos': {
      classeId: 1,
      nomeClasse: 'mirtilos_organicos',
      nome: 'Mirtilos Orgânicos',
      preco: 12.90,
      urlMiniatura: 'assets/imagens/mirtilos.png'
    },
    'agua_mineral_gas': {
      classeId: 2,
      nomeClasse: 'agua_mineral_gas',
      nome: 'Água Mineral Gás',
      preco: 3.25,
      urlMiniatura: 'assets/imagens/agua.png'
    },
    'mouse_computador': {
      classeId: 3,
      nomeClasse: 'mouse_computador',
      nome: 'Mouse de Computador',
      preco: 49.90,
      urlMiniatura: 'assets/imagens/mouse.png'
    },
    'pessoa_gabriel': {
      classeId: 4,
      nomeClasse: 'pessoa_gabriel',
      nome: 'Gabriel',
      preco: 0.00,
      urlMiniatura: 'assets/imagens/pessoa.png'
    }
  };

  private readonly produtosEstoqueIniciais: ProdutoEstoque[] = [];

  private readonly sujeitoProdutosEstoque = new BehaviorSubject<ProdutoEstoque[]>(this.produtosEstoqueIniciais);
  produtosEstoque$: Observable<ProdutoEstoque[]> = this.sujeitoProdutosEstoque.asObservable();

  obterCatalogoCompleto(): EntradaCatalogoProduto[] {
    return Object.values(this.catalogo);
  }

  buscarPorClasse(nomeClasse: string): EntradaCatalogoProduto | undefined {
    return this.catalogo[nomeClasse];
  }

  buscarPorIdClasse(classeId: number): EntradaCatalogoProduto | undefined {
    return Object.values(this.catalogo).find(p => p.classeId === classeId);
  }

  obterProdutosEstoque(): ProdutoEstoque[] {
    return this.sujeitoProdutosEstoque.getValue();
  }

  adicionarProdutoEstoque(novoProduto: Omit<ProdutoEstoque, 'id'>): void {
    const listaAtual = this.obterProdutosEstoque();
    const produtoCompleto: ProdutoEstoque = {
      ...novoProduto,
      id: `prod_${Date.now()}`
    };
    this.sujeitoProdutosEstoque.next([produtoCompleto, ...listaAtual]);
  }

  removerProdutoEstoque(id: string): void {
    const listaFiltrada = this.obterProdutosEstoque().filter(p => p.id !== id);
    this.sujeitoProdutosEstoque.next(listaFiltrada);
  }
}
