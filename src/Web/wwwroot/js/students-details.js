/**
 * students-details.js
 * Handles UI interactions and edit logic for student detail pages.
 */
document.addEventListener('DOMContentLoaded', function() {
    var editBtn = document.getElementById('editButton');
    var wrapper = document.getElementById('saveCancelWrapper');
    var form = document.getElementById('editForm');
    if (!editBtn || !wrapper || !form) return;

    // Ensure save/cancel wrapper is hidden on initial load
    wrapper.style.display = 'none';
    var internalSave = wrapper.querySelector('.save-btn');
    var internalCancel = wrapper.querySelector('.cancel-btn');
    var internalPrivacy = wrapper.querySelector('input[type=checkbox]');
    if (internalSave) internalSave.style.display = 'none';
    if (internalCancel) internalCancel.style.display = 'none';
    if (internalPrivacy) internalPrivacy.closest('.form-check').style.display = 'none';

    function enableEditMode() {
        var header = document.getElementById('headerButtons');
        if (header) header.style.display = 'none';

        form.querySelectorAll('input,select,textarea').forEach(function(el) {
            if (el.hasAttribute('readonly')) el.removeAttribute('readonly');
            if (el.hasAttribute('disabled')) el.removeAttribute('disabled');
            el.classList.remove('bg-light');
        });

        document.querySelectorAll('.edit-only').forEach(function(e){ e.style.display = 'inline'; });

        wrapper.style.display = 'block';
        if (internalSave) internalSave.style.display = '';
        if (internalCancel) internalCancel.style.display = '';
        if (internalPrivacy) internalPrivacy.closest('.form-check').style.display = '';
    }

    editBtn.addEventListener('click', function() { enableEditMode(); });
});
