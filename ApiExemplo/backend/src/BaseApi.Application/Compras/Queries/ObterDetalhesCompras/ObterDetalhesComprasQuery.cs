using MediatR;

namespace BaseApi.Application.Compras.Queries.ObterDetalhesCompra;

/// <summary>
/// Busca os detalhes completos de uma compra específica do histórico do usuário.
/// Uma "compra" é um Carrinho com Status = "FINALIZADO".
/// </summary>
public record ObterDetalhesCompraQuery(Guid CompraId, Guid UsuarioId) : IRequest<CompraDetalheDto>;

public record CompraDetalheDto(
    Guid CompraId,
    DateTime DataCompra,
    List<ItemCompraDto> Itens,
    decimal ValorTotal
);

public record ItemCompraDto(
    Guid Id,
    Guid ProdutoId,
    string NomeProduto,       // [Critério] Exibir nome dos produtos
    int Quantidade,           // [Critério] Exibir quantidade de cada produto
    decimal PrecoUnitario,    // [Critério] Exibir preço unitário
    decimal Subtotal          // [Critério] Exibir subtotal por item
);
