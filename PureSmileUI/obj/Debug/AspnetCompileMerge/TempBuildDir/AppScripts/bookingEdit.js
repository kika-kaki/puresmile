$(function () {
  $('#datetimepicker1').datetimepicker({
      pickTime: false
    })
    .on('changeDate', function(e) {
      getPossibleBookList();
      $(this).datetimepicker('hide');
    });

  $('#datetimepicker2').datetimepicker({
    pickTime: false
  }).on('changeDate', function (e) {
    $(this).datetimepicker('hide');
  });
});

function getPossibleBookList() {

  var date = $('#bookDate');
  var clinic = $('*#bookClinic:checked');

  $.ajax({
    url: GetTimeListLink + "?clinic=" + clinic.val() + "&date=" + date.val(),
    method: "GET",
    datatype: "json"
  }).done(function(data) {

    $('#bookTime').empty();

    $.each(data.list, function(i, item) {
      $('#bookTime').append($('<option>', {
        value: item.Name,
        text: item.Name + (item.IsBooked ? " (reserved)" : ""),
        disabled: item.IsBooked
      }));
    });
  });
}

function ConfirmRefund() {
    var sum = GetSum();
    if (sum == 0) {
        $("#txtRefundSum").val("");
        $("#txtRefundSum").focus();
        return;
    }
    $("#txtRefundSum").val(sum);
    $(".bConfirmSumm").text(sum);
    $("#RefundSum").val(sum);
    if (!ValidateSum(sum)) {
        $("#dlgConfirmOverRefund").dialog({
            title: "Confirm refund",
            modal: true,
            closeOnEscape: true,
            position: {
                my: "center",
                at: "center",
                of: ".mainbar"
            },
            open: function (event, ui) {
                $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
            }
        });
        return;
    }
    $("#dlgConfirmRefund").dialog({
        title: "Confirm refund",
        modal: true,
        closeOnEscape: true,
        position: {
            my: "center",
            at: "center",
            of: ".mainbar"
        },
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
        }
    });
    return;
}

function GetSum() {
    var sum = $("#txtRefundSum").val().replace(',', '.');
    if (isNaN(sum) || sum == '') {
        return 0;
    }
    sum = Math.round(parseFloat(sum) * 100) / 100;
    if (sum == 0 || sum < 0.01) {
        return 0;
    }
    return (sum + '').replace('.', ',');
}

function ValidateSum(sum) {
    var s = parseFloat(sum.replace(',', '.'));
    var max = parseFloat(maxAmountToRefund.replace(',', '.'));
    return s < max;
}

function fillClientData(id) {
    if (id && id != 0) {
        var user = $.ajax({
            url: "/User/GetUser/" + id,
            method: "GET",
            datatype: "json"
        }).done(function (data) {
            $('#txtClientEmail').val(data.Email);
            $('#txtClientPhone').val(data.PhoneNumber);
            $('#txtFirstName').val(data.ClientData.FirstName);
            $('#txtLastName').val(data.ClientData.LastName);
        });
    }
    else {
        $('#txtClientEmail').val('');
        $('#txtClientPhone').val('');
        $('#txtClientName').val('');
    }
}

function ShowRefundDialog() {
    $("#dlgRefund").dialog({
        title: "Refund",
        modal: true,
        closeOnEscape: true,
        position: {
            my: "center",
            at: "center",
            of: ".mainbar"
        },
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
        }
    });
}