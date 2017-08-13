$(document).ready(function () {
    var grid = $("#jqGrid").jqGrid({
        url: 'GetUserBookingList',
        mtype: "GET",
        datatype: "json",
        page: 1,
        emptyrecords: 'No record to display',
        styleUI: "Bootstrap",
        colModel: [
            {
                label: 'Book date',
                name: 'BookDateTime',
                formatter: 'date',
                formatoptions: { srcformat: "ISO8601Long", newformat: "m/d/Y - H:i" }
            },
            {
                label: 'Treatment category',
                name: 'TreatmentCategory'
            },
            {
                label: 'Treatment',
                name: 'TreatmentName',
                formatter: function (cellvalue, options, rowObject) {
                    return "<a class='btn-link' href='Edit/" + rowObject.Id +
                        "'><u>" + rowObject.TreatmentName + "</u></a>";
                }
            },
            {
                label: 'Clinic',
                name: 'ClinicName'
            },
            {
                label: 'Comments',
                name: 'Comments',
            },
            {
                label: 'Status',
                name: 'BookingStatus'
            },
            {
                 label: 'Payment success',
                 name: 'PaymentSuccess'
            }

        ],
        loadonce: true,
        viewrecords: true,
        height: 'auto'
    });
    $(".ui-jqgrid-htable").addClass("table table-hover");
    $('#txtSearch').attr('placeholder', 'search in treatment or clinic');
});