var onRatioChanged = function () {
    var ratio = $('#txtRatio').val();
    if (ratio && ratio != 0 && !isNaN(ratio)) {
        $('#paymentRatioDiff').text(100 - ratio);
    }
    else {
        $('#paymentRatioDiff').text(0);
    }
}

$(document).ready(function () {
    onRatioChanged();
});