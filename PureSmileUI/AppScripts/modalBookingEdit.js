var clinicsHidden = false;

function setMainButtonsPosition() {

    if ($(document).width() >= 768) {

        var confirmBtnPos = $('.btn-confirm-div').position();
        var modalDialog = $('.base-modal').position();
        var modalDialogOffset = $('.base-modal').offset();
        $('.btn-confirm-div').offset({ top: $('.modal-content').height() - 33 + modalDialog.top });
    }

}

function hideClinicsExcept(clinic)
{
    $('.jsClinics').each(function () { if ($(this)[0] != clinic) $(this).hide(); });
    clinicsHidden = true;
}

function showAllClinics() {
    $('.jsClinics').show();
    clinicsHidden = false;
}

$(function () {
    $('.dtpicker').datetimepicker({
        pickTime: false
    })
      .on('changeDate', function (e) {
          getPossibleBookList();
          $(this).datetimepicker('hide');
          SetModalBookingDate($(this).find("input"));
      });

    $('.modal').appendTo($('body'));
    SetStep(0);

    // buttons fixed position for PC
    $(document).on('shown.bs.modal', '#treatmentModal', function () {
        setMainButtonsPosition();

    })

    // hide close button in some cases
    if (document.location.href.indexOf('hideclose') != -1)
        $('.redirect-btn').hide();


    var ismobile = navigator.userAgent.match(/(iPad)|(iPhone)|(iPod)|(android)|(webOS)/i);
    if (ismobile)
        $('.datepicker.dropdown-menu').hide();

    $('#cmdSelectTreatment').click();
});

//window.onresize = function () {
//    setMainButtonsPosition();
//};
var allowReservNotification;
var allowNotification = function(value) {
    return  allowReservNotification = value;
}
var paymentAmount = 0;
var cmdSelectClinic = function (obj, id, name, description, price) {
    $('#treatmentId').val(id);
    //$(".jsBookBtn").removeClass("active-btn");
    //$(obj).addClass("active-btn");
    //$("#btnSelectDateLocations").removeAttr("disabled");
    $("#step4TreatmentTitle").text(name);
    $("#step4TreatmentPrice").text('$' + price);
    $("#step4TreatmentDescription").text(description);
    $("#step4TotalPrice").text('$' + price);
    paymentAmount = price;
};

var cmdSelectTreatmentCategory = function (obj, id, name, description, price) {
    $('#treatmentCategoryId').val(id);
    //$(".jsSelectTretmentBtn").removeClass("active-btn");
    //$(obj).addClass("active-btn");
    //$("#btnSelectThreatmentCategory").removeAttr("disabled");
};

var OnSelectDateAndLocation = function () {
    if ($('#treatmentId').val() === '' || $('#treatmentId').val() == 0) {
        return;
    }

    SetStep(2);
    initMap();
    filterClinicsByCategory();
}

var btnSelectTreatmentCategory = function () {

    if ($('#treatmentCategoryId').val() === '' || $('#treatmentCategoryId').val() == 0) {
        return;
    }

    if (typeof braintree == "undefined")
        initializePayments();

    SetStep(1);
    filterTreatmentsByCategory();
}


var SetStep = function (step) {
    $('.event-message').hide();
    $(".step").hide();
    $("#bookingStep" + step).show();

    if (step == 0) {
        $('#txtTreatmentSearch').val('');
        OnSearchTreatment();
    }

    if (step == 1)
    {
        $('#txtClinicSearch').val('');
        OnSearchClinic();

    }
    /*if (step == 3) {
        ValidateStep3();
    }*/
    if (step == 4) {
        FillStep4Info();
    }
}

