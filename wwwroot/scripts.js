/* ==========================================
   1. СИСТЕМА ТЕМ
   ========================================== */
window.applyTheme = function (theme) {
    if (theme === 'dark') {
        document.body.classList.add('dark-theme');
    } else {
        document.body.classList.remove('dark-theme');
    }

    const icon = document.getElementById('themeIcon');
    if (icon) {
        if (theme === 'dark') {
            icon.classList.remove('fa-moon');
            icon.classList.add('fa-sun');
        } else {
            icon.classList.remove('fa-sun');
            icon.classList.add('fa-moon');
        }
    }
};

window.toggleTheme = function () {
    const currentTheme = localStorage.getItem('theme') === 'dark' ? 'light' : 'dark';
    localStorage.setItem('theme', currentTheme);
    window.applyTheme(currentTheme);
};

window.restoreTheme = function () {
    window.applyTheme(localStorage.getItem('theme') || 'light');
};

// 1. Стандартная загрузка и синхронизация между окнами браузера
document.addEventListener("DOMContentLoaded", window.restoreTheme);
window.addEventListener('storage', (event) => { if (event.key === 'theme') window.restoreTheme(); });

// 2. Официальная поддержка .NET 8 (Enhanced Navigation)
document.addEventListener('blazor:enhancedload', window.restoreTheme);

const themeGuard = new MutationObserver(() => {
    const theme = localStorage.getItem('theme');

    // Работаем только если должна быть включена темная тема
    if (theme === 'dark') {

        // Если Blazor стер класс с body - мгновенно возвращаем
        if (!document.body.classList.contains('dark-theme')) {
            document.body.classList.add('dark-theme');
        }

        // Если Blazor перерисовал шапку и вернул иконку луны - меняем на солнце
        const icon = document.getElementById('themeIcon');
        if (icon && icon.classList.contains('fa-moon')) {
            icon.classList.remove('fa-moon');
            icon.classList.add('fa-sun');
        }
    }
});

// Запускаем непрерывную слежку за изменениями структуры сайта и классов
themeGuard.observe(document.body, { childList: true, subtree: true, attributes: true, attributeFilter: ['class'] });


/* ==========================================
   2. ОГЛАВЛЕНИЕ ДЛЯ СТАТЕЙ (TOC)
   ========================================== */
window.buildToc = function () {
    const content = document.getElementById('article-content');
    const tocList = document.getElementById('toc-list');
    const tocContainer = document.getElementById('toc-container');
    const mobileTocList = document.getElementById('mobile-toc-list');
    const mobileTocContainer = document.getElementById('mobile-toc-container');
    const mobileDetails = document.getElementById('mobile-toc-details');

    if (!content) return;

    const headers = content.querySelectorAll('h1, h2, h3');

    if (headers.length === 0) {
        if (tocContainer) tocContainer.style.display = 'none';
        if (mobileTocContainer) mobileTocContainer.style.display = 'none';
        return;
    }

    if (tocContainer) tocContainer.style.display = '';
    if (mobileTocContainer) mobileTocContainer.style.display = '';
    if (tocList) tocList.innerHTML = '';
    if (mobileTocList) mobileTocList.innerHTML = '';

    headers.forEach((header, index) => {
        if (!header.id) header.id = 'heading-' + index;

        const createListItem = (isMobile) => {
            const li = document.createElement('li');
            li.className = 'toc-item toc-' + header.tagName.toLowerCase();

            const a = document.createElement('a');
            a.href = '#' + header.id;
            a.className = 'toc-link';
            a.textContent = header.textContent;

            a.onclick = (e) => {
                e.preventDefault();
                const y = header.getBoundingClientRect().top + window.scrollY - 100;
                window.scrollTo({ top: y, behavior: 'smooth' });

                if (isMobile && mobileDetails) {
                    mobileDetails.removeAttribute('open');
                }
            };

            li.appendChild(a);
            return li;
        };

        if (tocList) tocList.appendChild(createListItem(false));
        if (mobileTocList) mobileTocList.appendChild(createListItem(true));
    });
};


