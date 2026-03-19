document.addEventListener("DOMContentLoaded", function () {
    const statusEl = document.getElementById("status");
    const statusDescriptionEl = document.getElementById("statusDescription");

    if (!statusEl) return; // safeguard if element doesn't exist

    const status = statusEl.value;
    const statusDescription = statusDescriptionEl ? statusDescriptionEl.value : "";

    if (status) {
        if (status.toString().trim().toLowerCase() === 'success') {

            let timerInterval;

            Swal.fire({
                title: "Success!",
                html: `${statusDescription}<br><br>
                        <b>You will be redirected in <span id="countdown">3</span> seconds...</b>
                       `,
                icon: "success",
                timer: 3000,
                timerProgressBar: true,
                showConfirmButton: false,
                didOpen: () => {
                    //fadeIn(loaderParent, 200); // fast fade in
                    if (window.Loader) window.Loader.fadeIn(window.Loader.parent, 200);// fast fade in
                    let countdownEl = document.getElementById('countdown');
                    let timeLeft = 3;

                    timerInterval = setInterval(() => {
                        timeLeft--;
                        if (countdownEl && timeLeft >= 0) {
                            countdownEl.textContent = timeLeft;
                        }
                    }, 1000);
                },
                willClose: () => {
                    clearInterval(timerInterval);
                }
            }).then(() => {
                $(".loadingDiv-parent").fadeIn("fast");
                //if (window.Loader) window.Loader.fadeIn(window.Loader.parent, 200);// fast fade in
                window.location.href = window.location.origin + "/Identity/Account/Login";
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
document.getElementById('btnChangePwd').addEventListener('click', async function () {
    const currentPassword = document.getElementById('currentPassword').value;
    const newPassword = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirm').value;

    if (currentPassword.toString().trim() === '') {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: `Current password is required.`,
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
    //if (window.Loader) window.Loader.fadeIn(window.Loader.parent, 200);// fast fade in

    $(".loadingDiv-parent").fadeIn("fast");
    document.getElementById('btnSubmitForm').click();
});