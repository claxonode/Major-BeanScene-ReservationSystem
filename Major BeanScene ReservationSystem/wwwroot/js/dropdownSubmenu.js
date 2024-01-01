$(() => {
    function toggleSubmenu() {
        $('.navbar-nav .dropdown-item[role="button"]').on('click', function (e) {
            e.preventDefault();
            e.stopPropagation();

            $(this).parent().toggleClass('active');

            $('.navbar-nav .dropdown-item[role="button"]').not(this).parent().removeClass('active');
        });
    }

    if ($(window).width() <= 575) {
        toggleSubmenu();
    }
    $(window).on('resize', function () {
        if ($(window).width() <= 575) {
            toggleSubmenu();
        }
    });
});