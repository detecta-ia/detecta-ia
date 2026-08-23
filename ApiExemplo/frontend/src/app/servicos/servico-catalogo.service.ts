import { Injectable } from '@angular/core';
import { EntradaCatalogoProduto } from '../modelos/entrada-catalogo.model';
import { ProdutoEstoque } from '../modelos/produto-estoque.model';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ServicoCatalogoProdutos {
  private readonly catalogo: Record<string, EntradaCatalogoProduto> = {
    'banana_prata': {
      classeId: 5,
      nomeClasse: 'banana_prata',
      nome: 'Banana Prata',
      preco: 5.00,
      urlMiniatura: 'assets/imagens/banana.png'
    },
    'banana_da_terra': {
      classeId: 7,
      nomeClasse: 'banana_da_terra',
      nome: 'Banana da Terra',
      preco: 6.50,
      urlMiniatura: 'assets/imagens/banana.png'
    },
    'mouse': {
      classeId: 8,
      nomeClasse: 'mouse',
      nome: 'Mouse Óptico',
      preco: 45.00,
      urlMiniatura: 'assets/imagens/mouse.png'
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
