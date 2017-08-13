$(document).ready(function () {
    var grid = $("#jqGrid").jqGrid({
        url: 'GetUserList',
        mtype: "GET",
        datatype: "json",
        page: 1,
        pager: $('#pager'),
        rowNum: 10,
        rowList: [5, 10, 20, 50],
        emptyrecords: 'No record to display',
        colModel: [
            {
                label: 'Id',
                name: 'Id',
                align: 'center',
                key: true,
                width: '35'
            },
            {
                label: 'Login',
                name: 'UserName',
                formatter: function (cellvalue, options, rowObject) {
                    return "<a class='btn-link' href='Edit/" + rowObject.Id +
                        "'><u>" + rowObject.UserName + "</u></a>";
                },
                width: '250'
            },
            {
                label: 'Email',
                name: 'Email',
                width: '250'
            },
            {
                label: 'First name',
                name: 'FirstName'
            },
            {
                label: 'Last name',
                name: 'LastName'
            },
            {
                label: 'City',
                name: 'City',
            },
            {
                label: 'Type',
                name: 'Type',
            },
            {
                label: 'Lockout',
                name: 'LockoutEnabled',
                formatter: "checkbox",
                formatoptions: { disabled: true },
                align: 'center',
                width: '80'
            },
            {
                label: 'Phone number',
                name: 'PhoneNumber',
                align: 'right'
            }
        ],
        loadonce: false,
        viewrecords: true,
        height: 'auto'
    });
    $(".ui-jqgrid-htable").addClass("table table-hover");
});
