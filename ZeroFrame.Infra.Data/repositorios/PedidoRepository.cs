using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ZeroFrame.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Infra.Data.Repositorios
{
    // Classe que implementa o repositório da entidade.
    // Ela contém os métodos responsáveis por manipular os dados no sistema.
    public class PedidoRepository : IPedidoRepository
    {
        private readonly ApplicationDbContext _context;

        public PedidoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Busca um pedido pelo Id, incluindo as informações do usuário, itens e pagamento.
        public async Task<Pedidos?> ObterPorIdAsync(int id)
        {
            return await _context.pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.VariacaoProduto)
                        .ThenInclude(v => v!.Produto)
                            .ThenInclude(p => p!.Categoria)
                .Include(p => p.Pagamento)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Busca todos os pedidos de um usuário
        public async Task<List<Pedidos>> ObterPorUsuarioAsync(int usuarioId)
        {
            return await _context.pedidos
                .Include(p => p.Itens)
                    .ThenInclude(i => i.VariacaoProduto)
                        .ThenInclude(v => v!.Produto)
                            .ThenInclude(p => p!.Categoria)
                .Include(p => p.Pagamento)
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync();
        }

        // Cria um novo pedido no banco de dados.
        public async Task CriarPedidoAsync(Pedidos pedido)
        {
            await _context.pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();
        }

        // Atualiza um pedido existente no banco de dados.
        public async Task AtualizarPedidoAsync(Pedidos pedido)
        {
            _context.pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }

        // Método público que chama o método privado para atualizar um pedido.
        public async Task AtualizarAsync(Pedidos pedido)
        {
            await AtualizarPedidoAsync(pedido);
        }

        // Cancela um pedido existente no banco de dados.
        public async Task CancelarPedidoAsync(int id)
        {
            var pedido = await _context.pedidos.FindAsync(id);

            if (pedido is null)
                return;

            pedido.Status = "Cancelado";

            _context.pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }
    }
}
