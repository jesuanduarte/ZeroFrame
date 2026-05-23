using ZeroFrame.Application.DTOS.AvaliacaoProduto;
using ZeroFrame.Application.DTOS.Common;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Interfaces;

namespace ZeroFrame.Application.Servicos
{
    public class AvaliacaoProdutoService : IAvaliacaoProdutoService
    {
        private const int ComentarioTamanhoMaximo = 500;

        private readonly IAvaliacaoProdutoRepository _avaliacaoProdutoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public AvaliacaoProdutoService(
            IAvaliacaoProdutoRepository avaliacaoProdutoRepository,
            IProdutoRepository produtoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _avaliacaoProdutoRepository = avaliacaoProdutoRepository;
            _produtoRepository = produtoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<AvaliacaoProdutoGetDto> CriarAsync(int produtoId, AvaliacaoProdutoPostDto dto, int? usuarioId)
        {
            var usuario = await ObterUsuarioAutenticadoOuFalharAsync(usuarioId);
            var produto = await ObterProdutoAtivoOuFalharAsync(produtoId);

            if (dto.ProdutoId != 0 && dto.ProdutoId != produtoId)
                throw new ArgumentException("ProdutoId do corpo diferente do ProdutoId da rota.");

            ValidarNota(dto.Nota);
            ValidarComentario(dto.Comentario);

            var avaliacaoExistente = await _avaliacaoProdutoRepository.ObterAtivaPorUsuarioEProdutoAsync(usuario.Id, produtoId);

            if (avaliacaoExistente != null)
                throw new InvalidOperationException("Voce ja avaliou este produto. Utilize o endpoint de atualizacao para alterar sua avaliacao.");

            var avaliacao = new AvaliacaoProduto
            {
                UsuarioId = usuario.Id,
                Usuario = usuario,
                ProdutoId = produto.Id,
                Produto = produto,
                Nota = dto.Nota,
                Comentario = NormalizarComentario(dto.Comentario),
                Ativo = true,
                DataCriacao = DateTime.Now
            };

            await _avaliacaoProdutoRepository.CriarAsync(avaliacao);
            return MapearAvaliacaoGetDto(avaliacao);
        }

        public async Task<AvaliacaoProdutoGetDto> AtualizarMinhaAvaliacaoAsync(int produtoId, AvaliacaoProdutoPutDto dto, int? usuarioId)
        {
            var usuario = await ObterUsuarioAutenticadoOuFalharAsync(usuarioId);
            await ObterProdutoAtivoOuFalharAsync(produtoId);

            ValidarNota(dto.Nota);
            ValidarComentario(dto.Comentario);

            var avaliacao = await _avaliacaoProdutoRepository.ObterAtivaPorUsuarioEProdutoAsync(usuario.Id, produtoId);

            if (avaliacao == null)
                throw new KeyNotFoundException("Avaliacao ativa do usuario para este produto nao encontrada.");

            if (avaliacao.UsuarioId != usuario.Id)
                throw new UnauthorizedAccessException("Usuario nao pode editar avaliacoes de outros usuarios.");

            avaliacao.Nota = dto.Nota;
            avaliacao.Comentario = NormalizarComentario(dto.Comentario);
            avaliacao.DataAtualizacao = DateTime.Now;

            await _avaliacaoProdutoRepository.AtualizarAsync(avaliacao);
            return MapearAvaliacaoGetDto(avaliacao);
        }

        public async Task<List<AvaliacaoProdutoGetDto>> ListarAtivasPorProdutoAsync(int produtoId)
        {
            await ObterProdutoOuFalharAsync(produtoId);

            var avaliacoes = await _avaliacaoProdutoRepository.ListarAtivasPorProdutoAsync(produtoId);
            return avaliacoes.Select(MapearAvaliacaoGetDto).ToList();
        }

        public async Task<PagedResponse<AvaliacaoProdutoGetDto>> ListarAtivasPorProdutoPaginadoAsync(
            int produtoId,
            PaginationParams paginationParams)
        {
            await ObterProdutoOuFalharAsync(produtoId);

            var resultado = await _avaliacaoProdutoRepository.ListarAtivasPorProdutoPaginadoAsync(
                produtoId,
                paginationParams.PageNumber,
                paginationParams.PageSize);

            var items = resultado.Items.Select(MapearAvaliacaoGetDto).ToList();
            return PagedResponse<AvaliacaoProdutoGetDto>.Create(items, resultado.TotalItems, paginationParams);
        }

        public async Task<AvaliacaoResumoDto> ObterResumoAsync(int produtoId)
        {
            await ObterProdutoOuFalharAsync(produtoId);

            var avaliacoes = await _avaliacaoProdutoRepository.BuscarResumoAvaliacoesAsync(produtoId);
            var media = await _avaliacaoProdutoRepository.CalcularMediaAvaliacoesAsync(produtoId);

            return new AvaliacaoResumoDto
            {
                MediaAvaliacoes = Math.Round(media, 1),
                TotalAvaliacoes = avaliacoes.Count,
                Quantidade1Estrela = avaliacoes.Count(a => a.Nota > 0m && a.Nota <= 1m),
                Quantidade2Estrelas = avaliacoes.Count(a => a.Nota > 1m && a.Nota <= 2m),
                Quantidade3Estrelas = avaliacoes.Count(a => a.Nota > 2m && a.Nota <= 3m),
                Quantidade4Estrelas = avaliacoes.Count(a => a.Nota > 3m && a.Nota <= 4m),
                Quantidade5Estrelas = avaliacoes.Count(a => a.Nota > 4m && a.Nota <= 5m)
            };
        }

        public async Task DesativarAsync(int avaliacaoId, bool usuarioAdministrador)
        {
            ValidarAdministrador(usuarioAdministrador);
            await ObterAvaliacaoOuFalharAsync(avaliacaoId);
            await _avaliacaoProdutoRepository.DesativarAsync(avaliacaoId);
        }

        public async Task ApagarAsync(int avaliacaoId, bool usuarioAdministrador)
        {
            ValidarAdministrador(usuarioAdministrador);
            await ObterAvaliacaoOuFalharAsync(avaliacaoId);
            await _avaliacaoProdutoRepository.ApagarAsync(avaliacaoId);
        }

        public async Task<AvaliacaoProdutoGetDto> ModerarComentarioAsync(
            int avaliacaoId,
            AvaliacaoProdutoComentarioModeracaoDto dto,
            bool usuarioAdministrador)
        {
            ValidarAdministrador(usuarioAdministrador);
            ValidarComentario(dto.Comentario);

            var avaliacao = await ObterAvaliacaoOuFalharAsync(avaliacaoId);
            avaliacao.Comentario = NormalizarComentario(dto.Comentario);
            avaliacao.DataAtualizacao = DateTime.Now;

            await _avaliacaoProdutoRepository.AtualizarAsync(avaliacao);
            return MapearAvaliacaoGetDto(avaliacao);
        }

        private async Task<Usuario> ObterUsuarioAutenticadoOuFalharAsync(int? usuarioId)
        {
            if (!usuarioId.HasValue || usuarioId.Value <= 0)
                throw new UnauthorizedAccessException("Usuario autenticado nao identificado.");

            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId.Value);

            if (usuario == null || !usuario.Ativo)
                throw new UnauthorizedAccessException("Usuario autenticado invalido ou inativo.");

            return usuario;
        }

