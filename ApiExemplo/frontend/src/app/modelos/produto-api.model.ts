/**
 * Interfaces TypeScript que espelham os DTOs do backend C#.
 * Usadas pelo ServicoProdutoApi para tipagem das requisições e respostas.
 */

// --- Wrapper genérico de resposta (espelha RespostaApi<T> do C#) ---
export interface RespostaApi<T> {
  ok: boolean;
  mensagem: string;
  dados?: T;
  erros?: string[];
}

// --- Criar Produto ---

/** Payload enviado no body do POST /api/Produtos */
export interface CriarProdutoRequisicao {
  nome: string;
  preco: number;
  categoria: string;
}

/** Dados retornados após criação bem-sucedida */
export interface CriarProdutoResposta {
  id: string;
  nome: string;
  preco: number;
  categoria: string;
}

// --- Listar Produtos (paginado) ---

export interface ProdutoListaDto {
  id: string;
  nome: string;
  preco: number;
  categoria: string;
}

export interface ResultadoPaginado<T> {
  itens: T[];
  total: number;
  pagina: number;
  tamanhoPagina: number;
  totalPaginas: number;
  temProximaPagina: boolean;
  temPaginaAnterior: boolean;
}

// --- Detalhe do Produto ---

export interface ProdutoDetalheDto {
  id: string;
  nome: string;
  preco: number;
  categoria: string;
  criadoEm: string;
  atualizadoEm: string;
}
