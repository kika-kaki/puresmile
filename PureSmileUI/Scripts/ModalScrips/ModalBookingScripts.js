var payButtonClicked = false;
$.showprogress();
$(document).ready(function () {
    $.hideprogress();
})

function initializePayments() {
    
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = 'https://js.braintreegateway.com/web/3.3.0/js/client.min.js';
    document.getElementsByTagName('head')[0].appendChild(script);

    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = 'https://js.braintreegateway.com/web/3.3.0/js/paypal.min.js';
    document.getElementsByTagName('head')[0].appendChild(script);

    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = 'https://js.braintreegateway.com/web/3.3.0/js/hosted-fields.js';

    script.onload = function () {

        // Fetch the button you are using to initiate the PayPal flow
        var paypalButton = document.getElementById('paypal-button');
        // Create a Client component
        braintree.client.create({
            authorization: AUTH_TOKEN
        }, function (clientErr, clientInstance) {
            if (clientErr != null) {
                ErrorLogger.LogError(clientErr);
            }
            braintree.paypal.create({
                client: clientInstance
            }, function (err, paypalInstance) {
                if (err != null) {
                    ErrorLogger.LogError(err);
                }
                paypalButton.addEventListener('click', function () {
                    if (payButtonClicked) return;
                    if (!checkCustomerAgreeToTerms()) return;
                    payButtonClicked = true;
                    paypalInstance.tokenize({
                        flow: 'checkout', // Required
                        amount: paymentAmount, // Required
                        currency: 'USD', // Required
                        locale: 'en_US',
                        enableShippingAddress: false
                    }, function (err, tokenizationPayload) {
                        if (err != null) {
                            ErrorLogger.LogError(err);
                            payButtonClicked = false;
                            return;
                        } else {
                            $.ajax({
                                url: PayAsyncLink + "?payload=" + tokenizationPayload.nonce + "&bookingId=" + createdBookingId,
                                method: "GET",
                                datatype: "json"
                            }).done(function (data) {
                                payButtonClicked = false;
                                if (data.success) {
                                    SetStep(7);
                                } else {
                                    alert("Error occured while processing your payment. Please try again later");
                                }
                            });
                        }
                    });
                });
            });
        });



    }

    document.getElementsByTagName('head')[0].appendChild(script);




}


function setErrorMsg(err) {
    if (!$('.cvv-div').hasClass('reset-bottom-margin')) $('.cvv-div').addClass('reset-bottom-margin');
    $('.payment-errors').text(err);
}

function resetMarginonSuccessValidation() {
    $('.payment-errors').text('');
    $('.cvv-div').removeClass('reset-bottom-margin');
}

function checkCustomerAgreeToTerms() {
    if (!$('#terms').prop('checked')) {
        if (!$('.cvv-div').hasClass('reset-bottom-margin')) $('.cvv-div').addClass('reset-bottom-margin');
        setErrorMsg('Please indicate that you agree with Pure Smile’s terms and conditions before payment');
        return false;
    }

    resetMarginonSuccessValidation();
    return true;
}

$(function () {

    $('.input-date').each(function () {
        $(this).datepicker({
            format: 'yyyy-mm-dd',
            onRender: function (date) {
                var todaydate = new Date();
                var today = new Date(todaydate.getFullYear(), todaydate.getMonth(), todaydate.getDate());
                return date.valueOf() < today.valueOf() ? 'disabled' : '';
            }
        }).on('changeDate', function () {
            getPossibleBookList();
            SetModalBookingDate(this);
        });

     
    });
});

var cardInitialized = false;

