$(document).ready(function () {
    var grid = $("#jqGrid").jqGrid({
        url: 'GetEmailList',
        mtype: "GET",
        datatype: "json",
        pager: '#pager',
        rowNum: 10,
        rowList: [5, 10, 20, 50],
        page: 1,
        sortname: 'CreatedOn',
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
                width: '30'
            },
            {
              label: 'Created',
              name: 'CreatedOn',
              formatter: "date",
              formatoptions: { srcformat: "ISO8601Long", newformat: "m/d/Y - H:i" }
            },
            {
              label: 'Recipient',
              name: 'RecipientEmail',
              align: "left"
            },
            {
                label: 'Subject',
                name: 'Subject',
                formatter: function (cellvalue, options, rowObject) {
                    return "<a class='btn-link' href='View/" + rowObject.Id +
                        "'><u>" + rowObject.Subject + "</u></a>";
                },
                width:'400'
            },
            {
                label: 'Sent',
                name: 'SentOn',
                width: '60',
                formatter: "date",
                formatoptions: { srcformat: "ISO8601Long", newformat: "m/d/Y - H:i" }
            },
            {
                label: 'Status',
                name: 'EmailStatus',
                align: "center",
                width: '60'
            }
        ],
        loadonce: false,
        viewrecords: true,
        height: 'auto'
    });
    $(".ui-jqgrid-htable").addClass("table table-hover");
});
