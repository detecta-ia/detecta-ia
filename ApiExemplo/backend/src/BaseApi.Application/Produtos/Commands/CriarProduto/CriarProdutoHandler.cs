using BaseApi.Domain.Entidades;
using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using System.IO;

namespace BaseApi.Application.Produtos.Commands.CriarProduto
{
    public class CriarProdutoHandler(IProdutoRepositorio repositorio) : IRequestHandler<CriarProdutoCommand, CriarProdutoResposta>
    {
        public async Task<CriarProdutoResposta> Handle(CriarProdutoCommand request, CancellationToken cancellationToken)
        {
            var produto = new Produto
            {
                Nome = request.Nome,
                Preco = request.Preco,
                Categoria = request.Categoria,
                CriadoEm = DateTime.UtcNow,
                AtualizadoEm = DateTime.UtcNow
            };

            // Salvar produto inicialmente para gerar o Id
            await repositorio.AdicionarAsync(produto, cancellationToken);

            // Processar imagem Base64, se fornecida
            if (!string.IsNullOrWhiteSpace(request.ImagemBase64))
            {
                try
                {
                    var bytes = Convert.FromBase64String(request.ImagemBase64);
                    const int maxSize = 2 * 1024 * 1024; // 2 MB
                    if (bytes.Length > maxSize)
                        throw new Exception($"Tamanho da imagem excede {maxSize} bytes.");

                    // Detectar tipo da imagem
                    string ext;
                    if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                        ext = ".png"; // PNG signature
                    else if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                        ext = ".jpg"; // JPEG signature
                    else
                        throw new Exception("Formato de imagem não suportado. Use PNG ou JPEG.");

                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    Directory.CreateDirectory(uploadFolder);
                    var filePath = Path.Combine(uploadFolder, $"{produto.Id}{ext}");
                    await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);

                    produto.ImagemBase64 = request.ImagemBase64;
                }
                catch (Exception ex)
                {
                    // Falha ao processar a imagem, aborta a operação
                    throw new Exception($"Falha ao processar a imagem: {ex.Message}");
                }
            }

            // Atualizar entidade com possível ImagemUrl
            repositorio.Atualizar(produto);
            await repositorio.AdicionarAsync(produto, cancellationToken);
            await repositorio.SalvarAsync(cancellationToken);

            return new CriarProdutoResposta(produto.Id, produto.Nome, produto.Preco, produto.Categoria);
        }
    }
}
