namespace BaseApi.Domain.Entidades;

public class Carrinho
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
    public string Status { get; set; } = "ATIVO"; // ATIVO, FINALIZADO
    public List<CarrinhoItem> Itens { get; set; } = new();
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
}