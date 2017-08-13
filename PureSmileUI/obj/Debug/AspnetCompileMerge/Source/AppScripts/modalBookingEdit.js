$(function() {
  $('.dtpicker').datetimepicker({
      pickTime: false
    })
    .on('changeDate', function(e) {
      getPossibleBookList();
      $(this).datetimepicker('hide');
      SetModalBookingDate($(this).find("input"));
    });

  $('.modal').appendTo($('body'));

  $('#cmdSelectTreatment').click();
});

var paymentAmount = 0;
var cmdSelectClinic = function (obj, id, name, description, price) {
  $('#treatmentId').val(id);
  $(".jsBookBtn").removeClass("btn-primary").addClass("btn-info");
  $(obj).removeClass("btn-info").addClass("btn-primary");
  $("#btnSelectDateLocations").removeAttr("disabled");
  $("#step4TreatmentTitle").text(name);
  $("#step4TreatmentPrice").text('$' + price);
  $("#step4TreatmentDescription").text(description);
  $("#step4TotalPrice").text('$' + price);
  paymentAmount = price;
};

var OnSelectDateAndLocation = function () {
  if ($('#treatmentId').val() === '' || $('#treatmentId').val() == 0) {
    return;
  }
  SetStep(2);
  initMap();
}

var SetStep = function(step) {
  $(".step").hide();
  $("#bookingStep" + step).show();

  if (step == 3) {
    ValidateStep3();
  }
  if (step == 4) {
    FillStep4Info();
  }
}

var position;
var ToggleLocationsShow = function(obj) {
  $(obj).find("a").toggle();
  var showClosest = $("#cbxShowClosest").val();
  showClosest = showClosest === "false" ? "true" : "false";
  $("#cbxShowClosest").val(showClosest);
  if (showClosest === "true") {
    if (position) {
      SelectByPosition();
    } else {
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function(pos) {
          position = pos;
          SelectByPosition();
        });
      } else {
        // disable location toggling
      }
    }
  } else {
    $(".jsClinics").show();
  }
}

function SelectByPosition() {
  var maxDistance = parseInt(MaxRadiusForClosestClinicsOnMapInMeters);
  var lat = position.coords.latitude;
  var lng = position.coords.longitude;

  $(".jsClinics").hide();

  for (var i in maps) {
    var m = maps[i];
    distance = GetDistance(lat, lng, m.lat, m.lng);
    if (distance < maxDistance) {
      $("#jsClinic" + m.clinicId).show();
    }
  }
}

function GetDistance(lat1, lng1, lat2, lng2) {
  lat1 = deg2rad(lat1);
  lng1 = deg2rad(lng1);
  lat2 = deg2rad(lat2);
  lng2 = deg2rad(lng2);

  return Math.round(6378137 * Math.acos(Math.cos(lat1) * Math.cos(lat2) * Math.cos(lng2 - lng1) + Math.sin(lat1) * Math.sin(lat2)));
}

function deg2rad(deg) {
  return deg * Math.PI / 180;
}

var cmdSelectTime = function (id, name, location, address) {
    $('#clinicId').val(id);
    getPossibleBookList();
    $(".jsBookHere").removeClass("btn-primary");
    $(".jsBookHere").addClass("btn-info");

    $("#cmdSelectTime" + id).removeClass("btn-info");
    $("#cmdSelectTime" + id).addClass("btn-primary");

    $('*[id*=divClinicTime]').each(function () {
        $(this).hide();
    });
    $('#divClinicTime_' + id).show();
    $('#cmdSelectDetails').removeAttr('disabled');
    $("#cmdSelectDetails").prop("disabled", false);
    $("#step4LocationName").text(name);
    $("#step4LocationAddress").text(address);
    SetModalBookingDate($("#bookDate" + id));
    var top = document.getElementById("jsClinic" + id).offsetTop - document.getElementById("ulClinicList").offsetTop; 
    $("#ulClinicList").scrollTop(top);
    $("#bookDate" + id).focus();
};

