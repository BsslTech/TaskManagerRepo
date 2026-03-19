document.addEventListener("DOMContentLoaded", function () {
    table = $('#roleDataTable').DataTable({
        responsive: true,
        pageLength: 10,
        order: [],
        columnDefs: [
            { orderable: false, targets: 0 }
        ]
    });
    const statusEl = document.getElementById("status");
    const statusDescriptionEl = document.getElementById("statusDescription");

    if (!statusEl) return; // safeguard if element doesn't exist

    const status = statusEl.value;
    const statusDescription = statusDescriptionEl ? statusDescriptionEl.value : "";

    if (status && status.trim() !== "") {
        if (status.trim().toLowerCase() === "success") {
            Swal.fire({
                icon: "success",
                title: "Success!",
                text: statusDescription
            });
        } else {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: statusDescription
            });
        }
    }
});

document.getElementById('btnChangePwd').addEventListener('click', async function () {
    const roleId = document.getElementById('roleId').value;
    const roleName = document.getElementById('roleName').value;
    if (roleId.toString().trim() === '') {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: `Role code is required.`,
        });
        return false;
    }
    if (roleName.toString().trim() === '') {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: `Role description is required.`,
            });
            return false;
    }

    $(".loadingDiv-parent").fadeIn("fast");
    //if (window.Loader) window.Loader.fadeIn(window.Loader.parent, 200);
    document.getElementById('btnSubmitForm').click();
});
document.getElementById('roleId').addEventListener('change', async function () {
    const roleId = document.getElementById('roleId').value.trim();

    // Reset fields
    document.getElementById('roleName').value = "";
    document.getElementById('id').value = "";

    if (!roleId) return;

    $(".loadingDiv-parent").fadeIn("fast");
    try {
        const response = await fetch(
            `${window.location.origin}/Api/UserManagement/GetUserRoleDetails?role=${encodeURIComponent(roleId)}&option=1`,
            {
                method: "GET",
                credentials: "include" // replaces xhrFields.withCredentials
            }
        );

        // Hide loading indicator
        //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);

        $(".loadingDiv-parent").fadeOut("slow");
        if (!response.ok) {
            document.getElementById('roleId').value = "";
            document.getElementById('id').value = "";
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Network response was not ok"
            });
            return;
            //throw new Error("Network response was not ok");
        }

        const data = await response.json();

        
        if (data.roleName !== null) {
          
            swalWithBootstrapButtons.fire({
                title: "Role code exists",
                text: "Do you want to continue?",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Yes, continue!",
                cancelButtonText: "No, return!",
                reverseButtons: false
            }).then((result) => {
                if (result.isConfirmed) {
                    document.getElementById('id').value = data.id;
                    document.getElementById('roleName').value = data.roleName;
                } else if (
                    /* Read more about handling dismissals below */
                    result.dismiss === Swal.DismissReason.cancel
                ) {
                    document.getElementById('id').value = "";
                    document.getElementById('roleId').value = "";
                    document.getElementById('roleName').value = "";
                    document.getElementById('roleId').focus();                    
                }
            });
            return;
        }
    } catch (error) {
        document.getElementById('roleId').value = "";
        document.getElementById('id').value = "";
        document.getElementById('roleName').value = "";

        $(".loadingDiv-parent").fadeOut("slow");
        //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);
        console.error("Error fetching user details:", error);
        Swal.fire({
            icon: "error",
            title: "Error",
            text: "Failed to fetch user role. Please try again later."
        });
    }
});

document.getElementById('roleName').addEventListener('change', async function () {
    const roleId = document.getElementById('roleId').value.trim();
    if (roleId.toString().trim() === '') {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: `Role code is required.`,
        }).then(() => {
            document.getElementById('roleName').value = "";
            document.getElementById('roleId').focus();
        });
        return false;
    }
    // Reset fields
    const roleName = document.getElementById('roleName').value.trim();

    if (!roleName) return;

    $(".loadingDiv-parent").fadeIn("fast");
    try {
        const response = await fetch(
            `${window.location.origin}/Api/UserManagement/GetUserRoleDetails?role=${encodeURIComponent(roleName)}&option=2`,
            {
                method: "GET",
                credentials: "include" // replaces xhrFields.withCredentials
            }
        );

        // Hide loading indicator
        //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);

        $(".loadingDiv-parent").fadeOut("slow");
        if (!response.ok) {
            document.getElementById('roleName').value = "";
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Network response was not ok"
            });
            return;
            //throw new Error("Network response was not ok");
        }

        const data = await response.json();


        if (data.roleName !== null) {
           
            swalWithBootstrapButtons.fire({
                title: "User role description exists",
                text: "Do you want to continue?",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Yes, continue!",
                cancelButtonText: "No, return!",
                reverseButtons: false
            }).then((result) => {
                if (result.isConfirmed) {
                    document.getElementById('roleName').value = data.roleName;
                } else if (
                    /* Read more about handling dismissals below */
                    result.dismiss === Swal.DismissReason.cancel
                ) {
                    document.getElementById('roleName').value = "";
                    document.getElementById('roleName').focus();
                }
            });
            return;
        }
    } catch (error) {
        document.getElementById('roleName').value = "";

        $(".loadingDiv-parent").fadeOut("slow");
        //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);
        console.error("Error fetching user details:", error);
        Swal.fire({
            icon: "error",
            title: "Error",
            text: "Failed to fetch role details. Please try again later."
        });
    }
});
function editRow(index, id, code, description) {
    document.getElementById('id').value = id || '';
    document.getElementById('roleId').value = code || '';
    document.getElementById('roleName').value = description || '';
    // Scroll to form
    document.getElementById('roleId').scrollIntoView({ behavior: 'smooth', block: 'center' });
    document.getElementById('roleId').focus();
}
