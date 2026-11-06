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

// 6. Sao chép mã khuyến mãi vào Clipboard
function copyPromoCode(code, successMsg) {
    if (!code) return;
    if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(code).then(function () {
            showToast(successMsg || code, true);
        }).catch(function () {
            showToast(code, true);
        });
    } else {
        var tempInput = document.createElement("input");
        tempInput.value = code;
        document.body.appendChild(tempInput);
        tempInput.select();
        document.execCommand("copy");
        document.body.removeChild(tempInput);
        showToast(successMsg || code, true);
    }
}

// 7. AJAX Lọc sản phẩm & Phân trang (Debounce 300ms + AbortController + History State)
var catalogFilterAbortController = null;
var catalogSearchDebounceTimer = null;

function applyCatalogFilters(resetPage, errorMsg) {
    var form = document.getElementById("catalog-filter-form");
    var container = document.getElementById("catalog-product-container");
    if (!form || !container) return;

    if (resetPage) {
        var pageInput = form.querySelector("input[name='trang']");
        if (pageInput) pageInput.value = "1";
    }

    var formData = new FormData(form);
    var params = new URLSearchParams();

    formData.forEach(function (value, key) {
        if (value && value.toString().trim() !== "" && value !== "all") {
            params.append(key, value.toString().trim());
        }
    });

    var url = form.action.split("?")[0] + (params.toString() ? "?" + params.toString() : "");
    fetchCatalogProducts(url, true, errorMsg);
}

function fetchCatalogProducts(url, updateHistory, errorMsg) {
    var container = document.getElementById("catalog-product-container");
    if (!container) return;

    if (catalogFilterAbortController) {
        catalogFilterAbortController.abort();
    }
    catalogFilterAbortController = new AbortController();

    // Hiệu ứng mờ nhẹ báo trạng thái đang tải
    container.classList.add("opacity-50", "pointer-events-none");

    fetch(url, {
        headers: {
            "X-Requested-With": "XMLHttpRequest"
        },
        signal: catalogFilterAbortController.signal
    })
    .then(function (response) {
        if (!response.ok) {
            throw new Error("HTTP status " + response.status);
        }
        return response.text();
    })
    .then(function (html) {
        container.innerHTML = html;
        container.classList.remove("opacity-50", "pointer-events-none");

        if (updateHistory) {
            window.history.pushState(null, "", url);
        }
    })
    .catch(function (err) {
        container.classList.remove("opacity-50", "pointer-events-none");
        if (err.name === "AbortError") {
            return;
        }
        showToast(errorMsg || "Lỗi tải dữ liệu. Vui lòng thử lại.", false);
    });
}

function onCatalogSearchInput(input, errorMsg) {
    clearTimeout(catalogSearchDebounceTimer);
    catalogSearchDebounceTimer = setTimeout(function () {
        applyCatalogFilters(true, errorMsg);
    }, 300);
}

function onCatalogFilterChange(errorMsg) {
    applyCatalogFilters(true, errorMsg);
}

function onCatalogPaginationClick(e, link, errorMsg) {
    if (e) e.preventDefault();
    if (!link || !link.href) return;
    fetchCatalogProducts(link.href, true, errorMsg);
}

window.addEventListener("popstate", function () {
    if (document.getElementById("catalog-product-container")) {
        fetchCatalogProducts(window.location.href, false);
    }
});
