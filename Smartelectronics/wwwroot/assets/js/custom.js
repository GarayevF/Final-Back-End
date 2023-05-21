$(document).ready(function () {
    $(document).on('click', '.color, .colorSlider, .productLoanRange, .addtobasket, .basket-card-delete-btn, .subbasket, .addtowish, .addtocompare, .deletewish', function (e) {

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

        if ($(this).hasClass('addtobasket')) {
            e.preventDefault();

            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.basket-box').html(data)
                    if (window.location.pathname.split('/')[1].toLowerCase() == 'basket') {
                        fetch(url.replace("addbasket/" + url.split('/')[url.split('/').length - 1], 'mainbasket'))
                            .then(res2 => {
                                return res2.text()
                            })
                            .then(data2 => {
                                $('.basket-main-div').html(data2)
                            })
                    }
                })

        }

        if ($(this).hasClass('basket-card-delete-btn')) {
            e.preventDefault();

            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.basket-box').html(data)
                    if (window.location.pathname.split('/')[1].toLowerCase() == 'basket') {
                        fetch(url.replace("deletebasket/" + url.split('/')[url.split('/').length - 1], 'mainbasket'))
                            .then(res2 => {
                                return res2.text()
                            })
                            .then(data2 => {
                                $('.basket-main-div').html(data2)
                            })
                    }
                })
        }

        if ($(this).hasClass('subbasket')) {
            e.preventDefault();

            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.basket-box').html(data)
                    if (window.location.pathname.split('/')[1].toLowerCase() == 'basket') {
                        fetch(url.replace("removebasket/" + url.split('/')[url.split('/').length - 1], 'mainbasket'))
                            .then(res2 => {
                                return res2.text()
                            })
                            .then(data2 => {
                                $('.basket-main-div').html(data2)
                            })
                    }
                })

        }

        if ($(this).hasClass('addtowish')) {
            e.preventDefault();
            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.wishlist-box').html(data)
                    if (window.location.pathname.split('/')[1].toLowerCase() == 'wishlist') {
                        fetch(url.replace("wishlist/" + url.split('/')[url.split('/').length - 1], 'mainwishlist'))
                            .then(res2 => {
                                return res2.text()
                            })
                            .then(data2 => {
                                $('.wishlist-main-div').html(data2)
                            })
                    }
                })

        }

        if ($(this).hasClass('deletewish')) {
            e.preventDefault();
            let url = $(this).attr('href');
            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.wishlist-box').html(data)
                    if (window.location.pathname.split('/')[1].toLowerCase() == 'wishlist') {
                        fetch(url.replace("wishlist/" + url.split('/')[url.split('/').length - 1], 'mainwishlist'))
                            .then(res2 => {
                                return res2.text()
                            })
                            .then(data2 => {
                                $('.wishlist-main-div').html(data2)
                            })
                    }
                })

        }

        if ($(this).hasClass('addtocompare')) {
            e.preventDefault();
            let url = $(this).attr('href');

            fetch(url)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.compare-main-div').html(data)
                    fetch('compare/GetBasketCount')
                            .then(res2 => {
                                return res2.text()
                            })
                            .then(data2 => {
                                $('#compare-count-span').html(data2)
                            })
                })

        }
    })
        .on('change', '#compare-category-select', function (e) {
            let selectId = $('#compare-category-select').val();
            console.log(selectId)
            if (selectId) {
                fetch(`/compare/GetCompareByCategory/${selectId}`)
                    .then(response => response.text())
                    .then(data => {
                        $('.compare-main-div').html(data)

                        $('.owl-compare').owlCarousel({
                            loop: false,
                            margin: 10,
                            infinity: false,
                            nav: true,
                            navText: ["<img src='/assets/images/more.png'>", "<img src='/assets/images/more.png'>"],
                            items: 3,
                            autoplay: false,
                            responsive: {
                                0: {
                                    items: 1
                                },
                                767.98: {
                                    items: 2
                                },
                                991.98: {
                                    items: 3
                                }
                            }
                        });
                    });

            } 
        })
    
})