using BaseApi.Application.Comum.Modelos;
using BaseApi.Application.Produtos.Commands.AtualizarProduto;
using BaseApi.Application.Produtos.Commands.CriarProduto;
using BaseApi.Application.Produtos.Commands.ExcluirProduto;
using BaseApi.Application.Produtos.Queries.ListarProdutos;
using BaseApi.Application.Produtos.Queries.ObterProdutoPorId;
using BaseApi.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Mozilla;

namespace BaseApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProdutosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(RespostaApi<ResultadoPaginado<ProdutoListaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
    [FromQuery] int pagina = 1,
    [FromQuery] int tamanhoPagina = 10,
    [FromQuery] string? busca = null,
    CancellationToken ct = default)
    {
        var resultado = await mediator.Send(new ListarProdutosQuery(pagina, tamanhoPagina, busca), ct);
        return Ok(RespostaApi<ResultadoPaginado<ProdutoListaDto>>.Sucesso(resultado));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RespostaApi<ProdutoDetalheDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaApi), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct = default)
    {
        var resultado = await mediator.Send(new ObterProdutoPorIdQuery(id), ct);
        if (resultado is null)
            return NotFound(RespostaApi.Falha("Produto não encontrado."));
        return Ok(RespostaApi<ProdutoDetalheDto>.Sucesso(resultado));
    }

    [HttpPut]
    [ProducesResponseType(typeof(RespostaApi<ProdutoDetalheDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaApi), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaApi), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar([FromBody] AtualizarProdutoCommand comando, CancellationToken ct)
    {
        var command = new AtualizarProdutoCommand
        {
            Id = comando.Id,
            Nome = comando.Nome,
            Categoria = comando.Categoria,
            Preco = comando.Preco
        };

        await mediator.Send(command, ct);
        return Ok(RespostaApi.Sucesso("Produto atualizado com sucesso."));
    }

    [HttpPost]
    [ProducesResponseType(typeof(RespostaApi<ProdutoDetalheDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(RespostaApi), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarProdutoCommand comando, CancellationToken ct)
    {
        var resultado = await mediator.Send(comando, ct);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = resultado.Id }, RespostaApi<CriarProdutoResposta>.Sucesso(resultado, "Produto criado com sucesso."));
    }

    [HttpDelete]
    [ProducesResponseType(typeof(RespostaApi), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaApi), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        await mediator.Send(new ExcluirProdutoCommand(id), ct);
        return Ok(RespostaApi.Sucesso("Produto excluído com sucesso."));
    }
}

public record AtualizarProdutoRequest(
    Guid Id,
    string Nome,
    string Categoria,
    decimal Preco
);