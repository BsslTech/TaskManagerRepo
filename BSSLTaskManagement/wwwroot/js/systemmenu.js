// ─── GLOBALS ───────────────────────────────────────────────────
let table; // DataTable instance — accessible by all functions

// ─── CLEAR FORM ────────────────────────────────────────────────
function clearForm() {
    document.getElementById('menuId').value = '';
    document.getElementById('menuCode').value = '';
    document.getElementById('menuDescription').value = '';
    document.querySelectorAll('tbody tr').forEach(row => row.classList.remove('table-info'));
}

// ─── EDIT ROW ──────────────────────────────────────────────────
// Populate the single-save form when a row's Select button is clicked
function editRow(index, id, code, description) {
    document.getElementById('menuId').value = id || '';
    document.getElementById('menuCode').value = code || '';
    document.getElementById('menuDescription').value = description || '';

    // Highlight the selected row
    document.querySelectorAll('tbody tr').forEach(row => row.classList.remove('table-info'));
    document.getElementById('row_' + index).classList.add('table-info');

    // Scroll to form
    document.getElementById('menuCode').scrollIntoView({ behavior: 'smooth', block: 'center' });
    document.getElementById('menuCode').focus();
}

//// Clear single save form
//function clearForm() {
//    document.getElementById('menuId').value = '';
//    document.getElementById('menuCode').value = '';
//    document.getElementById('menuDescription').value = '';

//    // Remove highlight
//    document.querySelectorAll('tbody tr').forEach(row => row.classList.remove('table-info'));
//}



//// Enable editing on double click in table
//document.querySelectorAll('.menu-code-input, .menu-desc-input').forEach(input => {
//    input.addEventListener('dblclick', function () {
//        this.removeAttribute('readonly');
//        this.classList.add('bg-warning');
//    });

//    input.addEventListener('blur', function () {
//        this.setAttribute('readonly', 'readonly');
//        this.classList.remove('bg-warning');
//    });
//});

// Form validation
document.getElementById('menuForm').addEventListener('submit', function (e) {
    const code = document.getElementById('menuCode').value.trim();
    const description = document.getElementById('menuDescription').value.trim();
    const currentId = document.getElementById('menuId').value.trim();

    if (!code || !description) {
        e.preventDefault();
        alert('Please enter both menu code and description');
        return false;
    }
});

// ─── DOCUMENT READY ────────────────────────────────────────────
$(document).ready(function() {

    // Initialise DataTable
    // Before init, add data-search attributes to each input cell so DataTables
    // can read the plain-text value for search/sort without touching the HTML.
    $('#menuDataTable tbody tr').each(function() {
        const code = $(this).find('.menuCode').val() || '';
        const name = $(this).find('.menuName').val() || '';
        $(this).find('td:eq(2)').attr('data-search', code).attr('data-order', code);
        $(this).find('td:eq(3)').attr('data-search', name).attr('data-order', name);
    });

    table = $('#menuDataTable').DataTable({
        responsive: true,
        pageLength: 10,
        order: [],
        columnDefs: [
            { orderable: false, targets: 0 }
        ]
    });
    const status = document.getElementById('status').value;
    const statusDescription = document.getElementById('statusDescription').value;
    if (status) {
        if (status.toString().trim().toLowerCase() === 'success') {
            Swal.fire({
                title: "Success!",
                text: statusDescription,
                icon: "success"
            });
            return;
        } else {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: statusDescription,
            });
            return;
        }
    }
});

// ─── SUBMIT ────────────────────────────────────────────────────
   function submitTable() {

       let valid = true;

       // Loop through every row in the DataTable (including rows on other pages)
       const nodes = table.rows().nodes().toArray();

       for (let i = 0; i < nodes.length; i++) {
           const node = nodes[i];
           const menuCode = $(node).find('.menuCode').val();
           const menuName = $(node).find('.menuName').val();

           // If a code is filled in, a name must also be filled in
           if (menuCode && menuCode.trim() !== '') {
               if (!menuName || menuName.trim() === '') {
                   Swal.fire({
                       icon: "error",
                       title: "Oops...",
                       text: `Menu Name is required for Menu Code: ${menuCode}`
                   });
                   valid = false;
                   break;
               }
           }
       }

    if (!valid) return;

       // All good — trigger your actual form submission here
       alert('Form submitted successfully!');
}