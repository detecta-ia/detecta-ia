using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Necessário para o FirstOrDefaultAsync
using BaseApi.Domain.Entidades;
using BaseApi.Infrastructure.Dados;

namespace BaseApi.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/ia")]
    public class IaController : ControllerBase
    {
        // ⚠️ SUBSTITUA 'SeuDbContext' pelo nome real do seu DbContext do Entity Framework
        // Se o seu projeto usa o padrão Repository, você pode trocar por: private readonly IProdutoRepository _produtoRepository;
        private readonly AppDbContext _context;

        public IaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("detectar-produto")]
        public async Task<IActionResult> DetectarEBuscarProduto([FromBody] ImagemRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Base64Imagem))
                return BadRequest("Imagem não enviada.");

            try
            {
                // 1. Salva a imagem temporária vinda do Angular
                string base64Data = request.Base64Imagem.Contains(",")
                    ? request.Base64Imagem.Split(',')[1]
                    : request.Base64Imagem;

                byte[] bytes = Convert.FromBase64String(base64Data);
                string caminhoImagemTemp = Path.Combine(Directory.GetCurrentDirectory(), "temp_frame.jpg");
                await System.IO.File.WriteAllBytesAsync(caminhoImagemTemp, bytes);

                // 2. Executa o script Python (YOLO)
                string caminhoScriptPython = @"C:\Users\Aluno\Downloads\AAAA\ApiExemplo\backend-python\principal.py";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"{caminhoScriptPython}\" --source \"{caminhoImagemTemp}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                string resultadoDoYolo = "";
                using (Process process = Process.Start(startInfo))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        resultadoDoYolo = await reader.ReadToEndAsync();
                        process.WaitForExit();
                    }
                }

                resultadoDoYolo = resultadoDoYolo.Trim();

                // 3. Processa o texto retornado pelo Python e busca no banco de dados REAL
                if (resultadoDoYolo.StartsWith("Detectado:"))
                {
                    // Extrai o nome da classe (ex: "mouse")
                    string classeDetectada = resultadoDoYolo.Split(':')[1].Split('(')[0].Trim().ToLower();

                    // BUSCA REAL NO BANCO DE DADOS:
                    // Procura na tabela de Produtos um registro onde o nome seja igual ao detectado pela IA
                    var produtoEncontrado = await _context.Produtos
                        .FirstOrDefaultAsync(p => p.Nome.ToLower() == classeDetectada);

                    if (produtoEncontrado != null)
                    {
                        // Retorna o produto real direto do seu banco (com preço atualizado, Guid, etc.)
                        return Ok(new
                        {
                            sucesso = true,
                            mensagem = resultadoDoYolo,
                            produto = produtoEncontrado
                        });
                    }
                }

                return NotFound(new
                {
                    sucesso = false,
                    mensagem = $"A IA detectou '{resultadoDoYolo}', mas esse produto não está cadastrado no banco de dados."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no Controller: {ex.Message}");
            }
        }
    }

    public class ImagemRequest
    {
        public string Base64Imagem { get; set; } = string.Empty;
    }
}