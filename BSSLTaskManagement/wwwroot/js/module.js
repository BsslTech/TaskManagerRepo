
// ── In-memory data store ───────────────────────────────────────────
// Replace with a fetch('/ModuleSetup/GetAll') call to load from server.
const moduleData = [
    { id: 1, systemType: 'Financial', youtubeHash: '', moduleCode: '5', description: 'PAYABLE/CREDITORS', fileName: '' },
    { id: 2, systemType: 'Financial', youtubeHash: '', moduleCode: '7', description: 'ACCOUNT CODE SETTINGS/END-OF-YEAR PROCESS', fileName: '' },
    { id: 3, systemType: 'Financial', youtubeHash: '', moduleCode: '1', description: 'ADMINISTRATION', fileName: '' },
    { id: 4, systemType: 'Financial', youtubeHash: '', moduleCode: '14', description: 'AUDIT, REPORTS & ENQUIRIES', fileName: '' },
    { id: 5, systemType: 'Financial', youtubeHash: '', moduleCode: '10', description: 'BUDGET', fileName: '' },
    { id: 6, systemType: 'Financial', youtubeHash: '', moduleCode: '4', description: 'GENERAL LEDGER', fileName: '' },
    { id: 7, systemType: 'Financial', youtubeHash: '', moduleCode: '20', description: 'INVESTMENT', fileName: '' },
    { id: 8, systemType: 'Financial', youtubeHash: '', moduleCode: '3', description: 'NON-CURRENT ASSET', fileName: '' },
    { id: 9, systemType: 'Financial', youtubeHash: '', moduleCode: '17', description: 'PROCUREMENT', fileName: '' },
    { id: 10, systemType: 'Financial', youtubeHash: '', moduleCode: '6', description: 'RECEIVABLE/DEBTOR, SALES & INVOICING', fileName: '' },
    { id: 11, systemType: 'Financial', youtubeHash: '', moduleCode: '11', description: 'REVENUE COLLECTION', fileName: '' },
    { id: 12, systemType: 'Financial', youtubeHash: '', moduleCode: '9', description: 'STOCK AND INVENTORY', fileName: '' }
];

// ── Pagination config ──────────────────────────────────────────────
const PAGE_SIZE = 10;
let currentPage = 1;
let nextId = moduleData.length + 1;

// ── DOM references ─────────────────────────────────────────────────
const form = document.getElementById('moduleForm');
const editIdInput = document.getElementById('editId');
const systemTypeEl = document.getElementById('systemType');
const youtubeHashEl = document.getElementById('youtubeHash');
const moduleCodeEl = document.getElementById('moduleCode');
const moduleDescEl = document.getElementById('moduleDesc');
const moduleFileEl = document.getElementById('moduleFile');
const fileNameDisplay = document.getElementById('fileNameDisplay');
const tableBody = document.getElementById('ModuleTableBody');
const paginationEl = document.getElementById('pagination');
const cancelBtn = document.getElementById('cancelBtn');
const toastEl = document.getElementById('liveToast');
const toastMsgEl = document.getElementById('toastMessage');

// Bootstrap Toast instance
const bsToast = new bootstrap.Toast(toastEl, { delay: 3500 });

// ── File input feedback ────────────────────────────────────────────
moduleFileEl.addEventListener('change', () => {
    fileNameDisplay.textContent = moduleFileEl.files.length
        ? moduleFileEl.files[0].name
        : 'No file chosen';
});

// ── Toast helper ───────────────────────────────────────────────────
function showToast(message, type = 'success') {
    toastEl.className = `toast align-items-center border-0 text-white bg-${type === 'error' ? 'danger' : 'success'}`;
    toastMsgEl.textContent = message;
    bsToast.show();
}

// ── Reset form to blank state ──────────────────────────────────────
function resetForm() {
    form.reset();
    editIdInput.value = '';
    fileNameDisplay.textContent = 'No file chosen';
    form.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
}

// ── HTML-escape utility ────────────────────────────────────────────
function escHtml(str) {
    const d = document.createElement('div');
    d.appendChild(document.createTextNode(str || ''));
    return d.innerHTML;
}

// ── Find record index by id ────────────────────────────────────────
function findIdx(id) {
    return moduleData.findIndex(r => r.id === id);
}

// ── Render current page ────────────────────────────────────────────
function renderTable() {
    const start = (currentPage - 1) * PAGE_SIZE;
    const page = moduleData.slice(start, start + PAGE_SIZE);

    if (moduleData.length === 0) {
        tableBody.innerHTML =
            `<tr><td colspan="6" class="text-center text-muted py-4">No records found.</td></tr>`;
        paginationEl.innerHTML = '';
        return;
    }

    tableBody.innerHTML = page.map((row, idx) => `
                <tr data-id="${row.id}">
                    <td class="ps-3 text-secondary fw-semibold">${start + idx + 1}</td>
                    <td class="text-primary fw-semibold">${escHtml(row.systemType)}</td>
                    <td>${escHtml(row.moduleCode)}</td>
                    <td>${escHtml(row.description)}</td>
                    <td class="text-muted">${escHtml(row.fileName)}</td>
                    <td>
                        <div class="action-btns">
                            <button class="btn btn-edit btn-outline-primary btn-sm" onclick="editRow(${row.id})">Edit</button>
                            <button class="btn btn-delete btn-outline-danger  btn-sm" onclick="deleteRow(${row.id})">Delete</button>
                        </div>
                    </td>
                </tr>`).join('');

    renderPagination();
}

