using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BaseApi.Application.Produtos.Commands.AtualizarProduto
{
    public class AtualizarProdutoHandler : IRequest<Unit>
    {
        private readonly IProdutoRepositorio _repositorio;

        public AtualizarProdutoHandler(IProdutoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<Unit> Handle(AtualizarProdutoCommand command, CancellationToken ct)
        {
            var produto = await _repositorio.ObterPorIdAsync(command.Id, ct)
                ?? throw new Exception($"Produto com Id '{command.Id}' não encontrado.");

            produto.Nome = command.Nome;
            produto.Categoria = command.Categoria;
            produto.Preco = command.Preco;

            produto.AtualizadoEm = DateTime.UtcNow;

            _repositorio.Atualizar(produto);
            await _repositorio.SalvarAsync(ct);

            return Unit.Value;
        }
    }
}
