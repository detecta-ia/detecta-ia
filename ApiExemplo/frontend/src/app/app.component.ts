import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResumoCompraComponent } from './resumo-compra/resumo-compra.component';
import { PainelPagamentoComponent } from './painel-pagamento/painel-pagamento.component';
import { CarrinhoService, DadosCarrinho } from './servicos/carrinho.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, ResumoCompraComponent, PainelPagamentoComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  carrinho: DadosCarrinho | null = null;
  carregando = true;
  erroCarregamento: string | null = null;
  processandoPagamento = false;
  erroPagamento: string | null = null;
  pagamentoConcluido = false;
  formaPagamentoEscolhida: number | null = null;

  modoDemonstracao = false;
  conexaoServidor = true;

  constructor(private carrinhoService: CarrinhoService) {}

  ngOnInit(): void {
    this.obterRevisaoCarrinho();
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

    this.carrinhoService.obterRevisaoCarrinho().subscribe({
      next: (resultado) => {
        this.carrinho = resultado.carrinho;
        this.modoDemonstracao = resultado.modoDemonstracao;
        this.conexaoServidor = !resultado.modoDemonstracao;
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
      // Simulação offline com delay
      setTimeout(() => {
        // Simulação de erro 400 intencional para testes
        if (formaPagamentoId === 3 && Math.random() > 0.8) {
          this.erroPagamento = 'Erro da API (400 Bad Request): Limite de transações físicas diárias excedido para o terminal de checkout.';
          this.processandoPagamento = false;
          return;
        }
        this.pagamentoConcluido = true;
        this.processandoPagamento = false;
      }, 1800);
      return;
    }

    this.carrinhoService.avancarPagamento(formaPagamentoId).subscribe({
      next: () => {
        this.pagamentoConcluido = true;
        this.processandoPagamento = false;
      },
      error: (erro) => {
        if (erro.status === 400) {
          const dados = erro.error;
          this.erroPagamento = dados?.mensagem || dados?.error || dados?.title || 'Erro de validação na API (400 Bad Request).';
        } else {
          this.erroPagamento = `Erro de conectividade com a API do backend. Status: ${erro.status || 'desconhecido'}`;
        }
        this.processandoPagamento = false;
      }
    });
  }

  reiniciarCheckout(): void {
    this.pagamentoConcluido = false;
    this.formaPagamentoEscolhida = null;
    this.erroPagamento = null;
    this.obterRevisaoCarrinho();
  }

  obterNomeFormaPagamento(id: number | null): string {
    switch (id) {
      case 1: return 'PIX QR Code';
      case 2: return 'Cartão de Crédito/Débito';
      case 3: return 'Dinheiro (Caixa Físico)';
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
