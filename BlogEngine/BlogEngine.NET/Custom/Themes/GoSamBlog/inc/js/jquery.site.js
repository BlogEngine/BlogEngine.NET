jQuery(document).ready(function($){

	// menu
	$('.menu ul').superfish({
		autoArrows	: false
	});
	// Toggle
	$(".togglebox, .toggle-code").hide();
	//slide up and down when click over heading 2
	$("h2").click(function(){
	// slide toggle effect set to slow you can set it to fast too.
		$(this).toggleClass("active").next(".togglebox").slideToggle("slow");
		return true;
	});
	$('.toggle-code-link').click(function(e){
		$(this).toggleClass("active").next(".toggle-code").slideToggle("slow");
		$('a span',this).toggleClass('hide');
		e.preventDefault();
	});
	
	// Tabs
	$(".dc-tab-content").hide(); // Hide all content
	$("ul.tabs li:first").addClass("active").show(); // Activate first tab
	$(".dc-tab-content:first").show(); // Show first tab content
	
	//On Click Event
	$("ul.tabs li a").click(function(e) {
		$("ul.tabs li").removeClass("active"); // Reset "active" class
		$(this).parent().addClass("active"); // Add "active" class to selected tab
		$(".dc-tab-content").hide(); // Hide all tab content
		var i = $(this).index("ul.tabs li a");
		$(".dc-tab-content:eq("+i+")").fadeIn(); // Fade in the active tab
		e.preventDefault();
	});
	$('#link-go').click(function(e){
		$('#dcsmt-form').submit();
	});
	$('.tbl tr:odd').addClass('odd');
});