# ZeroFrame

ZeroFrame é uma API de e-commerce desenvolvida em ASP.NET Core.  
O projeto foi criado com objetivo acadêmico para demonstrar o funcionamento de uma loja virtual no backend,
utilizando arquitetura em camadas API, Application, Domain, Infra.Data e IoC, Entity Framework Core e SQL Server.

## Objetivo

O sistema simula as principais operações de um e-commerce, como cadastro de usuários, produtos, categorias, 
carrinho de compras, pedidos, controle de estoque e pagamentos.

## Tecnologias Utilizadas

- ASP.NET Core
- C#
- Entity Framework Core
- SQL Server
- Swagger
- Arquitetura em camadas

## Estrutura do Projeto

```text
ZeroFrame.API           # Camada de apresentação da API, com controllers e configuração do Swagger
ZeroFrame.Application   # Camada de aplicação, com serviços, interfaces e DTOs
ZeroFrame.domain        # Camada de domínio, com entidades e interfaces dos repositórios
ZeroFrame.Infra.Data    # Camada de infraestrutura, com DbContext, migrations e repositórios
ZeroFrame.infra.ioc     # Camada de injeção de dependência
