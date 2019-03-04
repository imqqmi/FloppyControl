( function( $ ) {
	$( document ).on( "click",  '.cptch_reload_button', function() {
		var captcha = $( this ).parent().parent( '.cptch_wrap' );
		if ( captcha.length && ! $( this ).hasClass( 'cptch_active' ) ) {
			$( this ).addClass( 'cptch_active' );
			$.ajax({
				type: 'POST',
				url: cptch_vars.ajaxurl,
				data: {
					action:      'cptch_reload',
					cptch_nonce: cptch_vars.nonce
				},
				success: function( result ) {
					captcha.parent().find( '.cptch_to_remove' ).remove();
					captcha.replaceWith( result );
				},
				error : function ( xhr, ajaxOptions, thrownError ) {
					alert( xhr.status );
					alert( thrownError );
				}
			});
		}
	}).on( "touchstart", function( event ) {
		if ( cptch_vars.enlarge == '1' ) {
			var element = $( event.target );
			if ( element.hasClass( 'cptch_img' ) ) {
				event.preventDefault();
				element.toggleClass( 'cptch_reduce' );
				$( '.cptch_img' ).not( element ).removeClass( 'cptch_reduce' );
			} else {
				$( '.cptch_img' ).removeClass( 'cptch_reduce' );
			}
		}
	});
})(jQuery);