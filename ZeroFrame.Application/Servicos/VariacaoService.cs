using ZeroFrame.Application.DTOS;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;

namespace ZeroFrame.Application.Servicos
{
    // Serviço responsável por concentrar as regras de negócio da variação de produtos.
    // Ele faz a comunicação entre a Controller e o Repository.
    // Também realiza a conversão entre DTOs e Entidades.
    public class VariacaoService : IVariacaoService
    {
        private readonly IVariacaoRepository _variacaoRepository;

        // Recebe o repositório por injeção de dependência.
        public VariacaoService(IVariacaoRepository variacaoRepository)
        {
            _variacaoRepository = variacaoRepository;
        }

        // Busca todas as variações de produtos cadastradas.
        public async Task<List<VariacaoGetDto>> ObterTodosAsync()
        {
            var variacoesProdutos = await _variacaoRepository.ObterTodosAsync();

            // Converte a lista de entidades para lista de DTOs.
            return variacoesProdutos.Select(variacaoProduto => new VariacaoGetDto
            {
                Id = variacaoProduto.Id,
                Tamanho = variacaoProduto.Tamanho,
                Cor = variacaoProduto.Cor,
                Estoque = variacaoProduto.Estoque,
                ProdutoId = variacaoProduto.ProdutoId
            }).ToList();
        }

        // Busca uma variação de produto pelo Id.
        public async Task<VariacaoGetDto?> ObterPorIdAsync(int id)
        {
            var variacaoProduto = await _variacaoRepository.ObterPorIdAsync(id);

            // Caso não encontre, retorna nulo.
            if (variacaoProduto == null)
                return null;

            // Converte a entidade encontrada para DTO.
            return new VariacaoGetDto
            {
                Id = variacaoProduto.Id,
                Tamanho = variacaoProduto.Tamanho,
                Cor = variacaoProduto.Cor,
                Estoque = variacaoProduto.Estoque,
                ProdutoId = variacaoProduto.ProdutoId
            };
        }

        // Cria uma nova variação de produto.
        public async Task<VariacaoGetDto> CriarAsync(VariacaoPostDto variacaoPostDto)
        {
            // Converte o DTO recebido em entidade.
            var variacaoProduto = new VariacaoProdutos
            {
                Tamanho = variacaoPostDto.Tamanho,
                Cor = variacaoPostDto.Cor,
                Estoque = variacaoPostDto.Estoque,
                ProdutoId = variacaoPostDto.ProdutoId
            };

            await _variacaoRepository.AdicionarAsync(variacaoProduto);

            // Retorna os dados cadastrados em formato DTO.
            return new VariacaoGetDto
            {
                Id = variacaoProduto.Id,
                Tamanho = variacaoProduto.Tamanho,
                Cor = variacaoProduto.Cor,
                Estoque = variacaoProduto.Estoque,
                ProdutoId = variacaoProduto.ProdutoId
            };
        }

        // Atualiza uma variação de produto existente.
        public async Task AtualizarAsync(VariacaoPutDto variacaoPutDto)
        {
            // Converte o DTO de atualização em entidade.
            var variacaoProduto = new VariacaoProdutos
            {
                Id = variacaoPutDto.Id,
                Tamanho = variacaoPutDto.Tamanho,
                Cor = variacaoPutDto.Cor,
                Estoque = variacaoPutDto.Estoque,
                ProdutoId = variacaoPutDto.ProdutoId
            };

            await _variacaoRepository.AtualizarAsync(variacaoProduto);
        }

        // Remove uma variação de produto pelo Id.
        public async Task RemoverAsync(int id)
        {
            await _variacaoRepository.RemoverAsync(id);
        }
    }
}