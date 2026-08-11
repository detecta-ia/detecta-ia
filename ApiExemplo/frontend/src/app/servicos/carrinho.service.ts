import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, catchError, map } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ItemCarrinho {
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
  servidorConectado?: boolean;
}

const DADOS_CARRINHO_MOCK: DadosCarrinho = {
  itens: [

  ],
  taxaServicoCents: 0,
  descontoCents: 0
};

@Injectable({
  providedIn: 'root'
})
export class CarrinhoService {
  private urlBasePadrao = environment.apiUrlSwagger.replace(/\/(swagger(\/index\.html)?|api)\/?$/i, '').replace(/\/$/, '');
  private urlsDisponiveis = [this.urlBasePadrao];
  private urlBaseAtiva = this.urlBasePadrao;

  constructor(private http: HttpClient) { }

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

  obterRevisaoCarrinho(usuarioId?: string): Observable<ResultadoRevisao> {
    const url = usuarioId
      ? `${this.urlBaseAtiva}/api/Carrinhos/revisao?usuarioId=${encodeURIComponent(usuarioId)}`
      : `${this.urlBaseAtiva}/api/Carrinhos/revisao`;

    return this.http.get<any>(url).pipe(
      map((resposta) => {
        const dados = resposta?.dados ?? resposta;
        const itensValidos = Array.isArray(dados?.itens) && dados.itens.length > 0 ? dados.itens : DADOS_CARRINHO_MOCK.itens;
        return {
          carrinho: {
            itens: itensValidos,
            taxaServicoCents: 0,
            descontoCents: 0
          },
          modoDemonstracao: false,
          servidorConectado: true
        };
      }),
      catchError((erro) => {
        // Se erro.status !== 0, o servidor HTTP do backend respondeu (ex: status 400 ou 500)
        const servidorConectado = erro.status !== 0;
        console.warn(`[CarrinhoService] Status backend (${erro.status}). Servidor ${servidorConectado ? 'Conectado' : 'Offline'}:`, erro.message);
        return of({
          carrinho: DADOS_CARRINHO_MOCK,
          modoDemonstracao: !servidorConectado,
          servidorConectado: servidorConectado
        });
      })
    );
  }

  avancarPagamento(formaPagamento: number, usuarioId?: string): Observable<any> {
    const payload = {
      usuarioId: usuarioId ?? 'usuario-teste-123',
      formaPagamento: formaPagamento
    };
    return this.http.post(`${this.urlBaseAtiva}/api/Carrinhos/avancar-pagamento`, payload);
  }

  verificarConexaoBackend(): Observable<boolean> {
    return this.http.get(`${this.urlBaseAtiva}/api/Produtos`).pipe(
      map(() => true),
      catchError((erro) => of(erro.status !== 0))
    );
  }
}
