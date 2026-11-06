// Serene Cosmetics - Site JS Scripts

// 1. Mobile Menu Toggle
function toggleMobileMenu() {
    var menu = document.getElementById("mobile-menu");
    if (menu) {
        menu.classList.toggle("hidden");
    }
}

// 2. Admin Sidebar Toggle
function toggleAdminSidebar() {
    var sidebar = document.getElementById("admin-sidebar");
    if (sidebar) {
        sidebar.classList.toggle("hidden");
    }
}

// 3. Toast Notifications
function showToast(message, isSuccess = true) {
    var toast = document.getElementById("toast");
    var toastMsg = document.getElementById("toast-msg");
    var toastIcon = document.getElementById("toast-icon");
    var toastTitle = document.getElementById("toast-title");

    if (!toast || !toastMsg) return;

    toastMsg.innerText = message;

    if (toastIcon) {
        toastIcon.className = isSuccess 
            ? "ph-fill ph-check-circle text-statusSuccess text-xl shrink-0" 
            : "ph-fill ph-x-circle text-statusDanger text-xl shrink-0";
    }

    toast.classList.remove("hidden");

    // Auto hide after 4 seconds
    setTimeout(function () {
        hideToast();
    }, 4000);
}

function hideToast() {
    var toast = document.getElementById("toast");
    if (toast) {
        toast.classList.add("hidden");
    }
}

// 4. Chống bấm đúp (Double-click submit prevention)
document.addEventListener("DOMContentLoaded", function () {
    var forms = document.querySelectorAll("form");
    forms.forEach(function (form) {
        form.addEventListener("submit", function (e) {
            // Nếu dùng jQuery unobtrusive validation và form không hợp lệ thì không disable nút
            if (window.jQuery && typeof jQuery(form).valid === "function") {
                if (!jQuery(form).valid()) {
                    return;
                }
            }

            var submitBtns = form.querySelectorAll("button[type='submit'], input[type='submit']");
            submitBtns.forEach(function (btn) {
                // Đặt timeout nhẹ để form kịp gửi request
                setTimeout(function () {
                    btn.disabled = true;
                    btn.classList.add("opacity-60", "cursor-not-allowed");
                }, 50);
            });
        });
    });
});

// 5. Ẩn / Hiện mật khẩu (Toggle Password Visibility)
function togglePassword(inputId, btn) {
    var input = document.getElementById(inputId);
    if (!input) return;

    var icon = btn.querySelector("i");
    if (input.type === "password") {
        input.type = "text";
        if (icon) {
            icon.classList.remove("ph-eye");
            icon.classList.add("ph-eye-slash");
        }
    } else {
        input.type = "password";
        if (icon) {
            icon.classList.remove("ph-eye-slash");
            icon.classList.add("ph-eye");
        }
    }
}
