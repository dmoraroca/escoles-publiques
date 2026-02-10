/**
 * entity-modal.js
 * Handles modal form logic for creating and editing entities, including validation and error display.
 */
(function() {
    const modals = document.querySelectorAll('.modal[data-modal-id]');
    
    modals.forEach(modalElement => {
        const modalId = modalElement.dataset.modalId;
        const entityName = modalElement.dataset.entityName;
        
        const form = modalElement.querySelector('form');
        const errorAlert = document.getElementById(modalId + '-error');
        const errorMessage = document.getElementById(modalId + '-error-message');
        const submitButton = form.querySelector('button[type="submit"]');
        const privacyCheck = form.querySelector('.js-privacy-check');

        function updateSubmitState() {
            if (!privacyCheck || !submitButton) return;
            submitButton.disabled = !privacyCheck.checked;
        }
        
        modalElement.addEventListener('show.bs.modal', function() {
            errorAlert.classList.add('d-none');
            form.classList.remove('was-validated');
            updateSubmitState();
        });

        if (privacyCheck) {
            privacyCheck.addEventListener('change', updateSubmitState);
            updateSubmitState();
        }

        // Student email existence check (create modal)
        if (modalId === 'createStudentModal') {
            const emailInput = form.querySelector('input[name="Email"]');
            if (emailInput) {
                let checkTimer = null;
                let lastValue = '';
                const getMessageEl = () => {
                    let el = emailInput.parentElement?.querySelector('.js-email-exists');
                    if (!el) {
                        el = document.createElement('div');
                        el.className = 'text-danger small js-email-exists d-none';
                        el.textContent = "Aquest email ja existeix.";
                        emailInput.parentElement?.appendChild(el);
                    }
                    return el;
                };

                const setEmailValidity = (exists) => {
                    const msgEl = getMessageEl();
                    if (exists) {
                        emailInput.setCustomValidity('Email existent');
                        msgEl.classList.remove('d-none');
                    } else {
                        emailInput.setCustomValidity('');
                        msgEl.classList.add('d-none');
                    }
                };

                const checkEmail = async (value) => {
                    const trimmed = value.trim();
                    if (!trimmed) {
                        setEmailValidity(false);
                        return;
                    }
                    try {
                        const res = await fetch(`/Students/CheckEmail?email=${encodeURIComponent(trimmed)}`, {
                            headers: {
                                'X-Requested-With': 'XMLHttpRequest',
                                'Accept': 'application/json'
                            }
                        });
                        if (!res.ok) return;
                        const data = await res.json();
                        setEmailValidity(!!data.exists);
                    } catch (e) {
                        // ignore network errors
                    }
                };

                const scheduleCheck = () => {
                    const value = emailInput.value;
                    if (value === lastValue) return;
                    lastValue = value;
                    if (checkTimer) clearTimeout(checkTimer);
                    checkTimer = setTimeout(() => checkEmail(value), 300);
                };

                emailInput.addEventListener('blur', scheduleCheck);
                emailInput.addEventListener('input', scheduleCheck);
            }
        }
        
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
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'Accept': 'application/json'
                    },
                    body: formData,
                    redirect: 'manual'
                });
                
                if (response.status === 401 || response.status === 403) {
                    errorMessage.textContent = 'Sessió caducada. Torna a iniciar sessió.';
                    errorAlert.classList.remove('d-none');
                    window.location.href = '/Auth/Login';
                    return;
                }

                if (response.type === 'opaqueredirect' || response.status === 302 || response.status === 301) {
                    const location = response.headers.get('location');
                    if (location) {
                        window.location.href = location;
                        return;
                    }
                    // Redirigir a la pàgina actual sense /Index
                    window.location.href = window.location.pathname.replace(/\/Index$/, '');
                    return;
                }
                
                if (!response.ok) {
                    // Intentar llegir resposta JSON
                    const contentType = response.headers.get('content-type');
                    if (contentType && contentType.includes('application/json')) {
                        const json = await response.json();
                        errorMessage.textContent = json.error || 'Error creant el registre.';
                    } else {
                        const text = await response.text();
                        const parser = new DOMParser();
                        const doc = parser.parseFromString(text, 'text/html');
                        const tempDataError = doc.querySelector('.alert-danger');
                        
                        if (tempDataError) {
                            errorMessage.textContent = tempDataError.textContent.trim();
                        } else {
                            errorMessage.textContent = 'Error creant el registre. Si us plau, verifica les dades i intenta-ho de nou.';
                        }
                    }
                    errorAlert.classList.remove('d-none');
                    errorAlert.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
                } else {
                    let successMessage = 'Operació completada correctament.';
                    const contentType = response.headers.get('content-type');
                    if (contentType && contentType.includes('application/json')) {
                        const json = await response.json();
                        if (json && json.message) {
                            successMessage = json.message;
                        }
                    }
                    try {
                        sessionStorage.setItem('flashSuccess', successMessage);
                    } catch (e) {
                        // ignore storage errors
                    }
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
