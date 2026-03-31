function AddModuleSetup() {
    const inputSystemTypeName = document.getElementById('inputSystemTypeName').value;
    if (inputSystemTypeName.toString().trim().toLowerCase() === '') {
        Swal.fire({
            title: "Warning!",
            text: "Select system type",
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

    table = $('#moduleSetupTab').DataTable({
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
    for (let i = 0; i < nodes.length; i++) {
        const node = nodes[i];
        const moduleCode = $(node).find('.moduleCode').val();
        const moduleDescription = $(node).find('.moduleDescription').val();

        if (moduleCode && moduleCode.toString().trim() !== '') {
            if (!moduleDescription || moduleDescription.toString().trim() === '') {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: `Module description is required for module code ${moduleCode}.`,
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
async function SystemTypeChange() {
    try { 
        // If uploading a file, send multipart/form-data to a dedicated handler
        let response;
        $(".loadingDiv-parent").fadeIn("fast");
        const inputSystemTypeName = document.getElementById('inputSystemTypeName').value;
        response = await fetch(`?handler=SystemModules&systemId=${inputSystemTypeName}`, {
            method: 'GET',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
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
            if (result.statusDescription.length > 0) {
                result.statusDescription.sort((a, b) => {
                    const numA = Number(a.code);
                    const numB = Number(b.code);

                    if (numA !== numB) {
                        return numA - numB; // numeric sort
                    }

                    // If same number (e.g., 001, 01, 1), prioritize longer string
                    return b.code.length - a.code.length;
                });
                var div = '';
                for (var i = 0; i < result.statusDescription.length; i++) {
                    var item = result.statusDescription[i];
                    const className = (i + 1) % 2 === 0 ? 'even' : 'odd';
                    div += `<tr id="${i}" role="row" class="${className}">
                              <td style="padding:10px; border:1px solid lightgrey; width:20px;" class="dtr-control" tabindex="0">${(i+1)}</td>
                              <td class="col-1" style="text-align:left;padding:10px;border:1px solid lightgrey;">
                                <input class="code ${i}" type="hidden" id="ModuleSetup_ModuleDetails_${i}__SystemCode" name="ModuleSetup.ModuleDetails[${i}].SystemCode" value="${item.code}">
                                <input onchange="UpdateTextFile(this,1,1)" id="moduleCode" style="width:150px" type="text" class="form-control border-primary moduleCode" value="${item.moduleCode}" name="ModuleSetup.ModuleDetails[${i}].ModuleCode">
                            </td>

                            <td class="col-1" style="text-align:center;padding:10px;border:1px solid lightgrey;">
                                <input onchange="UpdateTextFile(this,2,1)" style="width:450px" type="text" id="moduleDescription" class="form-control border-primary moduleDescription" value="${item.moduleDescription}" name="ModuleSetup.ModuleDetails[${i}].ModuleDescription">
                            </td>

                            <td style="text-align:center;padding:10px;border:1px solid lightgrey; width:150px">

                                <button type="button" class="btn btn-danger removeRow">
                                    <i class="fa fa-trash"></i> Delete Row
                                </button>
                            </td>
                        </tr>`
                    }
                jQuery('#tableBody').empty();
                jQuery('#tableBody').append(div);

                // build jQuery collection of <tr> elements from your HTML string
                const $rows = $(div);

                // replace table content via DataTables API
                table.clear();
                table.rows.add($rows.toArray()).draw(false);

                // then reflow/responsive after draw
                setTimeout(() => {
                    table.columns.adjust().responsive.recalc();
                }, 0);

                table.columns.adjust().responsive.recalc();
                    jQuery(".loadingDiv-parent").fadeOut("slow");
                
            }
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
async function UpdateTextFile(element, option, todo) {

    try {

        let row = $(element).closest("tr");
        let uniqno = row.find(".code").val();
        let moduleCode = row.find(".moduleCode").val();

        let moduleDescription = row.find(".moduleDescription").val();

        let value = [];

        if (option === 1) {
            value = ["1", moduleCode];
        }
        else if (option === 2) {
            value = ["2", moduleDescription];
        }

        // Check for duplicates in the table (ignore the current row)
        try {
            //showLoading();

            $(".loadingDiv-parent").fadeIn("fast");
            //if (window.Loader) window.Loader.fadeIn(window.Loader.parent, 200);// fast fade in
            if (option === 1 && moduleCode) {
                let duplicate = false;
                table.rows().every(function () {
                    const node = this.node();
                    if (node === row[0]) return; // skip current row
                    const val = $(node).find('.moduleCode').val();
                    if (val && val.toString().trim().toLowerCase() === moduleCode.toString().trim().toLowerCase()) {
                        duplicate = true; row.find(".moduleCode").val('');
                        return false; // break
                    }
                });
                if (duplicate) {
                    $(".loadingDiv-parent").fadeOut("slow");
                    //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);// fast fade in
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: `Module code '${moduleCode}' already exists in the table.`,
                    });
                    return;
                }
            }

            if (option === 2 && moduleDescription) {
                let duplicate = false;
                table.rows().every(function () {
                    const node = this.node();
                    if (node === row[0]) return; // skip current row
                    const val = $(node).find('.moduleDescription').val();
                    if (val && val.toString().trim().toLowerCase() === moduleDescription.toString().trim().toLowerCase()) {
                        duplicate = true;
                        row.find(".moduleDescription").val('');
                        return false; // break
                    }
                });
                if (duplicate) {
                    $(".loadingDiv-parent").fadeOut("slow");
                    //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);// fast fade in
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: `Module description '${systemDescription}' already exists in the table.`,
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
                    uniqueUpdated: moduleCode,
                    todo: todo,
                    value: value,
                    systemId: document.getElementById('inputSystemTypeName').value,
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
                row.find(".code").val(moduleCode);
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


