// JScript File
(function ($) {
    $.showprogress = function () {
        $.hideprogress();
        $("body").append('<div class="modal-spinner"><div class="spinjs"></div></div>');
        $(".spinjs").spin();
        $('.spinner').css('position', 'absolute');
        $('.spinner').css('left', '50%');
        $('.spinner').css('top', '50%');
        $("body").addClass("loading");
       
    },
    $.hideprogress = function () {
        $(".modal-spinner").remove();
        $("body").removeClass("loading");        
    }
})(jQuery);

