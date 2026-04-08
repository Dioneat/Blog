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

const scrollBtn = document.getElementById('scrollToTop');

function checkScrollAndAnimations() {
    if (scrollBtn) {
        if (document.body.scrollTop > 400 || document.documentElement.scrollTop > 400) {
            scrollBtn.classList.add('show');
        } else {
            scrollBtn.classList.remove('show');
        }
    }

    const fadeElements = document.querySelectorAll('.fade-up');
    fadeElements.forEach(element => {
        const elementTop = element.getBoundingClientRect().top;
        const windowHeight = window.innerHeight;
        if (elementTop < windowHeight - 100) {
            element.classList.add('visible');
        }
    });
}

window.onscroll = checkScrollAndAnimations;

function scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

checkScrollAndAnimations();


const observer = new MutationObserver(checkScrollAndAnimations);
observer.observe(document.body, { childList: true, subtree: true });


function toggleTheme() {
    document.body.classList.toggle('dark-theme');
    const icon = document.getElementById('themeIcon');
    if (document.body.classList.contains('dark-theme')) {
        icon.classList.remove('fa-moon');
        icon.classList.add('fa-sun');
        localStorage.setItem('theme', 'dark');
    } else {
        icon.classList.remove('fa-sun');
        icon.classList.add('fa-moon');
        localStorage.setItem('theme', 'light');
    }
}


if (localStorage.getItem('theme') === 'dark') {
    document.body.classList.add('dark-theme');
    document.getElementById('themeIcon').classList.remove('fa-moon');
    document.getElementById('themeIcon').classList.add('fa-sun');
}

function toggleSearch() {
    const wrapper = document.getElementById('searchWrapper');
    wrapper.classList.toggle('active');
    if (wrapper.classList.contains('active')) {
        wrapper.querySelector('.search-input').focus();
    }
}


document.addEventListener('click', function (e) {
    const wrapper = document.getElementById('searchWrapper');

    if (wrapper && !wrapper.contains(e.target) && wrapper.classList.contains('active')) {
        wrapper.classList.remove('active');
    }
});



function toggleA11yPanel() {
    document.getElementById('a11yPanel').classList.toggle('open');
}


let currentFontSizeMod = 0;
function changeFontSize(direction) {
    currentFontSizeMod += direction;
    if (currentFontSizeMod < -1) currentFontSizeMod = -1;
    if (currentFontSizeMod > 3) currentFontSizeMod = 3;

    const baseSize = 16;
    document.documentElement.style.fontSize = (baseSize + currentFontSizeMod * 2) + 'px';
}

function resetFontSize() {
    currentFontSizeMod = 0;
    document.documentElement.style.fontSize = '16px';
}

function setContrastMode(enable) {
    if (enable) {
        document.body.classList.add('a11y-contrast');
    } else {
        document.body.classList.remove('a11y-contrast');
    }
}


function toggleImages(show) {
    if (!show) {
        document.body.classList.add('a11y-no-images');
    } else {
        document.body.classList.remove('a11y-no-images');
    }
}


const sections = document.querySelectorAll('section');
const navLi = document.querySelectorAll('.nav-links a');

window.addEventListener('scroll', () => {
    let current = '';
    sections.forEach(section => {
        const sectionTop = section.offsetTop;
        const sectionHeight = section.clientHeight;
        if (pageYOffset >= (sectionTop - sectionHeight / 3)) {
            current = section.getAttribute('id');
        }
    });

    navLi.forEach(a => {
        a.classList.remove('active-link');
        if (a.getAttribute('href').includes(current)) {
            a.classList.add('active-link');
        }
    });
});