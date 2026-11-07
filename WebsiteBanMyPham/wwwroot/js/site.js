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
function showToast(message, isSuccess = true, actionHtml = null) {
    var toast = document.getElementById("toast");
    var toastMsg = document.getElementById("toast-msg");
    var toastIcon = document.getElementById("toast-icon");
    var toastTitle = document.getElementById("toast-title");

    if (!toast || !toastMsg) return;

    if (actionHtml) {
        toastMsg.innerHTML = message + " " + actionHtml;
    } else {
        toastMsg.innerText = message;
    }

    if (toastTitle) {
        var titleSuccess = document.getElementById("msg-toast-success")?.value || "THÀNH CÔNG";
        var titleError = document.getElementById("msg-toast-error")?.value || "THÔNG BÁO";
        toastTitle.innerText = isSuccess ? titleSuccess : titleError;
    }

    if (toastIcon) {
        toastIcon.className = isSuccess 
            ? "ph-fill ph-check-circle text-statusSuccess text-xl shrink-0" 
            : "ph-fill ph-x-circle text-statusDanger text-xl shrink-0";
    }

    toast.classList.remove("hidden");

    // Auto hide after 4 seconds
    clearTimeout(window.__toastTimeout);
    window.__toastTimeout = setTimeout(function () {
        hideToast();
    }, 4500);
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
function togglePassword(btnOrInputId, legacyBtn) {
    var inputId = "";
    var btn = null;
    if (btnOrInputId instanceof HTMLElement || (btnOrInputId && btnOrInputId.nodeType === 1)) {
        btn = btnOrInputId;
        inputId = btn.dataset.target || btn.getAttribute("data-target") || "";
    } else if (typeof btnOrInputId === "string") {
        inputId = btnOrInputId;
        btn = legacyBtn;
    }
    var input = inputId ? document.getElementById(inputId) : null;
    if (!input) return;

    var icon = btn ? btn.querySelector("i") : null;
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
function copyPromoCode(btnOrCode, legacySuccessMsg) {
    var code = "";
    var successMsg = "";
    if (btnOrCode instanceof HTMLElement || (btnOrCode && btnOrCode.nodeType === 1)) {
        code = btnOrCode.dataset.code || btnOrCode.getAttribute("data-code") || "";
        successMsg = btnOrCode.dataset.copied || btnOrCode.getAttribute("data-copied") || "";
    } else if (typeof btnOrCode === "string") {
        code = btnOrCode;
        if (typeof legacySuccessMsg === "string") {
            successMsg = legacySuccessMsg;
        } else if (legacySuccessMsg instanceof HTMLElement) {
            successMsg = legacySuccessMsg.dataset.copied || legacySuccessMsg.getAttribute("data-copied") || "";
        }
    }
    if (!code) return;
    var finalMsg = successMsg || code;
    if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(code).then(function () {
            showToast(finalMsg, true);
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
        showToast(finalMsg, true);
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
        var networkMsg = document.getElementById("msg-network-error")?.value || errorMsg || "Lỗi kết nối máy chủ. Vui lòng thử lại.";
        showToast(networkMsg, false);
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

// 8. Hạ tầng gọi Web API qua Cookie & AntiForgery (apiFetch)
async function apiFetch(url, options) {
    options = options || {};
    options.headers = options.headers || {};

    var tokenMeta = document.querySelector('meta[name="csrf-token"]');
    if (tokenMeta && tokenMeta.content) {
        options.headers["RequestVerificationToken"] = tokenMeta.content;
    }

    if (options.body && typeof options.body === "object" && !(options.body instanceof FormData)) {
        options.headers["Content-Type"] = "application/json";
        options.body = JSON.stringify(options.body);
    }

    options.credentials = "same-origin";

    try {
        var response = await fetch(url, options);

        if (response.status === 401) {
            var currentUrl = encodeURIComponent(window.location.pathname + window.location.search);
            window.location.href = "/TaiKhoan/DangNhap?returnUrl=" + currentUrl;
            return { ok: false, status: 401 };
        }

        if (response.status === 204) {
            return { ok: true, status: 204, data: null };
        }

        var responseText = await response.text();
        var data = null;
        var isJson = false;
        if (responseText && responseText.trim().length > 0) {
            try {
                data = JSON.parse(responseText);
                isJson = true;
            } catch (e) {
                isJson = false;
            }
        }

        if (!response.ok) {
            // Điều chỉnh 6: Khi nhận 400 mà thân phản hồi rỗng hoặc không phải JSON (token chống giả mạo sai/hết hạn)
            if (response.status === 400 && (!isJson || !data)) {
                var expiredMsg = document.getElementById("msg-session-expired")?.value || "Phiên làm việc đã hết hạn. Vui lòng tải lại trang.";
                var reloadLabel = document.getElementById("msg-reload-page")?.value || "Tải lại trang";
                var reloadBtnHtml = '<button onclick="window.location.reload()" class="underline font-bold ml-1 text-pastel hover:text-white">' + reloadLabel + '</button>';
                showToast(expiredMsg, false, reloadBtnHtml);
                return { ok: false, status: 400, error: expiredMsg };
            }

            var errorMsg = (data && (data.detail || data.title)) 
                ? (data.detail || data.title) 
                : ("HTTP Error " + response.status);
            showToast(errorMsg, false);
            return { ok: false, status: response.status, data: data, error: errorMsg };
        }

        return { ok: true, status: response.status, data: data };
    } catch (err) {
        var networkMsg = document.getElementById("msg-network-error")?.value || "Lỗi kết nối máy chủ. Vui lòng thử lại sau.";
        showToast(networkMsg, false);
        return { ok: false, status: 0, error: err };
    }
}

// 9. Cập nhật Badge Giỏ hàng
function updateCartBadge(count) {
    var badge = document.getElementById("header-cart-count");
    if (!badge) return;
    var num = parseInt(count, 10) || 0;
    badge.innerText = num;
    if (num > 0) {
        badge.classList.remove("hidden");
    } else {
        badge.classList.add("hidden");
    }
}

function refreshCartBadge() {
    var roleMeta = document.querySelector('meta[name="user-role"]');
    if (roleMeta && roleMeta.content === "KhachHang") {
        apiFetch("/api/v1/gio-hang/so-luong", { method: "GET" }).then(function (res) {
            if (res && res.ok && res.data && typeof res.data.tongSoLuong !== "undefined") {
                updateCartBadge(res.data.tongSoLuong);
            }
        });
    }
}

// Điều chỉnh 8: Lắng nghe sự kiện pageshow khi nhấn Back/Forward (event.persisted là true)
window.addEventListener("pageshow", function (event) {
    if (event.persisted) {
        refreshCartBadge();
    }
});

// 10. Hàm Thêm vào giỏ hàng dùng chung (ProductCard & ProductDetail)
// Tham số inline chỉ gồm số nguyên (maSP, soLuong) hoặc this (btn). Tên sản phẩm đọc từ dataset.
async function themVaoGio(maSP, soLuong, btnOrTenSP, legacyBtn) {
    var roleMeta = document.querySelector('meta[name="user-role"]');
    if (roleMeta && roleMeta.content === "QuanTriVien") {
        var adminMsg = document.getElementById("msg-admin-cart")?.value || "Quản trị viên không sử dụng chức năng giỏ hàng";
        showToast(adminMsg, false);
        return;
    }

    var btn = null;
    var tenSP = "";
    if (btnOrTenSP instanceof HTMLElement || (btnOrTenSP && btnOrTenSP.nodeType === 1)) {
        btn = btnOrTenSP;
        tenSP = btn.dataset.tenSp || btn.getAttribute("data-ten-sp") || btn.getAttribute("data-tensp") || "";
    } else if (legacyBtn instanceof HTMLElement || (legacyBtn && legacyBtn.nodeType === 1)) {
        btn = legacyBtn;
        tenSP = typeof btnOrTenSP === "string" ? btnOrTenSP : (btn.dataset.tenSp || btn.getAttribute("data-ten-sp") || btn.getAttribute("data-tensp") || "");
    } else if (typeof btnOrTenSP === "string") {
        tenSP = btnOrTenSP;
    }

    if (btn) {
        btn.disabled = true;
        btn.classList.add("opacity-60", "pointer-events-none");
    }

    var qty = parseInt(soLuong, 10) || 1;
    var res = await apiFetch("/api/v1/gio-hang/items", {
        method: "POST",
        body: { maSP: maSP, soLuong: qty }
    });

    if (btn) {
        btn.disabled = false;
        btn.classList.remove("opacity-60", "pointer-events-none");
    }

    if (res && res.ok) {
        if (res.data && typeof res.data.tongSoLuongTrongGio !== "undefined") {
            updateCartBadge(res.data.tongSoLuongTrongGio);
        }
        var successMsg = document.getElementById("msg-cart-added")?.value || "Đã thêm sản phẩm vào giỏ hàng!";
        showToast(successMsg, true);
    }
}

// 11. Hàm Mua ngay dùng chung (ProductCard & ProductDetail)
function muaNgay(maSP, soLuong, btn) {
    var roleMeta = document.querySelector('meta[name="user-role"]');
    var role = roleMeta ? roleMeta.content : "";
    if (role === "QuanTriVien") {
        var adminMsg = document.getElementById("msg-admin-buy")?.value || document.getElementById("msg-admin-cart")?.value || "Quản trị viên không sử dụng chức năng mua hàng.";
        showToast(adminMsg, false);
        return;
    }

    var qty = parseInt(soLuong, 10) || 1;
    var targetUrl = "/DatHang?maSP=" + maSP + "&soLuong=" + qty;

    if (!role || role === "") {
        window.location.href = "/TaiKhoan/DangNhap?returnUrl=" + encodeURIComponent(targetUrl);
        return;
    }

    window.location.href = targetUrl;
}

function muaNgayDetail(maSP, btn) {
    var qty = typeof getDetailQty === "function" ? getDetailQty() : 1;
    muaNgay(maSP, qty, btn);
}
