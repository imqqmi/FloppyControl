$(function () {
    $('#web-subscribe button').click(function () {
        function shake () {
            for (var i = 0; i < 3; i++) {
                $('#web-subscribe-body').animate( {
                    'margin-left': '-=15'
                }, 50);
                $('#web-subscribe-body').animate( {
                    'margin-left': '+=15'
                }, 50);
            }
        }

        var email = $('#web-subscribe input[name="email"]').val();

        email = email.replace(/^\s+/, '').replace(/\s+$/, '');
        if (email.length == 0) {
            shake();
            return;
        }

        if (!/.+@.+\..+/.test(email)) {
            shake();
            return;
        }

        $.post('/api/web-subscribe', {
            email : email
        }, function (data) {
            $('#web-subscribe-body').hide();
            $('#web-subscribe-title').text("Thanks for subscribing to our updates!");
            $('#web-subscribe-extra-message').text(data);
            $('#web-subscribe-extra-message').fadeIn();
        });
    });

    $('#web-subscribe input').keypress(function(ev) {
        if(ev.which == 13) {
            $('#web-subscribe button').click();
        }
    });

    $('#web-subscribe input').focus(function (ev) {
        $.post('/api/ui-event', {
            event : 'web-email-subscribe-focus'
        });
    });

    try {
        console.log(
            "%c "+"Lbh'ir sbhaq n frperg! Hfr pbhcba pbqr PBAFBYRUNPXRE gb trg n qvfpbhag!".replace(/[a-zA-Z]/g,function(c){return String.fromCharCode((c<="Z"?90:122)>=(c=c.charCodeAt(0)+13)?c:c-26);}),  'color: rgb(180,180,180)'
        );
    }
    catch (err) {}
});
