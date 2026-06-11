/*
  ====================================
  SLIDER DE FUNDADORES (PÁGINA SOBRE)
  ====================================
  
  Componente que controla o slider de fundadores na página "Sobre Nós"
  Implementação idêntica ao slider de produtos
  Permite navegação através de botões anterior/próximo
*/

// Seleciona todos os slides (fundadores) do slider
const slider = document.querySelectorAll('.slider');

// Seleciona o botão de navegação anterior
const btnPrev = document.getElementById('prev-button');

// Seleciona o botão de navegação próximo
const btnNext = document.getElementById('next-button');

// Índice do slide atual (começa no primeiro)
let currentSlide = 0;

/*
  Função que esconde todos os slides
  Remove a classe 'on' que torna o slide visível
*/
function hideSlider() {
    slider.forEach(item => item.classList.remove('on'));
}

/*
  Função que exibe o slide atual
  Adiciona a classe 'on' para tornar o slide visível
*/
function showSlider() {
    slider[currentSlide].classList.add('on');
}

/* Event listener: botão para próximo fundador
   Clicando, vai para o próximo ou volta ao primeiro se for o último
*/
btnNext.addEventListener('click', function () {
    console.log('Indo ao próximo slide');
    hideSlider();
    if (currentSlide == slider.length - 1) {
        currentSlide = 0;
    } else {
        currentSlide++;
    }
    showSlider();
})

/* Event listener: botão para fundador anterior
   Clicando, vai para o anterior ou vai ao último se for o primeiro
*/
btnPrev.addEventListener('click', function () {
    console.log('Voltando ao slide anterior');
    hideSlider();
    if (currentSlide == 0) {
        currentSlide = slider.length - 1;
    } else {
        currentSlide--;
    }
    showSlider();
})