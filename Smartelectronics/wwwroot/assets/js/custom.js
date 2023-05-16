$(document).ready(function () {
    $(document).on('click', '.color, .colorSlider, .productLoanRange', function (e) {

        if ($(this).hasClass('color')) {
            e.preventDefault();

            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $(this).closest('.product-card').find('.image-link').html(data)
                })
        }

        if ($(this).hasClass('colorSlider')) {
            e.preventDefault();

            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $(this).closest('.product-card').find('.image-link-slider').html(data);
                    $(this).closest('.product-card').find('.image-link-slider .product-detail-slider').owlCarousel({
                        loop: true,
                        margin: 10,
                        nav: false,
                        items: 1,
                        autoplay: false,
                        dots: true,
                    })
                    $(this).closest('.product-card').find('.image-link-slider .product-detail-slider').trigger('refresh.owl.carousel');
                })
        }

        if ($(this).hasClass('productLoanRange')) {
            e.preventDefault();
            let dataIdVal = $(this).attr("data-id");
            $(this).closest('.credit-options').find('.prices-for-months span strong').text(dataIdVal);
        }

    })
})