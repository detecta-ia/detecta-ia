namespace BaseApi.Domain.Entidades;

public class CarrinhoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CarrinhoId { get; set; }
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public int PrecoUnitarioCents { get; set; }

    // Propriedade de Navegação para buscar a Imagem e Tipo do Produto cadastrado
    public Produto? Produto { get; set; }
}