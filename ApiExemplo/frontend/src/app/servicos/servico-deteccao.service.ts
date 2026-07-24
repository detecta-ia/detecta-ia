import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, Subject, Subscription, interval, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ObjetoDetectado } from '../modelos/objeto-detectado.model';
import { ServicoCatalogoProdutos } from './servico-catalogo.service';
import { EntradaCatalogoProduto } from '../modelos/entrada-catalogo.model';

interface RespostaInferencia {
  deteccoes: {
    class_id: number;
    class_name: string;
    confidence: number;
    bbox: { x1: number; y1: number; x2: number; y2: number };
  }[];
}

@Injectable({
  providedIn: 'root'
})
export class ServicoDeteccao {
  private readonly http = inject(HttpClient);
  private readonly servicoCatalogo = inject(ServicoCatalogoProdutos);

  private readonly URL_API = 'http://localhost:8000/api/detectar';
  private readonly LIMIAR_CONFIANCA = 0.3;
  private readonly FRAMES_PARA_CONFIRMAR = 5;

  private subDeteccoes = new BehaviorSubject<ObjetoDetectado[]>([]);
  deteccoes$: Observable<ObjetoDetectado[]> = this.subDeteccoes.asObservable();

  private subProdutoConfirmado = new Subject<EntradaCatalogoProduto>();
  produtoConfirmado$: Observable<EntradaCatalogoProduto> = this.subProdutoConfirmado.asObservable();

  private loopAssinatura?: Subscription;
  private videoElemento?: HTMLVideoElement;
  private canvasElemento?: HTMLCanvasElement;

  // Estado para estabilização/debounce
  private contagemFramesConsecutivos: Record<number, number> = {};
  private produtosAdicionadosNoCiclo = new Set<string>();

  // Estado da Simulação
  private simularAtivo = false;
  private tempoSimulacao = 0;

  iniciarDeteccao(elementoVideo?: HTMLVideoElement): void {
    this.pararDeteccao();
    this.videoElemento = elementoVideo;
    this.canvasElemento = document.createElement('canvas');
    this.contagemFramesConsecutivos = {};
    this.produtosAdicionadosNoCiclo.clear();

    // Loop a cada 300ms
    this.loopAssinatura = interval(300).subscribe(() => {
      if (this.videoElemento && this.videoElemento.readyState === this.videoElemento.HAVE_ENOUGH_DATA) {
        this.capturarEEnviarFrame();
      } else {
        // Se não houver feed de câmera ativo, executa o modo simulação automática
        this.executarSimulacao();
      }
    });
  }

  pararDeteccao(): void {
    if (this.loopAssinatura) {
      this.loopAssinatura.unsubscribe();
      this.loopAssinatura = undefined;
    }
    this.subDeteccoes.next([]);
    this.simularAtivo = false;
    this.tempoSimulacao = 0;
  }

  private capturarEEnviarFrame(): void {
    if (!this.videoElemento || !this.canvasElemento) return;

    const ctx = this.canvasElemento.getContext('2d');
    if (!ctx) return;

    // Redimensionar para inferência YOLO padrão
    this.canvasElemento.width = 640;
    this.canvasElemento.height = 640;
    ctx.drawImage(this.videoElemento, 0, 0, 640, 640);

    const imagemBase64 = this.canvasElemento.toDataURL('image/jpeg', 0.8);
    // Remove cabeçalho data:image/jpeg;base64,
    const dadosBase64 = imagemBase64.split(',')[1];

    this.http.post<RespostaInferencia>(this.URL_API, { imagem: dadosBase64 })
      .pipe(
        catchError(() => {
          // Em caso de erro na API (backend offline), cai para a simulação
          this.executarSimulacao();
          return of({ deteccoes: [] } as RespostaInferencia);
        })
      )
      .subscribe((resposta) => {
        if (resposta.deteccoes && resposta.deteccoes.length > 0) {
          this.processarDeteccoesReais(resposta.deteccoes);
        }
      });
  }

  private processarDeteccoesReais(deteccoes: RespostaInferencia['deteccoes']): void {
    const objetos: ObjetoDetectado[] = [];
    const classesDetectadasNesseFrame = new Set<number>();

    for (const d of deteccoes) {
      const produto = this.servicoCatalogo.buscarPorClasse(d.class_name);
      if (!produto) continue;

      classesDetectadasNesseFrame.add(d.class_id);

      // Incrementa o contador de frames consecutivos acima do limiar
      if (d.confidence >= this.LIMIAR_CONFIANCA) {
        this.contagemFramesConsecutivos[d.class_id] = (this.contagemFramesConsecutivos[d.class_id] || 0) + 1;
      } else {
        this.contagemFramesConsecutivos[d.class_id] = 0;
      }

      const frames = this.contagemFramesConsecutivos[d.class_id];
      const confirmado = frames >= this.FRAMES_PARA_CONFIRMAR;

      // Se confirmado e ainda não adicionado neste ciclo de checkout
      if (confirmado && !this.produtosAdicionadosNoCiclo.has(produto.sku)) {
        this.subProdutoConfirmado.next(produto);
        this.produtosAdicionadosNoCiclo.add(produto.sku);
      }

      // Converte bbox normalizado para percentual css
      const topo = d.bbox.y1 * 100;
      const esquerda = d.bbox.x1 * 100;
      const largura = (d.bbox.x2 - d.bbox.x1) * 100;
      const altura = (d.bbox.y2 - d.bbox.y1) * 100;

      objetos.push({
        id: `real_${d.class_id}_${Math.random()}`,
        rotulo: confirmado ? `${produto.nome} - R$ ${produto.preco.toFixed(2)}` : 'Identificando...',
        status: confirmado ? 'confirmado' : 'identificando',
        caixa: { topo, esquerda, largura, altura },
        classeId: d.class_id,
        confianca: d.confidence
      });
    }

    // Zera contagem de classes que sumiram do frame
    for (const classeIdStr of Object.keys(this.contagemFramesConsecutivos)) {
      const id = parseInt(classeIdStr, 10);
      if (!classesDetectadasNesseFrame.has(id)) {
        this.contagemFramesConsecutivos[id] = 0;
      }
    }

    this.subDeteccoes.next(objetos);
  }

