import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'barra-superior',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './barra-superior.component.html',
  styleUrls: ['./barra-superior.component.scss']
})
export class ComponenteBarraSuperior {
  @Input() nomeUsuario = 'Operador 042';
  @Input() urlAvatarUsuario = 'assets/imagens/avatar.png';
  @Input() ativo = true;
}
