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

    // Scroll the form into view
    document.getElementById('menuCode').scrollIntoView({ behavior: 'smooth', block: 'center' });
    document.getElementById('menuCode').focus();
}

// ─── DUPLICATE CHECK ───────────────────────────────────────────
// Returns true if the given code or description already exists in the table,
// ignoring the row that is currently being edited (matched by id).
function isDuplicate(newCode, newDescription, currentId) {
    const nodes = table.rows().nodes().toArray();

    for (const node of nodes) {
        const rowCode = $(node).find('.menuCode').val().trim().toLowerCase();
        const rowName = $(node).find('.menuName').val().trim().toLowerCase();
        const rowId = $(node).find('input[type="hidden"]').val(); // hidden Id field

        // Skip the row we are currently editing
        if (currentId && rowId && rowId === currentId) continue;
        // Skip empty rows
        if (!rowCode && !rowName) continue;

        if (rowCode === newCode.trim().toLowerCase()) {
            return { duplicate: true, field: 'Menu Code', value: newCode };
        }
        if (rowName === newDescription.trim().toLowerCase()) {
            return { duplicate: true, field: 'Menu Name', value: newDescription };
        }
    }
    return { duplicate: false };
}

// ─── FORM SUBMIT (single save) ─────────────────────────────────
document.getElementById('menuForm').addEventListener('submit', function(e) {
    const code = document.getElementById('menuCode').value.trim();
    const description = document.getElementById('menuDescription').value.trim();
    const currentId = document.getElementById('menuId').value.trim();

    if (!code || !description) {
        e.preventDefault();
        Swal.fire({
            icon: 'warning',
            title: 'Validation Error',
            text: 'Please enter both Menu Code and Menu Name.'
        });
        return false;
    }

    // Client-side duplicate guard
    const check = isDuplicate(code, description, currentId);
    if (check.duplicate) {
        e.preventDefault();
        Swal.fire({
            icon: 'error',
            title: 'Duplicate Entry',
            text: `A record with this ${check.field} ("${check.value}") already exists.`
        });
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

    // ── Show SweetAlert for server-side response, then clear the form ──
    const status = document.getElementById('status').value;
    const statusDescription = document.getElementById('statusDescription').value;

    if (status && status.trim() !== '') {
        if (status.trim().toLowerCase() === 'success') {
            Swal.fire({
                icon: 'success',
                title: 'Success!',
                text: statusDescription
            }).then(() => clearForm()); // clear form when user clicks OK
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: statusDescription
            }).then(() => clearForm()); // clear form even on error so user can retry fresh
        }
    }
});

// ─── SUBMIT TABLE (batch save) ─────────────────────────────────
function submitTable() {
    let valid = true;

    const nodes = table.rows().nodes().toArray();
    const seenCodes = [];
    const seenNames = [];

    for (let i = 0; i < nodes.length; i++) {
        const node = nodes[i];
        const menuCode = $(node).find('.menuCode').val().trim();
        const menuName = $(node).find('.menuName').val().trim();

        if (!menuCode && !menuName) continue; // skip truly empty rows

        // Code filled → name required
        if (menuCode && !menuName) {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: `Menu Name is required for Menu Code: ${menuCode}`
            });
            valid = false;
            break;
        }

        // Check for duplicates within the batch itself
        const codeLower = menuCode.toLowerCase();
        const nameLower = menuName.toLowerCase();

        if (seenCodes.includes(codeLower)) {
            Swal.fire({
                icon: 'error',
                title: 'Duplicate Entry',
                text: `Menu Code "${menuCode}" appears more than once in the table.`
            });
            valid = false;
            break;
        }
        if (seenNames.includes(nameLower)) {
            Swal.fire({
                icon: 'error',
                title: 'Duplicate Entry',
                text: `Menu Name "${menuName}" appears more than once in the table.`
            });
            valid = false;
            break;
        }

        seenCodes.push(codeLower);
        seenNames.push(nameLower);
    }

    if (!valid) return;

    // All good — trigger actual form submission
    document.getElementById('menuForm').submit();
} 