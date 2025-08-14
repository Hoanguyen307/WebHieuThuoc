$(document).ready(function () {

    function updateTotalPrice(card) {
        let price = parseFloat(card.find('.text-danger').text().replace(/[^\d]/g, ""));
        let qty = parseInt(card.find(".quantity").val());
        let total = price * qty;
        card.find(".item-total").text(total.toLocaleString('vi-VN') + " đ");
        updateCartTotal();
    }

    function updateCartTotal() {
        let total = 0;
        $(".item-total").each(function () {
            total += parseFloat($(this).text().replace(/[^\d]/g, ""));
        });
        $("#cart-total").text(total.toLocaleString('vi-VN') + " đ");
    }

    $(".btn-plus").click(function () {
        let card = $(this).closest(".cart-item");
        let qtyInput = card.find(".quantity");
        qtyInput.val(parseInt(qtyInput.val()) + 1).trigger("change");
    });

    $(".btn-minus").click(function () {
        let card = $(this).closest(".cart-item");
        let qtyInput = card.find(".quantity");
        let current = parseInt(qtyInput.val());
        if (current > 1) {
            qtyInput.val(current - 1).trigger("change");
        }
    });

    $(".quantity").change(function () {
        let card = $(this).closest(".cart-item");
        let cartItemId = card.closest("[data-id]").data("id");
        let qty = parseInt($(this).val());

        $.post("/Cart/UpdateQuantity", { cartItemId: cartItemId, quantity: qty }, function (res) {
            if (res.success) {
                if (qty <= 0) {
                    card.closest(".col-md-6").remove();
                } else {
                    updateTotalPrice(card);
                }
            } else {
                alert(res.message || "Có lỗi xảy ra khi cập nhật số lượng.");
            }
        });
    });

    $(".btn-remove").click(function () {
        let card = $(this).closest(".cart-item");
        let cartItemId = card.closest("[data-id]").data("id");

        if (confirm("Bạn có chắc muốn xóa sản phẩm này?")) {
            $.post("/Cart/RemoveItem", { cartItemId: cartItemId }, function (res) {
                if (res.success) {
                    card.closest(".col-md-6").remove();
                    updateCartTotal();
                } else {
                    alert(res.message || "Có lỗi xảy ra khi xóa sản phẩm.");
                }
            });
        }
    });

});
