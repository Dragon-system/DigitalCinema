/* ═══════════════════════════════════════════════════════
   login.js  –  CineMax Login Page Scripts
   ═══════════════════════════════════════════════════════ */

/* ── Generate floating stars ── */
(function generateStars() {
    const layer = document.getElementById('stars');
    if (!layer) return;

    const style = document.createElement('style');
    style.textContent = '@keyframes tw{0%,100%{opacity:0;transform:scale(.6)}50%{opacity:var(--op);transform:scale(1)}}';
    document.head.appendChild(style);

    for (let i = 0; i < 25; i++) {
        const s = document.createElement('div');
        const size = (Math.random() * 2 + 1).toFixed(1);

        Object.assign(s.style, {
            position:     'absolute',
            borderRadius: '50%',
            width:        size + 'px',
            height:       size + 'px',
            background:   '#f0d080',
            top:          Math.random() * 100 + '%',
            left:         Math.random() * 100 + '%',
            '--op':       (Math.random() * 0.4 + 0.15).toFixed(2),
            animation:    `tw ${(Math.random() * 4 + 3).toFixed(1)}s ${(Math.random() * 6).toFixed(1)}s ease-in-out infinite`
        });

        layer.appendChild(s);
    }
})();

/* ── Password visibility toggle ── */
function togglePass() {
    const input = document.getElementById('Password');
    const icon  = document.getElementById('eyeIco');

    if (!input) return;

    if (input.type === 'password') {
        input.type    = 'text';
        icon.className = 'bi bi-eye-slash';
    } else {
        input.type    = 'password';
        icon.className = 'bi bi-eye';
    }
}

/* ── Client-side form validation ── */
(function initValidation() {
    const form = document.getElementById('loginForm');
    if (!form) return;

    form.addEventListener('submit', function (e) {
        const emailInput    = form.querySelector('input[name="Email"]');
        const passwordInput = form.querySelector('input[name="Password"]');

        const email    = emailInput?.value.trim()    ?? '';
        const password = passwordInput?.value.trim() ?? '';

        if (!email || !password) {
            e.preventDefault();
            alert('يرجى إدخال البريد الإلكتروني وكلمة المرور.');
            return;
        }

        // Basic email format check
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            e.preventDefault();
            alert('يرجى إدخال بريد إلكتروني صحيح.');
        }
    });
})();
