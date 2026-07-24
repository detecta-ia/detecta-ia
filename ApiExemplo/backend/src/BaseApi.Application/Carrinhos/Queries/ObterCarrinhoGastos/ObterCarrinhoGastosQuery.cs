using MediatR;

namespace BaseApi.Application.Carrinhos.Queries.ObterResumoGastos;

/// <summary>
/// Query que retorna o resumo de gastos do usuário autenticado.
/// User Story: "Como cliente, eu quero visualizar um resumo dos meus gastos,
/// para que eu acompanhe minhas despesas ao longo do tempo."
/// </summary>
public record ObterResumoGastosQuery(Guid UsuarioId) : IRequest<ResumoGastosDto>;

/// <summary>
/// [RN01] Todos os valores são calculados automaticamente a partir das compras
/// (Carrinhos com Status = "FINALIZADO") registradas para o usuário.
/// [RN02] Como o cálculo é feito em tempo real a cada requisição, os indicadores
/// já refletem automaticamente qualquer nova compra concluída.
/// </summary>
public record ResumoGastosDto(
    decimal TotalGasto,
    int NumeroDeCompras,
    decimal MediaGastoPorCompra,
    DateTime? UltimaCompraRealizada
);
