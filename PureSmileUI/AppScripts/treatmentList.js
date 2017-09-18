$(document).ready(function () {
    var grid = $("#jqGrid").jqGrid({
        url: 'GetTreatmentList',
        mtype: "GET",
        datatype: "json",
        page: 1,
        sortname: 'Name',
        emptyrecords: 'No record to display',
        colModel: [
            {
                label: 'ID',
                name: 'Id',
                align: 'center',
                key: true,
                formatter: function (cellvalue, options, rowObject) {
                    return "<a class='btn-link' href='Edit/" + rowObject.Id +
                        "'><u>" + rowObject.Id + "</u></a>";
                },
                width: '30'
            },
            {
                label: 'Category',
                name: 'TreatmentCategoryName',
                width: '200'
            },
            {
                label: 'Treatment Name',
                name: 'Name',
                formatter: function (cellvalue, options, rowObject) {
                    return "<a class='btn-link' href='Edit/" + rowObject.Id +
                        "'><u>" + rowObject.Name + "</u></a>";
                },
                width: '200'
            },
            {
                label: 'Description',
                name: 'Description',
                width: '200'
            },
            {
                label: 'Price',
                name: 'Price'
            },
            {
                label: 'Is active',
                name: 'IsActive',
                align: "center",
                formatter: "checkbox",
                formatoptions: { disabled: true },
                width: '70'
            }
        ],
        loadonce: true,
        viewrecords: true,
        height: 'auto'
    });
    $(".ui-jqgrid-htable").addClass("table table-hover");
});