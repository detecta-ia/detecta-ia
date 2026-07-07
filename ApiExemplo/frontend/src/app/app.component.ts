import { Component } from '@angular/core';
import { ComponenteCascaApp } from './casca-app/casca-app.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ComponenteCascaApp],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'detecta-ia-frontend';
}

