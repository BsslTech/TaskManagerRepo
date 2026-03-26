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


//// Helper functions for fade in/out (standalone - do not depend on DOMContentLoaded)
//function fadeIn(el, duration = 300) {
//    if (!el) return;
//    el.style.opacity = 0;
//    el.style.display = "block";

//    let last = +new Date();
//    const tick = function () {
//        el.style.opacity = +el.style.opacity + (new Date() - last) / duration;
//        last = +new Date();

//        if (+el.style.opacity < 1) {
//            requestAnimationFrame(tick);
//        }
//    };
//    tick();
//}

//function fadeOut(el, duration = 300) {
//    if (!el) return;
//    el.style.opacity = 1;

//    let last = +new Date();
//    const tick = function () {
//        el.style.opacity = +el.style.opacity - (new Date() - last) / duration;
//        last = +new Date();

//        if (+el.style.opacity > 0) {
//            requestAnimationFrame(tick);
//        } else {
//            el.style.display = "none";
//        }
//    };
//    tick();
//}

//function initLoader() {
//    const loaderParent = document.querySelector(".loadingDiv-parent");

//    // expose helpers under a Loader namespace
//    try {
//        // If window.Loader already exists create/replace parent and keep functions
//        window.Loader = window.Loader || {};
//        window.Loader.parent = loaderParent;
//        const getLoaderEl = () => document.querySelector('.loadingDiv-parent');

//        // expose wrappers that always resolve the current loader element when called
//        window.Loader.fadeIn = function (elOrDuration, maybeDuration) {
//            let el;
//            let dur;
//            if (typeof elOrDuration === 'number') {
//                el = getLoaderEl();
//                dur = elOrDuration;
//            } else {
//                el = elOrDuration || getLoaderEl();
//                dur = maybeDuration || 300;
//            }
//            fadeIn(el, dur);
//        };

//        window.Loader.fadeOut = function (elOrDuration, maybeDuration) {
//            let el;
//            let dur;
//            if (typeof elOrDuration === 'number') {
//                el = getLoaderEl();
//                dur = elOrDuration;
//            } else {
//                el = elOrDuration || getLoaderEl();
//                dur = maybeDuration || 300;
//            }
//            fadeOut(el, dur);
//        };
//    }
//    catch (e) {
//        console.warn('Could not expose loader helpers to window.Loader:', e);
//    }

//    // Backwards-compatible globals for existing code
//    try {
//        // Backwards-compatible globals point to the Loader wrappers so they always find the element
//        window.fadeIn = function (elOrDuration, maybeDuration) { return window.Loader && window.Loader.fadeIn(elOrDuration, maybeDuration); };
//        window.fadeOut = function (elOrDuration, maybeDuration) { return window.Loader && window.Loader.fadeOut(elOrDuration, maybeDuration); };
//        window.loaderParent = loaderParent;
//    }
//    catch (e) {
//        console.warn('Could not expose loader globals:', e);
//    }

//    // Bind click event to anchor tags with valid hrefs
//    document.querySelectorAll(
//        'a[href]:not([href="#"]):not([href="javascript:void(0);"]):not([href="#detail"]):not([href="#acctdef"]):not([href="#dprecia"]):not([href="#reval"]):not([href="#optibal"]):not([href="#hstr"]):not([href="#taxTab"]):not([href="#taxTab2"]):not([href="#taxTab1"]):not([href="#taxTab3"]):not([href="#expenseTab"])'
//    ).forEach(anchor => {
//        anchor.addEventListener("click", function () {
//            fadeIn(loaderParent, 200); // fast fade in
//        });
//    });

//    // Hide loader once page is fully loaded
//    if (document.readyState === "complete") {
//        fadeOut(loaderParent, 600); // slow fade out
//    } else {
//        window.addEventListener("load", function () {
//            fadeOut(loaderParent, 600);
//        });
//    }
//}

//// Initialize immediately if DOM already loaded, otherwise wait for DOMContentLoaded
//if (document.readyState === 'loading') {
//    document.addEventListener('DOMContentLoaded', initLoader);
//} else {
//    initLoader();
//}


