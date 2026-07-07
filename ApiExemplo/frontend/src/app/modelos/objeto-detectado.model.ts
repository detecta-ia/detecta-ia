export interface ObjetoDetectado {
  id: string;
  rotulo: string;
  status: 'identificando' | 'confirmado';
  caixa: { topo: number; esquerda: number; largura: number; altura: number }; // em % normalizado
  classeId: number;
  confianca: number;
}
