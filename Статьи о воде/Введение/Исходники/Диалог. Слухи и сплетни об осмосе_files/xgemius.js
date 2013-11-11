// (c) 2000-2008 by Gemius SA

function gemius_parameters() {
	var d=document;
	var href=new String(d.location.href);
	var ref;
	if (d.referrer) { ref = new String(d.referrer); } else { ref = ""; }
	var t=typeof Error;
	if(t!='undefined') {
		eval("try { if (typeof(top.document.referrer)=='string') { ref = top.document.referrer } } catch(gemius_ex) { }")
	}
	var url='&tz='+(new Date()).getTimezoneOffset()+'&href='+escape(href.substring(0,299))+'&ref='+escape(ref.substring(0,299));
	if (screen) {
		var s=screen;
		if (s.width) url+='&screen='+s.width+'x'+s.height;
		if (s.colorDepth) url+='&col='+s.colorDepth;
	}
	return url;
}

function gemius_add_onload_event(obj,fn) {
	if (obj.attachEvent) { 
		obj.attachEvent("onload", fn);
	} else if(obj.addEventListener) {
		obj.addEventListener("load", fn, false);
	}

}

function gemius_append_script(xp_url) {
	if(typeof Error !='undefined') {
		eval("try { xp_javascript = document.createElement('script'); xp_javascript.src = xp_url; xp_javascript.type = 'text/javascript'; xp_javascript.defer = true; document.body.appendChild(xp_javascript); } catch(exception) { }");
	}
}

function gemius_obj_loaded() {
	window.pp_gemius_loaded+=1;
	if (window.pp_gemius_loaded==2 && window.pp_gemius_image.width && window.pp_gemius_image.width>1) {
		gemius_append_script(window.pp_gemius_script);
	}
}

if (typeof pp_gemius_identifier == 'undefined') {
	if (typeof gemius_identifier != 'undefined') {
		pp_gemius_identifier = gemius_identifier;
		gemius_identifier = 'USED_'+gemius_identifier;
	} else {
		pp_gemius_identifier = "";
	}
}

var gemius_proto;
if(document.location && document.location.protocol)
	gemius_proto = 'http'+((document.location.protocol=='https:')?'s':'')+':';
else
	gemius_proto = 'http:';
var pp_gemius_host = gemius_proto+'//ua.hit.gemius.pl/_'+(new Date()).getTime();

if (typeof window.pp_gemius_image != 'undefined') {
	if (typeof window.pp_gemius_images == 'undefined') {
	        window.pp_gemius_images = new Array();
	}
	var gemius_l = window.pp_gemius_images.length;
	window.pp_gemius_images[gemius_l]=new Image();
	window.pp_gemius_images[gemius_l].src = pp_gemius_host+'/redot.gif?id=ERR_'+pp_gemius_identifier.replace(/id=/,"id=ERR_")+gemius_parameters();
} else {
	window.pp_gemius_loaded = 0;
	window.pp_gemius_script = pp_gemius_host+'/pp.js?id='+pp_gemius_identifier;
	gemius_add_onload_event(window,gemius_obj_loaded);
	window.pp_gemius_image = new Image();
	gemius_add_onload_event(window.pp_gemius_image,gemius_obj_loaded);
	window.pp_gemius_image.src = pp_gemius_host+'/rexdot.gif?l=11&id='+pp_gemius_identifier+gemius_parameters();
}
pp_gemius_identifier = 'USED_'+pp_gemius_identifier;
