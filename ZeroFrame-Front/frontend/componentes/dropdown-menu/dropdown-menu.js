/*
  ====================================
  MENU DROPDOWN DE USUÁRIO
  ====================================
  
  Controla o menu dropdown que aparece ao clicar no ícone de usuário
  Oferece várias formas de fechar o menu (clique externo, ESC, clique em item)
*/

// Seleciona o menu dropdown do DOM
let dropdownMenu = document.getElementById("dropdownMenu");

// Seleciona o botão/ícone do usuário do DOM
let userIcon = document.getElementById("userMenuIcon");
if (!dropdownMenu || !userIcon) {
    console.warn("Dropdown de usuário não encontrado nesta página.");
}

// funcionamento do usuário no topo do menu dropdown 
function obterUsuarioSalvo() {
    const raw = localStorage.getItem("zf_usuario") || localStorage.getItem("usuario");

    try {
        return raw ? JSON.parse(raw) : null;
    } catch {
        return null;
    }
}

function obterCaminhoLogin() {
    return window.location.pathname.includes("/pages/")
        ? "../login-page/login.html"
        : "./pages/login-page/login.html";
}

function obterPrefixoPages() {
    return window.location.pathname.includes("/pages/")
        ? "../"
        : "./pages/";
}

function atualizarDadosUsuarioDropdown() {
    if (!dropdownMenu) return;

    const usuario = obterUsuarioSalvo();
    const nome = usuario?.nome || localStorage.getItem("nome") || "Visitante";
    const email = usuario?.email || localStorage.getItem("email") || "Entre na sua conta";
    const nomeEl = dropdownMenu.querySelector(".dropdown-header-text h3");
    const emailEl = dropdownMenu.querySelector(".dropdown-header-text p");

    if (nomeEl) nomeEl.textContent = nome;
    if (emailEl) emailEl.textContent = email;

    const perfil = usuario?.perfil || localStorage.getItem("perfil");
    const configLink = dropdownMenu.querySelector(".dropdown-header a");
    if (configLink) {
        configLink.href = perfil === "Administrador"
            ? `${obterPrefixoPages()}admin/admin.html`
            : `${obterPrefixoPages()}account-settings/account.html`;
    }

    // Oculta o link de logout quando não há usuário logado
    const sairLink = Array.from(dropdownMenu.querySelectorAll("a"))
        .find((link) => link.textContent.toLowerCase().includes("sair da conta"));

    if (!usuario) {
        if (sairLink) sairLink.style.display = "none";
    } else {
        if (sairLink) sairLink.style.display = "";
    }
}

function configurarLogout() {
    if (!dropdownMenu) return;

    const sairLink = Array.from(dropdownMenu.querySelectorAll("a"))
        .find((link) => link.textContent.toLowerCase().includes("sair da conta"));

    sairLink?.addEventListener("click", (event) => {
        event.preventDefault();
        ZeroFrameApi?.limparSessao?.();
        window.location.href = obterCaminhoLogin();
    });
}

atualizarDadosUsuarioDropdown();
configurarLogout();


/*
  Evento: ao clicar no ícone de usuário, alterna o menu entre aberto e fechado
  e.stopPropagation() previne que o clique propague para outros listeners
*/
userIcon?.addEventListener("click", (e) => {
    e.stopPropagation();
    // Alterna a classe 'open-menu' que controla a exibição do dropdown
    dropdownMenu?.classList.toggle("open-menu");
    // Alterna a classe 'active-dropdown' para mudar estilo do ícone
    userIcon.classList.toggle("active-dropdown");
})

/*
  Evento: ao clicar fora do menu, o fecha
  Valida se o clique foi fora tanto do menu quanto do ícone
*/
document.addEventListener("click", (e) => {
    if (!dropdownMenu || !userIcon) return;
    // Verifica se o clique não foi no menu e não foi no ícone
    const outsideClick = !dropdownMenu.contains(e.target) && !userIcon.contains(e.target);

    // Se foi clique externo, remove as classes que abrem o menu
    if (outsideClick) {
        dropdownMenu.classList.remove("open-menu");
        userIcon.classList.remove("active-dropdown");
    }
})

/*
  Evento: ao pressionar a tecla ESC, fecha o menu
  Permite ao usuário sair do menu com o teclado
*/
document.addEventListener("keydown", (e) => {
    if (e.key === "Escape") {
        dropdownMenu?.classList.remove("open-menu");
        userIcon?.classList.remove("active-dropdown");
    }
})

/*
  Evento: ao clicar em um item do menu, o fecha
  Converte a navegação em uma ação "um clique" - clica em um item e o menu fecha
*/
dropdownMenu?.addEventListener("click", () => {
    dropdownMenu.classList.remove("open-menu");
    userIcon?.classList.remove("active-dropdown");
})
