const hamburgerBtn = document.getElementById('hamburgerBtn');
const mainNav = document.getElementById('mainNav');

if (hamburgerBtn && mainNav) {
    hamburgerBtn.addEventListener('click', () => {
        mainNav.classList.toggle('nav-open');
        const icon = hamburgerBtn.querySelector('i');
        if (mainNav.classList.contains('nav-open')) {
            icon.classList.replace('fa-bars', 'fa-xmark');
        } else {
            icon.classList.replace('fa-xmark', 'fa-bars');
        }
    });

    document.addEventListener('click', (e) => {
        if (!hamburgerBtn.contains(e.target) && !mainNav.contains(e.target)) {
            mainNav.classList.remove('nav-open');
            const icon = hamburgerBtn.querySelector('i');
            icon.classList.replace('fa-xmark', 'fa-bars');
        }
    });
}
