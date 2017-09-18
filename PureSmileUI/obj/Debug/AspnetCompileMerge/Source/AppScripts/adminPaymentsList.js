$(document).ready(function () {

    $('#datetimepickerFrom').datetimepicker({
        pickTime: false
    });

    $('#datetimepickerTo').datetimepicker({
        pickTime: false
    });
    var hasId = typeof id != 'undefined';
    var paymentId = !hasId ? null : id;
    var treatmentId = !hasId ? null : $('#TreatmentList').val() == "" ? null : $('#TreatmentList').val();
    var treatmentCategoryId = !hasId ? null : $('#TreatmentCategoryList').val() == "" ? null : $('#TreatmentCategoryList').val();
    var clinicId = !hasId ? null : $('#ClinicList').val() == "" ? null : $('#ClinicList').val();
    var dateFrom = !hasId ? null : $('#dpFrom').val();
    var dateTo = !hasId ? null : $('#dpTo').val();

    var grid = $("#jqGrid").jqGrid({
        url: bookingListUrl,
        postData: { id: paymentId, treatmentId: treatmentId, treatmentCategoryId: treatmentCategoryId, clinicId: clinicId, dateFrom: dateFrom, dateTo: dateTo },
        mtype: "GET",
        datatype: "json",
        page: 1,
        pager: $('#pager'),
        rowNum: 5,
        rowList: [5, 10, 20, 50],
        emptyrecords: 'No record to display',
        colModel: [
            {
                label: 'Id',
                name: 'Id',
                width: 30
            },
            {
                label: 'Status',
                name: 'StringStatus',
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
            },
            {
              label: 'Transaction Info',
              name: 'TransactionCode',
              width: 400,
              formatter: function (cellvalue, options, rowObject) {
                if (rowObject.Total + rowObject.ProcessorResponseCode + rowObject.ProcessorResponseText +
                    rowObject.Message + rowObject.OrderId + rowObject.CreatedAt + rowObject.Status != '') {
                  var html = "<table>";
                  html += "<tr><td>Amount</td><td>" + rowObject.Total + "</td></tr>";
                  html += "<tr><td>Transaction Id</td><td>" + rowObject.TransactionId + "</td></tr>";
                  html += "<tr><td>Processor code</td><td>" + rowObject.ProcessorResponseCode + "</td></tr>";
                  html += "<tr><td>Processor text</td><td>" + rowObject.ProcessorResponseText + "</td></tr>";
                  html += "<tr><td>Message</td><td>" + rowObject.Message + "</td></tr>";
                  html += "<tr><td>Order id</td><td>" + rowObject.OrderId + "</td></tr>";
                  if (rowObject.CreatedAt) {
                    var date = new Date(parseInt(rowObject.CreatedAt.substr(6)));
                    html += "<tr><td>Create at</td><td>" + date.toString() + "</td></tr>";
                  } else {
                    html += "<tr><td>Create at</td><td></td></tr>";
                  }
                  html += "<tr><td>Status</td><td>" + rowObject.Status + "</td></tr>";
                  html += "</table>";
                  return html;
                } else {
                  return "no info";
                }
              }
            },
            {
                label: 'Paypal Info',
                name: 'PaypalInfo',
                width: 400,
                formatter: function (cellvalue, options, rowObject) {
                  if (rowObject.PaypalDebugId + rowObject.PayPalPaymentId + rowObject.PayPalTransactionFeeAmount +
                      rowObject.PayPalPayeeEmail + rowObject.PayPalPayerEmail != '') {
                    var html = "<table>";
                    html += "<tr><td>Paypal Debug Id</td><td>" + rowObject.PaypalDebugId + "</td></tr>";
                    html += "<tr><td>PayPal Payment Id</td><td>" + rowObject.PayPalPaymentId + "</td></tr>";
                    html += "<tr><td>PayPal Transaction<br/>Fee Amount</td><td>" + rowObject.PayPalTransactionFeeAmount + "</td></tr>";
                    html += "<tr><td>PayPalPayee Email</td><td>" + rowObject.PayPalPayeeEmail + "</td></tr>";
                    html += "<tr><td>PayPalPayer Email</td><td>" + rowObject.PayPalPayerEmail + "</td></tr>";
                    html += "</table>";
                    return html;
                  } else {
                    return "no info";
                  }
                }
            },
            {
              label: 'Card Info',
              name: 'CardInfo',
              width: 300,
              formatter: function (cellvalue, options, rowObject) {
                if (rowObject.CardBin + rowObject.CardCardholderName + rowObject.CardType + rowObject.CardExpirationDate +
                    rowObject.CardLastFour != '') {
                  var html = "<table>";
                  html += "<tr><td>Card Bin</td><td>" + rowObject.CardBin + "</td></tr>";
                  html += "<tr><td>Card Cardholder Name</td><td>" + rowObject.CardCardholderName + "</td></tr>";
                  html += "<tr><td>Card Type</td><td>" + rowObject.CardType + "</td></tr>";
                  html += "<tr><td>Card Expiration Date</td><td>" + rowObject.CardExpirationDate + "</td></tr>";
                  html += "<tr><td>Card Last Four</td><td>" + rowObject.CardLastFour + "</td></tr>";
                  html += "</table>";
                  return html;
                } else {
                  return "no info";
                }
              }
            }
        ],
        loadonce: false,
        viewrecords: true,
        height: 'auto'
    });
    $(".ui-jqgrid-htable").addClass("table table-hover");
});

function gridReload() {
    var hasId = id != undefined && id != '';
    var paymentId = hasId ? null : id;
    var treatmentId = hasId ? null : $('#TreatmentList').val() == "" ? null : $('#TreatmentList').val();
    var treatmentCategoryId = hasId ? null : $('#TreatmentCategoryList').val() == "" ? null : $('#TreatmentCategoryList').val();
    var clinicId = hasId ? null : $('#ClinicList').val() == "" ? null : $('#ClinicList').val();
    var dateFrom = hasId ? null : $('#dpFrom').val();
    var dateTo = hasId ? null : $('#dpTo').val();

    $("#jqGrid").jqGrid('setGridParam', {
        url: bookingListUrl,
        postData: { id: paymentId, treatmentId: treatmentId, treatmentCategoryId: treatmentCategoryId, clinicId: clinicId, dateFrom: dateFrom, dateTo: dateTo },
        datatype: "json",
        page: 1
    }).trigger("reloadGrid");
}