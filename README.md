# ZeroFrame API

ZeroFrame é uma API REST para e-commerce desenvolvida em ASP.NET Core

O projeto utiliza arquitetura em camadas, Entity Framework Core, SQL Server/LocalDB, Swagger e autenticação JWT.

## Objetivo

O objetivo da API é simular o backend de uma loja virtual, permitindo que o front-end se comunique com o banco de dados por meio de endpoints seguros e organizados. O acesso ao banco de dados não é feito diretamente pelo usuário ou pelo front-end. Toda comunicação passa pela API, que recebe as requisições, aplica as regras de negócio, acessa o banco internamente e retorna os dados em formato JSON.

## Tecnologias utilizadas

- ASP.NET Core
- C#
- Entity Framework Core
- SQL Server / LocalDB
- Swagger
- JWT Bearer
- Arquitetura em camadas
- Git e GitHub

## Arquitetura do projeto

O sistema está dividido em camadas:

- **ZeroFrame.API**: camada responsável pelos controllers, rotas, Swagger, autenticação e middleware.
- **ZeroFrame.Application**: camada responsável pelos serviços, DTOs e regras de aplicação.
- **ZeroFrame.Domain**: camada responsável pelas entidades e interfaces.
- **ZeroFrame.Infra.Data**: camada responsável pelo acesso ao banco de dados com Entity Framework Core.
- **ZeroFrame.Infra.IoC**: camada responsável pela injeção de dependência.
- <h2>Arquitetura da API</h2>

<p align="center">
  <img src="assets/arquitetura.png" width="1000"/>
</p>

## Funcionalidades principais

### Usuários

- Cadastro de usuários
- Login com geração de token JWT
- Busca de usuário por ID
- Atualização de nome e telefone
- Remoção de usuário

### Endereços

- Cadastro de endereço vinculado ao usuário
- Listagem de endereços
- Atualização de endereço
- Remoção de endereço

### Categorias

- Cadastro de categorias
- Listagem de categorias
- Atualização de categorias
- Remoção de categorias

### Produtos

- Cadastro de produtos
- Listagem de produtos
- Busca por ID
- Atualização de produtos
- Remoção de produtos

### Variações de produto

- Cadastro de variações por produto
- Controle de tamanho, cor e estoque
- Atualização de estoque

### Carrinho

- Criação de carrinho
- Busca de carrinho ativo por usuário
- Adição de itens ao carrinho
- Validação de estoque
- Atualização e remoção de itens

### Pedidos

- Criação de pedido por lista de itens
- Criação de pedido a partir do carrinho
- Cálculo do valor total
- Baixa automática no estoque
- Cancelamento de pedido com devolução ao estoque

### Pagamentos

- Criação de pagamento vinculado ao pedido
- Pagamento iniciado como pendente
- Busca de pagamento por ID
- Busca de pagamento por pedido
- Atualização de status do pagamento

## Regras de negócio

- O carrinho não baixa estoque, apenas valida se há quantidade disponível.
- O estoque é baixado somente no momento da criação do pedido.
- Um pedido criado a partir do carrinho exige que o carrinho esteja ativo e tenha itens.
- Ao cancelar um pedido, o estoque dos produtos é reposto.
- O pagamento não possui integração com gateway real, funcionando apenas como registro de status.
- O preço do item é copiado do produto para preservar o valor usado no momento da venda.

## Entity Framework Core

O Entity Framework Core é utilizado como ORM para mapear as entidades C# para tabelas no banco de dados.

## Documentação da API

A API possui documentação interativa com Swagger, permitindo testar os endpoints diretamente pelo navegador.

Exemplo de acesso local:

```bash
http://localhost:5140/swagger
