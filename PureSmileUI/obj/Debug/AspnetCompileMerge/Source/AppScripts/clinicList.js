$(document).ready(function () {
    var grid = $("#jqGrid").jqGrid({
        url: 'GetClinicList',
        mtype: "GET",
        datatype: "json",
        page: 1,
        emptyrecords: 'No record to display',
        colModel: [
            {
                label: 'Id',
                name: 'Id',
                align: 'center',
                key: true,
                width: '30',
                formatter: function (cellvalue, options, rowObject) {
                    return "<a class='btn-link' href='Edit/" + rowObject.Id +
                        "'><u>" + rowObject.Id + "</u></a>";
                }
            },
            {
                label: 'BusinessName',
                name: 'Name',
                formatter: function (cellvalue, options, rowObject) {
                    return "<a class='btn-link' href='Edit/" + rowObject.Id +
                        "'><u>" + rowObject.Name + "</u></a>";
                },
                width: '300'
            },
            {
                label: 'Address',
                name: 'Address',
                formatter: function (cellvalue, options, rowObject) {
                  return rowObject.Address + " " + rowObject.City + " " + rowObject.State;
                },
                width: '300'
            },
            {
                label: 'Is active',
                name: 'IsActive',
                align: "center",
                formatter: "checkbox",
                formatoptions: { disabled: true }
            }
        ],
        loadonce: true,
        viewrecords: true,
        height: 'auto'
    });
    $(".ui-jqgrid-htable").addClass("table table-hover");
});
