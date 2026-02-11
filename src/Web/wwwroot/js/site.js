/**
 * site.js
 * Provides global form validation and modal reset logic for the application.
 */
document.addEventListener('DOMContentLoaded', function() {
    // Show success message stored from AJAX operations
    try {
        const flashSuccess = sessionStorage.getItem('flashSuccess');
        if (flashSuccess) {
            sessionStorage.removeItem('flashSuccess');
            const main = document.querySelector('main');
            if (main) {
                const closeLabel = window.i18n ? window.i18n.t('site.js', 'Close', 'Tancar') : 'Tancar';
                const alert = document.createElement('div');
                alert.className = 'alert alert-success alert-dismissible fade show m-3';
                alert.setAttribute('role', 'alert');
                alert.innerHTML = '<i class="bi bi-check-circle-fill"></i> ' +
                    flashSuccess +
                    '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="' + closeLabel + '"></button>';
                main.prepend(alert);
            }
        }
    } catch (e) {
        // ignore storage errors
    }

    const forms = document.querySelectorAll('.needs-validation');
    
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            
            form.classList.add('was-validated');
        }, false);
    });
    
    document.querySelectorAll('.modal').forEach(modal => {
        modal.addEventListener('hidden.bs.modal', function () {
            const form = this.querySelector('.needs-validation');
            if (form) {
                form.classList.remove('was-validated');
            }
        });
    });
});
