/**
 * annualfees-create.js
 * Handles UI logic for creating annual fee records, including payment reference visibility.
 */
document.getElementById('isPaidCheck').addEventListener('change', function() {
    const paymentRefGroup = document.getElementById('paymentRefGroup');
    paymentRefGroup.style.display = this.checked ? 'block' : 'none';
});
