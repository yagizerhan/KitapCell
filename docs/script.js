document.addEventListener('DOMContentLoaded', () => {
    // Scroll Animation Observer
    const observerOptions = {
        root: null,
        rootMargin: '0px',
        threshold: 0.1
    };

    const observer = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animated');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    document.querySelectorAll('[data-animate]').forEach((el) => {
        observer.observe(el);
    });

    // Download Button Interaction
    const downloadBtn = document.getElementById('mainDownloadBtn');
    if (downloadBtn) {
        downloadBtn.addEventListener('click', (e) => {
            e.preventDefault();
            
            // Temporary interaction for the placeholder
            const originalText = downloadBtn.innerHTML;
            
            downloadBtn.innerHTML = `
                <span class="btn-icon">⏳</span>
                <div class="btn-text">
                    <strong>İndirme Başlıyor...</strong>
                    <span>Lütfen bekleyin</span>
                </div>
            `;
            
            downloadBtn.style.pointerEvents = 'none';
            downloadBtn.style.opacity = '0.8';

            // Simulate file preparation
            setTimeout(() => {
                // Here we will put the actual setup.exe link later.
                // window.location.href = "KitapCell_Setup_0.1.0-beta.exe";
                
                downloadBtn.innerHTML = `
                    <span class="btn-icon">✅</span>
                    <div class="btn-text">
                        <strong>İndirme Tamamlandı</strong>
                        <span>Kurulum dosyasına göz atın</span>
                    </div>
                `;
                
                setTimeout(() => {
                    downloadBtn.innerHTML = originalText;
                    downloadBtn.style.pointerEvents = 'auto';
                    downloadBtn.style.opacity = '1';
                }, 3000);
            }, 1500);
        });
    }

    // Smooth Scrolling for Anchors
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            if(targetId === '#') return;
            
            const targetElement = document.querySelector(targetId);
            if(targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth'
                });
            }
        });
    });
});
