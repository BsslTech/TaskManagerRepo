document.addEventListener('DOMContentLoaded', function () {

    const staffId = document.getElementById('staffId');
    const email = document.getElementById('email');
    const userName = document.getElementById('userName');
    if (staffId)
        staffId.addEventListener('change', async function () {
            document.getElementById('staffName').value = "";
            document.getElementById('userName').value = "";
            document.getElementById('email').value = "";
            document.getElementById('password').value = "";
            document.getElementById('confirm').value = "";

            if (!staffId.value) return;

            $(".loadingDiv-parent").fadeIn("fast");
            try {
                const response = await fetch(
                    `${window.location.origin}/Api/UserManagement/GetStaffDetails?staffId=${encodeURIComponent(staffId.value)}`,
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
                if (data.createAccount == "2") {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: `Account already created for this staff`,
                    }).then(() => {
                        document.getElementById('staffId').value = "";
                        document.getElementById('staffId').focus();
                    });
                    return false;
                }
                if (data.staffName !== null) {

                    document.getElementById('staffName').value = data.staffName;
                    document.getElementById('email').value = data.email;
                }
                else {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: `Invalid staff id supplied`,
                    }).then(() => {
                        document.getElementById('staffId').value = "";
                        document.getElementById('staffId').focus();
                    });
                    return false;
                }
                return false;

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
    if (email)
        email.addEventListener('change', async function () {
            const email = document.getElementById('email').value.trim();
            try {
                const response = await fetch(
                    `${window.location.origin}/Api/UserManagement/CheckEmailUserNameAsync?checkType=1&value=${encodeURIComponent(email)}`,
                    {
                        method: "GET",
                        credentials: "include" // replaces xhrFields.withCredentials
                    }
                );

                $(".loadingDiv-parent").fadeOut("slow");
                if (!response.ok) {
                    document.getElementById('email').value = "";
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: "Network response was not ok"
                    });
                    return;
                    //throw new Error("Network response was not ok");
                }

                const data = await response.json();

                if (data.status !== null) {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: `${data.statusDescription}`,
                    }).then(() => {
                        document.getElementById('email').value = "";
                        document.getElementById('email').focus();
                    });
                    return false;
                }
            }
            catch (error) {
                document.getElementById('email').value = "";
                document.getElementById('email').focus();
                $(".loadingDiv-parent").fadeOut("slow");
                //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);
                console.error("Error fetching email details:", error);
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "Failed to fetch email details. Please try again later."
                });
            }
        });
    if (userName)
        userName.addEventListener('change', async function () {
            const userName = document.getElementById('userName').value.trim();
            try {
                const response = await fetch(
                    `${window.location.origin}/Api/UserManagement/CheckEmailUserNameAsync?checkType=2&value=${encodeURIComponent(userName)}`,
                    {
                        method: "GET",
                        credentials: "include" // replaces xhrFields.withCredentials
                    }
                );

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

                if (data.status !== null) {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: `${data.statusDescription}`,
                    }).then(() => {
                        document.getElementById('userName').value = "";
                        document.getElementById('userName').focus();
                    });
                    return false;
                }
            }
            catch (error) {
                document.getElementById('userName').value = "";
                document.getElementById('userName').focus();
                $(".loadingDiv-parent").fadeOut("slow");
                //if (window.Loader) window.Loader.fadeOut(window.Loader.parent, 600);
                console.error("Error fetching username details:", error);
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "Failed to fetch username details. Please try again later."
                });
            }
        });
});
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
                text: "Account successfully created"
            }).then(() => {
                document.getElementById('staffId').value = "";
                document.getElementById('staffName').value = "";
                document.getElementById('email').value = "";
                document.getElementById('password').value = "";
                document.getElementById('confirm').value = "";
                document.getElementById('userName').value = "";
                window.location.href = `${window.location.origin}/Identity/Account/Login`
            });
        }
        else {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: statusDescription
            });
        }
    }
});