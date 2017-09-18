$(document).ready(function () {
    var grid = $("#jqGrid").jqGrid({
        url: GetNotificationList,
        postData: {
          text: $("#txtSearch").val(),
          from: $("#txtFromDate").val(),
          to: $("#txtToDate").val()
        },
        mtype: "GET",
        datatype: "json",
        pager: '#pager',
        rowNum: 10,
        rowList: [5, 10, 20, 50],
        page: 1,
        emptyrecords: 'No record to display',
        colModel: [
            {
                label: 'Id',
                name: 'Id',
                align: 'center',
                key: true,
                formatter: function (cellvalue, options, rowObject) {
                    return "<a class='btn-link' href='View/" + rowObject.Id +
                        "'><u>" + rowObject.Id + "</u></a>";
                },
                width: '40'
            },
            {
                label: 'Subject',
                name: 'Subject',
                formatter: function (cellvalue, options, rowObject) {
                    return "<a class='btn-link' href='View/" + rowObject.Id +
                        "'><u>" + rowObject.Subject + "</u></a>";
                },
                width: '500'
            },
            {
                label: 'Sent on',
                name: 'SentOn',
                formatter: "date",
                formatoptions: { srcformat: "ISO8601Long", newformat: "m/d/Y - H:i" }
            },
            {
                label: 'Status',
                name: 'EmailStatus',
                align: "center"
            }
        ],
        loadonce: false,
        viewrecords: true,
        height: 'auto'
    });
    $(".ui-jqgrid-htable").addClass("table table-hover");
});

$(function () {
  $('#datetimepickerFrom').datetimepicker({
    pickTime: false
  });
  $('#datetimepickerTo').datetimepicker({
    pickTime: false
  });
});

function Search() {
    $("#jqGrid").jqGrid('setGridParam', {
        datatype: 'json', postData: {
            text: $("#txtSearch").val(),
            from: $("#txtFromDate").val(),
            to: $("#txtToDate").val()
        }
    }).trigger('reloadGrid');
}

function Clear() {
  $("#txtSearch").val('');
  $("#txtFromDate").val('');
  $("#txtToDate").val('');
  Search();
}