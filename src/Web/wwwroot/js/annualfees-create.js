document.getElementById('isPaidCheck').addEventListener('change', function() {
    const paymentRefGroup = document.getElementById('paymentRefGroup');
    paymentRefGroup.style.display = this.checked ? 'block' : 'none';
});
