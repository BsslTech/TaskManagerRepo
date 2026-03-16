jQuery(document).ready(function ($) {
    // Bind click event only to anchor tags with an href attribute not equal to '#' or 'javascript:void(0);'
    $('a[href]:not([href="#"]):not([href="javascript:void(0);"]):not([href="javascript:void(0)"]):not([href="#detail"]):not([href="#acctdef"]):not([href="#dprecia"]):not([href="#reval"]):not([href="#optibal"]):not([href="#hstr"]):not([href="#taxTab"]):not([href="#taxTab2"]):not([href="#taxTab1"]):not([href="#taxTab3"]):not([href="#expenseTab"])').on('click', function () {
        // Fade in the loader
        $(".loadingDiv-parent").fadeIn("fast");
    });

    // Check if the page is fully loaded
    if (document.readyState === 'complete') {
        jQuery(".loadingDiv-parent").fadeOut("slow");
    } else {

        jQuery(".loadingDiv-parent").fadeOut("slow");

    }    
});

