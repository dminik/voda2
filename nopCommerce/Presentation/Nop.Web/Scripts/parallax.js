/* Scroll the background layers */
function parallaxScroll(){
    var scrolled = $(window).scrollTop();
    var x = (0 - (scrolled * .15));

    $('#parallax-bg').css('top', x + 'px');	
}

$(document).ready(function() {
	
	/* Scroll event handler */
    $(window).bind('scroll',function(e){
    	parallaxScroll();		
    });       
});