var position;
var ToggleLocationsShow = function (obj) {
    $('.jsDateSelect').hide();
    $(".jsBookHere").removeClass("active-btn");
    $(obj).find("a").toggle();
    var showClosest = $("#cbxShowClosest").val();
    showClosest = showClosest === "false" ? "true" : "false";
    $("#cbxShowClosest").val(showClosest);
    if (showClosest === "true") {
        if (position) {
            SelectByPosition();
        } else {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (pos) {
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
    $('.event-message').hide();
    $('#clinicId').val(id);
    getPossibleBookList();
    $(".jsBookHere").removeClass("active-btn");

    $("#cmdSelectTime" + id).addClass("active-btn");

    $('*[id*=divClinicTime]').each(function () {
        $(this).hide();
    });
    $('#divClinicTime_' + id).show();
    //disableConfirm();
    //enableConfirm();

    $("#step4LocationName").text(name);
    $("#step4LocationAddress").text(address);
    SetModalBookingDate($("#bookDate" + id));

    var top = document.getElementById("jsClinic" + id).offsetTop - document.getElementById("ulClinicList").offsetTop;
    $("#ulClinicList").scrollTop(top);
};

function getPossibleBookList() {
    $.showprogress();
    $('.spinner').offset({ top: 300 });

    var clinicId = $('#clinicId').val();
    var date = $('#bookDate' + clinicId).val();
    const bookTimeElm = $('#bookTime' + clinicId);
    const bookDateElm = $('#bookDate' + clinicId);
    var isFirstTime = bookDateElm.prop("firstTime") === undefined ? true : false;
    var hasFreeSlot = false;

    disableConfirm();
    
    $('#bookDate' + clinicId).prop("firstTime", false);
    $.ajax({
        url: GetTimeListLink + "?clinic=" + clinicId + "&date=" + date + "&isFirstTime=" + isFirstTime,
        method: "GET",
        datatype: "json"
    }).done(function (data) {
        bookTimeElm.empty();

        if (data.list.length) {
            const countOfFreeSlots = data.list.filter(i => { return !i.IsBooked; }).length;
            hasFreeSlot = (countOfFreeSlots > 0);
        }

        if (hasFreeSlot) {
            $.each(data.list, function (i, item) {
                if (!item.IsBooked) {
                    bookTimeElm.append($('<option>', {
                        value: item.Name,
                        text: item.Name + (item.IsBooked ? " (reserved)" : ""),
                        disabled: item.IsBooked
                    }));
                }
            });
            bookTimeElm.removeAttr("disabled");
            SetModalBookingTime(bookTimeElm);
            enableConfirm();
        } else {
            bookTimeElm.attr("disabled", "");
            bookTimeElm.append($('<option>', {
                value: 0,
                text: "All slots reserved"
            }));
        }
            
        $('.time-select').selectpicker('render');
        $('.time-select').selectpicker('refresh');

        if (data.date) {
            bookDateElm.val(data.date);
        }
        $.hideprogress();
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
    $(".jsClinicHours").each(function (i, v) {
        if (v.id != "clinicHourrs_" + id) {
            $(v).hide();
        }
    });
    $("#clinicHourrs_" + id).toggle();
    event.stopPropagation();
}

function OnEnterDetails() {
    var clinicId = $('#clinicId').val();
    var treatmentId = $('#treatmentId').val();
    var dateStr = $("#step4DateAndTime").prop("date");
    var time = $("#bookTime" + clinicId).val();
    var isValid = $("#bookDate" + clinicId).val() != '' && $("#bookTime" + clinicId).prop("disabled");

    if ($("#cmdSelectDetails").prop("disabled") === false) {
        $.ajax({
        type: "POST",
        url: "/calendar/CreateBlock",
        data: {
            date:dateStr, 
            time: time,
            //dateTime: dateTime["0"].textContent,
            clinicId: clinicId,
            treatmentId:treatmentId
        },
        success:function(resp) {
            console.log(resp);

        },
    });
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
                }, function (results, status) {
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

function filterClinicsByCategory() {
    var treatmentCategory = getSelectedTreatmentCategory();
    if (treatmentCategory) {
        $('.jsClinics').each(function () {
            var cats = $(this).attr('data-treatment-categories');
            cats = JSON.parse(cats);
            if (cats.indexOf(treatmentCategory) < 0) {
                $(this).addClass('hidden');
            }
            else {
                $(this).removeClass('hidden');
            }

        })
    }
}

function filterTreatmentsByCategory() {
    var treatmentCategory = $('#treatmentCategoryId').val();
    if (treatmentCategory) {
        $('.jsTreatments').each(function () {

            var category = $(this).attr('data-treatment-category')

            if (category != treatmentCategory) {
                $(this).addClass('hidden');
            }
            else {
                $(this).removeClass('hidden');
            }
        })
    }
}

function getSelectedTreatmentCategory() {
    return parseInt($("#jsTreatment" + $("#treatmentId").val()).attr("data-treatment-category"), 10);
}

function ValidateStep3(elm) {
    var isValid = true;
    $("#btnStep3").attr("disabled", true);
    if (elm) {
        switch (elm.id) {
            case "ClientEmail":
                var errorField = $('.ClientEmailErr');
                if (!validateEmail($(elm).val())) {
                    errorField.text('Enter valid E-mail');
                    isValid = false;
                } else {
                    errorField.text('');
                }
                break;
            default:
                var errorField = $('.' + elm.id + 'Err');
                if ($.trim($(elm).val()) === '') {
                    errorField.text('Required');
                    isValid = false;
                } else {
                    errorField.text('');
                }
                break;
        }
    }
    

    
    /*if ($.trim($("#FirstName").val()) === '') {
        $('.FirstNameErr').text('Required');
        isValid = false;
    } else $('.FirstNameErr').text('');

    if ($.trim($("#LastName").val()) === '') {
        $('.LastNameErr').text('Required');
        isValid = false;
    } else $('.LastNameErr').text('');

    if ($.trim($("#ClientEmail").val()) === '') {
        $('.ClientEmailErr').text('Required');
        isValid = false;
    } else $('.ClientEmailErr').text('');

    if (!validateEmail($("#ClientEmail").val())) {
        $('.ClientEmailErr').text('Enter valid E-mail');
        isValid = false;
    } else $('.ClientEmailErr').text('');

    if ($.trim($("#ClientPhone").val()) === '') {
        $('.ClientPhoneErr').text('Required');
        isValid = false;
    } else $('.ClientPhoneErr').text('');
    */
    if (isValid) {
        $("#btnStep3").removeAttr("disabled");
        return true;
    }
    return isValid;
}

function ButtonStep3Click() {
    if (ValidateStep3()) {
        SetStep(4);
    }
}

function enableConfirm() {
    var confirmBtn = $("#cmdSelectDetails");
    confirmBtn.parent().show();
    confirmBtn.removeAttr("disabled");
    confirmBtn.prop("disabled", false);
}

function disableConfirm() {
    var confirmBtn = $("#cmdSelectDetails");
    confirmBtn.parent().hide();
    confirmBtn.attr("disabled", "");
    confirmBtn.prop("disabled", true);
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
function restoreTimeMessage(clinicId) {
    $('.timeError' + clinicId).hide();
    $('.timeWarning' + clinicId).hide();
    $('.timeSuccess' + clinicId).hide();
    $('.timeFatalError' + clinicId).hide();
}

function SetModalBookingTime(obj) {
    $("#step4DateAndTime").prop("time", $(obj).val());
    var dateTime = $("#step4DateAndTime").text($("#step4DateAndTime").prop("date") + ' ' + $("#step4DateAndTime").prop("time"));
    $('.event-message').hide();

    var treatmentId = $('#treatmentId').val();
    var clinicId = $('#clinicId').val();
    var timeStr = obj.value;
    if (timeStr != null || timeStr != undefined) {
       
        $.ajax({
            method: "POST",
            url: getEventUrl,
            data: {
                dateTime:dateTime["0"].textContent,
                treatmentId: treatmentId,
                clinicId: clinicId
            }, beforeSend: restoreTimeMessage(clinicId)
        })
            
            .success(function (response) {
                $.hideprogress();
                enableConfirm();
                switch (response.StatusId) {
                    case 1:
                    {
                        $('#timeSuccess_' + clinicId).show();
                        
                    }break;
                    case 2: {
                        disableConfirm();
                        $('#timeError_' + clinicId).show();
                        }
                        break;
                    case 3: {
                        $('#timeWarning_' + clinicId).show();
                    } break;
                    //case 4: {
                    //    $('#timeFatalError_' + clinicId).show();
                    //} break;
                    
                    default:
                    {
                        restoreTimeMessage(clinicId);
                    }
                }
            });
    }
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
    SendBookingNotification();
    CreateCalendarEvent();
    var clinicId = $('#clinicId').val();
    bookingObject = {
        'FirstName': $("#FirstName").val(),
        'LastName': $("#LastName").val(),
        'ClientEmail': $("#ClientEmail").val(),
        'ClientPhone': $("#ClientPhone").val(),
        'BookDate': $('#bookDate' + clinicId).val(),
        'BookTime': $('#bookTime' + clinicId).val(),
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
    }).done(function (data) {
        if (data.success) {
            SetStep(6);
            createdBookingId = data.bookingId;
            CreditCardDialog();
        } else {
            alert(data.message);
        }
    });
}

function CreateCalendarEvent() {
    var firstName = $("#FirstName").val();
    var lastName = $("#LastName").val();
    var email = $("#ClientEmail").val();
    var treatmentId = $('#treatmentId').val();
    var dateStr = $("#step4DateAndTime").prop("date");
    var time = $("#bookTime5").val();
    var clinicId = $('#clinicId').val();
    $.ajax({
        type: "POST",
        url: "/calendar/CreateEvent",
        data: {
            clinicId: clinicId,
            treatmentId: treatmentId,
            dateTime: dateStr + " " + time,
            firstName: firstName,
            lastName: lastName,
            email: email,
            //time: time
        },
    });
}
function SendBookingNotification() {
    var fName = $("#FirstName").val();
    var lName = $("#LastName").val();
    var email = $("#ClientEmail").val();
    var treatmentId = $('#treatmentId').val();
    var dateStr = $("#step4DateAndTime").prop("date");
    var time = $("#bookTime5").val();
    var clinicId = $('#clinicId').val();

    $.ajax({
        type: "POST",
        url: "/email/SendBookingNotification",
        data: {
            name: fName,
            lastName: lName,
            email: email,
            clinicId: clinicId,
            treatmentId: treatmentId,
            date: dateStr,
            time: time
        },
        success: function (resp) {
            //console.log(resp.Status);
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
        $(".jsTreatmentName").each(function (i, v) {

            var id = $(v).attr("data");
            $("#jsTreatment" + id).show();

        });

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

        $(".jsClinicName").each(function (i, v) {
            var id = $(v).attr("data");
            $("#jsClinic" + id).show();

            return;
        });
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

// Сделано чтобы в случае я ссылки изменить только тут
function RedirectToPureSmile() {
    window.location = 'https://www.puresmile.com.au';
}function test() {
    console.log("selected!");
}