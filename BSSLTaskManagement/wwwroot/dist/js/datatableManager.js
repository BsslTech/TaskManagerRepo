/**
 * ============================================================
 * DataTableManager.js
 * Reusable DataTable manager for ASP.NET Razor Pages projects
 * Handles: init, add row, remove row, reindex, validate,
 *          live search sync, and row data extraction
 * ============================================================
 */

class DataTableManager {

    /**
     * @param {string} tableId - The HTML id of the <table> element
     * @param {object} options - Optional DataTables config overrides
     * 
     * Usage:
     *   let myTable = new DataTableManager('systemDataTable', { pageLength: 5 });
     */
    constructor(tableId, options = {}) {
        this.tableId = tableId;
        this.table = null;

        // Merge default options with any page-specific overrides
        this.options = {
            responsive: true,
            pageLength: options.pageLength || 10,
            order: options.order || [],
            columnDefs: options.columnDefs || [
                { orderable: false, targets: 0 } // S/No. column not sortable by default
            ],
            ...options
        };
    }


    // ================================================================
    //  INIT
    //  Initializes the DataTable and auto-binds the remove row handler
    //  Call: systemTable = new DataTableManager('systemDataTable').init();
    // ================================================================
    init() {
        this.table = $(`#${this.tableId}`).DataTable(this.options);
        this._bindRemoveRow(); // auto-bind .removeRow buttons
        return this;           // return this so you can chain: .init().bindSearchSync()
    }


    // ================================================================
    //  ADD ROW
    //  Clones a hidden template row, updates indexes, and adds to table
    //
    //  @param {string} templateId  - id of the hidden <tr> template
    //  @param {string} namePrefix  - e.g. 'SystemTypes' for SystemTypes[0].Code
    //  @param {object} callbacks   - optional: { beforeAdd(row, count), afterAdd(row, count) }
    //
    //  Usage:
    //    systemTable.addRow('rowTemplate', 'SystemTypes');
    //    dropdownTable.addRow('dropdownTemplate', 'Dropdowns', {
    //        afterAdd: (row, count) => console.log('Row added', count)
    //    });
    // ================================================================
    addRow(templateId, namePrefix, callbacks = {}) {
        let rowCount = this.table.rows().count();

        // Clone the hidden template and remove its id so there's no duplicate
        let newRow = $(`#${templateId}`).clone().removeAttr("id");
        newRow.show();

        // Set the serial number in the .sn cell
        newRow.find(".sn").text(rowCount + 1);

        // Update name and id attributes to use the correct index
        // e.g. SystemTypes[0].Code → SystemTypes[3].Code
        newRow.find("input, select, textarea").each(function () {
            let name = $(this).attr("name");
            if (name)
                $(this).attr("name", name.replace("[0]", `[${rowCount}]`));

            let id = $(this).attr("id");
            if (id)
                $(this).attr("id", id.replace("_0__", `_${rowCount}__`));

            // Clear values except hidden inputs (they may carry default values)
            if ($(this).attr("type") !== "hidden")
                $(this).val("");
        });

        // Run optional pre-add callback (e.g. set default dropdown values)
        if (callbacks.beforeAdd) callbacks.beforeAdd(newRow, rowCount);

        // Add the row to the DataTable and redraw without resetting to page 1
        this.table.row.add(newRow).draw(false);

        // Calculate which page the new row lands on and jump to it
        let pageLength = this.table.page.len();
        let newRowIndex = this.table.rows().count() - 1;
        let newPage = Math.floor(newRowIndex / pageLength);
        this.table.page(newPage).draw(false);

        // Recalculate column widths for responsive layout
        this.table.columns.adjust().responsive.recalc();

        // Run optional post-add callback
        if (callbacks.afterAdd) callbacks.afterAdd(newRow, rowCount);
    }


    // ================================================================
    //  REMOVE ROW
    //  Removes the row containing the clicked element, then reindexes
    //
    //  @param {HTMLElement} element  - the clicked Remove button
    //  @param {object}      callbacks - optional: { beforeRemove(row), afterRemove() }
    //
    //  Usage (manual call):
    //    systemTable.removeRow(this);
    //
    //  Note: .removeRow buttons are auto-bound in _bindRemoveRow()
    //        so you don't need to call this manually in most cases
    // ================================================================
    removeRow(element, callbacks = {}) {
        const row = $(element).closest("tr");

        // Run optional pre-remove callback (e.g. call server to delete record)
        if (callbacks.beforeRemove) callbacks.beforeRemove(row);

        // Remove from DataTable and redraw without resetting page
        this.table.row(row).remove().draw(false);

        // Reindex all remaining rows to keep indexes gapless
        this.reIndexRows();
        this.table.columns.adjust().responsive.recalc();

        // Run optional post-remove callback
        if (callbacks.afterRemove) callbacks.afterRemove();
    }


