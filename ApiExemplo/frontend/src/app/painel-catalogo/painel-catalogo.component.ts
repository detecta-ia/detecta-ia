import { Component, OnInit, OnDestroy, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ServicoCatalogoProdutos } from '../servicos/servico-catalogo.service';
import { ServicoProdutoApi } from '../servicos/servico-produto-api.service';
import { ProdutoEstoque } from '../modelos/produto-estoque.model';
import { CriarProdutoRequisicao } from '../modelos/produto-api.model';

@Component({
  selector: 'painel-catalogo',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './painel-catalogo.component.html',
  styleUrls: ['./painel-catalogo.component.scss']
})
export class ComponentePainelCatalogo implements OnInit, OnDestroy {
  private readonly servicoCatalogo = inject(ServicoCatalogoProdutos);
  private readonly servicoProdutoApi = inject(ServicoProdutoApi);
  private readonly detectorMudancas = inject(ChangeDetectorRef);
  private inscricaoProdutos?: Subscription;
  private temporizadorToast?: ReturnType<typeof setTimeout>;

  listaProdutos: ProdutoEstoque[] = [];
  termoBusca = '';
  paginaAtual = 1;
  itensPorPagina = 10;

  // Total base de catálogo conforme a imagem (1,284)
  totalBaseCatalogo = 1284;

  // Controle do modal de criação
  exibirModalNovoProduto = false;
  novoProduto: Omit<ProdutoEstoque, 'id'> = {
    nome: '',
    categoria: 'Eletrônicos',
    subcategoria: 'Acessórios',
    sku: '',
    preco: 0,
    estoqueQuantidade: 0,
    unidadeMedida: 'UN',
    urlMiniatura: 'assets/imagens/fone-headphone.png'
  };

  // Estado de integração com a API
  carregandoSalvar = false;
  mensagemSucesso = '';
  mensagemErro = '';

  ngOnInit(): void {
    // Garante que a lista inicial é obtida imediatamente do serviço
    this.listaProdutos = this.servicoCatalogo.obterProdutosEstoque();
    this.detectorMudancas.markForCheck();

    this.inscricaoProdutos = this.servicoCatalogo.produtosEstoque$.subscribe(produtos => {
      this.listaProdutos = produtos;
      this.detectorMudancas.markForCheck();
    });
  }

  ngOnDestroy(): void {
    if (this.inscricaoProdutos) {
      this.inscricaoProdutos.unsubscribe();
    }
    if (this.temporizadorToast) {
      clearTimeout(this.temporizadorToast);
    }
  }

  get produtosFiltrados(): ProdutoEstoque[] {
    if (!this.termoBusca.trim()) {
      return this.listaProdutos;
    }
    const termo = this.termoBusca.toLowerCase().trim();
    return this.listaProdutos.filter(p =>
      p.nome.toLowerCase().includes(termo) ||
      p.sku.toLowerCase().includes(termo) ||
      p.categoria.toLowerCase().includes(termo) ||
      p.subcategoria.toLowerCase().includes(termo)
    );
  }

  get produtosPaginados(): ProdutoEstoque[] {
    const inicio = (this.paginaAtual - 1) * this.itensPorPagina;
    return this.produtosFiltrados.slice(inicio, inicio + this.itensPorPagina);
  }

  get totalProdutosCalculado(): number {
    const offset = this.listaProdutos.length - 3;
    return Math.max(0, this.totalBaseCatalogo + offset);
  }

  get totalPaginas(): number {
    return Math.ceil(this.produtosFiltrados.length / this.itensPorPagina) || 1;
  }

  irParaPagina(pagina: number): void {
    if (pagina >= 1 && pagina <= this.totalPaginas) {
      this.paginaAtual = pagina;
    }
  }

  abrirModalNovoProduto(): void {
    this.novoProduto = {
      nome: '',
      categoria: 'Eletrônicos',
      subcategoria: 'Áudio',
      sku: `SKU-${Math.floor(1000 + Math.random() * 9000)}`,
      preco: 199.90,
      estoqueQuantidade: 15,
      unidadeMedida: 'UN',
      urlMiniatura: 'assets/imagens/fone-headphone.png'
    };
    this.mensagemErro = '';
    this.exibirModalNovoProduto = true;
  }

  fecharModalNovoProduto(): void {
    this.exibirModalNovoProduto = false;
    this.mensagemErro = '';
  }

  /**
   * Envia o produto para o backend via POST /api/Produtos.
   * Em caso de sucesso, adiciona o produto à lista local e exibe toast.
   * Em caso de erro, exibe a mensagem de erro inline no modal.
   */
  salvarNovoProduto(): void {
    // Validação local antes de enviar
    if (!this.novoProduto.nome.trim()) {
      this.mensagemErro = 'Por favor, informe o nome do produto.';
      return;
    }
    if (this.novoProduto.preco <= 0) {
      this.mensagemErro = 'O preço deve ser maior que zero.';
      return;
    }
    if (!this.novoProduto.categoria.trim()) {
      this.mensagemErro = 'Por favor, informe a categoria do produto.';
      return;
    }

    // Montar o payload que o backend espera (apenas nome, preco, categoria)
    const requisicao: CriarProdutoRequisicao = {
      nome: this.novoProduto.nome.trim(),
      preco: this.novoProduto.preco,
      categoria: this.novoProduto.categoria.trim()
    };

    // Ativar loading e limpar erros anteriores
    this.carregandoSalvar = true;
    this.mensagemErro = '';

    this.servicoProdutoApi.criarProduto(requisicao).subscribe({
      next: (resposta) => {
        this.carregandoSalvar = false;

        // Adiciona à lista local para feedback imediato na UI
        this.servicoCatalogo.adicionarProdutoEstoque(this.novoProduto);

        // Fecha o modal e exibe toast de sucesso
        this.fecharModalNovoProduto();
        this.exibirToastSucesso(
          resposta.mensagem || `Produto "${requisicao.nome}" criado com sucesso!`
        );

        this.detectorMudancas.markForCheck();
      },
      error: (mensagemErro: string) => {
        this.carregandoSalvar = false;
        this.mensagemErro = mensagemErro;
        this.detectorMudancas.markForCheck();
      }
    });
  }

  /** Exibe um toast de sucesso que desaparece automaticamente após 4 segundos. */
  private exibirToastSucesso(mensagem: string): void {
    this.mensagemSucesso = mensagem;

    if (this.temporizadorToast) {
      clearTimeout(this.temporizadorToast);
    }

    this.temporizadorToast = setTimeout(() => {
      this.mensagemSucesso = '';
      this.detectorMudancas.markForCheck();
    }, 4000);
  }

  fecharToastSucesso(): void {
    this.mensagemSucesso = '';
    if (this.temporizadorToast) {
      clearTimeout(this.temporizadorToast);
    }
  }

  removerProduto(id: string): void {
    if (confirm('Deseja realmente remover este produto do catálogo?')) {
      this.servicoCatalogo.removerProdutoEstoque(id);
    }
  }

  exportarCatalogo(): void {
    alert('Relatório de catálogo exportado com sucesso!');
  }
}
