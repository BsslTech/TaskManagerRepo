function AddSystemTypeRow() {

    let rowCount = table.rows().count();

    let newRow = $("#rowTemplate").clone().removeAttr("id");

    newRow.show();

    // set serial number
    newRow.find(".sn").text(rowCount+1);

    newRow.find("input").each(function () {

        let name = $(this).attr("name");
        if (name)
            $(this).attr("name", name.replace("[0]", `[${rowCount}]`));

        let id = $(this).attr("id");
        if (id)
            $(this).attr("id", id.replace("_0__", `_${rowCount}__`));

        $(this).val("");
    });

    // ADD THROUGH DATATABLE
    table.row.add(newRow).draw(false);

    // jump to the page containing the new row
    let pageLength = table.page.len();          // rows per page
    let newRowIndex = table.rows().count() - 1; // index of last row
    let newPage = Math.floor(newRowIndex / pageLength);

    table.page(newPage).draw(false);


    table.columns.adjust().responsive.recalc();
}
$(document).ready(function () {

    table = $('#systemDataTable').DataTable({
        responsive: true,
        pageLength: 10,
        order: [],
        columnDefs: [
            { orderable: false, targets: 0 }
        ]
    });

});
$(document).on('click', '.removeRow', async function () {
    const el = this; // the clicked button

    // call server to remove (todo != 1 => remove)
    await UpdateTextFile(el, 1, 2);

    // now remove the row and reindex
    table.row($(el).closest('tr')).remove().draw(false);
    ReIndexRows();
    table.columns.adjust().responsive.recalc();
});
$(document).on('click', '#btnSubmitForm', function () {
    let valid = true;
    const nodes = table.rows().nodes().toArray();
    for (let i = 0; i < nodes.length; i++) {
        const node = nodes[i];
        const systemCode = $(node).find('.systemCode').val();
        const systemDescription = $(node).find('.systemDescription').val();

        if (systemCode && systemCode.toString().trim() !== '') {
            if (!systemDescription || systemDescription.toString().trim() === '') {
                alert(`System description is required for system code ${systemCode}.`);
                valid = false;
                break;
            }
        }
    }

    if (!valid) return;
    document.getElementById('postForm').click();
});
function ReIndexRows() {

    table.rows().every(function (rowIdx) {

        let row = this.node();

        $(row).attr("id", rowIdx);

        $(row).find("td:first").text(rowIdx + 1);

        $(row).find("input").each(function () {

            let name = $(this).attr("name");
            if (name)
                $(this).attr("name", name.replace(/\[\d+\]/, `[${rowIdx}]`));

            let id = $(this).attr("id");
            if (id)
                $(this).attr("id", id.replace(/_\d+__/, `_${rowIdx}__`));
        });

    });

}
async function UpdateTextFile(element,option, todo) {

    try {

        let row = $(element).closest("tr");
        let uniqno = row.find(".code").val();
        let systemCode = row.find(".systemCode").val();
        
        let systemDescription = row.find(".systemDescription").val();

        let value = [];

        if (option === 1) {
            value = ["1", systemCode];
        }
        else if (option === 2) {
            value = ["2", systemDescription];
        }
        else if (option === 3) {
            value = ["3", "file"];
        }

        // Check for duplicates in the table (ignore the current row)
        try {
            if (option === 1 && systemCode) {
                let duplicate = false;
                table.rows().every(function () {
                    const node = this.node();
                    if (node === row[0]) return; // skip current row
                    const val = $(node).find('.systemCode').val();
                    if (val && val.toString().trim().toLowerCase() === systemCode.toString().trim().toLowerCase()) {
                        duplicate = true; row.find(".systemCode").val('');
                        return false; // break
                    }
                });
                if (duplicate) {
                    alert('System Code already exists in the table.');
                    return;
                }
            }

            if (option === 2 && systemDescription) {
                let duplicate = false;
                table.rows().every(function () {
                    const node = this.node();
                    if (node === row[0]) return; // skip current row
                    const val = $(node).find('.systemDescription').val();
                    if (val && val.toString().trim().toLowerCase() === systemDescription.toString().trim().toLowerCase()) {
                        duplicate = true;
                        row.find(".systemDescription").val('');
                        return false; // break
                    }
                });
                if (duplicate) {
                    alert('System Description already exists in the table.');
                    return;
                }
            }
        }
        catch (e) {
            // ignore duplicate-check errors and continue
            console.log('Duplicate check error', e);
        }

        // If uploading a file, send multipart/form-data to a dedicated handler
        let response;
        if (option === 3) {
            const fileInput = $(element)[0];
            const file = fileInput && fileInput.files && fileInput.files[0] ? fileInput.files[0] : null;
            if (!file) {
                alert('No file selected');
                return;
            }

            const formData = new FormData();
            // include identifying fields
            formData.append('uniqno', uniqno || '');
            // append the file; parameter name should match server parameter (helperFile)
            formData.append('helperFile', file);

            response = await fetch('?handler=UploadHelperFile', {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                    // don't set Content-Type; the browser will set multipart boundary
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                body: formData
            });
        }
        else {
            response = await fetch('?handler=UpdateTextFile', {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                body: JSON.stringify({
                    uniqno: uniqno,
                    uniqueUpdated: systemCode,
                    todo: todo,
                    value: value
                })
            });
        }

        if (!response.ok) {
            const text = await response.text();
            console.error('UpdateTextFile failed', response.status, text);
            return;
        }

        // Try to parse JSON, but guard against empty/non-JSON responses
        const contentType = response.headers.get('content-type') || '';
        if (contentType.indexOf('application/json') !== -1) {
            const result = await response.json();
            if (uniqno === '')
                row.find(".code").val(systemCode);
            //console.log(result);
        }
        else {
            const text = await response.text();
            console.log('UpdateTextFile response:', text);
        }

}
catch (e) {
    console.log(e);
}
}
