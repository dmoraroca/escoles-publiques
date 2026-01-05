document.addEventListener('DOMContentLoaded', function() {
    // Event listeners per botons amb data-action
    document.querySelectorAll('[data-action]').forEach(button => {
        button.addEventListener('click', function() {
            const action = this.getAttribute('data-action');
            switch(action) {
                case 'edit':
                    enableEditMode();
                    break;
                case 'cancel':
                    cancelEdit();
                    break;
                case 'submit':
                    submitForm();
                    break;
                case 'close-alert':
                    closeAlert();
                    break;
            }
        });
    });
});

function enableEditMode() {
    document.getElementById('viewButtons').style.display = 'none';
    document.getElementById('editButtons').style.display = 'block';
    
    document.querySelectorAll('#editForm input:not([disabled])').forEach(input => {
        input.removeAttribute('readonly');
        input.classList.remove('bg-light');
    });
    
    document.querySelectorAll('#editForm select').forEach(select => {
        select.removeAttribute('disabled');
        select.classList.remove('bg-light');
    });
    
    document.getElementById('statusDisplay').style.display = 'none';
    document.getElementById('statusEdit').style.display = 'block';
    
    document.querySelectorAll('.edit-only').forEach(elem => {
        elem.style.display = 'inline';
    });
}

function cancelEdit() {
    document.getElementById('editButtons').style.display = 'none';
    document.getElementById('viewButtons').style.display = 'block';
    
    document.querySelectorAll('#editForm input:not([disabled])').forEach(input => {
        input.setAttribute('readonly', 'readonly');
        input.classList.add('bg-light');
    });
    
    document.querySelectorAll('#editForm select').forEach(select => {
        select.setAttribute('disabled', 'disabled');
        select.classList.add('bg-light');
    });
    
    document.getElementById('statusDisplay').style.display = 'flex';
    document.getElementById('statusEdit').style.display = 'none';
    
    document.querySelectorAll('.edit-only').forEach(elem => {
        elem.style.display = 'none';
    });
    
    location.reload();
}

function submitForm() {
    const form = document.getElementById('editForm');
    
    const amountInput = document.getElementById('Amount');
    if (amountInput.value) {
        amountInput.value = amountInput.value.replace(/,/g, '.');
    }
    
    document.querySelectorAll('#editForm select').forEach(select => {
        select.removeAttribute('disabled');
    });
    
    form.classList.add('was-validated');
    
    if (!form.checkValidity()) {
        document.getElementById('errorAlert').style.display = 'block';
        window.scrollTo({ top: 0, behavior: 'smooth' });
        return false;
    }
    
    form.submit();
}

function closeAlert() {
    document.getElementById('errorAlert').style.display = 'none';
}
