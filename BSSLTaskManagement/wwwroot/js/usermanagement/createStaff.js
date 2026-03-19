document.addEventListener("DOMContentLoaded", function () {
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
        }
        else {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: statusDescription
            }).then(() => {
                const createAccount = document.getElementById('createAccount').value;
                if (createAccount == "1") {
                    elems.forEach(el => el.classList.remove('d-none'));
                } else {
                    elems.forEach(el => el.classList.add('d-none'));
                    document.getElementById('useEmail').value = "1";
                    document.getElementById('userName').value = "Staff@gamil.com";
                    document.getElementById('password').value = "Staff@gamil.com";
                    document.getElementById('confirm').value = "Staff@gamil.com";
                    document.getElementById('changePwd').value = "1";
                }
            });
        }
    }
});

let elems = document.querySelectorAll('.createAccount');
document.getElementById('btnCreateAccount').addEventListener('click', async function () {
    const staffId = document.getElementById('staffId').value;
    const staffName = document.getElementById('staffName').value;
    const email = document.getElementById('email').value;
    const staffType = document.getElementById('staffType').value;
    const createAccount = document.getElementById('createAccount').value;
    const useEmail = document.getElementById('useEmail').value;
    const newPassword = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirm').value;
    const changePwd = document.getElementById('changePwd').value;
    if (staffId.toString().trim() === '') {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: `Staff ID is required.`,
        });
        return false;
    }
    if (staffName.toString().trim() === '') {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: `Staff name is required.`,
        });
        return false;
    }
    if (email.toString().trim() === '') {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: `Email is required.`,
        });
        return false;
    }
    if (staffType.toString().trim() === '') {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: `Staff role is required.`,
        });
        return false;
    }
    if (createAccount.toString().trim() === '') {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: `Select if to create account.`,
        });
        return false;
    }
    if (createAccount.toString().trim() === '1') {
        if (useEmail.toString().trim() === '') {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: `Select if to use email as username.`,
            });
            return false;
        }
        if (userName.toString().trim() === '') {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: `Username/Email is required.`,
            });
            return false;
        }
        if (newPassword.toString().trim() === '') {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: `New password is required.`,
            });
            return false;
        }
        if (confirmPassword.toString().trim() === '') {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: `Confirm password is required.`,
            });
            return false;
        }
        if (changePwd.toString().trim() === '') {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: `Select if to change password on login.`,
            });
            return false;
        }
    }
    
    $(".loadingDiv-parent").fadeIn("fast");
    document.getElementById('createAccount').disabled = false;
    //if (window.Loader) window.Loader.fadeIn(window.Loader.parent, 200);
    document.getElementById('btnSubmitForm').click();
});
document.getElementById('staffId').addEventListener('change', async function () {
    const staffId = document.getElementById('staffId').value.trim();


    if (!staffId) return;

    // Show loading indicator
    //window.Loader.fadeIn(document.querySelector('.loadingDiv-parent'), 200)
    $(".loadingDiv-parent").fadeIn("fast");
    try {
        const response = await fetch(
            `${window.location.origin}/Api/UserManagement/GetStaffDetails?staffId=${encodeURIComponent(staffId)}`,
            {
                method: "GET",
                credentials: "include" // replaces xhrFields.withCredentials
            }
        );

        $(".loadingDiv-parent").fadeOut("slow");
        if (!response.ok) {
            document.getElementById('staffId').value = "";
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Network response was not ok"
            });
            return;
            //throw new Error("Network response was not ok");
        }

        const data = await response.json();

        if (data.staffName !== null) {

            swalWithBootstrapButtons.fire({
                title: "Staff ID exists",
                text: "Do you want to continue?",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Yes, continue!",
                cancelButtonText: "No, return!",
                reverseButtons: false
            }).then((result) => {
                if (result.isConfirmed) {
                    document.getElementById('id').value = data.id;
                    document.getElementById('staffName').value = data.staffName;
                    document.getElementById('email').value = data.email;
                    document.getElementById('oldemail').value = data.email;
                    document.getElementById('staffType').value = data.staffType;
                    //document.getElementById('suspend').value = data.suspend;
                    document.getElementById('suspend').value = "0";
                    document.getElementById('createAccount').value = data.createAccount;
                    if (data.createAccount === "2")
                        document.getElementById('createAccount').disabled = true;
                        else
                        document.getElementById('createAccount').disabled = false;


                    document.getElementById('useEmail').value = "1";
                    document.getElementById('userName').value = "Staff@gamil.com";
                    document.getElementById('password').value = "Staff@gamil.com";
                    document.getElementById('confirm').value = "Staff@gamil.com";
                    document.getElementById('changePwd').value = "1";
                    elems.forEach(el => el.classList.add('d-none'));
                } else if (
                    /* Read more about handling dismissals below */
                    result.dismiss === Swal.DismissReason.cancel
                ) {
                    document.getElementById('id').value = "";
                    document.getElementById('staffId').value = "";
                    document.getElementById('staffId').focus();
                    document.getElementById('staffName').value = "";
                    document.getElementById('email').value = "";
                    document.getElementById('oldemail').value = "";
                    document.getElementById('staffType').value = "";
                    document.getElementById('suspend').value = "";
                    document.getElementById('createAccount').value = "";

                    elems.forEach(el => el.classList.add('d-none'));
                }
            });
            return;
        }
        
    } catch (error) {
        document.getElementById('staffId').value = "";
        document.getElementById('staffId').focus();
        $(".loadingDiv-parent").fadeOut("slow");
        //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);
        console.error("Error fetching user details:", error);
        Swal.fire({
            icon: "error",
            title: "Error",
            text: "Failed to fetch staff details. Please try again later."
        });
    }
});
document.getElementById('createAccount').addEventListener('change', async function () {
    const staffId = document.getElementById('staffId').value.trim();
    const createAccount = document.getElementById('createAccount').value.trim();
    const staffName = document.getElementById('staffName').value.trim();
    const email = document.getElementById('email').value.trim();

    if (staffId.toString().trim() === '') {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: `Enter staff id.`,
        }).then(() => {
            document.getElementById('createAccount').value = "";
            document.getElementById('staffId').focus();
        });
        return false;
    }
    if (staffName.toString().trim() === '') {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: `Enter staff name.`,
        }).then(() => {
            document.getElementById('createAccount').value = "";
            document.getElementById('staffName').focus();
        });
        return false;
    }
    if (email.toString().trim() === '') {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: `Email is required.`,
        }).then(() => {
            document.getElementById('createAccount').value = "";
            document.getElementById('email').focus();
        });
        return false;
    }
    if (!createAccount) return;
    if (createAccount == "1") {
        elems.forEach(el => el.classList.remove('d-none'));
        document.getElementById('useEmail').value = "";
        document.getElementById('userName').value = "";
        document.getElementById('password').value = "";
        document.getElementById('confirm').value = "";
        document.getElementById('changePwd').value = "";
    } else {
        elems.forEach(el => el.classList.add('d-none'));
        document.getElementById('useEmail').value = "1";
        document.getElementById('userName').value = "Staff@gamil.com";
        document.getElementById('password').value = "Staff@gamil.com";
        document.getElementById('confirm').value = "Staff@gamil.com";
        document.getElementById('changePwd').value = "1";
    }
});
document.getElementById('useEmail').addEventListener('change', async function () {
    const useEmail = document.getElementById('useEmail').value.trim();
    const email = document.getElementById('email').value.trim();
    if (useEmail === "1") {
        document.getElementById('userName').value = email;
        document.getElementById('userName').readOnly = true;
    } else {
        document.getElementById('userName').value = "";
        document.getElementById('userName').readOnly = false;
    }
});
document.getElementById('email').addEventListener('change', async function () {
    const email = document.getElementById('email').value.trim();
    const id = document.getElementById('id').value.trim();
    if (id === null) {
        document.getElementById('oldemail').value = email;
    }
});