function CreditCardDialog() {
    cardInitialized = true;
    braintree.client.create({
        authorization: AUTH_TOKEN
    }, function (err, clientInstance) {
        if (err) {
            ErrorLogger.LogError(err);
            return;
        }

        braintree.hostedFields.create({
            client: clientInstance,
            styles: {
                'input': {
                    'font-size': '14px',
                    'font-family': 'helvetica, tahoma, calibri, sans-serif',
                    'color': '#3a3a3a'
                },
                ':focus': {
                    'color': 'black'
                }
            },
            fields: {
                number: {
                    selector: '#card-number',
                    placeholder: '4111 1111 1111 1111'
                },
                cvv: {
                    selector: '#cvv',
                    placeholder: '123'
                },
                expirationMonth: {
                    selector: '#expiration-month',
                    placeholder: 'MM'
                },
                expirationYear: {
                    selector: '#expiration-year',
                    placeholder: 'YY'
                }
            }
        }, function (err, hostedFieldsInstance) {
            if (err) {
                ErrorLogger.LogError(err);
                return;
            }

            hostedFieldsInstance.on('validityChange', function (event) {
                var field = event.fields[event.emittedBy];

                if (field.isValid) {
                    if (event.emittedBy === 'expirationMonth' || event.emittedBy === 'expirationYear') {
                        if (!event.fields.expirationMonth.isValid || !event.fields.expirationYear.isValid) {
                            return;
                        }
                    } else if (event.emittedBy === 'number') {
                        $('#card-number').next('span').text('');
                    }

                    // Apply styling for a valid field
                    $(field.container).parents('.form-group').addClass('has-success');
                } else if (field.isPotentiallyValid) {
                    // Remove styling  from potentially valid fields
                    $(field.container).parents('.form-group').removeClass('has-warning');
                    $(field.container).parents('.form-group').removeClass('has-success');
                    if (event.emittedBy === 'number') {
                        $('#card-number').next('span').text('');
                    }
                } else {
                    // Add styling to invalid fields
                    $(field.container).parents('.form-group').addClass('has-warning');
                    // Add helper text for an invalid card number
                    if (event.emittedBy === 'number') {
                        $('#card-number').next('span').text('Looks like this card number has an error.');
                    }
                }
            });

            hostedFieldsInstance.on('cardTypeChange', function (event) {
                // Handle a field's change, such as a change in validity or credit card type
                if (event.cards.length === 1) {
                    $('#card-type').text(event.cards[0].niceType);
                } else {
                    $('#card-type').text('Card');
                }
            });

            $('#btnSubmit').bind("click", function (event) {

                event.preventDefault();
                if (!payButtonClicked) {
                    payButtonClicked = true;
                    hostedFieldsInstance.tokenize(function (err, payload) {
                        if (err) {
                            ErrorLogger.LogError(err);
                            if (!$('.cvv-div').hasClass('reset-bottom-margin')) $('.cvv-div').addClass('reset-bottom-margin');

                            if (err.code == 'HOSTED_FIELDS_FIELDS_INVALID')
                                $('.payment-errors').text('Credit card information incorrect or incomplete - please check entered fields');

                            if (err.code == 'HOSTED_FIELDS_FIELDS_EMPTY')
                                $('.payment-errors').text('Plese enter your credit card information');
                            payButtonClicked = false;
                            return;
                        }

                        if (!checkCustomerAgreeToTerms()) { payButtonClicked = false; return; }

                        $('.payment-errors').text('');
                        $('.cvv-div').removeClass('reset-bottom-margin');

                        // send payload to server async
                        $.ajax({
                            url: PayAsyncLink + "?payload=" + payload.nonce + "&bookingId=" + createdBookingId,
                            method: "GET",
                            datatype: "json"
                        }).done(function (data) {
                            payButtonClicked = false;
                            if (data.success) {
                                SetStep(7);
                            } else {
                                alert("Error occured while processing your payment. Please try again later");
                                SetStep(6);
                            }
                        });
                    });
                }
            });
        });
    });
}

function ShowError(text) {
    $("#dlgErrorModal").text(text);
    $("#dlgErrorModal").dialog(
      {
          title: "Error",
          modal: true,
          closeOnEscape: true,
          position: {
              my: "center",
              at: "center",
              of: ".matter"
          }
      }
    );
}