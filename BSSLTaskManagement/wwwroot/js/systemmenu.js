// Edit row - populate single save form
function editRow(index, id, code, description) {
    document.getElementById('menuId').value = id || '';
    document.getElementById('menuCode').value = code || '';
    document.getElementById('menuDescription').value = description || '';

    // Highlight selected row
    document.querySelectorAll('tbody tr').forEach(row => row.classList.remove('table-info'));
    document.getElementById('row_' + index).classList.add('table-info');

    // Toast notification
    Swal.fire({
        icon: 'info',
        title: 'Record Loaded',
        text: `"${description}" is ready to edit.`,
        timer: 1500,
        showConfirmButton: false,
        position: 'top-end',
        toast: true
    });

    // Scroll to form
    document.getElementById('menuCode').scrollIntoView({ behavior: 'smooth', block: 'center' });
    document.getElementById('menuCode').focus();
}


// Form validation + SweetAlert confirm on submit
document.getElementById('menuForm').addEventListener('submit', function (e) {
    e.preventDefault();

    const form = this;
    const code = document.getElementById('menuCode').value.trim();
    const description = document.getElementById('menuDescription').value.trim();

    if (!code || !description) {
        Swal.fire({
            icon: 'warning',
            title: 'Missing Fields',
            text: 'Please enter both Menu Code and Menu Name before submitting.',
            confirmButtonColor: '#ffc107'
        });
        return;
    }

    Swal.fire({
        title: 'Save Menu?',
        text: `Save "${description}" (${code})?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#198754',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, Save it!'
    }).then((result) => {
        if (result.isConfirmed) {
            form.submit();
        }
    });
});


$(document).ready(function () {

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
                icon: 'success',
                title: 'Success!',
                text: statusDescription,
                confirmButtonColor: '#198754'
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: statusDescription,
                confirmButtonColor: '#dc3545'
            });
        }
    }
});


// ─── SUBMIT TABLE (batch) ──────────────────────────────────────
function submitTable() {

    let valid = true;

    const nodes = table.rows().nodes().toArray();

    for (let i = 0; i < nodes.length; i++) {
        const node = nodes[i];
        const menuCode = $(node).find('.menuCode').val();
        const menuName = $(node).find('.menuName').val();

        if (menuCode && menuCode.trim() !== '') {
            if (!menuName || menuName.trim() === '') {
                Swal.fire({
                    icon: 'error',
                    title: 'Validation Error',
                    text: `Menu Name is required for Menu Code: ${menuCode}`,
                    confirmButtonColor: '#dc3545'
                });
                valid = false;
                break;
            }
        }
    }

    if (!valid) return;

    Swal.fire({
        title: 'Submit All?',
        text: 'Are you sure you want to save all menu records?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#198754',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, Submit!'
    }).then((result) => {
        if (result.isConfirmed) {
            // Trigger actual form submission here
            document.getElementById('menuForm').submit();
        }
    });
}