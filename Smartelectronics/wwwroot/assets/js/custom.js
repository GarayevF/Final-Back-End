$(document).ready(function () {

    $(document).on('keyup', '#search, #search-m', function () {
        let search = $(this).val();

        if (search.length >= 3) {
            fetch('/product/search?search=' + search)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $('.searchBody').html(data)
                })
        } else {
            $('.searchBody').html('')
        }
    })

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    let successInput = $("input[name='success']");
    if (successInput.val()?.length > 0) {
        toastr["success"](successInput.val())
    }

    let errorInput = $("input[name='error']");
    if (errorInput.val()?.length > 0) {
        toastr["error"](errorInput.val())
    }

    //$(function () {
    //    $(".collapse").on('show.bs.collapse', function (e) {
    //        if ($(this).is(e.target)) {
    //            snippet.log(this.id)
    //        }
    //    })
    //});

    $(document).on('click', '.color, .buy-credit-btn, .buy-cash-btn, .colorSlider, .productLoanRange, .addtobasket, .basket-card-delete-btn, .subbasket, .addtowish, .addtocompare, .deletewish', function (e) {

        //if ($(this).hasClass('credit-drop-atag')) {
        //    e.preventDefault();
        //    $('.dropbtn').html(`<span>${$(this).text() }</span>`)
        //}

        if ($(this).hasClass('buy-credit-btn')) {
            e.preventDefault();
            if ($('input[name="creditCondition"]:checked').length > 0) {
                var selectedRange = $('input[name="creditCondition"]:checked').data('range');
                var selectedPrice = $('input[name="creditCondition"]:checked').data('price');
                
                if ($('input[name="creditCondition"]:checked').attr('id') !== 'creditCondition-0') {
                    var total = (selectedRange * selectedPrice).toFixed(2);
                    var newUrl = window.location.href.replace("product/detail/", `order/checkoutcredit?method=Kredit%20${selectedRange}&price=${selectedPrice}&total=${total}&productId=`);
                    window.location.href = newUrl;
                } else {
                    selectedPrice = Math.floor(parseFloat($('#ayliqodenis').text().replace(/[^0-9.-]+/g, "")) * 100) / 100;
                    selectedRange = parseInt($('#kreditspantag').text().replace(/[^0-9.-]+/g, ""));
                    selectedInitial = parseInt($('.pristine').text().replace(/[^0-9.-]+/g, ""));
                    var total = (selectedRange * selectedPrice).toFixed(2);
                    var newUrl = window.location.href.replace("product/detail/", `order/checkoutcredit?method=Kredit%20${selectedRange}&price=${selectedPrice}&total=${total}&productId=`);
                    window.location.href = newUrl;
                }
            }
            
        }

        if ($(this).hasClass('buy-cash-btn')) {
            e.preventDefault();
            var newUrl = window.location.href.replace("product/detail/", `order/checkoutsingle?method=Nağd&productId=`);
            window.location.href = newUrl;

        }

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
                    toastr["success"]("Məhsul Səbətə Əlavə Olundu")
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
                    toastr["success"]("Məhsul Səbətdən Silindi")
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
                    toastr["success"]("Məhsul Seçilmişlərə Əlavə Olundu")
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
                    toastr["success"]("Məhsul Seçilmişlərdən Çıxarıldı")
                    if (window.location.pathname.split('/')[1].toLowerCase() == 'wishlist') {
                        fetch("/wishlist/mainwishlist")
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