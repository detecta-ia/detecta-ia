import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'caixa-deteccao',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './caixa-deteccao.component.html',
  styleUrls: ['./caixa-deteccao.component.scss']
})
export class ComponenteCaixaDeteccao {
  @Input() caixa!: { topo: number; esquerda: number; largura: number; altura: number };
  @Input() rotulo!: string;
  @Input() status: 'identificando' | 'confirmado' = 'identificando';
}
