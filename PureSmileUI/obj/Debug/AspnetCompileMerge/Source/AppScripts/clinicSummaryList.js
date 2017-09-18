$(document).ready(function () {

    $('#datetimepickerFrom').datetimepicker({
        pickTime: false
    });

    $('#datetimepickerTo').datetimepicker({
        pickTime: false
    });

    var grid = $("#jqGrid").jqGrid({
        url: 'GetClinicSummaryList',
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
                label: 'Number of Treatments Performed',
                name: 'TreatmentCount',
                width: '200'
            },
            {
                label: 'Number of Customers',
                name: 'CustomersCount',
            },
            {
                label: 'Money Spent',
                name: 'MoneySpent',
            },
            {
                label: 'Money Earned',
                name: 'MoneyEarned',
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
        url: "GetClinicSummaryList",
        postData: { treatmentId: treatmentId, dateFrom: dateFrom, dateTo: dateTo },
        datatype: "json",
        page: 1
    }).trigger("reloadGrid");
}