  private executarSimulacao(): void {
    this.simularAtivo = true;
    this.tempoSimulacao++;

    const objetos: ObjetoDetectado[] = [];

    // Ciclo de simulação baseado no tempoSimulacao (a cada incremento de 1 é 300ms)
    // Duração do ciclo: 40 incrementos (~12 segundos)
    const ciclo = this.tempoSimulacao % 40;

    // Leite Integral: aparece de 2 a 12 (0.6s a 3.6s)
    if (ciclo >= 2 && ciclo <= 12) {
      const classeId = 0; // Leite
      const produto = this.servicoCatalogo.buscarPorIdClasse(classeId);
      if (produto) {
        const confianca = this.calcularConfiancaSimulada(ciclo, 2, 7);
        const status = confianca >= this.LIMIAR_CONFIANCA ? 'confirmado' : 'identificando';

        if (status === 'confirmado' && !this.produtosAdicionadosNoCiclo.has(produto.sku)) {
          this.subProdutoConfirmado.next(produto);
          this.produtosAdicionadosNoCiclo.add(produto.sku);
        }

        // Simula movimento suave na esteira (esquerda para a direita)
        const progresso = (ciclo - 2) / 10; // 0 a 1
        const esquerda = 30 + progresso * 15; // 30% a 45%
        const topo = 25 + progresso * 5; // 25% a 30%

        objetos.push({
          id: 'sim_leite',
          rotulo: status === 'confirmado' ? `${produto.nome} - R$ ${produto.preco.toFixed(2)}` : 'Identificando...',
          status,
          caixa: { topo, esquerda, largura: 18, altura: 38 },
          classeId,
          confianca
        });
      }
    }

    // Mirtilos Orgânicos: aparece de 14 a 24 (4.2s a 7.2s)
    if (ciclo >= 14 && ciclo <= 24) {
      const classeId = 1; // Mirtilos
      const produto = this.servicoCatalogo.buscarPorIdClasse(classeId);
      if (produto) {
        const confianca = this.calcularConfiancaSimulada(ciclo, 14, 19);
        const status = confianca >= this.LIMIAR_CONFIANCA ? 'confirmado' : 'identificando';

        if (status === 'confirmado' && !this.produtosAdicionadosNoCiclo.has(produto.sku)) {
          this.subProdutoConfirmado.next(produto);
          this.produtosAdicionadosNoCiclo.add(produto.sku);
        }

        const progresso = (ciclo - 14) / 10;
        const esquerda = 40 + progresso * 10;
        const topo = 40 + progresso * 4;

        objetos.push({
          id: 'sim_mirtilos',
          rotulo: status === 'confirmado' ? `${produto.nome} - R$ ${produto.preco.toFixed(2)}` : 'Identificando...',
          status,
          caixa: { topo, esquerda, largura: 16, altura: 16 },
          classeId,
          confianca
        });
      }
    }

    // Água Mineral: aparece de 26 a 36 (7.8s a 10.8s)
    if (ciclo >= 26 && ciclo <= 36) {
      const classeId = 2; // Água
      const produto = this.servicoCatalogo.buscarPorIdClasse(classeId);
      if (produto) {
        const confianca = this.calcularConfiancaSimulada(ciclo, 26, 31);
        const status = confianca >= this.LIMIAR_CONFIANCA ? 'confirmado' : 'identificando';

        if (status === 'confirmado' && !this.produtosAdicionadosNoCiclo.has(produto.sku)) {
          this.subProdutoConfirmado.next(produto);
          this.produtosAdicionadosNoCiclo.add(produto.sku);
        }

        const progresso = (ciclo - 26) / 10;
        const esquerda = 45 + progresso * 12;
        const topo = 20 + progresso * 6;

        objetos.push({
          id: 'sim_agua',
          rotulo: status === 'confirmado' ? `${produto.nome} - R$ ${produto.preco.toFixed(2)}` : 'Identificando...',
          status,
          caixa: { topo, esquerda, largura: 14, altura: 34 },
          classeId,
          confianca
        });
      }
    }

    // Limpa a lista de controle quando reinicia o ciclo
    if (ciclo === 0) {
      this.produtosAdicionadosNoCiclo.clear();
    }

    this.subDeteccoes.next(objetos);
  }

  private calcularConfiancaSimulada(ciclo: number, inicio: number, estabilizado: number): number {
    if (ciclo < inicio) return 0;
    if (ciclo >= estabilizado) return 0.92 + Math.random() * 0.05; // Confiança alta
    // Sobe a confiança gradualmente
    const progresso = (ciclo - inicio) / (estabilizado - inicio);
    return 0.3 + progresso * 0.45 + Math.random() * 0.05;
  }

  limparCicloProdutos(): void {
    this.produtosAdicionadosNoCiclo.clear();
  }
}
