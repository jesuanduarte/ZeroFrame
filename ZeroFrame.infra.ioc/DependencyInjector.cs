using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.Application.Servicos;
using ZeroFrame.Domain.Account;
using ZeroFrame.Domain.Interfaces;
using ZeroFrame.Infra.Data.Context;
using ZeroFrame.Infra.Data.Identity;
using ZeroFrame.Infra.Data.Repositorios;
using ZeroFrame.Infra.Data.UnitOfWork;

namespace ZeroFrame.Infra.IoC
{
    public static class DependencyInjector
    {

        // Classe responsável por registrar as dependências da camada de infraestrutura.
        // Aqui são configurados os serviços necessários para o funcionamento da aplicação.
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configura o banco de dados SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                )
            );

            var secretKey = configuration["Jwt:SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("A chave JWT SecretKey não foi configurada.");
            }

            // Configura a autenticação com JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)
                    ),

                    ClockSkew = TimeSpan.Zero
                };
            });

            // Repositórios e Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
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
            services.AddScoped<IAvaliacaoProdutoRepository, AvaliacaoProdutoRepository>();

            // Serviços de aplicação
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
            services.AddScoped<IAvaliacaoProdutoService, AvaliacaoProdutoService>();
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            services.AddScoped<IFreteService, FreteService>();

            //autenticação JWT e geração de token
            services.AddScoped<IAuthenticate, AuthenticateService>();

            return services;
        }
    }
}
