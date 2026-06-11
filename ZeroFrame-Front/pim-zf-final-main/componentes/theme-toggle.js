/*
  ====================================
  TOGGLE DE TEMA - MODO ESCURO/CLARO
  ====================================
  
  Arquivo que controla o alternador de tema da aplicação
  Permite ao usuário alternar entre modo claro e modo escuro
  Salva a preferência no localStorage para persistir entre sessões
*/

// Recupera do localStorage se o modo escuro está ativo (valor: 'active' ou null)
let modoEscuro = localStorage.getItem('modoescuro');

// Recupera o botão de alternância de tema do DOM
const themeToggle = document.getElementById('toggle-button');
if (!themeToggle) {
    console.warn("Botão de tema não encontrado nesta página.");
}

/*
  Função para ativar o modo escuro
  - Adiciona a classe 'modoescuro' ao body (o CSS define as cores)
  - Salva a preferência no localStorage
*/
function ativarModoEscuro() {
    document.body.classList.add('modoescuro');
    localStorage.setItem('modoescuro', 'active');
}

/*
  Função para desativar o modo escuro
  - Remove a classe 'modoescuro' do body (volta ao tema claro)
  - Salva a preferência no localStorage
*/
function desativarModoEscuro() {
    document.body.classList.remove('modoescuro');
    localStorage.setItem('modoescuro', null);
}

/* Verifica a preferência salva e aplica ao carregar a página */
if (modoEscuro === 'active') {
    ativarModoEscuro();
} else {
    desativarModoEscuro();
}

/* Event listener: ao clicar no botão de alternância, troca o tema */
themeToggle?.addEventListener('click', () => {
    // Recupera novamente o valor atual do localStorage
    modoEscuro = localStorage.getItem('modoescuro')
    
    // Se não está ativo, ativa; se está ativo, desativa
    if (modoEscuro !== 'active') {
        ativarModoEscuro();
    }
    else {
        desativarModoEscuro();
    }
})
