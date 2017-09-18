$(document).ready(function () {

    $('#datetimepickerFrom').datetimepicker({
        pickTime: false
    });

    $('#datetimepickerTo').datetimepicker({
        pickTime: false
    });

    var grid = $("#jqGrid").jqGrid({
        url: bookingListUrl,
        mtype: "GET",
        datatype: "json",
        page: 1,
        emptyrecords: 'No record to display',
        colModel: [
            {
                label: 'Id',
                name: 'Id',
                width: 30
            },
            {
                label: 'Status',
                name: 'StatusId',
                width: 80
            },
            {
                label: 'Created on',
                name: 'CreatedOn',
                formatter: "date",
                formatoptions: { srcformat: "ISO8601Long", newformat: "m/d/Y - H:i" },
                width: 120
            },
            {
                label: 'Refund',
                name: 'IsRefund',
                width: 60
            },
            {
                label: 'Booking',
                name: 'BookingId',
                width: 60
            },
            {
                label: 'User',
                name: 'UserId',
                width: 40
            },
            {
              label: 'Refund by User',
              name: 'RefundByUserId',
              width: 100
            }
        ],
        loadonce: true,
        viewrecords: true,
        height: 'auto'
    });
    $(".ui-jqgrid-htable").addClass("table table-hover");
});

function gridReload() {

    $("#jqGrid").jqGrid('setGridParam', {
        url: bookingListUrl,
        datatype: "json",
        page: 1
    }).trigger("reloadGrid");
}