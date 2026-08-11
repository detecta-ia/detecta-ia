import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import {
  RespostaApi,
  CriarProdutoRequisicao,
  CriarProdutoResposta,
  ResultadoPaginado,
  ProdutoListaDto
} from '../modelos/produto-api.model';

/**
 * Service responsável pela comunicação HTTP com o endpoint /api/Produtos
 * do backend ASP.NET Core.
 *
 * Usa a URL base definida em environment.apiUrlSwagger para evitar
 * URLs hardcoded.
 */
@Injectable({
  providedIn: 'root'
})
export class ServicoProdutoApi {

  private readonly urlBase = `${environment.apiUrlSwagger}/api/Produtos`;

  constructor(private readonly http: HttpClient) {}

  /**
   * Lista produtos do backend com paginação e busca opcional.
   *
   * GET /api/Produtos?pagina=1&tamanhoPagina=100&busca=
   * Resposta: 200 OK com RespostaApi<ResultadoPaginado<ProdutoListaDto>>
   */
  listarProdutos(
    pagina: number = 1,
    tamanhoPagina: number = 100,
    busca?: string
  ): Observable<RespostaApi<ResultadoPaginado<ProdutoListaDto>>> {
    let parametros = new HttpParams()
      .set('pagina', pagina.toString())
      .set('tamanhoPagina', tamanhoPagina.toString());

    if (busca && busca.trim()) {
      parametros = parametros.set('busca', busca.trim());
    }

    return this.http
      .get<RespostaApi<ResultadoPaginado<ProdutoListaDto>>>(this.urlBase, { params: parametros })
      .pipe(
        catchError(this.tratarErro)
      );
  }

  /**
   * Cria um novo produto no backend.
   *
   * POST /api/Produtos
   * Body: { nome, preco, categoria }
   * Resposta: 201 Created com RespostaApi<CriarProdutoResposta>
   */
  criarProduto(requisicao: CriarProdutoRequisicao): Observable<RespostaApi<CriarProdutoResposta>> {
    return this.http
      .post<RespostaApi<CriarProdutoResposta>>(this.urlBase, requisicao)
      .pipe(
        catchError(this.tratarErro)
      );
  }

  /**
   * Exclui um produto do backend pelo ID.
   *
   * DELETE /api/Produtos?id={guid}
   * Resposta: 200 OK com RespostaApi
   */
  deletarProduto(id: string): Observable<RespostaApi<unknown>> {
    const parametros = new HttpParams().set('id', id);

    return this.http
      .delete<RespostaApi<unknown>>(this.urlBase, { params: parametros })
      .pipe(
        catchError(this.tratarErro)
      );
  }

  /**
   * Tratamento centralizado de erros HTTP.
   * Extrai mensagens amigáveis do wrapper RespostaApi quando possível.
   */
  private tratarErro(erro: HttpErrorResponse): Observable<never> {
    let mensagemUsuario = 'Ocorreu um erro inesperado. Tente novamente.';

    if (erro.status === 0) {
      // Erro de rede / backend offline
      mensagemUsuario = 'Não foi possível conectar ao servidor. Verifique se o backend está rodando.';
    } else if (erro.error && typeof erro.error === 'object') {
      // Tenta extrair mensagem do wrapper RespostaApi
      const respostaApi = erro.error as RespostaApi<unknown>;
      if (respostaApi.mensagem) {
        mensagemUsuario = respostaApi.mensagem;
      }
      // Se houver lista de erros de validação, concatena
      if (respostaApi.erros && respostaApi.erros.length > 0) {
        mensagemUsuario = respostaApi.erros.join(' | ');
      }
    } else if (typeof erro.error === 'string') {
      mensagemUsuario = erro.error;
    }

    console.error('[ServicoProdutoApi] Erro HTTP:', {
      status: erro.status,
      mensagem: mensagemUsuario,
      detalhes: erro
    });

    return throwError(() => mensagemUsuario);
  }
}
