// schools-details.js
// Handles edit mode toggling for school details page.
document.addEventListener('DOMContentLoaded', function() {
    var editBtn = document.getElementById('editButton');
    var wrapper = document.getElementById('saveCancelWrapper');
    var form = document.getElementById('editForm');
    if (!editBtn || !wrapper || !form) return;

    // Ensure save/cancel wrapper is hidden on initial load
    wrapper.style.display = 'none';
    // Hide internal buttons and privacy checkbox initially
    var internalSave = wrapper.querySelector('.save-btn');
    var internalCancel = wrapper.querySelector('.cancel-btn');
    var internalPrivacy = wrapper.querySelector('input[type=checkbox]');
    if (internalSave) internalSave.style.display = 'none';
    if (internalCancel) internalCancel.style.display = 'none';
    if (internalPrivacy && internalPrivacy.closest('.form-check')) {
        internalPrivacy.closest('.form-check').style.display = 'none';
    }

    function enableEditMode() {
        // hide header buttons (Tornar / Editar)
        var header = document.getElementById('headerButtons');
        if (header) header.style.display = 'none';

        // enable inputs/selects
        form.querySelectorAll('input,select,textarea').forEach(function(el) {
            if (el.hasAttribute('readonly')) el.removeAttribute('readonly');
            if (el.hasAttribute('disabled')) el.removeAttribute('disabled');
        });

        // show editable controls
        var favDisplay = document.getElementById('favoriteDisplay');
        var favEdit = document.getElementById('favoriteEdit');
        if (favDisplay) favDisplay.style.display = 'none';
        if (favEdit) favEdit.style.display = 'block';
        document.querySelectorAll('.edit-only').forEach(function(e){ e.style.display = 'inline'; });

        // show wrapper and its buttons and privacy checkbox
        wrapper.style.display = 'block';
        if (internalSave) internalSave.style.display = '';
        if (internalCancel) internalCancel.style.display = '';
        if (internalPrivacy && internalPrivacy.closest('.form-check')) {
            internalPrivacy.closest('.form-check').style.display = '';
        }
    }

    editBtn.addEventListener('click', function() { enableEditMode(); });
});
