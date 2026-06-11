document.addEventListener("DOMContentLoaded", () => {
    const cartAnchor = document.querySelector('a.user-actions-icon[href*="carrinho"]') || document.querySelector('a.user-actions-icon');
    if (!cartAnchor) return;

    cartAnchor.classList.add('cart-count-anchor');

    let badge = cartAnchor.querySelector('.cart-count-badge');
    if (!badge) {
        badge = document.createElement('span');
        badge.className = 'cart-count-badge';
        cartAnchor.appendChild(badge);
    }

    async function atualizarCarrinhoBadge() {
        if (!window.ZeroFrameApi?.estaLogado?.()) {
            badge.style.display = 'none';
            return;
        }

        try {
            const itens = await CarrinhoService.listarItens();
            const total = itens.reduce((soma, item) => soma + Number(item.quantidade ?? item.Quantidade ?? 1), 0);

            if (total > 0) {
                badge.textContent = total;
                badge.style.display = 'inline-flex';
            } else {
                badge.style.display = 'none';
            }
        } catch (error) {
            badge.style.display = 'none';
        }
    }

    window.atualizarCarrinhoBadge = atualizarCarrinhoBadge;
    atualizarCarrinhoBadge();
});
