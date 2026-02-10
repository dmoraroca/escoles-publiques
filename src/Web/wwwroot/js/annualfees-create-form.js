// annualfees-create-form.js
// Handles privacy checkbox and enrollment/student filtering on annual fee create.
(function() {
    function initPrivacyToggle() {
        var checkbox = document.getElementById('agreesToPrivacy');
        var createBtn = document.getElementById('createButton');
        if (!createBtn) return;
        createBtn.disabled = true;
        if (!checkbox) return;
        createBtn.disabled = !checkbox.checked;
        checkbox.addEventListener('change', function () {
            createBtn.disabled = !this.checked;
        });
    }

    function initEnrollmentFilters() {
        var enrollmentSelect = document.getElementById('enrollmentSelect');
        var studentSelect = document.getElementById('studentSelect');
        var schoolInput = document.getElementById('schoolReadonly');

        function filterEnrollmentsByStudent() {
            if (!enrollmentSelect || !studentSelect) return;
            var selectedStudentId = studentSelect.value;
            for (var i = 0; i < enrollmentSelect.options.length; i++) {
                var opt = enrollmentSelect.options[i];
                if (opt.value === '') {
                    opt.style.display = '';
                    continue;
                }
                var studentId = opt.getAttribute('data-student-id');
                opt.style.display = (selectedStudentId === '' || studentId === selectedStudentId) ? '' : 'none';
            }
            if (enrollmentSelect.selectedIndex > 0 && enrollmentSelect.options[enrollmentSelect.selectedIndex].style.display === 'none') {
                enrollmentSelect.selectedIndex = 0;
            }
        }

        function updateSchoolField() {
            if (!enrollmentSelect || !studentSelect || !schoolInput) return;
            var enrollmentSelected = enrollmentSelect.value !== '';
            if (enrollmentSelected) {
                var selected = enrollmentSelect.options[enrollmentSelect.selectedIndex];
                var school = selected.getAttribute('data-school');
                schoolInput.value = school ? school : '';
                schoolInput.readOnly = true;
                schoolInput.style.backgroundColor = '#e9ecef';
                schoolInput.style.color = '#6c757d';
            } else {
                var selectedStudent = studentSelect.options[studentSelect.selectedIndex];
                var schoolFromStudent = selectedStudent ? selectedStudent.getAttribute('data-school') : '';
                schoolInput.value = schoolFromStudent ? schoolFromStudent : '';
                schoolInput.readOnly = false;
                schoolInput.style.backgroundColor = '';
                schoolInput.style.color = '';
            }
        }

        if (enrollmentSelect) {
            enrollmentSelect.addEventListener('change', updateSchoolField);
        }
        if (studentSelect) {
            studentSelect.addEventListener('change', function() {
                filterEnrollmentsByStudent();
                updateSchoolField();
            });
        }

        filterEnrollmentsByStudent();
        updateSchoolField();
    }

    document.addEventListener('DOMContentLoaded', function() {
        initPrivacyToggle();
        initEnrollmentFilters();
    });
})();