    // ================================================================
    //  REINDEX ROWS
    //  After adding or removing rows, this keeps all row indexes
    //  sequential so ASP.NET model binding works correctly
    //
    //  e.g. if row 2 of 4 is deleted:
    //    Before: [0], [1], [2], [3]  →  gap at [1]
    //    After:  [0], [1], [2]       →  gapless
    //
    //  Usage: systemTable.reIndexRows(); (called automatically by removeRow)
    // ================================================================
    reIndexRows() {
        this.table.rows().every(function (rowIdx) {
            let row = this.node();

            // Update the row's id attribute
            $(row).attr("id", rowIdx);

            // Update the S/No. display in the first cell
            $(row).find("td:first").text(rowIdx + 1);

            // Update name and id attributes on all inputs/selects/textareas
            $(row).find("input, select, textarea").each(function () {
                let name = $(this).attr("name");
                if (name)
                    // Replace any [n] with the correct [rowIdx]
                    $(this).attr("name", name.replace(/\[\d+\]/, `[${rowIdx}]`));

                let id = $(this).attr("id");
                if (id)
                    // Replace any _n__ with the correct _rowIdx__
                    $(this).attr("id", id.replace(/_\d+__/, `_${rowIdx}__`));
            });
        });
    }


    // ================================================================
    //  VALIDATE
    //  Validates all rows before form submission
    //  Returns true if all rules pass, false if any fail (shows SweetAlert)
    //
    //  @param {Array} rules - array of rule objects:
    //    { selector: '.systemCode', label: 'System Code', required: true }
    //
    //  Usage:
    //    const valid = systemTable.validate([
    //        { selector: '.systemCode',        label: 'System Code',        required: true },
    //        { selector: '.systemDescription', label: 'System Description', required: true }
    //    ]);
    //    if (valid) document.getElementById('postForm').click();
    // ================================================================
    validate(rules) {
        let valid = true;
        const nodes = this.table.rows().nodes().toArray();

        for (let i = 0; i < nodes.length; i++) {
            const node = nodes[i];

            for (let rule of rules) {
                const value = $(node).find(rule.selector).val();

                // Required check
                if (rule.required && (!value || value.trim() === "")) {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: `${rule.label} is required on row ${i + 1}.`
                    });
                    valid = false;
                    break;
                }

                // Max length check (optional)
                if (rule.maxLength && value && value.length > rule.maxLength) {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: `${rule.label} on row ${i + 1} cannot exceed ${rule.maxLength} characters.`
                    });
                    valid = false;
                    break;
                }

                // Custom validator function (optional)
                // rule.validator = (value) => true/false
                if (rule.validator && !rule.validator(value)) {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: rule.validatorMessage || `${rule.label} is invalid on row ${i + 1}.`
                    });
                    valid = false;
                    break;
                }
            }
            if (!valid) break;
        }
        return valid;
    }


    // ================================================================
    //  BIND SEARCH SYNC
    //  Keeps data-search attributes in sync with input field values
    //  so DataTables search works on editable input cells
    //
    //  Without this, DataTables reads DOM text — not input values.
    //  data-search holds the searchable value per cell.
    //
    //  Usage:
    //    Tables WITH inputs:    .init().bindSearchSync()
    //    Tables WITHOUT inputs: .init()   ← not needed
    // ================================================================
    bindSearchSync() {
        const self = this;

        $(document).on('input change', `#${this.tableId} input[type="text"], #${this.tableId} select`, function () {
            const td = $(this).closest('td');

            // Update the cell's data-search value to match the current input value
            td.attr('data-search', $(this).val());

            // Tell DataTables to re-read the DOM and refresh search index
            // draw(false) prevents resetting to page 1
            self.table.rows().invalidate('dom').draw(false);
        });

        return this; // allow chaining
    }


    // ================================================================
    //  GET ROW DATA
    //  Extracts all row values into a plain JS array of objects
    //  Useful for AJAX submissions or custom processing
    //
    //  @param {object} fieldMap - maps key names to jQuery selectors
    //
    //  Usage:
    //    const data = systemTable.getRowData({
    //        code:        '.systemCode',
    //        description: '.systemDescription'
    //    });
    //    // Returns: [{ code: 'SYS01', description: 'Payroll' }, ...]
    // ================================================================
    getRowData(fieldMap) {
        const results = [];

        this.table.rows().nodes().toArray().forEach(node => {
            const obj = {};
            for (let [key, selector] of Object.entries(fieldMap)) {
                obj[key] = $(node).find(selector).val();
            }
            results.push(obj);
        });

        return results;
    }


    // ================================================================
    //  PRIVATE: BIND REMOVE ROW
    //  Automatically wires up all buttons with class .removeRow
    //  inside this table — no need to add onclick manually
    //
    //  Called internally by init()
    // ================================================================
    _bindRemoveRow() {
        const self = this;

        // Use event delegation so it works on dynamically added rows too
        $(document).on("click", `#${this.tableId} .removeRow`, function () {
            self.removeRow(this);
        });
    }
}