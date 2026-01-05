(function() {
    const modals = document.querySelectorAll('.modal[data-modal-id]');
    
    modals.forEach(modalElement => {
        const modalId = modalElement.dataset.modalId;
        const entityName = modalElement.dataset.entityName;
        
        const form = modalElement.querySelector('form');
        const errorAlert = document.getElementById(modalId + '-error');
        const errorMessage = document.getElementById(modalId + '-error-message');
        const submitButton = form.querySelector('button[type="submit"]');
        
        modalElement.addEventListener('show.bs.modal', function() {
            errorAlert.classList.add('d-none');
            form.classList.remove('was-validated');
            form.reset();
        });
        
        form.addEventListener('submit', async function(event) {
            event.preventDefault();
            event.stopPropagation();
            
            errorAlert.classList.add('d-none');
            
            if (form.checkValidity() === false) {
                form.classList.add('was-validated');
                errorAlert.classList.remove('d-none');
                errorMessage.textContent = 'Si us plau, omple tots els camps obligatoris correctament.';
                errorAlert.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
                return;
            }
            
            try {
                submitButton.disabled = true;
                submitButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Creant...';
                
                const formData = new FormData(form);
                const response = await fetch(form.action, {
                    method: 'POST',
                    body: formData,
                    redirect: 'manual'
                });
                
                if (response.type === 'opaqueredirect' || response.status === 302 || response.status === 301) {
                    window.location.href = form.action.replace('/Create', '');
                    return;
                }
                
                if (!response.ok) {
                    const text = await response.text();
                    const parser = new DOMParser();
                    const doc = parser.parseFromString(text, 'text/html');
                    const tempDataError = doc.querySelector('.alert-danger');
                    
                    if (tempDataError) {
                        errorMessage.textContent = tempDataError.textContent.trim();
                    } else {
                        errorMessage.textContent = 'Error creant el registre. Si us plau, verifica les dades i intenta-ho de nou.';
                    }
                    errorAlert.classList.remove('d-none');
                } else {
                    window.location.reload();
                }
            } catch (error) {
                console.error('Error:', error);
                errorMessage.textContent = 'Error de connexió. Si us plau, intenta-ho de nou.';
                errorAlert.classList.remove('d-none');
            } finally {
                submitButton.disabled = false;
                submitButton.innerHTML = '<i class="bi bi-check-lg"></i> Crear ' + entityName;
            }
        });
    });
})();
