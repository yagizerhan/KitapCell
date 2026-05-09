document.addEventListener('DOMContentLoaded', () => {
    const bookRows = document.querySelectorAll('.book-row');
    const bookCards = document.querySelectorAll('.book-card');
    const viewContainer = document.getElementById('library-view');
    const viewToggles = document.querySelectorAll('.view-toggle');
    const detailPanel = document.querySelector('.detail-panel');
    const closeBtn = document.querySelector('.close-btn');
    const detailBookTitle = document.querySelector('.detail-body .book-title');
    const detailBookAuthor = document.querySelector('.detail-body .book-author');

    // Sample book data
    const bookData = {
        '1': { title: 'Sefiller', author: 'Victor Hugo' },
        '2': { title: 'Dune', author: 'Frank Herbert' },
        '3': { title: '1984', author: 'George Orwell' }
    };

    function updateDetailPanel(id) {
        const data = bookData[id];
        if (data) {
            detailBookTitle.textContent = data.title;
            detailBookAuthor.textContent = data.author;
            detailPanel.style.display = 'flex';
        }
    }

    // Handle book row selection
    bookRows.forEach(row => {
        row.addEventListener('click', () => {
            bookRows.forEach(r => r.classList.remove('selected'));
            row.classList.add('selected');
            updateDetailPanel(row.dataset.id);
        });
    });

    // Handle grid card selection
    bookCards.forEach(card => {
        card.addEventListener('click', () => {
            bookCards.forEach(c => c.style.borderColor = '');
            card.style.borderColor = 'var(--accent-blue)';
            updateDetailPanel(card.dataset.id);
        });
    });

    // View Switching
    viewToggles.forEach(toggle => {
        toggle.addEventListener('click', () => {
            viewToggles.forEach(t => t.classList.remove('active'));
            toggle.classList.add('active');
            
            const view = toggle.dataset.view;
            if (view === 'grid') {
                viewContainer.classList.remove('list-view-active');
                viewContainer.classList.add('grid-view-active');
            } else {
                viewContainer.classList.remove('grid-view-active');
                viewContainer.classList.add('list-view-active');
            }
        });
    });

    // Modals
    const addBtn = document.getElementById('add-book-btn');
    const addModal = document.getElementById('add-book-modal');
    const modalClose = addModal.querySelector('.modal-close');
    const modalCancel = addModal.querySelector('.modal-cancel');

    const showModal = (modal) => modal.style.display = 'flex';
    const hideModal = (modal) => modal.style.display = 'none';

    addBtn.addEventListener('click', () => showModal(addModal));
    [modalClose, modalCancel].forEach(btn => {
        btn.addEventListener('click', () => hideModal(addModal));
    });

    // Close detail panel
    closeBtn.addEventListener('click', () => {
        detailPanel.style.display = 'none';
    });

    // Simple search filter simulation
    const searchInput = document.querySelector('.search-container input');
    searchInput.addEventListener('input', (e) => {
        const term = e.target.value.toLowerCase();
        bookRows.forEach(row => {
            const text = row.textContent.toLowerCase();
            row.style.display = text.includes(term) ? '' : 'none';
        });
        bookCards.forEach(card => {
            const text = card.textContent.toLowerCase();
            card.style.display = text.includes(term) ? '' : 'none';
        });
    });

    // --- Authentication and Admin Logic ---
    const authButtons = document.getElementById('auth-buttons');
    const userProfile = document.getElementById('user-profile');
    const loginBtn = document.getElementById('login-btn');
    const registerBtn = document.getElementById('register-btn');
    const logoutBtn = document.getElementById('logout-btn');
    const displayUserName = document.getElementById('display-user-name');
    const displayUserRole = document.getElementById('display-user-role');
    const adminNavSection = document.getElementById('admin-nav-section');
    
    const libraryView = document.getElementById('library-view');
    const membersView = document.getElementById('members-view');
    const navMembers = document.getElementById('nav-members');
    const navLibraryItems = document.querySelectorAll('.nav-section a.nav-item:not(#nav-members)');

    let currentUser = null; 
    
    const mockMembers = [
        { username: 'admin_user', role: 'Admin', date: '01 Oca 2024', status: 'Aktif' },
        { username: 'johndoe', role: 'Kullanıcı', date: '15 Şub 2024', status: 'Aktif' },
        { username: 'janedoe', role: 'Kullanıcı', date: '10 Mar 2024', status: 'Pasif' }
    ];

    function updateAuthUI() {
        if (currentUser) {
            authButtons.classList.add('hidden');
            userProfile.classList.remove('hidden');
            displayUserName.textContent = currentUser.username;
            displayUserRole.textContent = currentUser.role === 'admin' ? 'Yönetici' : 'Üye';
            
            if (currentUser.role === 'admin') {
                adminNavSection.classList.remove('hidden');
            } else {
                adminNavSection.classList.add('hidden');
            }
        } else {
            authButtons.classList.remove('hidden');
            userProfile.classList.add('hidden');
            adminNavSection.classList.add('hidden');
            showLibraryView();
        }
    }

    loginBtn.addEventListener('click', () => {
        // Toggle: Admin -> User login
        if (!currentUser) {
            currentUser = { username: 'AdminAccount', role: 'admin' };
            alert('Admin olarak giriş yapıldı (Test)');
        }
        updateAuthUI();
    });
    
    registerBtn.addEventListener('click', () => {
        if (!currentUser) {
            currentUser = { username: 'NewUser', role: 'user' };
            alert('Kullanıcı olarak kayıt olundu/giriş yapıldı (Test)');
        }
        updateAuthUI();
    });

    logoutBtn.addEventListener('click', () => {
        currentUser = null;
        updateAuthUI();
    });

    // View Navigation
    function showLibraryView() {
        libraryView.classList.remove('hidden');
        membersView.classList.add('hidden');
        navMembers.classList.remove('active');
    }

    function showMembersView() {
        libraryView.classList.add('hidden');
        membersView.classList.remove('hidden');
        navLibraryItems.forEach(item => item.classList.remove('active'));
        navMembers.classList.add('active');
        populateMembersList();
    }

    navMembers.addEventListener('click', (e) => {
        e.preventDefault();
        showMembersView();
    });

    navLibraryItems.forEach(item => {
        item.addEventListener('click', (e) => {
            if(item.parentElement.id !== 'admin-nav-section') {
                navLibraryItems.forEach(i => i.classList.remove('active'));
                item.classList.add('active');
                showLibraryView();
            }
        });
    });

    function populateMembersList() {
        const tbody = document.getElementById('members-list-body');
        tbody.innerHTML = '';
        mockMembers.forEach(member => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${member.username}</td>
                <td><strong>${member.role}</strong></td>
                <td>${member.date}</td>
                <td>${member.status}</td>
            `;
            tbody.appendChild(tr);
        });
    }

    updateAuthUI();
});