// ── Pagination ─────────────────────────────────────────────────────
function renderPagination() {
    const totalPages = Math.ceil(moduleData.length / PAGE_SIZE);
    if (totalPages <= 1) { paginationEl.innerHTML = ''; return; }

    let html = `<nav><ul class="pagination pagination-sm mb-0">`;
    html += `<li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                        <button class="page-link" onclick="goToPage(${currentPage - 1})">&laquo;</button>
                     </li>`;

    for (let p = 1; p <= totalPages; p++) {
        html += `<li class="page-item ${p === currentPage ? 'active' : ''}">
                            <button class="page-link" onclick="goToPage(${p})">${p}</button>
                         </li>`;
    }

    html += `<li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
                        <button class="page-link" onclick="goToPage(${currentPage + 1})">&raquo;</button>
                     </li>`;
    html += `</ul></nav>`;
    paginationEl.innerHTML = html;
}

function goToPage(page) {
    const totalPages = Math.ceil(moduleData.length / PAGE_SIZE);
    if (page < 1 || page > totalPages) return;
    currentPage = page;
    renderTable();
}

// ── Edit row ───────────────────────────────────────────────────────
function editRow(id) {
    const rec = moduleData[findIdx(id)];
    if (!rec) return;

    editIdInput.value = rec.id;
    systemTypeEl.value = rec.systemType;
    youtubeHashEl.value = rec.youtubeHash;
    moduleCodeEl.value = rec.moduleCode;
    moduleDescEl.value = rec.description;
    fileNameDisplay.textContent = rec.fileName || 'No file chosen';

    window.scrollTo({ top: 0, behavior: 'smooth' });
}

// ── Delete row ─────────────────────────────────────────────────────
function deleteRow(id) {
    if (!confirm('Are you sure you want to delete this record?')) return;
    const idx = findIdx(id);
    if (idx === -1) return;
    moduleData.splice(idx, 1);

    const totalPages = Math.ceil(moduleData.length / PAGE_SIZE);
    if (currentPage > totalPages) currentPage = Math.max(1, totalPages);

    renderTable();
    showToast('Record deleted successfully.');

    // ── Real AJAX delete (uncomment to activate) ──────────────────
    // fetch(`/ModuleSetup/Delete/${id}`, {
    //     method: 'POST',
    //     headers: { 'RequestVerificationToken': getAntiForgeryToken() }
    // }).then(r => r.json()).then(res => {
    //     if (res.success) { moduleData.splice(idx, 1); renderTable(); showToast('Deleted.'); }
    //     else { showToast(res.message, 'error'); }
    // });
}

// ── Form submit: add or update ─────────────────────────────────────
form.addEventListener('submit', function (e) {
    e.preventDefault();

    const systemType = systemTypeEl.value.trim();
    const youtubeHash = youtubeHashEl.value.trim();
    const moduleCode = moduleCodeEl.value.trim();
    const description = moduleDescEl.value.trim();
    const file = moduleFileEl.files[0];
    const fileName = file ? file.name : '';

    // Inline Bootstrap validation
    let valid = true;
    [systemTypeEl, moduleCodeEl, moduleDescEl].forEach(el => el.classList.remove('is-invalid'));

    if (!systemType) { systemTypeEl.classList.add('is-invalid'); valid = false; }
    if (!moduleCode) { moduleCodeEl.classList.add('is-invalid'); valid = false; }
    if (!description) { moduleDescEl.classList.add('is-invalid'); valid = false; }
    if (!valid) { showToast('Please fill in all required fields.', 'error'); return; }

    const existingId = editIdInput.value ? parseInt(editIdInput.value) : null;

    if (existingId) {
        const idx = findIdx(existingId);
        if (idx !== -1) {
            moduleData[idx] = {
                ...moduleData[idx],
                systemType, youtubeHash, moduleCode, description,
                fileName: fileName || moduleData[idx].fileName
            };
        }
        showToast('Record updated successfully.');
    } else {
        moduleData.push({ id: nextId++, systemType, youtubeHash, moduleCode, description, fileName });
        currentPage = Math.ceil(moduleData.length / PAGE_SIZE);
        showToast('Record added successfully.');
    }

    resetForm();
    renderTable();

    // ── Real AJAX save (uncomment to activate) ────────────────────
    // const formData = new FormData(form);
    // fetch('/ModuleSetup/Save', {
    //     method: 'POST',
    //     body: formData,
    //     headers: { 'RequestVerificationToken': getAntiForgeryToken() }
    // }).then(r => r.json()).then(res => {
    //     if (res.success) { showToast('Saved.'); resetForm(); renderTable(); }
    //     else { showToast(res.message, 'error'); }
    // });
});

// ── Cancel button ──────────────────────────────────────────────────
cancelBtn.addEventListener('click', resetForm);

// ── Anti-forgery token helper ──────────────────────────────────────
function getAntiForgeryToken() {
    const el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
}

// ── Clear validation state on user input ───────────────────────────
[systemTypeEl, moduleCodeEl, moduleDescEl].forEach(el => {
    el.addEventListener('input', () => el.classList.remove('is-invalid'));
});

// ── Initial render ─────────────────────────────────────────────────
renderTable();