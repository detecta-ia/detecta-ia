import { Injectable } from '@angular/core';
import { EntradaCatalogoProduto } from '../modelos/entrada-catalogo.model';

@Injectable({
  providedIn: 'root'
})
export class ServicoCatalogoProdutos {
  private readonly catalogo: Record<string, EntradaCatalogoProduto> = {
    'leite_integral_1l': {
      classeId: 0,
      nomeClasse: 'leite_integral_1l',
      nome: 'Leite Integral 1L',
      sku: '90210-A',
      preco: 4.50,
      urlMiniatura: 'assets/imagens/leite.png'
    },
    'mirtilos_organicos': {
      classeId: 1,
      nomeClasse: 'mirtilos_organicos',
      nome: 'Mirtilos Orgânicos',
      sku: '33401-B',
      preco: 12.90,
      urlMiniatura: 'assets/imagens/mirtilos.png'
    },
    'agua_mineral_gas': {
      classeId: 2,
      nomeClasse: 'agua_mineral_gas',
      nome: 'Água Mineral Gás',
      sku: '11045-G',
      preco: 3.25,
      urlMiniatura: 'assets/imagens/agua.png'
    },
    'mouse_computador': {
      classeId: 3,
      nomeClasse: 'mouse_computador',
      nome: 'Mouse de Computador',
      sku: '55020-M',
      preco: 49.90,
      urlMiniatura: 'assets/imagens/mouse.png'
    }
  };

  obterCatalogoCompleto(): EntradaCatalogoProduto[] {
    return Object.values(this.catalogo);
  }

  buscarPorClasse(nomeClasse: string): EntradaCatalogoProduto | undefined {
    return this.catalogo[nomeClasse];
  }

  buscarPorIdClasse(classeId: number): EntradaCatalogoProduto | undefined {
    return Object.values(this.catalogo).find(p => p.classeId === classeId);
  }
}
