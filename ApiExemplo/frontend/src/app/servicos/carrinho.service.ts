import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, catchError, map } from 'rxjs';

export interface ItemCarrinho {
  sku: string;
  nome: string;
  categoria: string;
  quantidade: number;
  precoUnitarioCents: number;
  imagemUrl: string;
}

export interface DadosCarrinho {
  itens: ItemCarrinho[];
  taxaServicoCents: number;
  descontoCents: number;
}

export interface ResultadoRevisao {
  carrinho: DadosCarrinho;
  modoDemonstracao: boolean;
}

const DADOS_CARRINHO_MOCK: DadosCarrinho = {
  itens: [
    {
      sku: 'SKU-99823',
      nome: 'Leitor de Código de Barras Honeywell Xenon 1900g',
      categoria: 'Hardware de Escaneamento',
      quantidade: 2,
      precoUnitarioCents: 125000,
      imagemUrl: '/imagens/leitor_honeywell.png'
    },
    {
      sku: 'SKU-44321',
      nome: 'Licença ScanIQ Pro AI Cloud Enterprise (1 Ano)',
      categoria: 'Softwares & SaaS',
      quantidade: 1,
      precoUnitarioCents: 299900,
      imagemUrl: '/imagens/licenca_scaniq.png'
    },
    {
      sku: 'SKU-10293',
      nome: 'Suporte Articulado Industrial para Scanners Pro',
      categoria: 'Acessórios',
      quantidade: 3,
      precoUnitarioCents: 32000,
      imagemUrl: '/imagens/suporte_articulado.png'
    }
  ],
  taxaServicoCents: 4500,
  descontoCents: 15000
};

@Injectable({
  providedIn: 'root'
})
export class CarrinhoService {
  private urlsDisponiveis = ['https://localhost:7218', 'http://localhost:5107'];
  private urlBaseAtiva = 'https://localhost:7218';

  constructor(private http: HttpClient) {}

  obterUrlsDisponiveis(): string[] {
    return this.urlsDisponiveis;
  }

  definirUrlAtiva(url: string): void {
    if (this.urlsDisponiveis.includes(url)) {
      this.urlBaseAtiva = url;
    }
  }

  obterUrlAtiva(): string {
    return this.urlBaseAtiva;
  }

  obterRevisaoCarrinho(): Observable<ResultadoRevisao> {
    return this.http.get<DadosCarrinho>(`${this.urlBaseAtiva}/api/Carrinhos/revisao`).pipe(
      map((dados) => ({ carrinho: dados, modoDemonstracao: false })),
      catchError((erro) => {
        console.warn('Backend offline. Ativando dados de demonstração locais:', erro.message);
        return of({ carrinho: DADOS_CARRINHO_MOCK, modoDemonstracao: true });
      })
    );
  }

  avancarPagamento(formaPagamento: number): Observable<any> {
    const payload = {
      usuarioId: 'usuario-teste-123',
      formaPagamento: formaPagamento
    };
    return this.http.post(`${this.urlBaseAtiva}/api/Carrinhos/avancar-pagamento`, payload);
  }
}
