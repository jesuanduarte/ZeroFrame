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
        private readonly IProdutoRepository _produtoRepository;

        // Recebe o repositório por injeção de dependência.
        public VariacaoService(IVariacaoRepository variacaoRepository, IProdutoRepository produtoRepository)
        {
            _variacaoRepository = variacaoRepository;
            _produtoRepository = produtoRepository;
        }

        // Busca todas as variações de produtos cadastradas.
        public async Task<List<VariacaoGetDto>> ObterTodosAsync()
        {
            var variacoesProdutos = await _variacaoRepository.ObterTodosAsync();

            // Converte a lista de entidades para lista de DTOs.
            return variacoesProdutos.Select(MapearVariacaoGetDto).ToList();
        }

        // Busca uma variação de produto pelo Id.
        public async Task<VariacaoGetDto?> ObterPorIdAsync(int id)
        {
            var variacaoProduto = await _variacaoRepository.ObterPorIdAsync(id);

            // Caso não encontre, retorna nulo.
            if (variacaoProduto == null)
                return null;

            // Converte a entidade encontrada para DTO.
            return MapearVariacaoGetDto(variacaoProduto);
        }

        // Busca as variações de um produto específico pelo Id do produto.
        public async Task<List<VariacaoGetDto>> ObterPorProdutoIdAsync(int produtoId)
        {
            await ValidarProdutoAsync(produtoId);

            var variacoesProdutos = await _variacaoRepository.ObterPorProdutoIdAsync(produtoId);

            return variacoesProdutos.Select(MapearVariacaoGetDto).ToList();
        }
        // Cria uma nova variação de produto.
        public async Task<VariacaoGetDto> CriarAsync(VariacaoPostDto variacaoPostDto)
        {
            await ValidarProdutoAsync(variacaoPostDto.ProdutoId);

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
            var variacaoProduto = await _variacaoRepository.ObterPorIdAsync(variacaoPutDto.Id);

            if (variacaoProduto == null)
                return;

            await ValidarProdutoAsync(variacaoPutDto.ProdutoId);

            variacaoProduto.Tamanho = variacaoPutDto.Tamanho;
            variacaoProduto.Cor = variacaoPutDto.Cor;
            variacaoProduto.Estoque = variacaoPutDto.Estoque;
            variacaoProduto.ProdutoId = variacaoPutDto.ProdutoId;

            await _variacaoRepository.AtualizarAsync(variacaoProduto);
        }

        // Método para validar se o produto existe antes de criar ou atualizar uma variação.
        private async Task ValidarProdutoAsync(int produtoId)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(produtoId);

            if (produto == null)
                throw new InvalidOperationException("Produto nao encontrado.");
        }

        // Método  para mapear uma entidade de variação de produto para um DTO de resposta.
        private static VariacaoGetDto MapearVariacaoGetDto(VariacaoProdutos variacaoProduto)
        {
            return new VariacaoGetDto
            {
                Id = variacaoProduto.Id,
                Tamanho = variacaoProduto.Tamanho,
                Cor = variacaoProduto.Cor,
                Estoque = variacaoProduto.Estoque,
                ProdutoId = variacaoProduto.ProdutoId
            };
        }
        // Remove uma variação de produto pelo Id.
        public async Task RemoverAsync(int id)
        {
            await _variacaoRepository.RemoverAsync(id);
        }
    }
}