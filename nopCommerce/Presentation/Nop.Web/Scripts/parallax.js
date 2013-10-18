/* Scroll the background layers */
function parallaxScroll(){
	var scrolled = $(window).scrollTop();
	$('#parallax-bg1-parallax').css('top', (0 - (scrolled * .15)) + 'px');
}

$(document).ready(function() {
	
	/* Scroll event handler */
    $(window).bind('scroll',function(e){
    	parallaxScroll();		
    });       
});


