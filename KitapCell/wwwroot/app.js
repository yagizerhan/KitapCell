/* ── KitapCell Web UI — app.js (shared helpers) ── */

// ── Security: XSS Protection ────────────────────────────────────────────────

function escapeHtml(str) {
  if (str === null || str === undefined) return '';
  return String(str)
    .replace(/&/g,  '&amp;')
    .replace(/</g,  '&lt;')
    .replace(/>/g,  '&gt;')
    .replace(/"/g,  '&quot;')
    .replace(/'/g,  '&#039;');
}

// ── Auth ─────────────────────────────────────────────────────────────────────

let currentUser = null;

// Tries to fetch the current session user.
// Returns the user object or null — never redirects.
async function fetchMe() {
  try {
    const r = await fetch('/api/auth/me');
    if (r.ok) { currentUser = await r.json(); return currentUser; }
  } catch {}
  return null;
}

// For pages that are only accessible when logged in (e.g. profile, favorites).
// Redirects to login.html if the session is missing.
async function requireAuth() {
  const u = await fetchMe();
  if (!u) { window.location.href = '/login.html'; return null; }
  return u;
}

// For public pages: loads the user if a session exists, otherwise returns null.
// Pages using this function stay accessible to guests.
async function getUser() {
  return await fetchMe();
}

async function logout() {
  await fetch('/api/auth/logout', { method: 'POST' });
  window.location.href = '/index.html';
}

// ── API ──────────────────────────────────────────────────────────────────────

async function api(method, url, body) {
  const opts = {
    method,
    headers: { 'Content-Type': 'application/json' },
  };
  if (body !== undefined) opts.body = JSON.stringify(body);
  const r = await fetch(url, opts);
  const data = await r.json().catch(() => ({}));
  return { ok: r.ok, status: r.status, data };
}

// ── Navbar ───────────────────────────────────────────────────────────────────

// Accepts user = null for guest visitors
function buildNavbar(user) {
  const nav = document.getElementById('navbar');
  if (!nav) return;

  const brandHtml = `
    <a class="navbar-brand" href="/index.html">
      <img src="/favicon.ico" alt="KitapCell" style="height:26px;width:26px;border-radius:5px;object-fit:contain;">
      <span class="brand-text">KitapCell</span>
    </a>
    <div class="navbar-search">
      <span class="icon">🔍</span>
      <input type="text" id="globalSearch" placeholder="Kitap veya yazar ara..." autocomplete="off">
    </div>
    <div class="navbar-actions">`;

  if (user) {
    // ── Logged-in user ──────────────────────────────────────────────────────
    const initials = (user.name || '?').split(' ').map(w => w[0]).join('').substring(0,2).toUpperCase();
    const isAdmin  = user.role === 'Admin';
    const canManage = isAdmin || user.canAddBook || user.canEditBook || user.canDeleteBook;

    const avatarHtml = user.hasProfileImage
      ? `<img src="/api/me/avatar?t=${Date.now()}" style="width:100%;height:100%;border-radius:50%;object-fit:cover;">`
      : escapeHtml(initials);

    nav.innerHTML = brandHtml + `
      <div class="user-menu">
        <button class="user-btn" id="userBtn">
          <div class="user-avatar">${avatarHtml}</div>
          <span>${escapeHtml(user.name)}</span>
          <span>▾</span>
        </button>
        <div class="dropdown" id="userDropdown">
          <a href="/index.html">🏠 Ana Sayfa</a>
          <a href="/topreads.html">🔥 En Çok Okunanlar</a>
          <a href="/history.html">🕒 Son Okuduklarım</a>
          <a href="/favorites.html">❤️ Favorilerim</a>
          <div class="dropdown-sep"></div>
          <a href="/profile.html">👤 Profilim</a>
          ${canManage ? '<a href="/admin.html">📚 Kitap Yönetimi</a>' : ''}
          <div class="dropdown-sep"></div>
          <button onclick="logout()">🚪 Çıkış Yap</button>
        </div>
      </div>
    </div>`;

    document.getElementById('userBtn').addEventListener('click', e => {
      e.stopPropagation();
      document.getElementById('userDropdown').classList.toggle('open');
    });
    document.addEventListener('click', () =>
      document.getElementById('userDropdown')?.classList.remove('open'));

  } else {
    // ── Guest ───────────────────────────────────────────────────────────────
    nav.innerHTML = brandHtml + `
      <a href="/login.html" class="btn btn-outline btn-sm">Giriş Yap</a>
      <a href="/register.html" class="btn btn-primary btn-sm">Kayıt Ol</a>
    </div>`;
  }

  // Global search (works for both guest and logged-in)
  const searchEl = document.getElementById('globalSearch');
  if (searchEl) {
    const params = new URLSearchParams(window.location.search);
    if (params.has('q')) searchEl.value = params.get('q');
    searchEl.addEventListener('keydown', e => {
      if (e.key === 'Enter' && searchEl.value.trim()) {
        window.location.href = `/index.html?q=${encodeURIComponent(searchEl.value.trim())}`;
      }
    });
  }
}

// ── Sidebar ──────────────────────────────────────────────────────────────────

// user = null is valid for guest mode
function buildSidebar(user) {
  const el = document.getElementById('sidebar');
  if (!el) return;

  const path = window.location.pathname;

  // Public links always shown
  const links = [
    { href: '/index.html',     icon: '📚', label: 'Tüm Kitaplar' },
    { href: '/topreads.html',  icon: '🔥', label: 'En Çok Okunanlar' },
    { href: '/members.html',   icon: '👥', label: 'Üyeler' },
  ];

  // Personal links only for logged-in users
  if (user) {
    links.splice(2, 0,
      { href: '/history.html',   icon: '🕒', label: 'Son Okuduklarım' },
      { href: '/favorites.html', icon: '❤️', label: 'Favorilerim' }
    );
  }

  const isActive = href => {
    if (href === '/index.html') return path === '/' || path === '/index.html';
    return path === href || path.endsWith(href.replace('/', ''));
  };

  const navHtml = `
    <div style="margin-bottom:18px;">
      <h3 style="margin-bottom:10px;">Keşfet</h3>
      <ul style="list-style:none;display:flex;flex-direction:column;gap:4px;">
        ${links.map(l => `
          <li>
            <a href="${l.href}" class="sidebar-nav-btn${isActive(l.href) ? ' active' : ''}">
              ${l.icon} ${l.label}
            </a>
          </li>`).join('')}
      </ul>
    </div>
    ${!user ? `
    <div style="border-top:1px solid var(--border);padding-top:14px;display:flex;flex-direction:column;gap:8px;">
      <a href="/login.html"    class="btn btn-outline btn-sm" style="justify-content:center;">Giriş Yap</a>
      <a href="/register.html" class="btn btn-primary btn-sm" style="justify-content:center;">Kayıt Ol</a>
    </div>` : ''}
  `;

  const existingNav = el.querySelector('#sidebarNav');
  if (existingNav) {
    existingNav.innerHTML = navHtml;
  } else {
    const div = document.createElement('div');
    div.id = 'sidebarNav';
    div.innerHTML = navHtml;
    el.insertBefore(div, el.firstChild);
  }
}

// ── Toast notifications ──────────────────────────────────────────────────────

function showToast(msg, type = 'info') {
  const t = document.createElement('div');
  t.className = `toast ${type}`;
  const icons = { success: '✅', error: '❌', info: 'ℹ️', warning: '⚠️' };
  t.innerHTML = `<span>${icons[type] ?? 'ℹ️'}</span> <span>${msg}</span>`;
  document.body.appendChild(t);
  setTimeout(() => t.remove(), 3500);
}

// ── Stars helper ─────────────────────────────────────────────────────────────

function renderStars(score, max = 5) {
  let s = '';
  for (let i = 1; i <= max; i++)
    s += `<span>${i <= score ? '⭐' : '☆'}</span>`;
  return s;
}

function renderStarsSmall(score) {
  return '⭐'.repeat(Math.round(score)) + '☆'.repeat(5 - Math.round(score));
}

// ── Cover helper ─────────────────────────────────────────────────────────────

function coverHtml(book, height = '200px') {
  if (book.hasCover)
    return `<img src="/cover/${book.id}" alt="${escapeHtml(book.title)}" loading="lazy" style="height:${height};width:100%;object-fit:cover;">`;
  return '📘';
}
