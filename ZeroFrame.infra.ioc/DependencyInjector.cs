using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.Application.Servicos;
using ZeroFrame.domain.Interface;
using ZeroFrame.Infra.Data.BDconexao;
using ZeroFrame.Infra.Data.repositorios;

namespace ZeroFrame.infra.ioc
{
    public static class DependencyInjector
    {
        // Classe responsável por registrar as dependências da camada de infraestrutura.
        // Aqui são configurados os serviços necessários para o funcionamento da aplicação.
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            services.AddScoped<IVariacaoRepository, VariacaoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();  
            services.AddScoped<IPagamentoRepository, PagamentoRepository>();
            services.AddScoped<IItemCarrinhoRepository, ItemCarrinhoRepository>();
            services.AddScoped<IItemPedidoRepository, ItemPedidoRepository>();
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<ICarrinhoRepository, CarrinhoRepository>(); 
            
            
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<IVariacaoService, VariacaoService>();
            services.AddScoped<ICarrinhoService, CarrinhoService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IEnderecoService, EnderecoService>();
            services.AddScoped<IItemCarrinhoService, ItemCarrinhoService>();
            services.AddScoped<IItemPedidoService, ItemPedidoService>();
            services.AddScoped<IPagamentoService, PagamentoService>();
            services.AddScoped<IPedidoService, PedidoService>();

            return services;
        }
    }
}