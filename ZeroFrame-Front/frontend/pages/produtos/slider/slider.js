/*
  ====================================
  SLIDER DE PRODUTOS
  ====================================

  Controla a galeria da pagina de produto. A lista de slides pode ser recriada
  pela API, entao o slider reinicializa sempre com as imagens atuais do produto.
*/

const ZeroFrameProductSlider = (() => {
    let slides = [];
    let thumbnails = [];
    let currentSlide = 0;
    const btnPrev = document.getElementById("prev-button");
    const btnNext = document.getElementById("next-button");
    const thumbnailsContainer = document.querySelector(".product-thumbnails");

    function hideSlider() {
        slides.forEach((item) => item.classList.remove("on"));
    }

    function showSlider() {
        if (!slides.length) return;
        slides[currentSlide].classList.add("on");
        thumbnails.forEach((item, index) => {
            item.classList.toggle("selected", index === currentSlide);
        });
    }

    function updateButtons() {
        const shouldShow = slides.length > 1;
        if (btnPrev) btnPrev.style.display = shouldShow ? "" : "none";
        if (btnNext) btnNext.style.display = shouldShow ? "" : "none";
        if (thumbnailsContainer) thumbnailsContainer.style.display = shouldShow ? "" : "none";
    }

    function renderThumbnails() {
        thumbnails = [];
        if (!thumbnailsContainer) return;

        thumbnailsContainer.textContent = "";
        if (slides.length <= 1) return;

        // As miniaturas sao recriadas a partir dos slides atuais, que ja vieram do produto aberto.
        slides.forEach((slide, index) => {
            const button = document.createElement("button");
            button.type = "button";
            button.className = "product-thumbnail";
            button.setAttribute("aria-label", `Ver imagem ${index + 1}`);

            const image = document.createElement("img");
            image.src = slide.src;
            image.alt = slide.alt || `Imagem ${index + 1} do produto`;

            button.appendChild(image);
            button.addEventListener("click", () => goToIndex(index));
            thumbnailsContainer.appendChild(button);
            thumbnails.push(button);
        });
    }

    function init() {
        slides = Array.from(document.querySelectorAll(".slider"));
        currentSlide = 0;
        renderThumbnails();
        hideSlider();
        showSlider();
        updateButtons();
    }

    function goToIndex(index) {
        if (slides.length <= 1) return;
        hideSlider();
        currentSlide = (index + slides.length) % slides.length;
        showSlider();
    }

    function goTo(direction) {
        goToIndex(currentSlide + direction);
    }

    btnNext?.addEventListener("click", () => goTo(1));
    btnPrev?.addEventListener("click", () => goTo(-1));

    return { init };
})();

window.ZeroFrameProductSlider = ZeroFrameProductSlider;
document.addEventListener("DOMContentLoaded", () => ZeroFrameProductSlider.init());
