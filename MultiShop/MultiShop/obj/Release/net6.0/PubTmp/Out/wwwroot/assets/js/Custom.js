document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('click', function (e) {
        if (e.target.closest('.quantity-increase')) {
            let row = e.target.closest('tr');
            let input = row.querySelector('.quantity-value');
            let count = parseInt(input.value) || 1;
            count++;
            input.value = count;
            updateTotal(row, count);
            updateCookie(row.dataset.id, count);
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
                        //calculateSummary(); // Silindikdə yenilə
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
        //calculateSummary(); // Yenilə
    }

    function updateCookie(id, count) {
        fetch(`/basket/updatecookie?id=${id}&count=${count}`, {
            method: 'POST'
        });
    }

    //function calculateSummary() {
    //    let subtotal = 0;
    //    document.querySelectorAll('.total-price').forEach(cell => {
    //        let priceText = cell.textContent.replace('$', '').trim();
    //        let price = parseFloat(priceText) || 0;
    //        subtotal += price;
    //    });

    //    let shipping = parseFloat(document.getElementById('shippingAmount').textContent.replace('$', '')) || 0;
    //    let total = subtotal + shipping;

    //    document.getElementById('subtotalAmount').textContent = '$' + subtotal.toFixed(2);
    //    document.getElementById('totalAmount').textContent = '$' + total.toFixed(2);
    //}

    //// İlk yüklənmədə cəmi hesabla
    //calculateSummary();
});

document.getElementById("checkoutBtn").addEventListener("click", function () {
    fetch('/Account/IsAuthenticated') 
        .then(res => res.json())
        .then(data => {
            if (data.isAuthenticated) {
                window.location.href = '/Basket/Checkout';
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
