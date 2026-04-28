using ZeroFrame.Application.DTOS;
using ZeroFrame.Application.DTOS.Produto;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;

namespace ZeroFrame.Application.Servicos
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<List<ProdutoGetDto>> ObterTodosAsync()
        {
            var produtos = await _produtoRepository.ObterTodosAsync();
            return produtos.Select(MapearProdutoGetDto).ToList();
        }

        public async Task<ProdutoGetDto?> ObterPorIdAsync(int id)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);

            if (produto == null)
                return null;

            return MapearProdutoGetDto(produto);
        }

        public async Task<ProdutoGetDto> CriarAsync(ProdutoPostDto produtoPostDto)
        {
            var produto = new Produto
            {
                Nome = produtoPostDto.Nome,
                Descricao = produtoPostDto.Descricao,
                Preco = produtoPostDto.Preco,
                CategoriaId = produtoPostDto.CategoriaId,
                Ativo = true
            };

            await _produtoRepository.AdicionarAsync(produto);

            return MapearProdutoGetDto(produto);
        }

        public async Task AtualizarAsync(ProdutoPutDto produtoPutDto)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(produtoPutDto.Id);

            if (produto == null)
                return;

            produto.Nome = produtoPutDto.Nome;
            produto.Descricao = produtoPutDto.Descricao;
            produto.Preco = produtoPutDto.Preco;
            produto.CategoriaId = produtoPutDto.CategoriaId;
            produto.Ativo = produtoPutDto.Ativo;

            await _produtoRepository.AtualizarAsync(produto);
        }

        public async Task RemoverAsync(int id)
        {
            await _produtoRepository.RemoverAsync(id);
        }

        private static ProdutoGetDto MapearProdutoGetDto(Produto produto)
        {
            return new ProdutoGetDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                CategoriaId = produto.CategoriaId,
                Ativo = produto.Ativo,
                Variacoes = produto.VariacoesProdutos.Select(variacao => new VariacaoGetDto
                {
                    Id = variacao.Id,
                    Tamanho = variacao.Tamanho,
                    Cor = variacao.Cor,
                    Estoque = variacao.Estoque,
                    ProdutoId = variacao.ProdutoId
                }).ToList()
            };
        }
    }
}