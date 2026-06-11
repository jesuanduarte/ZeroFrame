(function () {
    const filterToggleBtn = document.getElementById('filterToggleBtn');
    const filterCloseBtn  = document.getElementById('filterCloseBtn');
    const filterBackdrop  = document.getElementById('filterBackdrop');
    const searchFilters   = document.getElementById('searchFilters');
    const filterCount     = document.getElementById('filterCount');
    const filterClearBtn  = document.getElementById('filterClearBtn');
    const filterApplyBtn  = document.getElementById('filterApplyBtn');

    if (!filterToggleBtn || !searchFilters) return;

    /* ---- drawer open / close ---- */

    function openDrawer() {
        searchFilters.classList.add('filter-open');
        filterBackdrop.classList.add('active');
        document.body.style.overflow = 'hidden';
    }

    function closeDrawer() {
        searchFilters.classList.remove('filter-open');
        filterBackdrop.classList.remove('active');
        document.body.style.overflow = '';
    }

    filterToggleBtn.addEventListener('click', openDrawer);
    filterCloseBtn.addEventListener('click', closeDrawer);
    filterBackdrop.addEventListener('click', closeDrawer);
    filterApplyBtn.addEventListener('click', closeDrawer);

    /* ---- active filter count badge ---- */

    function updateCount() {
        const n = searchFilters.querySelectorAll('input[type="checkbox"]:checked').length;
        filterCount.textContent = n > 0 ? n : '';
    }

    searchFilters.addEventListener('change', updateCount);

    /* ---- clear all ---- */

    filterClearBtn.addEventListener('click', () => {
        searchFilters.querySelectorAll('input[type="checkbox"]').forEach(cb => {
            cb.checked = false;
        });
        updateCount();
    });

    /* ---- accordion per category ---- */

    document.querySelectorAll('.filter-category').forEach(category => {
        const toggle = category.querySelector('.filter-category-toggle');
        const options = category.querySelector('.filter-options');
        if (!toggle || !options) return;

        if (window.matchMedia('(min-width: 1025px)').matches) {
            category.classList.add('open');
            toggle.setAttribute('aria-expanded', 'true');
            options.classList.add('open');
        } else {
            toggle.setAttribute('aria-expanded', 'false');
        }

        toggle.addEventListener('click', () => {
            const isOpen = category.classList.contains('open');
            category.classList.toggle('open', !isOpen);
            options.classList.toggle('open', !isOpen);
            toggle.setAttribute('aria-expanded', String(!isOpen));
        });
    });
})();
