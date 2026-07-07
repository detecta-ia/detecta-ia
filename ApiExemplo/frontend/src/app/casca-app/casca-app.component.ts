import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Title } from '@angular/platform-browser';
import { Subscription } from 'rxjs';
import { ComponenteBarraSuperior } from '../barra-superior/barra-superior.component';
import { ComponenteBarraLateral } from '../barra-lateral/barra-lateral.component';
import { ComponentePainelScan } from '../painel-scan/painel-scan.component';
import { ComponentePainelCarrinho } from '../painel-carrinho/painel-carrinho.component';
import { ServicoDeteccao } from '../servicos/servico-deteccao.service';
import { ItemCarrinhoCompra } from '../modelos/item-carrinho.model';
import { EntradaCatalogoProduto } from '../modelos/entrada-catalogo.model';

@Component({
  selector: 'casca-app',
  standalone: true,
  imports: [
    CommonModule,
    ComponenteBarraSuperior,
    ComponenteBarraLateral,
    ComponentePainelScan,
    ComponentePainelCarrinho
  ],
  templateUrl: './casca-app.component.html',
  styleUrls: ['./casca-app.component.scss']
})
export class ComponenteCascaApp implements OnInit, OnDestroy {
  private readonly servicoDeteccao = inject(ServicoDeteccao);
  private readonly servicoTitulo = inject(Title);

  carrinhoItens: ItemCarrinhoCompra[] = [];
  itemNavegacaoAtivo = 'scan';
  sistemaAtivo = true;
  nomeUsuario = 'Operador 042';
  urlAvatarUsuario = 'assets/imagens/avatar.png';

  private subConfirmados?: Subscription;

  ngOnInit(): void {
    this.servicoTitulo.setTitle('Interface de Escaneamento AI');

    // Assina evento de confirmação de produto para adicionar ao carrinho
    this.subConfirmados = this.servicoDeteccao.produtoConfirmado$.subscribe(
      (produto: EntradaCatalogoProduto) => {
        this.adicionarAoCarrinho(produto);
      }
    );
  }

  ngOnDestroy(): void {
    if (this.subConfirmados) {
      this.subConfirmados.unsubscribe();
    }
  }

  adicionarAoCarrinho(produto: EntradaCatalogoProduto): void {
    // Evita duplicados (conforme especificação: "se ainda não estiver na lista, é adicionado")
    const jaAdicionado = this.carrinhoItens.some(item => item.sku === produto.sku);
    if (!jaAdicionado) {
      this.carrinhoItens = [
        ...this.carrinhoItens,
        {
          id: `item_${produto.sku}_${Date.now()}`,
          nome: produto.nome,
          sku: produto.sku,
          preco: produto.preco,
          urlMiniatura: produto.urlMiniatura
        }
      ];
    }
  }

  lidarSelecaoMenu(menuId: string): void {
    this.itemNavegacaoAtivo = menuId;
    if (menuId === 'sair') {
      alert('Sessão encerrada!');
      // Mantém o estado de pausar detecção se sair
      this.servicoDeteccao.pararDeteccao();
      this.sistemaAtivo = false;
    } else {
      this.sistemaAtivo = true;
    }
  }

  lidarLimparCarrinho(): void {
    this.carrinhoItens = [];
    this.servicoDeteccao.limparCicloProdutos();
  }

  lidarFinalizarCompra(): void {
    const total = this.carrinhoItens.reduce((s, i) => s + i.preco, 0);
    alert(`Compra finalizada com sucesso! Total: R$ ${total.toFixed(2).replace('.', ',')}`);
    this.carrinhoItens = [];
    this.servicoDeteccao.limparCicloProdutos();
  }
}
