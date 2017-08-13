$(document).ready(function () {


    var grid = $("#jqGrid").jqGrid({
        url: 'GetAdminBookingList',
        mtype: "GET",
        datatype: "json",
        pager: '#pager',
        rowNum: 10,
        rowList: [5, 10, 20, 50],
        page: 1,
        postData: {
            text: $("#txtSearch").val(),
            bookDate: $("#txtBookDate").val(),
            paidStatus: paidStatus
        },
        emptyrecords: 'No record to display',
        colModel: [
            {
                label: 'Id',
                name: 'Id',
                align: 'center',
                width: 55,
                key: true,
                formatter: function (cellvalue, options, rowObject) {
                    return "<a class='btn-link' href='AdminEdit/" + rowObject.Id +
                        "'><u>" + rowObject.Id + "</u></a>";
                }
            },
            {
                label: 'User Name',
                name: 'UserName'
            },
            {
                label: 'Client first name',
                name: 'FirstName'
            },
            {
                label: 'Client last name',
                name: 'LastName'
            },
            {
                label: 'Phone',
                name: 'ClientPhone'
            },
            {
                label: 'Email',
                name: 'ClientEmail'
            },
            {
                label: 'Treatment category',
                name: 'TreatmentCategory'
            },
            {
                label: 'Treatment',
                name: 'TreatmentName'
            },
            {
                label: 'Clinic',
                name: 'ClinicName'
            },
            {
                label: 'Book date',
                name: 'BookDateTime',
                formatter: "date",
                formatoptions: { srcformat: "ISO8601Long", newformat: "m/d/Y - H:i" }
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
                label: 'Is paid',
                name: 'IsPaid'
            },
            {
                label: 'Payment success',
                name: 'PaymentSuccess'
            },
            {
                label: 'Payments',
                formatter: function (cellvalue, options, rowObject) {
                    return "<a class='btn-link' href='" + rowObject.PaymentDetailsUrl + "'><u>View</u></a>";
                }
            },
             {
                 label: 'Paid to clinic on',
                 name: 'PaidToClinicOn',
                 formatter: "date",
                 formatoptions: { srcformat: "ISO8601Long", newformat: "m/d/Y - H:i" }
             },
            {
                label: 'Paid to Clinic by User',
                name: 'PaidToClinicByUserName'
            },
        ],
        loadonce: false,
        viewrecords: true,
        height: 'auto',
    });
    $(".ui-jqgrid-htable").addClass("table table-hover");

    $('.modal').appendTo($('body'));

    //$('#cmdSelectTreatment').click(function () {
    //    var url = $('#treatmentModal').data('url');

    //    $.get(url, function (data) {
    //        $('#treatmentContainer').html(data);
    //    });
    //});    
});





//var cmdSelectClinic = function (model)
//{
//    var url = 'ClinicModalSelect?item=' + model;

//    $.get(url, function (data) {
//        $('#treatmentContainer').html(data);
//    });
//};