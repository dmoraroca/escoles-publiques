/**
 * annualfees-details.js
 * Handles UI interactions and edit logic for annual fee detail pages, including form actions and alerts.
 */
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

    // Inputs editables: PaidAt, PaymentRef, Amount, DueDate
    ['PaidAt', 'PaymentRef', 'Amount', 'DueDate'].forEach(function(id) {
        var el = document.getElementById(id);
        if (el) {
            el.removeAttribute('readonly');
            el.classList.remove('bg-light');
        }
    });

    // Select editable (Status)
    var statusSel = document.getElementById('Status');
    if (statusSel) {
        statusSel.removeAttribute('disabled');
        statusSel.classList.remove('bg-light');
    }

    // Checkbox editable (si existeix)
    var paidCheckbox = document.getElementById('MarkAsPaid');
    if (paidCheckbox) {
        paidCheckbox.removeAttribute('disabled');
    }

    // Inputs readonly: tots menys PaidAt, PaymentRef, Amount, DueDate
    document.querySelectorAll('#editForm input').forEach(input => {
        if (['PaidAt', 'PaymentRef', 'MarkAsPaid', 'Amount', 'DueDate'].indexOf(input.id) === -1) {
            input.setAttribute('readonly', 'readonly');
            input.classList.add('bg-light');
        }
    });

    // Selects readonly: només Status editable
    document.querySelectorAll('#editForm select').forEach(select => {
        if (select.id !== 'Status') {
            select.setAttribute('disabled', 'disabled');
            select.classList.add('bg-light');
        }
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
