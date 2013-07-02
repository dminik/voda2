﻿/*
** nopCommerce custom js functions
*/

function setClickToCompareCheckBox(checkBoxId, urlToAdd, urlToRemove) {
    var checkBox = $('#' + checkBoxId);
    checkBox.click(function () {

        var url = checkBox.prop("checked") ? urlToAdd : urlToRemove;
        
        $.ajax({
            cache: false,
            type: 'POST',
            url: url,
            data: { productId: checkBox.val() },
            dataType: 'json',
            success: function(data) {
                
                checkBox.attr("selected", $(checkBox).prop("checked"));
                
                // increase compare product counter
                var counterSpan = $('.compare-list-count');
                var oldVal = parseInt(counterSpan.html());
                var newVal = checkBox.prop("checked") ? oldVal + 1 : oldVal - 1;
                $('.compare-list-count').html(newVal);                
            },
            error: function(xhr, ajaxOptions, thrownError) {
                alert(thrownError);
            }
        });
        return true;
    });
}

function setClickToCompareDeleteButton(buttonId, urlToRemove) {
    var button = $('#' + buttonId);   
        $.ajax({
            cache: false,
            type: 'POST',
            url: urlToRemove,
            data: { productId: button.prop("id") },
            dataType: 'json',
            success: function (data) {
                
                // delete product column
                $('td[product-id="' + buttonId + '"]').remove();
                
                // increase compare product counter
                var counterSpan = $('.compare-list-count');
                var oldVal = parseInt(counterSpan.html());
                var newVal = oldVal - 1;
                $('.compare-list-count').html(newVal);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError);
            }
        });            
}


function getE(name) {
    //Obsolete since nopCommerce 2.60. But still here for backwards compatibility (in case of some plugin developers used it in their plugins or customized solutions)
    if (document.getElementById)
        var elem = document.getElementById(name);
    else if (document.all)
        var elem = document.all[name];
    else if (document.layers)
        var elem = document.layers[name];
    return elem;
}

function OpenWindow(query, w, h, scroll) {
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;

    winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    var f = window.open(query, "_blank", winprops);
}

function setLocation(url) {
    window.location.href = url;
}

function displayAjaxLoading(display) {
    if (display) {
        $('.ajax-loading-block-window').show();
    }
    else {
        $('.ajax-loading-block-window').hide('slow');
    }
}

function displayPopupNotification(message, messagetype, modal) {
    //types: success, error
    var container;
    if (messagetype == 'success') {
        //success
        container = $('#dialog-notifications-success');
    }
    else if (messagetype == 'error') {
        //error
        container = $('#dialog-notifications-error');
    }
    else {
        //other
        container = $('#dialog-notifications-success');
    }

    //we do not encode displayed message
    var htmlcode = '';
    if ((typeof message) == 'string') {
        htmlcode = '<p>' + message + '</p>';
    } else {
        for (var i = 0; i < message.length; i++) {
            htmlcode = htmlcode + '<p>' + message[i] + '</p>';
        }
    }

    container.html(htmlcode);

    var isModal = (modal ? true : false);
    container.dialog({modal:isModal});
}


var barNotificationTimeout;
function displayBarNotification(message, messagetype, timeout) {
    clearTimeout(barNotificationTimeout);

    //types: success, error
    var cssclass = 'success';
    if (messagetype == 'success') {
        cssclass = 'success';
    }
    else if (messagetype == 'error') {
        cssclass = 'error';
    }
    //remove previous CSS classes and notifications
    $('#bar-notification')
        .removeClass('success')
        .removeClass('error');
    $('#bar-notification .content').remove();

    //we do not encode displayed message

    //add new notifications
    var htmlcode = '';
    if ((typeof message) == 'string') {
        htmlcode = '<p class="content">' + message + '</p>';
    } else {
        for (var i = 0; i < message.length; i++) {
            htmlcode = htmlcode + '<p class="content">' + message[i] + '</p>';
        }
    }
    $('#bar-notification').append(htmlcode)
        .addClass(cssclass)
        .fadeIn('slow')
        .mouseenter(function ()
            {
                clearTimeout(barNotificationTimeout);
            });

    $('#bar-notification .close').unbind('click').click(function () {
        $('#bar-notification').fadeOut('slow');
    });

    //timeout (if set)
    if (timeout > 0) {
        barNotificationTimeout = setTimeout(function () {
            $('#bar-notification').fadeOut('slow');
        }, timeout);
    }
}

function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}