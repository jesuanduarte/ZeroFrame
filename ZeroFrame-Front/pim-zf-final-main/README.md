# Zero Frame 🛍️

## Visão Geral

Zero Frame é uma loja de e-commerce frontend criada para oferecer uma experiência de compra online fluida e moderna. O projeto reúne navegação intuitiva, catálogo de produtos, páginas de autenticação, carrinho de compras, suporte e temas claro/escuro.

## Principais Recursos

- Navegação estruturada com seções de originais, multimarcas e informações institucionais
- Busca de produtos com filtros e resultados dinâmicos
- Carrinho de compras funcional com adição e remoção de itens
- Autenticação de usuário via páginas de login e cadastro
- Alternância entre modo claro e escuro com persistência local
- Carrossel de categorias com efeitos visuais e interação responsiva
- Seções de favoritos, pedidos, endereços e suporte ao cliente
- Design responsivo para desktop e dispositivos móveis

## Tecnologias

- HTML5
- CSS3 (variáveis CSS, responsividade e transições)
- JavaScript puro para lógica e interatividade
- React 18 no componente do carrossel de categorias
- Babel Standalone para compilação de JSX no navegador
- Font Awesome para ícones
- Google Fonts (Montserrat)

## Como Executar

1. Baixe ou clone o repositório.
2. Abra o arquivo `index.html` em um navegador moderno.
3. Navegue pelo site usando o menu principal ou acesse páginas internas diretamente.

> Esta aplicação é frontend puro e não requer servidor backend para ser executada localmente.

## Estrutura do Projeto

- `index.html` — página principal
- `style.css` — estilos globais
- `assets/` — imagens e recursos de produtos
- `componentes/` — módulos reutilizáveis
  - `theme-toggle.js` — alternância de tema claro/escuro
  - `dropdown-menu/` — menu de perfil
  - `carrossel-categorias/` — carrossel interativo de categorias
- `styles/` — estilos compartilhados para cabeçalho, rodapé e cards de produtos
- `pages/` — páginas internas da aplicação
  - `account-settings/` — configurações do perfil do usuário
  - `admin/` — painel administrativo e controle de conteúdo
  - `login-page/` — login de usuário
  - `register-page/` — cadastro de usuário
  - `search/` — pesquisa e filtros de produtos
  - `carrinho/` — carrinho de compras
  - `favoritos/` — produtos salvos
  - `meus-enderecos/` — gerenciamento de endereços
  - `meus-pedidos/` — histórico de pedidos
  - `produtos/` — detalhes do produto e slider
  - `sobre/` — informações sobre a marca e fundadores
  - `suporte/` — contato e suporte ao cliente

## Páginas Disponíveis

| Página | Caminho | Descrição |
|---|---|---|
| Início | `index.html` | Página principal com destaques e navegação |
| Login | `pages/login-page/login.html` | Autenticação de usuário |
| Cadastro | `pages/register-page/register.html` | Registro de novo usuário |
| Busca | `pages/search/search.html` | Pesquisa com filtros de produtos |
| Carrinho | `pages/carrinho/carrinho.html` | Gestão de itens no carrinho |
| Favoritos | `pages/favoritos/favoritos.html` | Produtos salvos pelo usuário |
| Produtos | `pages/produtos/product.html` | Página de detalhes do produto |
| Endereços | `pages/meus-enderecos/enderecos.html` | Gerenciamento de endereços de entrega |
| Pedidos | `pages/meus-pedidos/pedidos.html` | Histórico de compras |
| Configurações | `pages/account-settings/account.html` | Ajustes de perfil e preferências do usuário |
| Administração | `pages/admin/admin.html` | Painel administrativo e gerenciamento interno |
| Sobre | `pages/sobre/sobre.html` | Apresentação da marca e equipe |
| Suporte | `pages/suporte/suporte.html` | Formulário de contato |

## Componentes Principais

- `componentes/theme-toggle.js`: alternância de tema com persistência em `localStorage`
- `componentes/dropdown-menu/`: menu de usuário com comportamento de fechamento automático
- `componentes/carrossel-categorias/`: carrossel interativo em React para navegação por categorias
- `pages/produtos/slider/` e `pages/sobre/slider fundadores/`: sliders de imagens com navegação manual e loop contínuo

## Observações Técnicas

- O tema escuro é ativado com a classe `modoescuro` no `body`.
- Preferência de tema salva localmente no browser.
- Validações de formulário realizadas em JavaScript.
- Eventos de clique e teclado são usados para controles de interface e fechamento de menus.

## Contato

Para dúvidas ou sugestões, consulte a página de suporte em `pages/suporte/suporte.html`.
