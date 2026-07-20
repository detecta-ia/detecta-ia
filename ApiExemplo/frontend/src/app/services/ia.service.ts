import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class IaService {
  private urlDetectar = `${environment.apiUrl}/ia/detectar`;

  constructor(private http: HttpClient) { }

  // Envia a imagem em formato Base64 para o Backend
  enviarImagemParaDetecao(base64Imagem: string): Observable<any> {
    const corpoRequisicao = { base64Imagem: base64Imagem };
    return this.http.post<any>(this.urlDetectar, corpoRequisicao);
  }
}