var paidStatus = -1;
var firstOpen = true;


function Search() {

    if ($('#rdbDefault').is(':checked')) paidStatus = -1;
    else
        if ($('#rdbPaid').is(':checked')) paidStatus = 1;
        else
            if ($('#rdbNotPaid').is(':checked')) paidStatus = 0;


    $("#jqGrid").jqGrid('setGridParam', {
        datatype: 'json', postData: {
            text: $("#txtSearch").val(),
            bookDate: $("#txtBookDate").val(),
            paidStatus: paidStatus
        }
    }).trigger('reloadGrid');
}

function Clear() {
    $("#txtSearch").val('');
    $('#txtBookDate').val('');
    $('input[name="paid-status"]').prop('checked', false);
    $('#rdbDefault').prop('checked', true);
    Search();
}

$(function () {
    $('#datetimepickerBookDate').datetimepicker({
        clearBtn: true
    });
});

