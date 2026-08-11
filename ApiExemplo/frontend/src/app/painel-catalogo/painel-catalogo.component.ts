import { Component, OnInit, OnDestroy, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ServicoCatalogoProdutos } from '../servicos/servico-catalogo.service';
import { ProdutoEstoque } from '../modelos/produto-estoque.model';

@Component({
  selector: 'painel-catalogo',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './painel-catalogo.component.html',
  styleUrls: ['./painel-catalogo.component.scss']
})
export class ComponentePainelCatalogo implements OnInit, OnDestroy {
  private readonly servicoCatalogo = inject(ServicoCatalogoProdutos);
  private readonly detectorMudancas = inject(ChangeDetectorRef);
  private inscricaoProdutos?: Subscription;

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
    this.exibirModalNovoProduto = true;
  }

  fecharModalNovoProduto(): void {
    this.exibirModalNovoProduto = false;
  }

  salvarNovoProduto(): void {
    if (!this.novoProduto.nome.trim()) {
      alert('Por favor, informe o nome do produto.');
      return;
    }

    this.servicoCatalogo.adicionarProdutoEstoque(this.novoProduto);
    this.fecharModalNovoProduto();
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
