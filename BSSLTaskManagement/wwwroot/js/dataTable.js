$(document).ready(function () {
    $('#systemTable').DataTable({
        responsive: true,
        pageLength: 10,
        ordering: true,
        searching: true,
        lengthChange: true,
        lengthMenu: [10, 25, 50, 100],
        columnDefs: [
            { orderable: false, targets: 2 }
        ]
    });
});