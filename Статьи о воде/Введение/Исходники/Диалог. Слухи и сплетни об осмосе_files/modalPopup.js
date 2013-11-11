function ModalPopup(popupId, options) {
	//option default settings
	options = options || {};
	this.hasBackground = (options.hasBackground != null) ? options.hasBackground : true;
	this.backgroundColor = options.backgroundColor || '#000000';
	this.backgroundOpacity = options.backgroundOpacity || 60; // 1-100
	this.backgroundOpacity = (this.backgroundOpacity > 0) ? this.backgroundOpacity : 1;	
	this.zIndex =  options.zIndex || 90000;
	this.addLeft = options.AddLeft || 0; //in px
	this.addTop = options.AddTop || 0; //in px 
	this.popup = document.getElementById(popupId);	
	if (!this.popup) {return;}
}

ModalPopup.prototype.show = function() {
	//display the popup layer
	this.popup.style.display = "block";
	this.popup.style.visibility = "visible";	
	this.setPosition();
	if (this.hasBackground) {		
		if (!this._BackgroundDiv) {
			this._BackgroundDiv = document.createElement('div');
			this._BackgroundDiv.style.display = "none";
			this._BackgroundDiv.style.width = "100%";
			this._BackgroundDiv.style.height = "100%";
			this._BackgroundDiv.style.position = "fixed";
			this._BackgroundDiv.style.top = "0px";
			this._BackgroundDiv.style.left = "0px";
			document.body.appendChild(this._BackgroundDiv);
		}
		this._BackgroundDiv.style.background = this.backgroundColor;	
		this._BackgroundDiv.style.height = "100%";
		this._BackgroundDiv.style.filter = "progid:DXImageTransform.Microsoft.Alpha(opacity=" + this.backgroundOpacity +")";
		this._BackgroundDiv.style.MozOpacity = this.backgroundOpacity / 100;
		this._BackgroundDiv.style.opacity = this.backgroundOpacity / 100;
		this._BackgroundDiv.style.zIndex = this.zIndex; 
		//Display the background
		this._BackgroundDiv.style.display = "";
	}	
}

ModalPopup.prototype.hide = function() {
	if (this.popup) {
		this.popup.style.display = "none";
		this.popup.style.visibility = "hidden";
	} 
	if (this._BackgroundDiv) {
		this._BackgroundDiv.style.display = "none";
	}
}


ModalPopup.prototype.convertOffset = function(val) {
	if (!val) {return;}
	val = val.replace("px","");
	if (isNaN(val)) {return 0;}
	return parseInt(val);
}

ModalPopup.prototype.setPosition = function() {
	//set the popup layer styles
	var winW = (document.layers||(document.getElementById&&!document.all)) ? window.outerWidth : (document.all ? document.body.clientWidth : 0);
	var winH = window.innerHeight ? window.innerHeight :(document.getBoxObjectFor ? Math.min(document.documentElement.clientHeight, document.body.clientHeight) : ((document.documentElement.clientHeight != 0) ? document.documentElement.clientHeight : (document.body ? document.body.clientHeight : 0)));	
	var currentStyle;
	if (this.popup.currentStyle)	{ 
		currentStyle = this.popup.currentStyle; 
	} else if (window.getComputedStyle) {
		currentStyle = document.defaultView.getComputedStyle(this.popup, null);
	} else {
		currentStyle = this.popup.style;
	}
 
	var elemW = this.popup.offsetWidth -
		this.convertOffset(currentStyle.marginLeft) -
		this.convertOffset(currentStyle.marginRight) -
		this.convertOffset(currentStyle.borderLeftWidth) -
		this.convertOffset(currentStyle.borderRightWidth);
 
	var elemH = this.popup.offsetHeight -
		this.convertOffset(currentStyle.marginTop) -
		this.convertOffset(currentStyle.marginBottom) -
		this.convertOffset(currentStyle.borderTopWidth) -
		this.convertOffset(currentStyle.borderBottomWidth);
 
	this.popup.style.position = "fixed";
	this.popup.style.left = (winW/2 - elemW/2) + this.addLeft + "px";
	this.popup.style.top = (winH/2 - elemH/2 - 10) + this.addTop + "px";
	this.popup.style.zIndex = this.zIndex + 1;
}