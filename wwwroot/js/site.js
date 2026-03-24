/* ========================================
   HRMS — Interactions (site.js)
   ======================================== */

document.addEventListener('DOMContentLoaded', () => {
    initSidebar();
    initThemeToggle();
    initGlobalSearch();
    initCounterAnimation();
    initTiltCards();
    initMagneticButtons();
    initTopbarScroll();
    initDeleteModal();
    initCursorGlow();
    initAutoAlertDismiss();
    initActiveNav();
    pageCurtain();
});

/* Sidebar */
function initSidebar() {
    const toggle = document.getElementById('sidebar-toggle');
    const sidebar = document.getElementById('sidebar');
    if (!toggle || !sidebar) return;
    toggle.addEventListener('click', () => sidebar.classList.toggle('show'));
    document.addEventListener('click', (e) => {
        if (window.innerWidth <= 768 && !sidebar.contains(e.target) && !toggle.contains(e.target))
            sidebar.classList.remove('show');
    });
}

/* Theme */
function initThemeToggle() {
    const btn = document.getElementById('themeToggle');
    if (!btn) return;
    const saved = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-theme', saved);
    btn.querySelector('i').className = saved === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
    btn.addEventListener('click', () => {
        const curr = document.documentElement.getAttribute('data-theme');
        const next = curr === 'dark' ? 'light' : 'dark';
        document.documentElement.setAttribute('data-theme', next);
        localStorage.setItem('theme', next);
        btn.querySelector('i').className = next === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
    });
}

/* Global AJAX search */
function initGlobalSearch() {
    const input = document.getElementById('globalSearchInput');
    const results = document.getElementById('searchResults');
    if (!input || !results) return;
    let timer;
    input.addEventListener('input', () => {
        clearTimeout(timer);
        const term = input.value.trim();
        if (term.length < 2) { results.classList.remove('show'); return; }
        timer = setTimeout(async () => {
            try {
                const res = await fetch(`/Search/GlobalSearch?term=${encodeURIComponent(term)}`);
                const data = await res.json();
                if (data.length === 0) { results.innerHTML = '<div class="search-item text-muted">Không tìm thấy</div>'; }
                else {
                    results.innerHTML = data.map(d =>
                        `<a href="/Employee/Details/${d.id}" class="search-item"><div><strong>${d.maNV}</strong> ${d.hoTen}<br><small class="text-muted">${d.phongBan} — ${d.chucVu}</small></div></a>`
                    ).join('');
                }
                results.classList.add('show');
            } catch (e) { results.classList.remove('show'); }
        }, 300);
    });
    document.addEventListener('click', (e) => { if (!e.target.closest('.global-search')) results.classList.remove('show'); });
}

/* Counter animation */
function initCounterAnimation() {
    document.querySelectorAll('[data-counter]').forEach(el => {
        const target = parseInt(el.textContent) || 0;
        el.textContent = '0';
        const dur = 1200;
        const start = performance.now();
        const easeOut = t => 1 - Math.pow(1 - t, 3);
        function tick(now) {
            const t = Math.min((now - start) / dur, 1);
            el.textContent = Math.round(easeOut(t) * target);
            if (t < 1) requestAnimationFrame(tick);
        }
        requestAnimationFrame(tick);
    });
}

/* 3D Tilt */
function initTiltCards() {
    document.querySelectorAll('.tilt-card').forEach(card => {
        const inner = card.querySelector('.tilt-card-inner') || card;
        card.addEventListener('mousemove', (e) => {
            const r = card.getBoundingClientRect();
            const x = (e.clientX - r.left) / r.width;
            const y = (e.clientY - r.top) / r.height;
            const ry = (x - .5) * 10; const rx = (y - .5) * -10;
            inner.style.transform = `rotateX(${rx}deg) rotateY(${ry}deg)`;
            card.style.setProperty('--mouse-x', `${e.clientX - r.left}px`);
            card.style.setProperty('--mouse-y', `${e.clientY - r.top}px`);
        });
        card.addEventListener('mouseleave', () => { inner.style.transform = ''; });
    });
}

/* Magnetic buttons */
function initMagneticButtons() {
    document.querySelectorAll('.btn-magnetic').forEach(btn => {
        const text = btn.querySelector('.btn-text') || btn;
        btn.addEventListener('mousemove', (e) => {
            const r = btn.getBoundingClientRect();
            const dx = (e.clientX - (r.left + r.width / 2)) * 0.3;
            const dy = (e.clientY - (r.top + r.height / 2)) * 0.3;
            btn.style.transform = `translate(${dx}px, ${dy}px)`;
            text.style.transform = `translate(${dx * 0.2}px, ${dy * 0.2}px)`;
        });
        btn.addEventListener('mouseleave', () => { btn.style.transform = ''; text.style.transform = ''; });
    });
}

/* Topbar scroll shadow */
function initTopbarScroll() {
    const topbar = document.querySelector('.topbar');
    if (!topbar) return;
    window.addEventListener('scroll', () => topbar.classList.toggle('scrolled', window.scrollY > 10), { passive: true });
}

/* Delete modal */
function initDeleteModal() {
    window.confirmDelete = function (name, action) {
        const modal = document.getElementById('deleteModal');
        if (!modal) return;
        document.getElementById('deleteItemName').textContent = name;
        document.getElementById('deleteForm').action = action;
        new bootstrap.Modal(modal).show();
    };
}

/* Cursor glow */
function initCursorGlow() {
    if (window.innerWidth < 768) return;
    const glow = document.createElement('div');
    glow.className = 'cursor-glow';
    document.body.appendChild(glow);
    document.addEventListener('mousemove', (e) => { glow.style.left = e.clientX + 'px'; glow.style.top = e.clientY + 'px'; });
}

/* Auto dismiss alerts */
function initAutoAlertDismiss() {
    document.querySelectorAll('.alert').forEach(alert => {
        setTimeout(() => {
            alert.style.transition = 'opacity .4s, transform .4s, max-height .3s';
            alert.style.opacity = '0'; alert.style.transform = 'translateY(-8px)';
            setTimeout(() => alert.remove(), 400);
        }, 5000);
    });
}

/* Active nav highlight */
function initActiveNav() {
    const path = window.location.pathname;
    document.querySelectorAll('.sidebar-nav a').forEach(a => {
        if (a.getAttribute('href') && path.startsWith(a.getAttribute('href')))
            a.classList.add('active');
    });
}

/* Page curtain on load */
function pageCurtain() {
    const curtain = document.createElement('div');
    curtain.className = 'page-curtain';
    curtain.innerHTML = '<div class="curtain-half curtain-left"></div><div class="curtain-half curtain-right"></div>';
    document.body.appendChild(curtain);
    setTimeout(() => curtain.remove(), 1200);
}