        private async Task<Produto> ObterProdutoAtivoOuFalharAsync(int produtoId)
        {
            var produto = await ObterProdutoOuFalharAsync(produtoId);

            if (!produto.Ativo)
                throw new InvalidOperationException("Produto inativo nao pode receber avaliacoes.");

            return produto;
        }

        private async Task<Produto> ObterProdutoOuFalharAsync(int produtoId)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(produtoId);

            if (produto == null)
                throw new KeyNotFoundException("Produto nao encontrado.");

            return produto;
        }

        private async Task<AvaliacaoProduto> ObterAvaliacaoOuFalharAsync(int avaliacaoId)
        {
            var avaliacao = await _avaliacaoProdutoRepository.ObterPorIdAsync(avaliacaoId);

            if (avaliacao == null)
                throw new KeyNotFoundException("Avaliacao nao encontrada.");

            return avaliacao;
        }

        private static void ValidarAdministrador(bool usuarioAdministrador)
        {
            if (!usuarioAdministrador)
                throw new UnauthorizedAccessException("Apenas Administrador pode executar esta acao.");
        }

        private static void ValidarNota(decimal nota)
        {
            if (nota < 0.5m || nota > 5.0m || nota % 0.5m != 0)
                throw new ArgumentException("A nota deve estar entre 0,5 e 5,0 usando intervalos de 0,5.");
        }

        private static void ValidarComentario(string? comentario)
        {
            if (comentario == null)
                return;

            if (string.IsNullOrWhiteSpace(comentario))
                throw new ArgumentException("O comentario nao pode ser vazio quando enviado.");

            if (comentario.Trim().Length > ComentarioTamanhoMaximo)
                throw new ArgumentException($"O comentario deve conter no maximo {ComentarioTamanhoMaximo} caracteres.");
        }

        private static string? NormalizarComentario(string? comentario)
        {
            return comentario?.Trim();
        }

        private static AvaliacaoProdutoGetDto MapearAvaliacaoGetDto(AvaliacaoProduto avaliacao)
        {
            return new AvaliacaoProdutoGetDto
            {
                Id = avaliacao.Id,
                ProdutoId = avaliacao.ProdutoId,
                NomeProduto = avaliacao.Produto?.Nome ?? string.Empty,
                UsuarioId = avaliacao.UsuarioId,
                NomeUsuario = avaliacao.Usuario?.Nome ?? string.Empty,
                Nota = avaliacao.Nota,
                Comentario = avaliacao.Comentario,
                Ativo = avaliacao.Ativo,
                DataCriacao = avaliacao.DataCriacao,
                DataAtualizacao = avaliacao.DataAtualizacao
            };
        }
    }
}
