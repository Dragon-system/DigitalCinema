/* ═══════════════════════════════════════════════════════
   layout.js  –  CineMax Shared Layout Scripts
   ═══════════════════════════════════════════════════════ */

/* ── Sidebar toggle ── */
let sidebarCollapsed = false;

function toggleSidebar() {
    sidebarCollapsed = !sidebarCollapsed;

    const sidebar   = document.getElementById('sidebar');
    const mainArea  = document.getElementById('mainArea');
    const hamburger = document.getElementById('hamburgerIcon');

    sidebar.classList.toggle('collapsed', sidebarCollapsed);
    mainArea.classList.toggle('expanded', sidebarCollapsed);
    hamburger.className = sidebarCollapsed
        ? 'bi bi-layout-sidebar-reverse'
        : 'bi bi-list';

    // Mobile: slide in/out instead of collapsing
    if (window.innerWidth <= 768) {
        sidebar.classList.toggle('mobile-open');
    }
}

/* ── Sidebar tooltips (shown only when collapsed) ── */
const tip = document.getElementById('sidebarTip');

document.querySelectorAll('.sidebar-item[data-tip]').forEach(item => {
    item.addEventListener('mouseenter', () => {
        if (!sidebarCollapsed) return;
        tip.textContent = item.dataset.tip;
        const rect = item.getBoundingClientRect();
        tip.style.top = (rect.top + rect.height / 2 - 16) + 'px';
        tip.classList.add('show');
    });
    item.addEventListener('mouseleave', () => tip.classList.remove('show'));
});
