/*
  ====================================
  CARROSSEL DE CATEGORIAS
  ====================================
  
  Componente React que exibe um carrossel 3D de categorias de produtos
  Permite navegação automática e manual (mouse/click)
  Realiza rotação 3D dos cards com efeitos visuais
*/

// Índice atual do card ativo
let index = 0;

// Seleciona todos os cards do carrossel
const cards = document.querySelectorAll('.card');

// Seleciona o container do slider
const slider = document.querySelector('.slider-wrapper');

// Total de cards disponíveis
const total = cards.length;

// Timer para a rotação automática a cada 4 segundos
let autoPlay = setInterval(() => changeSlide(1), 4000);

/*
  Função responsável por atualizar o visual dos cards
  Define quais cards aparecem (active, next, prev, hide)
  Aplica transformações CSS para o efeito 3D
*/
function update() {
  cards.forEach((card, i) => {
    // Reseta estilos inline do mouse e classes padrão
    card.style.transform = ''; 
    card.className = 'card'; 

    // Define a classe do card baseado em sua posição
    if (i === index) {
      // Card central (ativo/principal)
      card.classList.add('active');
    } else if (i === (index + 1) % total) {
      // Próximo card (à direita)
      card.classList.add('next');
    } else if (i === (index - 1 + total) % total) {
      // Card anterior (à esquerda)
      card.classList.add('prev');
    } else {
      // Cards que não são visíveis
      card.classList.add('hide');
    }
  });
}

/* Evento: ao entrar com o mouse no slider, pausa a rotação automática */
slider.addEventListener('mouseenter', () => {
  clearInterval(autoPlay); 
});

/* Evento: ao sair com o mouse do slider, retoma a rotação automática */
slider.addEventListener('mouseleave', () => {
  resetTimer(); // Retoma o slider automático
  
  // Volta o card ativo para sua posição estática (sem efeitos do mouse)
  const activeCard = document.querySelector('.card.active');
  if (activeCard) {
    activeCard.style.transform = `translate3d(0, 0, 150px) rotateY(0deg) rotateX(0deg)`;
  }
});

/*
  Função para trocar para o próximo ou anterior card
  @param direction: 1 para próximo, -1 para anterior
*/
function changeSlide(direction) {
  // Calcula o novo índice (com loop circular)
  index = (index + direction + total) % total;
  // Atualiza o visual
  update();
  // Reseta o timer de auto-play
  resetTimer();
}

/*
  Função para pular para um card específico
  @param clickedIndex: índice do card clicado
*/
function clickToSlide(clickedIndex) {
  if (index !== clickedIndex) {
    index = clickedIndex;
    update();
    resetTimer();
  }
}

/*
  Função para resetar o timer de rotação automática
  Limpa o intervalo anterior e cria um novo
*/
function resetTimer() {
  clearInterval(autoPlay);
  autoPlay = setInterval(() => changeSlide(1), 4000);
}

// Inicializa o carrossel com a primeira atualização
update();