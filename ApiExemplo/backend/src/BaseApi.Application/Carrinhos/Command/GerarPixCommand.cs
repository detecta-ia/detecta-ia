using MediatR;
using BaseApi.Application.Comum.Modelos;
using BaseApi.Domain.Interfaces.Repositorios;

namespace BaseApi.Application.Carrinhos.Command
{
    // O comando recebe o ID do carrinho para calcular o total
    public record GerarPixCommand(Guid CarrinhoId) : IRequest<RespostaApi<PixResponseDto>>;

    // DTO de retorno para o frontend
    public record PixResponseDto(string QrCodeBase64, string CopiaEVola, DateTime ExpiraEm);

    public class GerarPixCommandHandler : IRequestHandler<GerarPixCommand, RespostaApi<PixResponseDto>>
    {
        private readonly ICarrinhoRepositorio _carrinhoRepositorio;

        public GerarPixCommandHandler(ICarrinhoRepositorio carrinhoRepositorio)
        {
            _carrinhoRepositorio = carrinhoRepositorio;
        }

        public async Task<RespostaApi<PixResponseDto>> Handle(GerarPixCommand request, CancellationToken cancellationToken)
        {
            // 1.  Usando o método correto que retorna a entidade Carrinho
            var carrinho = await _carrinhoRepositorio.ObterAtivoPorUsuarioIdAsync(request.CarrinhoId, cancellationToken);

            if (carrinho == null)
            {
                return RespostaApi<PixResponseDto>.Falha("Carrinho ativo não encontrado para este usuário.");
            }

            // 2. Define o tempo de expiração para 5 minutos
            var expiraEm = DateTime.UtcNow.AddMinutes(5);

            // 3. Simula os códigos Pix baseados nos dados reais
            string codigoCopiaEColaFicticio = $"00020101021226830014br.gov.bcb.pix2561api.scaniq.com.br/v2/cob/scaniq{carrinho.Id.ToString().Substring(0, 8)}5204000053039865802BR5909SCAN_IQ6009SAO_PAULO62070503***6304ABCD";

            string qrCodePlaceholder = "PIX_QR_CODE_MOCK_BASE64";

            var resultado = new PixResponseDto(qrCodePlaceholder, codigoCopiaEColaFicticio, expiraEm);

            return RespostaApi<PixResponseDto>.Sucesso(resultado);
        }
    }
}