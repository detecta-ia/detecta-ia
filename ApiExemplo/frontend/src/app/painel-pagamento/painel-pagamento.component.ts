import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-painel-pagamento',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './painel-pagamento.component.html'
})
export class PainelPagamentoComponent implements OnChanges, OnDestroy {
  @Input() subtotal = 0;
  @Input() taxaServicoCents = 0;
  @Input() descontoCents = 0;
  @Input() processando = false;
  @Input() erroApi: string | null = null;

  @Output() aoConcluir = new EventEmitter<number>();

  formaPagamento: number | null = null;
  tempoRestante = 299;
  private intervaloId: any = null;

  get totalAPagar(): number {
    return Math.max(0, this.subtotal + this.taxaServicoCents - this.descontoCents);
  }

  ngOnChanges(mudancas: SimpleChanges): void {}

  ngOnDestroy(): void {
    this.pararCronometro();
  }

  selecionarFormaPagamento(id: number): void {
    this.formaPagamento = id;
    this.pararCronometro();

    if (id === 1) {
      this.tempoRestante = 299;
      this.intervaloId = setInterval(() => {
        if (this.tempoRestante <= 1) {
          this.tempoRestante = 0;
          this.pararCronometro();
        } else {
          this.tempoRestante--;
        }
      }, 1000);
    }
  }

  formatarMoeda(centavos: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(centavos / 100);
  }

  formatarTempo(segundos: number): string {
    const minutos = Math.floor(segundos / 60);
    const segsRestantes = segundos % 60;
    return `${String(minutos).padStart(2, '0')}:${String(segsRestantes).padStart(2, '0')}`;
  }

  lidarComEnvio(): void {
    if (this.formaPagamento) {
      this.aoConcluir.emit(this.formaPagamento);
    }
  }

  private pararCronometro(): void {
    if (this.intervaloId) {
      clearInterval(this.intervaloId);
      this.intervaloId = null;
    }
  }
}
