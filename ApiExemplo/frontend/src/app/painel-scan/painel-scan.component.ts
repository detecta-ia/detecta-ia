import { Component, OnInit, OnDestroy, ViewChild, ElementRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ServicoDeteccao } from '../servicos/servico-deteccao.service';
import { ComponenteCaixaDeteccao } from './caixa-deteccao/caixa-deteccao.component';
import { ObjetoDetectado } from '../modelos/objeto-detectado.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'painel-scan',
  standalone: true,
  imports: [CommonModule, ComponenteCaixaDeteccao],
  templateUrl: './painel-scan.component.html',
  styleUrls: ['./painel-scan.component.scss']
})
export class ComponentePainelScan implements OnInit, OnDestroy {
  private readonly servicoDeteccao = inject(ServicoDeteccao);

  @ViewChild('feedVideo', { static: false }) elementoVideo?: ElementRef<HTMLVideoElement>;

  deteccoes$: Observable<ObjetoDetectado[]> = this.servicoDeteccao.deteccoes$;
  cameraAtiva = false;
  erroCamera = false;
  fluxoVideo?: MediaStream;

  ngOnInit(): void {
    this.iniciarCamera();
  }

  ngOnDestroy(): void {
    this.desativarCamera();
  }

  async iniciarCamera() {
    try {
      this.erroCamera = false;
      this.fluxoVideo = await navigator.mediaDevices.getUserMedia({
        video: { width: 640, height: 640, facingMode: 'environment' },
        audio: false
      });
      
      this.cameraAtiva = true;

      // Inicia a detecção assim que o elemento de vídeo estiver pronto
      setTimeout(() => {
        if (this.elementoVideo) {
          const video = this.elementoVideo.nativeElement;
          video.srcObject = this.fluxoVideo!;
          video.play();
          this.servicoDeteccao.iniciarDeteccao(video);
        }
      }, 300);

    } catch (erro) {
      console.warn('Câmera não disponível, iniciando modo simulação automática:', erro);
      this.erroCamera = true;
      this.cameraAtiva = false;
      // Inicia a detecção em modo simulação (sem passar elemento de vídeo)
      this.servicoDeteccao.iniciarDeteccao();
    }
  }

  desativarCamera(): void {
    this.servicoDeteccao.pararDeteccao();
    if (this.fluxoVideo) {
      this.fluxoVideo.getTracks().forEach(track => track.stop());
      this.fluxoVideo = undefined;
    }
    this.cameraAtiva = false;
  }
}
