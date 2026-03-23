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
    const userName = document.getElementById('userName').value;
    const staffName = document.getElementById('staffName').value;
    const newPassword = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirm').value;
    const changePwd = document.getElementById('changePwd').value;
    if (userName.toString().trim() === '') {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: `Username/Email is required.`,
        });
        return false;
    }
    if (userName.toString().trim() === '') {

        if (staffName.toString().trim() === '') {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: `Staff name is required.`,
            });
            return false;
        }
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
    $(".loadingDiv-parent").fadeIn("fast");
    //if (window.Loader) window.Loader.fadeIn(window.Loader.parent, 200);
    document.getElementById('btnSubmitForm').click();
});
document.getElementById('userName').addEventListener('change', async function () {
    const userName = document.getElementById('userName').value.trim();

    // Reset fields
    document.getElementById('staffName').value = "";
    document.getElementById('password').value = "";
    document.getElementById('confirm').value = "";
    document.getElementById('changePwd').value = "";

    if (!userName) return;

    // Show loading indicator
    //window.Loader.fadeIn(document.querySelector('.loadingDiv-parent'), 200)
    $(".loadingDiv-parent").fadeIn("fast");
    try {
        const response = await fetch(
            `${window.location.origin}/Api/UserManagement/GetUserDetails?userName=${encodeURIComponent(userName)}`,
            {
                method: "GET",
                credentials: "include" // replaces xhrFields.withCredentials
            }
        );

        // Hide loading indicator
        //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);

        $(".loadingDiv-parent").fadeOut("slow");
        if (!response.ok) {
            document.getElementById('userName').value = "";
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Network response was not ok"
            });
            return;
            //throw new Error("Network response was not ok");
        }

        const data = await response.json();

        if (!data) {
            document.getElementById('userName').value = "";
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Username/Email not found. Please check and try again"
            });
            return;
        }
        if (data.name === null) {
            document.getElementById('userName').value = "";
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Username/Email not found. Please check and try again"
            });
            return;
        }
        // Populate staffName
        document.getElementById('staffName').value = data.name || "";
    } catch (error) {
        document.getElementById('userName').value = "";

        $(".loadingDiv-parent").fadeOut("slow");
        //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);
        console.error("Error fetching user details:", error);
        Swal.fire({
            icon: "error",
            title: "Error",
            text: "Failed to fetch user details. Please try again later."
        });
    }
});
