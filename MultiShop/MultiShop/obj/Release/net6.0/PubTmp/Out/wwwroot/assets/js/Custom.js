﻿document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('click', function (e) {
        if (e.target.closest('.quantity-increase')) {
            let row = e.target.closest('tr');
            let input = row.querySelector('.quantity-value');
            let count = parseInt(input.value) || 1;
            count++;
            input.value = count;
            updateTotal(row, count);
            updateCookie(row.dataset.id, count);
            calculateSummary();
        }

        if (e.target.closest('.quantity-decrease')) {
            let row = e.target.closest('tr');
            let input = row.querySelector('.quantity-value');
            let count = parseInt(input.value) || 1;
            if (count > 1) {
                count--;
                input.value = count;
                updateTotal(row, count);
                updateCookie(row.dataset.id, count);
                calculateSummary();
            }
        }

        if (e.target.closest('.btn-delete')) {
            let row = e.target.closest('tr');
            let id = row.dataset.id;

            fetch(`/basket/delete?id=${id}`, {
                method: 'POST'
            })
                .then(response => {
                    if (response.ok) {
                        row.remove();
                        calculateSummary(); // Silindikdə yenilə
                    } else {
                        alert("Silinmə zamanı xəta baş verdi.");
                    }
                });
        }
        if (e.target.closest('.btn-delete-wish')) {
            let row = e.target.closest('tr');
            let id = row.dataset.id;

            fetch(`/wish/delete?id=${id}`, {
                method: 'POST'
            })
                .then(response => {
                    if (response.ok) {
                        row.remove();
                    } else {
                        alert("Silinmə zamanı xəta baş verdi.");
                    }
                });
        }
    });

    function updateTotal(row, count) {
        let price = parseFloat(row.querySelector('.quantity-value').dataset.price);
        let totalCell = row.querySelector('.total-price');
        totalCell.textContent = (price * count).toFixed(2) + ' $';
        calculateSummary(); 
    }

    function updateCookie(id, count) {
        fetch(`/basket/updatecookie?id=${id}&count=${count}`, {
            method: 'POST'
        });
    }

    //  SUBTOTAL və TOTAL avtomatik hesablama
    function calculateSummary() {
        let subtotal = 0;
        document.querySelectorAll('.total-price').forEach(cell => {
            let priceText = cell.textContent.replace('$', '').trim();
            let price = parseFloat(priceText) || 0;
            subtotal += price;
        });

        document.getElementById('subtotalAmount').textContent = '$' + subtotal.toFixed(2);

        let couponRow = document.getElementById('couponDiscountRow');
        let couponDiscountText = document.getElementById('couponDiscountValue').textContent.replace('$', '').trim();
        let discount = parseFloat(couponDiscountText) || 0;

        let total = subtotal - discount;
        document.getElementById('totalAmount').textContent = '$' + total.toFixed(2);
    }

    function isBasketEmpty() {
        return document.querySelectorAll("tr[data-id]").length === 0;
    }

    document.getElementById("couponForm").addEventListener("submit", function (e) {
        e.preventDefault();
        let code = document.getElementById("couponInput").value;

        if (isBasketEmpty()) {
            let modal = new bootstrap.Modal(document.getElementById("emptyBasketModal"));
            modal.show();
            return; // sorğunu göndərmə
        }
        fetch(`/basket/applycoupon?code=${code}`, {
            method: 'POST'
        })
            .then(res => res.json())
            .then(data => {
                const msg = document.getElementById("couponMessage");
                if (data.success) {
                    msg.textContent = data.message;
                    document.getElementById("couponDiscountValue").textContent = `$${data.discount.toFixed(2)}`;
                    document.getElementById("couponDiscountRow").style.display = 'flex';
                    document.getElementById("subtotalAmount").textContent = `$${data.subtotal.toFixed(2)}`;
                    document.getElementById("totalAmount").textContent = `$${data.total.toFixed(2)}`;
                    //  Kupon kodunu yadda saxla
                    document.getElementById("appliedCouponCode").value = code;
                } else {
                    msg.textContent = data.message;
                    document.getElementById("couponDiscountRow").style.display = 'none';
                    document.getElementById("appliedCouponCode").value = "";
                }
            })
            .catch(err => {
                console.error("Kupon tətbiqi zamanı xəta baş verdi:", err);
            });
    });


    calculateSummary();
});


document.getElementById("checkoutBtn").addEventListener("click", function (e) {
    let rows = document.querySelectorAll("tr[data-id]");
    let hasMissingAttributes = false;

    rows.forEach(row => {
        let color = row.getAttribute("data-color");
        let size = row.getAttribute("data-size");

        if (!color || color === "N/A" || !size || size === "N/A") {
            hasMissingAttributes = true;
            row.classList.add("table-danger");
        } else {
            row.classList.remove("table-danger");
        }
    });

    if (hasMissingAttributes) {
        e.preventDefault();
        let modal = new bootstrap.Modal(document.getElementById("missingAttributesModal"));
        modal.show();
        return;
    }

    // Kupon kodunu oxu
    let coupon = document.getElementById("appliedCouponCode").value.trim();
    let checkoutUrl = '/Basket/Checkout';
    if (coupon) {
        checkoutUrl += `?coupon=${encodeURIComponent(coupon)}`;
    }

    fetch('/Account/IsAuthenticated')
        .then(res => res.json())
        .then(data => {
            if (data.isAuthenticated) {
                window.location.href = checkoutUrl;
            } else {
                let loginModal = new bootstrap.Modal(document.getElementById('loginPromptModal'));
                loginModal.show();
                document.getElementById("confirmLoginBtn").onclick = function () {
                    window.location.href = '/Account/Login';
                };
            }
        })
        .catch(error => {
            console.error("Xəta baş verdi:", error);
        });
});

