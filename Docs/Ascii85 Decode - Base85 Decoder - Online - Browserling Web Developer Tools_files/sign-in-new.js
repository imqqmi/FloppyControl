$(function () {
    // Make sign in link clickable
    $('#sign-in').click(function (ev) {
        ev.preventDefault();

        $.post('/api/ui-event', {
            event : 'sign-in-click'
        });

        if ($('#menu-sign-in').is(':visible')) {
            $('#sign-in').removeClass('active');
        }
        else {
            $('#sign-in').addClass('active');
        }

        $('#menu-sign-in-sign-up').css({
            top : $('#top-nav').position().top + $('#top-nav').height() + 2,
            left : $('#top-links').position().left +
                $('#top-links').width() -
                $('#menu-sign-in-sign-up').width()
        });

        $(window).resize(function () {
            $('#menu-sign-in-sign-up').css({
                top : $('#top-nav').position().top + $('#top-nav').height() + 2,
                left : $('#top-links').position().left +
                    $('#top-links').width() -
                    $('#menu-sign-in-sign-up').width()
            });
        })

        $('#menu-sign-in').slideToggle();
        $('#menu-sign-in input[name="email"]').focus();
        $('#menu-create-account').hide();
        $('#create-account').removeClass('active');
        $('#create-account').addClass('blue');

        if ($('#menu-forgot-password').is(':visible')) {
            $('#menu-forgot-password').slideToggle();
        }
    });

    // Make create account link clickable
    $('#create-account').click(function (ev) {
        ev.preventDefault();

        $.post('/api/ui-event', {
            event : 'create-account-click'
        });

        if ($('#menu-create-account').is(':visible')) {
            $('#create-account').removeClass('active');
            $('#create-account').addClass('blue');
        }
        else {
            $('#create-account').addClass('active');
            $('#create-account').removeClass('blue');
        }

        $('#menu-sign-in-sign-up').css({
            top : $('#top-nav').position().top + $('#top-nav').height() + 2,
            left : $('#top-links').position().left +
                $('#top-links').width() -
                $('#menu-sign-in-sign-up').width()
        });

        $(window).resize(function () {
            $('#menu-sign-in-sign-up').css({
                top : $('#top-nav').position().top + $('#top-nav').height() + 2,
                left : $('#top-links').position().left +
                    $('#top-links').width() -
                    $('#menu-sign-in-sign-up').width()
            });
        })

        $('#menu-create-account').slideToggle("slow");
        $('#menu-create-account input[name="email"]').focus();
        $('#menu-sign-in').hide();
        $('#sign-in').removeClass('active');

        if ($('#menu-forgot-password').is(':visible')) {
            $('#menu-forgot-password').slideToggle();
        }
    });

    // Make sign in form work
    $('#menu-sign-in input[type="submit"]').click(function (ev) {
        ev.preventDefault();

        var email = $('#menu-sign-in input[name="email"]').val();
        var pass = $('#menu-sign-in input[name="password"]').val();

        email = email.replace(/^\s+/, '').replace(/\s+$/, '');
        if (email.length == 0) {
            $('#menu-error-sign-in').text("Empty email");
            $('#menu-error-sign-in').show();
            return;
        }

        if (!/.+@.+\..+/.test(email)) {
            $('#menu-error-sign-in').text("Invalid email");
            $('#menu-error-sign-in').show();
            return;
        }

        if (pass.length == 0) {
            $('#menu-error-sign-in').text("Empty password");
            $('#menu-error-sign-in').show();
            return;
        }

        $('#menu-error-sign-in').hide();

        $.post('/api/user/login', {
            email : email,
            password : pass
        }, function (data) {
            if (/^error/i.test(data)) {
                $('#menu-error-sign-in').text(data);
                $('#menu-error-sign-in').show();
            }
            else {
                window.location.reload();
            }
        });
    });

    // Make create account form work
    $('#menu-create-account input[type="submit"]').click(function (ev) {
        ev.preventDefault();

        var email = $('#menu-create-account input[name="email"]').val();
        var pass1 = $('#menu-create-account input[name="password1"]').val();
        var pass2 = $('#menu-create-account input[name="password2"]').val();

        email = email.replace(/^\s+/, '').replace(/\s+$/, '');
        if (email.length == 0) {
            $('#menu-error-create-account').text("Empty email");
            $('#menu-error-create-account').show();
            return;
        }

        if (!/.+@.+\..+/.test(email)) {
            $('#menu-error-create-account').text("Invalid email");
            $('#menu-error-create-account').show();
            return;
        }

        if (pass1.length == 0) {
            $('#menu-error-create-account').text("Empty password");
            $('#menu-error-create-account').show();
            return;
        }

        if (pass2.length == 0) {
            $('#menu-error-create-account').text("Empty confirmation password");
            $('#menu-error-create-account').show();
            return;
        }

        if (pass1 != pass2) {
            $('#menu-error-create-account').text("Passwords don't match");
            $('#menu-error-create-account').show();
            return;
        }

        $('#menu-error-create-account').hide();

        $.post('/api/user/register', {
            email : email,
            password : pass1
        }, function (data) {
            if (/^error/i.test(data)) {
                $('#menu-error-create-account').text(data);
                $('#menu-error-create-account').show();
            }
            else {
                window.location.reload();
            }
        });
    });

    // make forgot password menu work
    $('#forgot-password a').click(function (ev) {
        ev.preventDefault();

        $.post('/api/ui-event', {
            event : 'forgot-password-click'
        });

        $('#menu-sign-in').slideToggle();
        $('#menu-forgot-password').slideToggle();
        $('#menu-forgot-password input[name="email"]').focus();
    });

    $('#forgot-password-back a').click(function (ev) {
        ev.preventDefault();
        $('#menu-sign-in').slideToggle();
        $('#menu-forgot-password').slideToggle();
        $('#menu-sign-in input[name="email"]').focus();
    })

    // make forgot password form work
    $('#menu-forgot-password input[type="submit"]').click(function (ev) {
        ev.preventDefault();

        $('#menu-error-forgot-password').hide();

        var email = $('#menu-forgot-password input[name="email"]').val();

        email = email.replace(/^\s+/, '').replace(/\s+$/, '');
        if (email.length == 0) {
            $('#menu-error-forgot-password').text("Empty email");
            $('#menu-error-forgot-password').show();
            return;
        }

        if (!/.+@.+\..+/.test(email)) {
            $('#menu-error-forgot-password').text("Invalid email");
            $('#menu-error-forgot-password').show();
            return;
        }

        $(this).prop('disabled', true);
        $.post('/api/user/forgot-password', {
            email : email
        }, function (data) {
            if (/^error/i.test(data)) {
                $(this).prop('disabled', false);
                $('#menu-error-forgot-password').text(data);
                $('#menu-error-forgot-password').show();
            }
            else {
                $('#menu-ok-forgot-password').show();
            }
        });
    })
});
