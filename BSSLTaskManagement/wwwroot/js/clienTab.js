// Edit row - populate single save form
function editRow(index, id, code, description) {
    document.getElementById('menuId').value = id || '';
    document.getElementById('menuCode').value = code || '';
    document.getElementById('menuDescription').value = description || '';
    // Scroll to form
    document.getElementById('menuCode').scrollIntoView({ behavior: 'smooth', block: 'center' });
    document.getElementById('menuCode').focus();
}


// Form validation
document.getElementById('menuForm').addEventListener('submit', function (e) {
    const code = document.getElementById('menuCode').value.trim();
    const description = document.getElementById('menuDescription').value.trim();

    if (!code) {
        e.preventDefault();
        Swal.fire({
            title: "Error!",
            text: "Enter client code",
            icon: "error"
        });
        return false;
    }
    if (!description) {
        e.preventDefault();
        Swal.fire({
            title: "Error!",
            text: "Enter client name",
            icon: "error"
        });
        return false;
    }
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
