using MediatR;

namespace BaseApi.Application.Carrinhos.Queries.ObterCarrinhoRevisao;

public record ObterCarrinhoRevisaoQuery(Guid UsuarioId) : IRequest<CarrinhoRevisaoDto>;

public record CarrinhoRevisaoDto(
    Guid CarrinhoId,
    decimal TotalGeral,
    List<GrupoCategoriaDto> Categorias
);

public record GrupoCategoriaDto(
    string Categoria, // Ex: "Hortifrúti", "Padaria" [RN01]
    List<ItemRevisaoDto> Itens
);

public record ItemRevisaoDto(
    Guid Id,
    Guid ProdutoId,
    string Nome,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal, // Atende ao critério de exibição do subtotal da linha
    string ImagemUrl // [RN02] Imagem do produto vinda do banco
);