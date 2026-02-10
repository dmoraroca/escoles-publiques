// enrollments-create.js
// Enable/disable create button based on privacy checkbox.
document.addEventListener('DOMContentLoaded', function () {
    var checkbox = document.getElementById('agreesToPrivacy');
    var createBtn = document.getElementById('createButton');
    if (!createBtn) return;
    createBtn.disabled = true;
    if (!checkbox) return;
    createBtn.disabled = !checkbox.checked;
    checkbox.addEventListener('change', function () {
        createBtn.disabled = !this.checked;
    });
});
