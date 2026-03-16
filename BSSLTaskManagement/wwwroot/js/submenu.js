$(document).ready(function () {

    table = $('#menuDataTable').DataTable({
        responsive: true,
        pageLength: 10,
        order: [],
        columnDefs: [
            { orderable: false, targets: 0 }
        ]
    });
});