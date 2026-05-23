using ZeroFrame.Application.DTOS.Categoria;
using ZeroFrame.Application.DTOS.Common;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Interfaces;

namespace ZeroFrame.Application.Servicos
{
    // Serviço responsável pelas regras de negócio da Categoria.
    // Ele faz a comunicação entre a Controller e o Repository.
    // Também realiza a conversão entre DTOs e Entidades.
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        // Recebe o repositório por injeção de dependência.
        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        // Busca todas as categorias cadastradas.
        public async Task<List<CategoriaGetDto>> ObterTodosAsync()
        {
            var categorias = await _categoriaRepository.ObterTodosAsync();

            // Converte a lista de entidades para lista de DTOs.
            return categorias.Select(MapearCategoriaGetDto).ToList();
        }

        public async Task<PagedResponse<CategoriaGetDto>> ObterTodosPaginadoAsync(PaginationParams paginationParams)
        {
            var resultado = await _categoriaRepository.ObterTodosPaginadoAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize);

            var items = resultado.Items.Select(MapearCategoriaGetDto).ToList();
            return PagedResponse<CategoriaGetDto>.Create(items, resultado.TotalItems, paginationParams);
        }

        // Busca uma categoria pelo Id.
        public async Task<CategoriaGetDto?> ObterPorIdAsync(int id)
        {
            var categoria = await _categoriaRepository.ObterPorIdAsync(id);

            // Caso não encontre, retorna nulo.
            if (categoria == null)
                return null;

            // Converte a entidade encontrada para DTO.
            return MapearCategoriaGetDto(categoria);
        }

        // Cria uma nova categoria.
        public async Task<CategoriaGetDto> CriarAsync(CategoriaPostDto categoriaPostDto)
        {
            // Converte o DTO recebido em entidade.
            var categoria = new Categoria
            {
                Nome = categoriaPostDto.Nome,
                Descricao = categoriaPostDto.Descricao ?? string.Empty
            };

            await _categoriaRepository.AdicionarAsync(categoria);

            // Retorna os dados cadastrados em formato DTO.
            return MapearCategoriaGetDto(categoria);
        }

        // Atualiza uma categoria existente.
        public async Task AtualizarAsync(CategoriaPutDto categoriaPutDto)
        {
            var categoria = await _categoriaRepository.ObterPorIdAsync(categoriaPutDto.Id);

            // Se não existir, encerra o método.
            if (categoria == null)
                return;

            // Atualiza os dados da categoria.
            categoria.Nome = categoriaPutDto.Nome;
            categoria.Descricao = categoriaPutDto.Descricao ?? string.Empty;

            await _categoriaRepository.AtualizarAsync(categoria);
        }

        // Remove uma categoria pelo Id.
        public async Task RemoverAsync(int id)
        {
            await _categoriaRepository.RemoverAsync(id);
        }

        // Converte a entidade Categoria para CategoriaGetDto.
        private static CategoriaGetDto MapearCategoriaGetDto(Categoria categoria)
        {
            return new CategoriaGetDto
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descricao = categoria.Descricao
            };
        }
    }
}
