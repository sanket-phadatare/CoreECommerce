function CartBadgeCount() {
$.ajax({
        url: "/cart/cartitemcount",
        type: 'GET',
        content: 'application/json',

        success: function (response) {
            console.log(response);
            $('#cart-badge-count').text(response);

        },

        error: function (reason) {
            console.log(reason);
            alert('api failed count cart items');
        }
    });
}
    