function getPossibleBookList() {
  var clinicId = $('#clinicId').val();
  var date = $('#bookDate' + clinicId).val();

  var isFirstTime = $('#bookDate' + clinicId).prop("firstTime") === undefined ? true : false;
  $('#bookDate' + clinicId).prop("firstTime", false);

  $.ajax({
    url: GetTimeListLink + "?clinic=" + clinicId + "&date=" + date + "&isFirstTime=" + isFirstTime,
    method: "GET",
    datatype: "json"
  }).done(function(data) {

    $('#bookTime' + clinicId).empty();
    if (data.list.length === 0) {
      $('#bookTime' + clinicId).attr("disabled", "");
      $("#cmdSelectDetails").attr("disabled", "");
      $("#cmdSelectDetails").prop("disabled", true);
      $('#bookTime' + clinicId).append($('<option>', {
        value: 0,
        text: "No time slots"
      }));
    } else {
      $('#bookTime' + clinicId).removeAttr("disabled");
      $("#cmdSelectDetails").removeAttr("disabled");
      $("#cmdSelectDetails").prop("disabled", false);
      var hasFreeSlot = false;
      $.each(data.list, function(i, item) {
        if (!hasFreeSlot) {
          hasFreeSlot = !item.IsBooked;
        }
        $('#bookTime' + clinicId).append($('<option>', {
          value: item.Name,
          text: item.Name + (item.IsBooked ? " (reserved)" : ""),
          disabled: item.IsBooked
        }));
      });
      if (!hasFreeSlot) {
        $("#cmdSelectDetails").attr("disabled", "");
        $("#cmdSelectDetails").prop("disabled", true);
        $('#bookTime' + clinicId).append($('<option>', {
          value: 0,
          text: "All slots reserved"
        }));
      }
      SetModalBookingTime($("#bookTime" + clinicId));
    }

    if (data.date) {
      $("#bookDate" + clinicId).val(data.date);
    }
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

function cmdShowOpeningHours(id, event) {
  $(".jsClinicHours").each(function(i, v) {
    if (v.id != "clinicHourrs_" + id) {
      $(v).hide();
    }
  });
  $("#clinicHourrs_" + id).toggle();
  event.stopPropagation();
}

function OnEnterDetails() {
  var clinicId = $('#clinicId').val();
  var isValid = $("#bookDate" + clinicId).val() != '' && $("#bookTime" + clinicId).prop("disabled");
  if ($("#cmdSelectDetails").prop("disabled") === false) {
    SetStep(3);
  }
}

var maps = [];
function initMap() {
  for (var i in maps) {
    var map = maps[i];
    if (map.lat == '' || map.lng == '') { // Search by address
      (function (_map) {
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({
          'address': map.address
        }, function(results, status) {
          if (status == google.maps.GeocoderStatus.OK) {
            var myOptions = {
              zoom: 16,
              center: results[0].geometry.location
            }
            var m = new google.maps.Map(document.getElementById(_map.id), myOptions);

            new google.maps.Marker({
              map: m,
              position: results[0].geometry.location
            });

            _map.lat = results[0].geometry.location.lat();
            _map.lng = results[0].geometry.location.lng();
          }
        });
      }).call(null, map);
    } else {
      var m = new google.maps.Map(document.getElementById(map.id), {
        center: { lat: parseFloat(map.lat), lng: parseFloat(map.lng) },
        zoom: 16
      });
      new google.maps.Marker({
        position: { lat: parseFloat(map.lat), lng: parseFloat(map.lng) },
        map: m,
        title: map.name
      });
    }
  }
}

var externalModalLogin;
function OpenModalExternalLogin(link) {
  var params = "menubar=no,location=no,resizable=no,scrollbars=no,status=no,height=480,width=400";
  externalModalLogin = window.open(link, "ExternalLogin", params);
}

function ValidateStep3() {
  $("#btnStep3").attr("disabled", true);
  if ($.trim($("#FirstName").val()) === '') {
    $("#FirstName").focus();
    return false;
  }

  if ($.trim($("#LastName").val()) === '') {
    $("#LastName").focus();
    return false;
  }

  if ($.trim($("#ClientEmail").val()) === '' || !validateEmail($("#ClientEmail").val())) {
    $("#ClientEmail").focus();
    return false;
  }

  if ($.trim($("#ClientPhone").val()) === '' && isNaN(data.PhoneNumber)) {
    $("#ClientPhone").focus();
    return false;
  }

  $("#btnStep3").removeAttr("disabled");
  return true;
}

function ButtonStep3Click() {
  if (ValidateStep3()) {
    SetStep(4);
  }
}

function FillStep4Info() {
  $("#step4FullName").text($("#FirstName").val() + ' ' + $("#LastName").val());
  $("#step4Email").text($("#ClientEmail").val());
  $("#step4Mobile").text($("#ClientPhone").val());
}

function SetModalBookingDate(obj) {
  $("#step4DateAndTime").prop("date", $(obj).val());
  $("#step4DateAndTime").text($("#step4DateAndTime").prop("date") + ' ' + $("#step4DateAndTime").prop("time"));
}

function SetModalBookingTime(obj) {
  $("#step4DateAndTime").prop("time", $(obj).val());
  $("#step4DateAndTime").text($("#step4DateAndTime").prop("date") + ' ' + $("#step4DateAndTime").prop("time"));
}

var uId;
function ExternalLoggedIn(userId) {
  externalModalLogin.close();
  $.ajax({
    url: GetUserInfoLink + "/" + userId,
    method: "GET",
    datatype: "json"
  }).done(function (data) {
    uId = userId;
    if (data.ClientData.FirstName) {
      $("#FirstName").val(data.ClientData.FirstName);
    }
    if (data.ClientData.LastName) {
      $("#LastName").val(data.ClientData.LastName);
    }
    $("#ClientEmail").val(data.Email);
    if (data.PhoneNumber) {
      $("#ClientPhone").val(data.PhoneNumber);
    }

    ValidateStep3();
    $(".bntExternalLogin").prop("disabled", true);
  });
}

var bookingObject;
var createdBookingId = 0;

function CreateBookingAsync() {
  SetStep(5);
  var clinicId = $('#clinicId').val();
  bookingObject = {
    'FirstName': $("#FirstName").val(),
    'LastName': $("#LastName").val(),
    'ClientEmail': $("#ClientEmail").val(),
    'ClientPhone': $("#ClientPhone").val(),
    'BookDate': $('#bookDate' + clinicId).val(),
    'BookTime':  $('#bookTime' + clinicId).val(),
    'ClinicId': clinicId,
    'TreatmentId': $('#treatmentId').val(),
    'UserId': uId,
    'Id': createdBookingId
  };
  $.ajax({
    url: SaveBookingAsyncLink,
    method: "POST",
    datatype: "json",
    data: bookingObject
  }).done(function(data) {
    if (data.success) {
      SetStep(6);
      createdBookingId = data.bookingId;
      CreditCardDialog();
    } else {
      alert(data.message);
    }
  });
}

function validateEmail(email) {
  var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  return re.test(email);
}

function OnSearchTreatment() {
  var val = $("#txtTreatmentSearch").val().toLowerCase();
  if (val == '') {
    $(".jsTreatments").show();
    return;
  }
  $(".jsTreatments").hide();
  $(".jsTreatmentName").each(function (i, v) {
    var text = $(v).text().toLowerCase();
    if (text.indexOf(val) !== -1) {
      var id = $(v).attr("data");
      $("#jsTreatment" + id).show();
    }
  });
}

function OnSearchClinic() {
  var val = $("#txtClinicSearch").val().toLowerCase();
  if (val == '') {
    $(".jsClinics").show();
    return;
  }
  $(".jsClinics").hide();
  $(".jsClinicName").each(function (i, v) {
    var text = $(v).text().toLowerCase();
    if (text.indexOf(val) !== -1) {
      var id = $(v).attr("data");
      $("#jsClinic" + id).show();
    }
  });
}

function CloseHours() {
  $(".jsClinicHours").hide();
}