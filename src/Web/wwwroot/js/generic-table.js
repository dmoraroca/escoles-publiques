function filterTable(input) {
    const filter = input.value.toLowerCase();
    const table = document.getElementById('genericTable');
    const rows = table.getElementsByTagName('tr');
    
    for (let i = 1; i < rows.length; i++) {
        const row = rows[i];
        const text = row.textContent || row.innerText;
        
        if (text.toLowerCase().indexOf(filter) > -1) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    }
}

function sortTable(header) {
    const table = document.getElementById('genericTable');
    const tbody = table.getElementsByTagName('tbody')[0];
    const rows = Array.from(tbody.getElementsByTagName('tr'));
    const columnIndex = Array.from(header.parentElement.children).indexOf(header);
    
    const isAscending = !header.classList.contains('sorted-asc');
    
    const headers = header.parentElement.getElementsByTagName('th');
    for (let h of headers) {
        h.classList.remove('sorted-asc', 'sorted-desc');
    }
    
    header.classList.add(isAscending ? 'sorted-asc' : 'sorted-desc');
    
    rows.sort((a, b) => {
        const cellA = a.getElementsByTagName('td')[columnIndex].textContent.trim();
        const cellB = b.getElementsByTagName('td')[columnIndex].textContent.trim();
        
        const numA = parseFloat(cellA.replace(/[^\d.-]/g, ''));
        const numB = parseFloat(cellB.replace(/[^\d.-]/g, ''));
        
        if (!isNaN(numA) && !isNaN(numB)) {
            return isAscending ? numA - numB : numB - numA;
        }
        
        return isAscending 
            ? cellA.localeCompare(cellB, 'ca')
            : cellB.localeCompare(cellA, 'ca');
    });
    
    rows.forEach(row => tbody.appendChild(row));
}