/* ==========================================
   3. ПАНЕЛЬ ДОСТУПНОСТИ (ДЛЯ СЛАБОВИДЯЩИХ)
   ========================================== */
window.toggleA11yPanel = function () {
    const panel = document.getElementById('a11yPanel');
    if (panel) panel.classList.toggle('open');
};

let currentFontSizeMod = 0;
window.changeFontSize = function (direction) {
    currentFontSizeMod += direction;
    if (currentFontSizeMod < -1) currentFontSizeMod = -1;
    if (currentFontSizeMod > 3) currentFontSizeMod = 3;

    const baseSize = 16;
    document.documentElement.style.fontSize = (baseSize + currentFontSizeMod * 2) + 'px';
};

window.resetFontSize = function () {
    currentFontSizeMod = 0;
    document.documentElement.style.fontSize = '16px';
};

window.setContrastMode = function (enable) {
    if (enable) {
        document.body.classList.add('a11y-contrast');
    } else {
        document.body.classList.remove('a11y-contrast');
    }
};

window.toggleImages = function (show) {
    if (!show) {
        document.body.classList.add('a11y-no-images');
    } else {
        document.body.classList.remove('a11y-no-images');
    }
};


/* ==========================================
   4. ПОИСК
   ========================================== */
window.toggleSearch = function () {
    const wrapper = document.getElementById('searchWrapper');
    if (wrapper) {
        wrapper.classList.toggle('active');
        if (wrapper.classList.contains('active')) {
            wrapper.querySelector('.search-input').focus();
        }
    }
};

document.addEventListener('click', function (e) {
    const wrapper = document.getElementById('searchWrapper');
    if (wrapper && !wrapper.contains(e.target) && wrapper.classList.contains('active')) {
        wrapper.classList.remove('active');
    }
});


/* ==========================================
   5. АНИМАЦИИ, КНОПКА "НАВЕРХ" И СВЕТИЛЬНИК (SCROLLSPY)
   ========================================== */
window.scrollToTop = function () {
    window.scrollTo({ top: 0, behavior: 'smooth' });
};

window.checkScrollAndAnimations = function () {
    // 1. Кнопка "Наверх"
    const scrollBtn = document.getElementById('scrollToTop');
    if (scrollBtn) {
        if (window.scrollY > 400) {
            scrollBtn.classList.add('show');
        } else {
            scrollBtn.classList.remove('show');
        }
    }

    // 2. Анимация появления элементов (Fade-Up)
    const fadeElements = document.querySelectorAll('.fade-up:not(.visible)');
    const windowHeight = window.innerHeight;

    fadeElements.forEach(element => {
        const elementTop = element.getBoundingClientRect().top;
        if (elementTop < windowHeight - 100) {
            element.classList.add('visible');
        }
    });

    // 3. ScrollSpy (Подсветка активного пункта меню на главной)
    // Ищем элементы каждый раз, так как в Blazor DOM может меняться
    const sections = document.querySelectorAll('section[id]');
    const navLinks = document.querySelectorAll('.nav-links a');

    if (sections.length > 0 && navLinks.length > 0) {
        let currentSectionId = '';

        sections.forEach(section => {
            const sectionTop = section.offsetTop;
            const sectionHeight = section.clientHeight;
            if (window.scrollY >= (sectionTop - sectionHeight / 3)) {
                currentSectionId = section.getAttribute('id');
            }
        });

        if (currentSectionId) {
            navLinks.forEach(a => {
                a.classList.remove('active-link');
                if (a.getAttribute('href') && a.getAttribute('href').includes('#' + currentSectionId)) {
                    a.classList.add('active-link');
                }
            });
        }
    }
};

// Привязываем обработчик скролла
window.addEventListener('scroll', window.checkScrollAndAnimations, { passive: true });

// Следим за изменениями DOM (специфика Blazor), чтобы вовремя анимировать новые элементы
const observer = new MutationObserver(() => {
    window.checkScrollAndAnimations();
});
observer.observe(document.body, { childList: true, subtree: true });

// Вызываем один раз при старте
window.checkScrollAndAnimations();