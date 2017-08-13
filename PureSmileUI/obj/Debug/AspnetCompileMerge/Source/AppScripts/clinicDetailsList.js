$(document).ready(function () {

    $('#datetimepickerFrom').datetimepicker({
        pickTime: false
    });

    $('#datetimepickerTo').datetimepicker({
        pickTime: false
    });

    var grid = $("#jqGrid").jqGrid({
        url: 'GetClinicDetailsList',
        pager: $('#pager'),
        rowNum: 10,
        rowList: [5, 10, 20, 50],
        mtype: "GET",
        datatype: "json",
        page: 1,
        sortname: 'ClinicName',
        emptyrecords: 'No record to display',
        colModel: [
            {
                label: 'Clinic',
                name: 'ClinicName',
                index: 'ClinicName',
                sorttype: "text",
                search: true
            },
            {
                label: 'Date',
                name: 'TreatmentDateTime'
            },
            {
                label: 'Treatment',
                name: 'TreatmentName'
            },
            {
                label: 'Booking Status',
                name: 'BookingStatus'
            },
            {
                label: 'Customer Name',
                name: 'CustomerName'
            },
            {
                label: 'Customer Feedback',
                name: 'CustomerFeedback'
            },
            {
                label: 'Money spent',
                name: 'MoneySpent',
                width: '100'
            },
            {
                label: 'Money Earned',
                name: 'MoneyEarned',
                width: '100'
            }
        ],
        loadonce: false,
        viewrecords: true,
        height: 'auto'
    });
    $(".ui-jqgrid-htable").addClass("table table-hover");
});

function gridReload() {
    var treatmentId = $('#TreatmentList').val();
    var dateFrom = $('#dpFrom').val();
    var dateTo = $('#dpTo').val();

    $("#jqGrid").jqGrid('setGridParam', {
        url: "GetClinicDetailsList",
        postData: { treatmentId: treatmentId, dateFrom: dateFrom, dateTo: dateTo },
        datatype: "json", page: 1
    }).trigger("reloadGrid");
}

