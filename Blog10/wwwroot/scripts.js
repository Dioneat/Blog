/* ==========================================
   1. ГЛОБАЛЬНАЯ ЗАЩИТА НАСТРОЕК (THEME & A11Y GUARD)
   ========================================== */

const stateGuard = new MutationObserver(() => {
    // Восстановление темной темы
    const theme = localStorage.getItem('theme');
    if (theme === 'dark') {
        if (!document.body.classList.contains('dark-theme')) {
            document.body.classList.add('dark-theme');
        }
        const icon = document.getElementById('themeIcon');
        if (icon && icon.classList.contains('fa-moon')) {
            icon.classList.remove('fa-moon');
            icon.classList.add('fa-sun');
        }
    }

    const savedA11y = localStorage.getItem('a11ySettings');
    if (savedA11y) {
        const state = JSON.parse(savedA11y);

        if (state.contrast && !document.body.classList.contains('a11y-contrast')) {
            document.body.classList.add('a11y-contrast');
        }

        if (state.noImages && !document.body.classList.contains('a11y-no-images')) {
            document.body.classList.add('a11y-no-images');
        }

        if (state.colorBlind && state.colorBlind !== 'none') {
            const cbClass = 'a11y-' + state.colorBlind;
            if (!document.body.classList.contains(cbClass)) {
                document.body.classList.add(cbClass);
            }
        }

        const expectedFontSize = (16 + (state.fontSizeMod || 0) * 2) + 'px';
        if (document.documentElement.style.fontSize !== expectedFontSize) {
            document.documentElement.style.fontSize = expectedFontSize;
        }
    }
});

stateGuard.observe(document.body, { attributes: true, attributeFilter: ['class'] });

/* ==========================================
   2. СИСТЕМА ТЕМ
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

/* ==========================================
   3. ПАНЕЛЬ ДОСТУПНОСТИ (A11Y) С СОХРАНЕНИЕМ
   ========================================== */
let currentFontSizeMod = 0;

window.toggleA11yPanel = function () {
    const panel = document.getElementById('a11yPanel');
    if (panel) panel.classList.toggle('open');
};

// Сохранение настроек в localStorage
window.saveA11yState = function () {
    let colorMode = 'none';
    if (document.body.classList.contains('a11y-protanopia')) colorMode = 'protanopia';
    else if (document.body.classList.contains('a11y-deuteranopia')) colorMode = 'deuteranopia';
    else if (document.body.classList.contains('a11y-tritanopia')) colorMode = 'tritanopia';

    const state = {
        fontSizeMod: currentFontSizeMod,
        contrast: document.body.classList.contains('a11y-contrast'),
        noImages: document.body.classList.contains('a11y-no-images'),
        colorBlind: colorMode
    };
    localStorage.setItem('a11ySettings', JSON.stringify(state));
};

window.loadA11yState = function () {
    const saved = localStorage.getItem('a11ySettings');
    if (saved) {
        const state = JSON.parse(saved);
        currentFontSizeMod = state.fontSizeMod || 0;
        document.documentElement.style.fontSize = (16 + currentFontSizeMod * 2) + 'px';

        if (state.contrast) document.body.classList.add('a11y-contrast');
        if (state.noImages) document.body.classList.add('a11y-no-images');
        if (state.colorBlind && state.colorBlind !== 'none') {
            document.body.classList.add('a11y-' + state.colorBlind);
        }
    }
};

window.changeFontSize = function (direction) {
    currentFontSizeMod += direction;
    if (currentFontSizeMod < -2) currentFontSizeMod = -2;
    if (currentFontSizeMod > 4) currentFontSizeMod = 4;
    document.documentElement.style.fontSize = (16 + currentFontSizeMod * 2) + 'px';
    window.saveA11yState();
};

window.resetFontSize = function () {
    currentFontSizeMod = 0;
    document.documentElement.style.fontSize = '16px';
    window.saveA11yState();
};

window.setContrastMode = function (enable) {
    if (enable) document.body.classList.add('a11y-contrast');
    else document.body.classList.remove('a11y-contrast');
    window.saveA11yState();
};

window.toggleImages = function (show) {
    if (!show) document.body.classList.add('a11y-no-images');
    else document.body.classList.remove('a11y-no-images');
    window.saveA11yState();
};

window.setColorBlindMode = function (mode) {
    document.body.classList.remove('a11y-protanopia', 'a11y-deuteranopia', 'a11y-tritanopia');

    if (mode !== 'none') {
        document.body.classList.add('a11y-' + mode);
    }
    window.saveA11yState();
};

document.addEventListener("DOMContentLoaded", () => {
    window.restoreTheme();
    window.loadA11yState();
});

/* ==========================================
   4. ПОИСК В ШАПКЕ
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
   5. ОГЛАВЛЕНИЕ ДЛЯ СТАТЕЙ (TOC)
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
   6. АНИМАЦИИ И УМНЫЙ СКРОЛЛ 
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
        if (elementTop < windowHeight - 50) {
            element.classList.add('visible');
        }
    });

    // 3. ScrollSpy (Подсветка активного пункта меню на главной)
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

window.addEventListener('scroll', window.checkScrollAndAnimations, { passive: true });

let animTimeout;
const domObserver = new MutationObserver(() => {
    clearTimeout(animTimeout);
    animTimeout = setTimeout(() => {
        window.checkScrollAndAnimations();
    }, 50);
});
domObserver.observe(document.body, { childList: true, subtree: true });

document.addEventListener('click', function (e) {
    const link = e.target.closest('a');
    if (!link) return;

    const href = link.getAttribute('href');
    if (!href) return;

    if (href === window.location.pathname) {
        e.preventDefault();
        window.scrollTo({ top: 0, behavior: 'smooth' });
        return;
    }

    if (href.includes('#')) {
        const hash = href.substring(href.indexOf('#'));
        const path = href.substring(0, href.indexOf('#'));

        if ((path === '' || path === window.location.pathname) && hash.length > 1) {
            const target = document.querySelector(hash);
            if (target) {
                e.preventDefault();
                const y = target.getBoundingClientRect().top + window.scrollY - 80;
                window.scrollTo({ top: y, behavior: 'smooth' });
                window.history.pushState(null, '', href);
            }
        }
    }
});

window.checkScrollAndAnimations();