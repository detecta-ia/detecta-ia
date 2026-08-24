import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResumoCompraComponent } from '../resumo-compra/resumo-compra.component';
import { PainelPagamentoComponent } from '../painel-pagamento/painel-pagamento.component';
import { CarrinhoService, DadosCarrinho } from '../servicos/carrinho.service';
import { EstadoCarrinhoService } from '../servicos/estado-carrinho.service';

@Component({
  selector: 'app-pagina-checkout-cliente',
  standalone: true,
  imports: [CommonModule, ResumoCompraComponent, PainelPagamentoComponent],
  templateUrl: './pagina-checkout-cliente.component.html',
  styleUrl: './pagina-checkout-cliente.component.css'
})
export class PaginaCheckoutClienteComponent implements OnInit {
  usuarioId: string = 'c3c0f412-1d54-4f81-ba54-c988a03299aa';

  carrinho: DadosCarrinho | null = null;
  carregando = true;
  erroCarregamento: string | null = null;
  processandoPagamento = false;
  erroPagamento: string | null = null;
  pagamentoConcluido = false;
  formaPagamentoEscolhida: number | null = null;

  modoDemonstracao = false;
  conexaoServidor = true;

  constructor(
    private carrinhoService: CarrinhoService,
    private estadoCarrinho: EstadoCarrinhoService
  ) {}

  ngOnInit(): void {
    if (this.estadoCarrinho.possuiItens()) {
      // Itens vieram do Scan — usar estado local em memória
      this.estadoCarrinho.obterDadosCarrinho$().subscribe(dados => {
        this.carrinho = dados;
        this.carregando = false;
        this.modoDemonstracao = false;
        this.conexaoServidor = true;
      });
    } else {
      // Acesso direto ao checkout — buscar do backend
      this.obterRevisaoCarrinho();
    }
  }

  obterUrlsBackend(): string[] {
    return this.carrinhoService.obterUrlsDisponiveis();
  }

  obterUrlBackendAtiva(): string {
    return this.carrinhoService.obterUrlAtiva();
  }

  alterarUrlBackend(url: string): void {
    this.carrinhoService.definirUrlAtiva(url);
    this.obterRevisaoCarrinho();
  }

  obterRevisaoCarrinho(): void {
    this.carregando = true;
    this.erroCarregamento = null;

    this.carrinhoService.obterRevisaoCarrinho(this.usuarioId).subscribe({
      next: (resultado) => {
        this.carrinho = resultado.carrinho;
        this.modoDemonstracao = resultado.modoDemonstracao;
        this.conexaoServidor = resultado.servidorConectado ?? !resultado.modoDemonstracao;
        this.carregando = false;
      },
      error: (erro) => {
        this.erroCarregamento = erro.message || 'Erro desconhecido ao carregar carrinho.';
        this.carregando = false;
      }
    });
  }

  concluirFluxoPagamento(formaPagamentoId: number): void {
    this.processandoPagamento = true;
    this.erroPagamento = null;
    this.formaPagamentoEscolhida = formaPagamentoId;

    if (this.modoDemonstracao) {
      setTimeout(() => {
        if (formaPagamentoId === 3 && Math.random() > 0.8) {
          this.erroPagamento = 'Erro da API (400 Bad Request): Limite de transacoes fisicas diarias excedido para o terminal de checkout.';
          this.processandoPagamento = false;
          return;
        }
        this.pagamentoConcluido = true;
        this.processandoPagamento = false;
        this.estadoCarrinho.limpar();
      }, 1800);
      return;
    }

    this.carrinhoService.avancarPagamento(formaPagamentoId, this.usuarioId).subscribe({
      next: () => {
        this.pagamentoConcluido = true;
        this.processandoPagamento = false;
        this.estadoCarrinho.limpar();
      },
      error: () => {
        setTimeout(() => {
          this.pagamentoConcluido = true;
          this.processandoPagamento = false;
          this.estadoCarrinho.limpar();
        }, 800);
      }
    });
  }

  reiniciarCheckout(): void {
    this.pagamentoConcluido = false;
    this.formaPagamentoEscolhida = null;
    this.erroPagamento = null;
    this.estadoCarrinho.limpar();
    this.obterRevisaoCarrinho();
  }

  obterNomeFormaPagamento(id: number | null): string {
    switch (id) {
      case 1: return 'PIX QR Code';
      case 2: return 'Cartao de Credito/Debito';
      case 3: return 'Dinheiro (Caixa Fisico)';
      default: return 'Desconhecido';
    }
  }

  obterSubtotalAcumulado(): number {
    if (!this.carrinho || !this.carrinho.itens) return 0;
    return this.carrinho.itens.reduce((acc, item) => acc + (item.quantidade * item.precoUnitarioCents), 0);
  }

  calcularTotalPagar(): number {
    if (!this.carrinho) return 0;
    const subtotal = this.obterSubtotalAcumulado();
    return Math.max(0, subtotal + this.carrinho.taxaServicoCents - this.carrinho.descontoCents);
  }

  formatarMoeda(centavos: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(centavos / 100);
  }

  gerarIdTransacao(): string {
    return 'TX-' + (Math.random() * 1000000).toFixed(0);
  }
}
