/* 

Sitemap Styler v0.1
written by Alen Grakalic, provided by Css Globe (cssglobe.com)
visit http://cssglobe.com/lab/sitemap_styler/
	
*/


$(document).ready(function () {


	console.log("window.location.href  -> " + window.location.href);
	console.log("window.location.pathname  -> " + window.location.pathname);


	var sitemap = document.getElementsByClassName("menu-my-menu");

	if (sitemap) {

		// rename tag nav to div
		$("nav:has(.menu-my-menu)").each(function () {
			var $this = $(this);
			$this.replaceWith($("<my_nav>" + $this.html() + "</my_nav>"));
		});

		var mysitemap = $(".menu-my-menu").attr('id', 'sitemap');

		// all li inside menu
		var myitemsCount = $(".menu-my-menu li").length;
		var myitems = $(".menu-my-menu li")
			.each(function (x, li) {

				// если есть чилды, то 
				if (li.getElementsByTagName("ul").length > 0) {
					var ul = li.getElementsByTagName("ul")[0]; // получаем вложенный список и скрываем его
					ul.style.display = "none";

					var span = document.createElement("span"); // добавляем место для плюсика вперед 
					span.className = "collapsed"; //отображаем плюсик


					ul.style.display = $(li).find(".active").length > 0 ? "block" : "none";
					span.className = ul.style.display == "none" ? "collapsed" : "expanded";

					span.onclick = function () // при клике по плюсику
					{
					    ul.style.display = (ul.style.display == "none") ? "block" : "none"; //отображаем вложенный список или скрываем его
					    this.className = (ul.style.display == "none") ? "collapsed" : "expanded"; // отображеем минусик
					};



				    // ищем ссылку в текущем li, которая должна работать не как ссылка, а как экспандер expander
					// var expander = $(li).find(".expander"); // нужно искать ссылку не по всему li а ближайшую li > a

				    var liId = li.id;
				    var expander = document.getElementById('a_categoryId_' + liId + 'expander');

				    if (expander) {
				        $(expander).click(function () // при клике по ссылке-экспандеру
				        {
				            ul.style.display = (ul.style.display == "none") ? "block" : "none"; //отображаем вложенный список или скрываем его
				            span.className = (ul.style.display == "none") ? "collapsed" : "expanded"; // отображеем минусик
				            return false;
				        });
				    } else {
				       
				    }


				    li.appendChild(span);
				}; //end if

				// Select element if it is current
				var currentPageUrl = window.location.pathname;

				var thesearcherText = $(this).children(':first').attr('href');
				console.log("founded menu link -> " + thesearcherText);

				if(currentPageUrl == thesearcherText)
					$(this).children(':first').addClass('my_nav_current');


			}); //end each

	};
});


