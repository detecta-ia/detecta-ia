export interface ProdutoEstoque {
  id: string;
  nome: string;
  categoria: string;
  subcategoria: string;
  sku: string;
  preco: number;
  estoqueQuantidade: number;
  unidadeMedida: string;
  urlMiniatura: string;
}
