function AddTwoFactorSetup() {
    const useType = document.getElementById('useType').value;
    const inputMaxNumber = document.getElementById('inputMaxNumber');
    if (useType.toString().trim().toLowerCase() === '') {
        Swal.fire({
            title: "Warning!",
            text: " Select user type",
            icon: "warning"
        });
        return;
    }
    if (inputMaxNumber.value.toString().trim().toLowerCase() === '') {
        inputMaxNumber.focus();
        Swal.fire({
            title: "Warning!",
            text: " Enter maximum period",
            icon: "warning"
        });
        return;
    }
    let rowCount = table.rows().count();

    let newRow = $("#rowTemplate").clone().removeAttr("id");

    newRow.show();

    // set serial number
    newRow.find(".sn").text(rowCount + 1);

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

    table = $('#twoFactorSetupTab').DataTable({
        responsive: true,
        ordering: false,
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
$(document).on('click', '.removeRow', async function () {
    const el = this; // the clicked button

    // call server to remove (todo != 1 => remove)
    await UpdateTextFile(el, 1, 2);

    // now remove the row and reindex
    table.row($(el).closest('tr')).remove().draw(false);
    ReIndexRows();
    table.columns.adjust().responsive.recalc();
});
$(document).on('click', '.btnSave', function () {
    let valid = true;
    const nodes = table.rows().nodes().toArray();
    const useType = document.getElementById('useType').value;
    const inputMaxNumber = document.getElementById('inputMaxNumber');
    if (useType.toString().trim().toLowerCase() === '') {
        Swal.fire({
            title: "Warning!",
            text: " Select user type",
            icon: "warning"
        });
        return;
    }
    if (inputMaxNumber.value.toString().trim().toLowerCase() === '') {
        inputMaxNumber.focus();
        Swal.fire({
            title: "Warning!",
            text: " Enter maximum period",
            icon: "warning"
        });
        return;
    }

    for (let i = 0; i < nodes.length; i++) {
        const node = nodes[i];
        const twoFactorCode = $(node).find('.twoFactorCode').val();
        const twoFactorDescription = $(node).find('.twoFactorDescription').val();

        if (twoFactorCode && twoFactorCode.toString().trim() !== '') {
            if (!twoFactorDescription || twoFactorDescription.toString().trim() === '') {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: `Two-factor authentication description is required for two-factor code ${twoFactorCode}.`,
                });
                valid = false;
                break;
            }
        }
    }

    if (!valid) return;

    $(".loadingDiv-parent").fadeIn("fast");
    //if (window.Loader) window.Loader.fadeIn(window.Loader.parent, 200);// fast fade in
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
async function TwoFactorChange(option) {
    const useType = document.getElementById('useType').value;
    let inputMaxNumber = document.getElementById('inputMaxNumber');
    let inputSeconds = document.getElementById('inputSeconds');
    if (option == 1) {
        inputMaxNumber.value = ""; inputSeconds.value = "";
    }
    else {
        if (useType.toString().trim().toLowerCase() === '') {
            inputMaxNumber.value = "";
            Swal.fire({
                title: "Warning!",
                text: " Select user type",
                icon: "warning"
            });
            return;
        }
        inputSeconds.value = "";

        let secondsInAday;
        if (useType.toString().trim().toLowerCase() === 'hours') {
            secondsInAday = Number(inputMaxNumber.value) * 60 * 60; // hours to seconds
        } else if (useType.toString().trim().toLowerCase() === 'days') {
            secondsInAday = Number(inputMaxNumber.value) * (24 * 60 * 60); // days to seconds
        }
        inputSeconds.value = secondsInAday;
    }
    
}
async function UpdateTextFile(element, option, todo) {
    let row = $(element).closest("tr");
    let uniqno = row.find(".code").val();
    let twoFactorCode = row.find(".twoFactorCode").val();

    let twoFactorDescription = row.find(".twoFactorDescription").val();
    const useType = document.getElementById('useType').value;
    const inputMaxNumber = document.getElementById('inputMaxNumber');
    if (useType.toString().trim().toLowerCase() === '') {
        if (option === 1) {
            row.find(".twoFactorCode").val(''); row.find(".twoFactorDescription").val('');
        }
        else
            row.find(".twoFactorDescription").val('');

        Swal.fire({
            title: "Warning!",
            text: " Select user type",
            icon: "warning"
        });
        return;
    }
    if (inputMaxNumber.value.toString().trim().toLowerCase() === '') {
        if (option === 1) {
            row.find(".twoFactorCode").val(''); row.find(".twoFactorDescription").val('');
        }
        else
            row.find(".twoFactorDescription").val('');

        inputMaxNumber.focus();
        Swal.fire({
            title: "Warning!",
            text: " Enter maximum period",
            icon: "warning"
        });
        return;
    }
    

    if (option === 2) {
        if (twoFactorCode.toString().trim().toLowerCase() === '') {
            row.find(".twoFactorDescription").val('');
            row.find(".twoFactorCode").focus();
            Swal.fire({
                title: "Warning!",
                text: " Enter two-factor authentication code",
                icon: "warning"
            });
            return;
        }
    }
    try {

        let value = [];

        if (option === 1) {
            value = ["1", twoFactorCode];
        }
        else if (option === 2) {
            value = ["2", twoFactorDescription];
        }

        // Check for duplicates in the table (ignore the current row)
        try {
            //showLoading();

            $(".loadingDiv-parent").fadeIn("fast");
            //if (window.Loader) window.Loader.fadeIn(window.Loader.parent, 200);// fast fade in
            if (option === 1 && twoFactorCode) {
                let duplicate = false;
                table.rows().every(function () {
                    const node = this.node();
                    if (node === row[0]) return; // skip current row
                    const val = $(node).find('.twoFactorCode').val();
                    if (val && val.toString().trim().toLowerCase() === twoFactorCode.toString().trim().toLowerCase()) {
                        duplicate = true; row.find(".twoFactorCode").val('');
                        return false; // break
                    }
                });
                if (duplicate) {
                    $(".loadingDiv-parent").fadeOut("slow");
                    //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);// fast fade in
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: `Two-factor authentication code '${twoFactorCode}' already exists in the table.`,
                    });
                    return;
                }
            }

            if (option === 2 && twoFactorDescription) {
                let duplicate = false;
                table.rows().every(function () {
                    const node = this.node();
                    if (node === row[0]) return; // skip current row
                    const val = $(node).find('.twoFactorDescription').val();
                    if (val && val.toString().trim().toLowerCase() === twoFactorDescription.toString().trim().toLowerCase()) {
                        duplicate = true;
                        row.find(".twoFactorDescription").val('');
                        return false; // break
                    }
                });
                if (duplicate) {
                    $(".loadingDiv-parent").fadeOut("slow");
                    //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);// fast fade in
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: `Two-factor authentication description '${twoFactorDescription}' already exists in the table.`,
                    });
                    return;
                }
            }
        }
        catch (e) {
            // ignore duplicate-check errors and continue
            $(".loadingDiv-parent").fadeOut("slow");
            //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);// fast fade in
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: `Duplicate check error, ${e}.`,
            });
            return;
        }

        // If uploading a file, send multipart/form-data to a dedicated handler
        let response;

        response = await fetch('?handler=UpdateTextFile', {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify({
                uniqno: uniqno,
                uniqueUpdated: twoFactorCode,
                todo: todo,
                value: value
            })
        });

        if (!response.ok) {
            const text = await response.text();
            $(".loadingDiv-parent").fadeOut("slow");
            //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);// fast fade in
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: `UpdateTextFile failed:, ${response.status}, ${text}.`,
            });
            return;
        }

        // Try to parse JSON, but guard against empty/non-JSON responses
        const contentType = response.headers.get('content-type') || '';
        if (contentType.indexOf('application/json') !== -1) {
            const result = await response.json();

            // Normalize result: handler may return a string or an object { status, statusDescription }
            if (typeof result === 'string') {
                if (result !== 'Success') {
                    $(".loadingDiv-parent").fadeOut('slow');
                    Swal.fire({ icon: 'error', title: 'Oops...', text: result });
                    return;
                }
            }
            else if (result && result.status) {
                if (result.status !== 'Success') {
                    $(".loadingDiv-parent").fadeOut("slow");
                    //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);// fast fade in
                    const msg = result.statusDescription || result.message || 'Operation failed';
                    Swal.fire({ icon: 'error', title: 'Oops...', text: msg });
                    return;
                }
            }

            if (uniqno === '')
                row.find(".code").val(twoFactorCode);
            $(".loadingDiv-parent").fadeOut("slow");
            //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);// fast fade in
        }
        else {
            const text = await response.text();
            $(".loadingDiv-parent").fadeOut("slow");
            //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);// fast fade in
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: `UpdateTextFile response:, ${text}.`,
            });
            return;
            //hideLoading();
        }

    }
    catch (e) {
        console.log(e);
        $(".loadingDiv-parent").fadeOut("slow");
        //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);// fast fade in
        //hideLoading();
    }
}


