/*
 * DC Social Media Tabs
 * Copyright (c) 2012 Design Chemical
 * http://www.designchemical.com/blog/index.php/premium-jquery-plugins/jquery-social-media-tabs-plugin/
 * Version 1.7.1 (30-05-2013)
 */
 
(function($){

	SocialTabsObject = function(el, options) {
		this.create(el, options);
	};
	
	$.extend(SocialTabsObject.prototype, {
		
		version   : '1.7.1',
	
		create: function(el, options) {
		
			this.defaults = {
				widgets: 'twitter,facebook,fblike,fbrec,google,rss,flickr,delicious,youtube,digg,pinterest,lastfm,dribbble,vimeo,stumbleupon,tumblr,deviantart,linkedin,instagram',
				twitter: {
					title: 'Latest Tweets',
					link: true,
					follow: 'Follow on Twitter',
					followId: '',
					limit: 10,
					retweets: false,
					replies: false,
					images: '', // large w: 786 h: 346, thumb w: 150 h: 150, medium w: 600 h: 264, small w: 340 h 150
					url: 'http://www.gosam.info/twitter.php',
					icon: 'twitter.png'
				},
				facebook: {
					title: 'Facebook',
					link: true,
					follow: 'Follow on Facebook',
					limit: 10,
					text: 'contentSnippet',
					icon: 'facebook.png'
				},
				fblike: {
					title: '',
					link: false,
					follow: '',
					limit: 36,
					stream: false,
					header: true,
					icon: 'fblike.png'
				},
				fbrec: {
					title: '',
					link: false,
					follow: '',
					header: true,
					icon: 'fbrec.png'
				},
				google: {
					title: 'Google +1',
					link: true,
					follow: 'Add to Circles',
					pageId: '',
					header: 0,
					image_width: 75,
					image_height: 75,
					api_key: 'AIzaSyB1UZNnscjMDjjH-pi_XbnLRld2wAqi3Ek',
					shares: true,
					limit: 10,
					icon: 'google.png'
				},
				youtube: {
					title: '',
					link: false,
					follow: '',
					limit: 10,
					feed: 'uploads', // favorites
					subscribe: true,
					icon: 'youtube.png'
				},
				flickr: {
					title: 'Flickr',
					link: true,
					follow: '',
					lang: 'en-us',
					limit: 20,
					icon: 'flickr.png'
				},
				delicious: {
					title: 'Delicious',
					link: true,
					follow: 'Follow on Delicious',
					limit: 10,
					icon: 'delicious.png'
				},
				digg: {
					title: 'Latest Diggs',
					link: false,
					limit: 10,
					icon: 'digg.png',
					hdrBg: "#ececec", 
					hdrTxt: "#555", 
					tabBg: "#4684be", 
					tabTxt: "#b3daff", 
					tabOnTxt: "#d41717", 
					bdyBg: "#fff", 
					stryBrdr: "#ccc", 
					lnk: "#105cb6", 
					descTxt: "#999", 
					subHd: "#999"
				},
				pinterest: {
					title: 'Pinterest',
					link: true,
					follow: 'Follow on Pinterest',
					limit: 10,
					icon: 'pinterest.png'
				},
				rss: {
					title: 'Subscribe to our RSS',
					link: true,
					follow: 'Subscribe',
					limit: 10,
					text: 'contentSnippet',
					icon: 'rss.png'
				},
				lastfm: {
					title: 'Last.fm',
					link: true,
					follow: '',
					limit: 20,
					feed: 'recenttracks',
					icon: 'lastfm.png'
				},
				dribbble: {
					title: 'Dribbble',
					link: true,
					follow: 'Follow on Dribbble',
					limit: 10,
					feed: 'shots',
					icon: 'dribbble.png'
				},
				vimeo: {
					title: 'Vimeo',
					link: true,
					follow: 'Follow on Vimeo',
					limit: 10,
					feed: 'likes',
					thumb: 'small',
					stats: true,
					icon: 'vimeo.png'
				},
				stumbleupon: {
					title: 'Stumbleupon',
					link: true,
					follow: 'Follow',
					limit: 10,
					feed: 'favorites',
					icon: 'stumbleupon.png'
				},
				tumblr: {
					title: 'Tumblr',
					link: true,
					follow: 'Follow',
					limit: 10,
					thumb: 250,
					video: 250,
					icon: 'tumblr.png'
				},
				deviantart: {
					title: 'Deviantart',
					link: true,
					follow: 'Follow',
					limit: 10,
					icon: 'deviantart.png'
				},
				linkedin: {
					plugins: 'CompanyProfile,MemberProfile,CompanyInsider,JYMBII',
					CompanyInsider: 'innetwork,newhires,jobchanges',
					MemberProfile: 'true',
					CompanyProfile: 'true',
					icon: 'linkedin.png'
				},
				instagram: {
					title: 'Instagram',
					limit: 10,
					accessToken: '8e34307071df47b09e264b95f96f141c',
					redirectUrl: 'http://www.gosam.info/',
					clientId: '3003f02d79054772877e9236c122b957',
					thumb: 'low_resolution',
					comments: 1,
					likes: 1,
					icon: 'instagram.png'
				},
				tweetId: '',
				share: true,
				external: true,
				method: 'slide',
				position: 'fixed',
				location: 'left',
				align: 'top',
				offset: 192,
				speed: 600,
				loadOpen: false,
				autoClose: false,
				width: 360,
				height: 570,
				start: 0,
				controls: true,
				rotate: {
					direction: 'down',
					delay: 6000
				},
				wrapper: 'dcsmt',
				content: 'dcsmt-content',
				slider: 'dcsmt-slider',
				slides: 'tab-content',
				tabs: 'social-tabs',
				classOpen: 'dcsmt-open',
				classClose: 'dcsmt-close',
				classToggle: 'dcsmt-toggle',
				classSlide: 'dcsmt-slide',
				active: 'active',
				zopen: 1000,
				imagePath: 'images/icons/'
			};
			this.o = {};
			this.timer_on = 0;
			this.id = 'dcsmt-'+$(el).index();
			this.timerId = '';
			
			this.o = $.extend(true,this.defaults,options);
			
			$(el).addClass(this.o.content).wrap('<div id="'+this.id+'" class="'+this.o.wrapper+'" />');
			var $a = $('#'+this.id),$c = $('.'+this.o.content,$a),ca = 'active';
			
			$a.css({width: this.o.width+'px'});
			$c.append('<ul class="'+this.o.tabs+'"></ul>').append('<ul class="'+this.o.slider+'"></ul>');
			
			var tabs = this.o.tabs, slider = this.o.slider, slides = this.o.slides, self = this;
			path = this.o.imagePath;
			
			$.each(this.o.widgets.split(','), function(i,v){
				var cl = i == 0 ? 'dcsmt-'+v+' first' : 'dcsmt-'+v ;
				$('.'+tabs,$c).append('<li class="'+cl+'"><a href="#" rel="'+i+'" title="'+v+'"><img src="' + '/Custom/Themes/GoSamBlog/' +path+self.o[v].icon+'" alt="" rel="'+v+'" /></a></li>');
				$('.'+slider,$c).append('<li class="'+slides+' tab-'+v+'"><div class="tab-inner"></div></li>');
			});
			
			var $r = $('.'+this.o.slider,$a), $s = $('.'+this.o.slides,$a), $t = $('.'+this.o.tabs,$a), $l = $('li',$t);
			
			if(this.o.method == 'slide'){
				var align = this.o.align == 'left' || this.o.align == 'right' ? 'align-'+this.o.align : 'align-top' ;
				$a.addClass(this.o.location).addClass(align).css({position: this.o.position});
			} else {
				$a.addClass('static');
			}
			
			hb = this.o.height-parseInt($s.css('border-top-width'),10)-parseInt($s.css('padding-top'),10)-parseInt($s.css('border-bottom-width'),10)-parseInt($s.css('padding-bottom'),10);
			wb = this.o.width-parseInt($s.css('border-right-width'),10)-parseInt($s.css('padding-right'),10)-parseInt($s.css('border-left-width'),10)-parseInt($s.css('padding-left'),10);
			$s.css({height: hb+'px', width: wb+'px'});
			$('.tab-inner',$s).css({height: hb+'px', width: wb+'px'});
			
			if(this.o.controls){
				$c.append('<div class="controls"><ul><li><a href="#" class="play"></a></li><li><a href="#" class="prev"></a></li><li><a href="#" class="next"></a></li><li><a href="#" class="'+this.o.classClose+' close"></a></li></ul></div>');
				$('.controls',$c).css({width: wb+'px'});
			}
			
			if(this.o.method == 'slide'){
				this.dcslide($a,$t,$s,$l);
			} else {
				this.dcstatic($a,$t,$l);
			}
			if(this.o.loadOpen == true){
				this.open($a);
			}
			this.slickTabs(this.o.start,$a,$t,$s);
			this.addevents($a,$t,$s,$l);
		},
		
		addevents: function(a,t,s,l){
			var self = this, ca = this.o.active, cw = this.o.wrapper, co = this.o.classOpen, cc = this.o.classClose, ct = this.o.classToggle, 
			cs = this.o.classSlide, m = this.o.method, start = this.o.start, external = this.o.external;
			$('a',l).click(function(){
				var i = parseInt($(this).attr('rel'),10);
				if($(this).parent().hasClass(ca)){
					if(m == 'slide'){
						self.close(a,l,s);
					}
				} else {
					if(!$('li.active',t).length && m == 'slide'){
						self.open(a);
					}
					self.slickTabs(i,a,t,s);
				}
				return false;
			});
			a.hover(function(){
				if($('.tab-active .stream').length){
					$('.controls',this).fadeIn();
				} else {
					$('.controls',this).hide();
				}
			},
				function(){$('.controls',this).fadeOut();
			});
			$('.controls',a).delegate('a','click',function(){
				var x = $(this).attr('class'), stream = $('.tab-active .stream',a);
				switch(x)
				{
					case 'prev':
					self.pauseTimer();
					ticker(stream,'prev');
					break;
					case 'next':
					self.pauseTimer();
					ticker(stream,'next');
					break;
					case 'play':
					self.rotate(a);
					$('.controls .play').removeClass('play').addClass('pause');
					break;
					case 'pause':
					self.pauseTimer();
					break;
				}
				return false;
			});
			if(this.o.method == 'slide'){
				$('.'+co).click(function(e){
					if(!a.hasClass(ca)){
						self.open(a);
					}
					var i = parseInt($(this).attr('rel'),10) ? parseInt($(this).attr('rel'),10) : start ;
					self.slickTabs(i,a,t,s);
					e.preventDefault();
				});
				$('.'+cc).click(function(e){
					if(a.hasClass(ca)){
						self.close(a,l,s);
					}
					e.preventDefault();
				});
				$('.'+ct).click(function(e){
					if(a.hasClass(ca)){
						self.close(a,l,s);
					} else {
						self.open(a);
						var i = parseInt($(this).attr('rel'),10) ? parseInt($(this).attr('rel'),10) : start ;
						self.slickTabs(i,a,t,s);
					}
					e.preventDefault();
				});
			}
			$('.'+cs).click(function(e){
				if(m == 'slide'){
					if(!a.hasClass(ca)){
						self.open(a);
					}
				}
				var i = parseInt($(this).attr('rel'),10) ? parseInt($(this).attr('rel'),10) : start ;
				self.slickTabs(i,a,t,s);
				e.preventDefault();
			});
			s.delegate('a','click',function(){
				if($(this).parent().hasClass('section-share')){
					var u = $(this).attr('href');
					window.open(u,'sharer','toolbar=0,status=0,width=626,height=436');
					return false;
				} else {
					if(external){this.target = '_blank';}
				}
			});
			s.delegate('a','click',function(){
				if($(this).parents().hasClass('tab-facebook')){
					var u = $(this).attr('href').split('/&');
					$(this).attr('href',u[0]);
				}
			});
			if(this.o.autoClose == true){
				$('body').mouseup(function(e){
					if(a.hasClass(ca) && !$(e.target).parents().hasClass(cw)){
						if(!$(e.target).hasClass(co) || !$(e.target).hasClass(cs)){
							self.close(a,l,s);
						}
					}
				});
			}
		},
		dcslide: function(a,t,s,l){
			t.css({position: 'absolute'});
			s.css({position: 'relative'});
			tw = l.outerWidth(true);
			th = t.outerHeight();
			var p1 = {marginLeft: '-'+this.o.width+'px', top: this.o.offset+'px', left: 0};
			var p2 = {top: 0, right: 0, marginRight: '-'+tw+'px', width: tw+'px'};
			switch(this.o.location){
				case 'right':
				p1 = {marginRight: '-'+this.o.width+'px', top: this.o.offset+'px', right: 0};
				p2 = {top: 0, left: 0, marginLeft: '-'+tw+'px', width: tw+'px'};
				break;
				case 'top':
				p1 = {marginTop: '-'+this.o.height+'px', top: 0};
				p2 = {bottom: 0, marginBottom: '-'+th+'px'};
				if(this.o.align == 'left'){
					a.css({left: this.o.offset+'px'});
					t.css({left: 0});
				} else {
					a.css({right: this.o.offset+'px'});
					t.css({right: 0});
				}
				break;
				case 'bottom':
				p1 = {marginBottom: '-'+this.o.height+'px', bottom: 0};
				p2 = {top: 0, marginTop: '-'+th+'px'};
				if(this.o.align == 'left'){
					a.css({left: this.o.offset+'px'});
					t.css({left: 0});
				} else {
					a.css({right: this.o.offset+'px'});
					t.css({right: 0});
				}
				break;
			}
			a.css(p1).addClass('sliding');
			t.css(p2);
		},
		dcstatic: function(a,t,l){
			th = l.outerHeight();
			a.addClass(this.o.active);
			t.css({height: th+'px'});
		},
		slickTabs: function(i,a,t,s){
			var self = this;
			$('li',t).removeClass(this.o.active).eq(i).addClass(this.o.active);
			s.removeClass('tab-active').hide().eq(i).addClass('tab-active').show();
			if(!$('li:eq('+i+')',t).hasClass('loaded') && a.hasClass(this.o.active)){
				var type = $('li:eq('+i+') img',t).attr('rel');
				var widget = createWidget(this.id,type,this.o[type+'Id'],this.o[type],this.o.width,this.o.height,this.o.share,this.o.tweetId);
				$('.'+this.o.slides+':eq('+i+') .tab-inner',a).empty().hide().append(widget).fadeIn(600).addClass('loaded');
				$('li:eq('+i+')',t).addClass('loaded');
				if(type == 'facebook' || type == 'fblike' || type == 'fbrec'){
					fbLink(this.o[type+'Id'], $('.btn-type-'+type));
				} else if(type == 'linkedin'){
					$.getScript("http://platform.linkedin.com/in.js?async=true", function(){
						IN.init();
					});
				}
			}
			if(!a.hasClass(this.o.active) && this.o.method == 'slide'){
				$('li',t).removeClass(this.o.active);
			}
			if(this.o.rotate.delay > 0){
				self.pauseTimer();
				self.rotate(a);
				$('.controls .play').removeClass('play').addClass('pause');
			}
		},
		open: function(a){
			var p1 = {marginBottom: "-=5px"},p2 = {marginBottom: 0},self = this;
			a.css({zIndex: this.o.zopen});
			switch (this.o.location) {
				case 'top': 
				p1 = {marginTop: "-=5px"},p2 = {marginTop: 0};
				break;
				case 'left':
				p1 = {marginLeft: "-=5px"},p2 = {marginLeft: 0};		
				break;
				case 'right': 
				p1 = {marginRight: "-=5px"},p2 = {marginRight: 0};
				break;
			}
			a.animate(p1, 100).animate(p2, this.o.speed).addClass(this.o.active);
		},
		close: function(a,l,s){
			var self = this, ca = this.o.active;
			if(a.hasClass(ca)){
				var p = {"marginBottom": "-"+this.o.height+'px'};
				switch (this.o.location) {
					case 'top': 
					p = {"marginTop": "-"+this.o.height+'px'};
					break;
					case 'left':
					p = {"marginLeft": "-"+this.o.width+'px'};		
					break;
					case 'right': 
					p = {"marginRight": "-"+this.o.width+'px'};
					break;
				}
				a.animate(p, this.o.speed, function(){
					a.removeClass(ca);
					l.removeClass(ca);
					s.removeClass('tab-active');
				});
				self.pauseTimer();
			}
		},
		rotate: function(a){
			var self = this, stream = $('.tab-active .stream',a), speed = this.o.speed, delay = this.o.rotate.delay, r = this.o.rotate.direction == 'up' ? 'prev' : 'next' ;
			this.timer_on = 1;
			this.timerId = setTimeout(function(){
				ticker(stream,r,speed);
				self.rotate(a);
			}, delay);
		},
		pauseTimer: function(){
			clearTimeout(this.timerId);
			this.timer_on = 0;
			$('.controls .pause').removeClass('pause').addClass('play');
		}
	});
	
	$.fn.dcSocialTabs = function(options, callback){
		var d = {};
		this.each(function(){
			var s = $(this);
			d = s.data("socialtabs");
			if (!d){
				d = new SocialTabsObject(this, options, callback);
				s.data("socialtabs", d);
			}
		});
		return d;
	};
	
	function createWidget(obj,type,id,o,w,h,share,tweetId){
		
		var ti = obj+'-'+type,c = '',t = '',p = '',href = '',n = o.limit,stream = '<ul id="'+ti+'" class="stream"></ul>',data,frl = 'https://ajax.googleapis.com/ajax/services/feed/load?v=1.0';
		
		switch (type) {

			case 'twitter':
			var cp = id.split('/'), cq = id.split('#'), cu = o.url.split('?'), replies = o.replies == true ? '&exclude_replies=false' : '&exclude_replies=true' ;
			var param = '&include_entities=true&include_rts='+o.retweets+replies;
			url1 = cu.length > 1 ? o.url + '&' : o.url + '?';
			url = cp.length > 1 ? url1 + 'url=list&list_id='+cp[1]+'&per_page='+n+param : url1 + 'url=timeline&screen_name='+id+'&count='+n+param;
			if(cq.length > 1){
				var rts = o.retweets == false ? '+exclude:retweets' : '' ;
				url = url1 + 'url=search&q='+encodeURIComponent(cq[1])+'&count='+n;
			}
			href = 'https://www.twitter.com/';
			href += cp.length > 1 || cq.length > 1 ? o.followId : id ;
			c += stream;
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
			
			case 'facebook':
			c += stream;
			url = 'https://www.facebook.com/feeds/page.php?id='+id+'&format=rss20';
			url = frl+'&num='+n+'&callback=?&q=' + encodeURIComponent(url);
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
			
			case 'fblike':
			src = 'https://www.facebook.com/plugins/likebox.php?id='+id+'&amp;width='+w+'&amp;connections='+n+'&amp;stream='+o.stream+'&amp;header='+o.header+'&amp;height='+h;
			c += getFrame(src,w,h);
			break;
			
			case 'fbrec':
			src = 'https://www.facebook.com/plugins/recommendations.php?site='+id+'&amp;width='+w+'&amp;height='+h+'&amp;header='+o.header+'&amp;colorscheme=light&amp;font&amp;border_color';
			c += getFrame(src,w,h);
			break;
			
			case 'google': 
			href = 'https://plus.google.com/'+id;
			if(o.header > 0){
				var ph = o.header == 1 ? 69 : 131 ;
				var gc = o.header == 1 ? 'small' : 'standard' ;
				c += '<link href="https://plus.google.com/'+o.pageId+'" rel="publisher" /><script type="text/javascript">window.___gcfg = {lang: "en"};(function(){var po = document.createElement("script");po.type = "text/javascript"; po.async = true;po.src = "https://apis.google.com/js/plusone.js";var s = document.getElementsByTagName("script")[0];s.parentNode.insertBefore(po, s);})();</script><div class="google-page '+gc+'"><g:plus href="https://plus.google.com/'+o.pageId+'" width="'+w+'" height="'+ph+'" theme="light"></g:plus></div>';
			}
			c += stream;
			url = 'https://www.googleapis.com/plus/v1/people/'+id+'/activities/public';
			data = {key: o.api_key, maxResults: n, prettyprint: false, fields:"items(id,kind,object(attachments(displayName,fullImage,id,image,objectType,url),id,objectType,plusoners,replies,resharers,url),published,title,url,verb)"};
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
			
			case 'youtube': 
			href = 'https://www.youtube.com/user/'+id;
			if(o.subscribe){
				c += '<iframe src="https://www.youtube.com/subscribe_widget?p='+id+'" class="youtube-subscribe" scrolling="no" frameBorder="0"></iframe>';
			}
			c += stream;
			url = 'https://gdata.youtube.com/feeds/base/users/'+id+'/'+o.feed+'?alt=rss&v=2&orderby=published&client=ytapi-youtube-profile';
			url = frl+'&num='+n+'&callback=?&q=' + encodeURIComponent(url);
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
			
			case 'flickr':
			var cq = id.split('/'), fd = cq.length > 1 ? 'groups_pool' : 'photos_public' ;
			id = cq.length > 1 ? cq[1] : id ;
			href = 'http://www.flickr.com/photos/'+id;
			c += stream;
			url = 'http://api.flickr.com/services/feeds/'+fd+'.gne?id='+id+'&lang='+o.lang+'&format=json&jsoncallback=?';
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;

			case 'delicious':
			href = 'https://www.delicious.com/'+id;
			c += stream;
			url = 'http://feeds.delicious.com/v2/json/'+id;
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
			
			case 'digg':
			href = 'https://digg.com/users/'+id;
			c += '<div id="'+ti+'"><a href="https://digg.com/users/'+id+'"></a></div><script type="text/javascript">(function() { var s, s1, diggWidget = {id: "'+ti+'", layout: 1, colors: {hdrBg: "'+o.hdrBg+'", hdrTxt: "'+o.hdrTxt+'", tabBg: "'+o.tabBg+'", tabTxt: "'+o.tabTxt+'", tabOnTxt: "'+o.tabOnTxt+'", bdyBg: "'+o.bdyBg+'", stryBrdr: "'+o.stryBrdr+'", lnk: "'+o.lnk+'", descTxt: "'+o.descTxt+'", subHd: "'+o.subHd+'"}, title: "'+o.title+'", width: '+w+', requests: [{t: "'+id+'", p: {count: "'+n+'", method: "user.getDugg", username: "'+id+'"}}], hide: {}}; if (window.DiggWidget) { if (typeof DiggWidget == "function") { new DiggWidget(diggWidget); } else { DiggWidget.push(diggWidget); } } else { DiggWidget = [diggWidget]; s = document.createElement("SCRIPT"); s.type = "text/javascript"; s.async = true; s.src = "https://widgets.digg.com/widgets.js";s1= document.getElementsByTagName("SCRIPT")[0]; s1.parentNode.insertBefore(s, s1); } })();</script>';
			break;
			
			case 'pinterest':
			var cp = id.split('/'), ext = cp.length > 1 ? '/rss' : '/feed.rss';
			href = 'https://www.pinterest.com/'+id;
			c += stream;
			url = frl+'&num='+n+'&callback=?&q=' + encodeURIComponent(href+ext);
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
			
			case 'rss':
			href = id;
			c += stream;
			url = frl+'&num='+n+'&callback=?&q=' + encodeURIComponent(href);
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
			
			case 'lastfm':
			href = 'https://www.last.fm/user/'+id;
			c += stream;
			var ver = o.feed == 'lovedtracks' ? '2.0' : '1.0' ;
			url = frl+'&num='+n+'&callback=?&q=' + encodeURIComponent('https://ws.audioscrobbler.com/'+ver+'/user/'+id+'/'+o.feed+'.rss');
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
			
			case 'dribbble':
			href = 'http://www.dribbble.com/'+id;
			url = o.feed == 'likes' ? 'http://api.dribbble.com/players/'+id+'/shots/likes' : 'http://api.dribbble.com/players/'+id+'/shots' ;
			c += stream;
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
			
			case 'vimeo':
			href = 'https://www.vimeo.com/'+id;
			c += stream;
			url = 'https://vimeo.com/api/v2/'+id+'/'+o.feed+'.json';
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
			
			case 'stumbleupon':
			href = 'http://www.stumbleupon.com/stumbler/'+id;
			c += stream;
			url = frl+'&num='+n+'&callback=?&q=' + encodeURIComponent('http://rss.stumbleupon.com/user/'+id+'/'+o.feed);
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
			
			case 'tumblr':
			href = 'http://'+id+'.tumblr.com';
			c += stream;
			url = 'http://'+id+'.tumblr.com/api/read/json?callback=?';
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
			
			case 'deviantart':
			href = 'https://'+id+'.deviantart.com';
			c += stream;
			url = frl+'&num='+n+'&callback=?&q=' + encodeURIComponent('https://backend.deviantart.com/rss.xml?type=deviation&q=by%3A'+id+'+sort%3Atime+meta%3Aall');
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
			
			case 'linkedin':
			id = id.split(',');
			$.each(o.plugins.split(','), function(i,v){
				if(id[i]){
					var mod = v == 'CompanyInsider' ? ' data-modules="'+o[v]+'"' : '' ;
					mod = v == 'MemberProfile' || v == 'CompanyProfile' ? ' data-related="'+o[v]+'"' : mod ;
					c += getLinkedIn(id[i],v,mod,w-20);
				}
			});
			break;
			
			case 'instagram':
			href = '#';
			c += stream;
			url = 'https://api.instagram.com/v1';
			var cp = id.substr(0,1), cq = id.split(cp), url1 = encodeURIComponent(cq[1]), qs = '';
			switch(cp)
			{
				case '?':
				var p = cq[1].split('/');
				qs = '&lat='+p[0]+'&lng='+p[1]+'&distance='+p[2];
				url += '/media/search';
				break;
				case '#':
				url += '/tags/'+url1+'/media/recent';
				break;
				case '!':
				url += '/users/'+url1+'/media/recent';
				break;
				case '@':
				url += '/locations/'+url1+'/media/recent';
				break;
			}
			if(o.accessToken == ''){
				if (location.hash) {
					o.accessToken = location.hash.split('=')[1] ;
				} else {
					location.href="https://instagram.com/oauth/authorize/?client_id="+o.clientId+"&redirect_uri="+o.redirectUrl+"&response_type=token"; 
				}
			}
			url += '?access_token='+o.accessToken+'&client_id='+o.clientId+qs;
			getFeed(ti,type,url,data,o,id,share,tweetId);
			break;
		}
		if(type != 'digg' && type != 'linkedin'){
			if(o.follow){
				p = o.follow != '' ? '<a href="'+href+'" class="dcsmt-btn btn-type-'+type+'">'+o.follow+'</a>' : '' ;
			}
			t = o.title != '' ? o.title : '' ;
			t = o.link ? '<a href="'+href+'" class="btn-type-'+type+'">'+t+'</a>' : t ;
			c = t != '' ? '<div class="profile"><h3>'+t+'</h3>'+p+'</div>'+c : c ;
		}
		return c;
	};
	
	function getFeed(target,type,url,data,o,id,share,tweetId){
		var x = '#'+target,html = [],d = '',dataType = type == 'twitter' ? 'json' : 'jsonp';
		jQuery.ajax({
			url: url,
			data: data,
			cache: true,
			dataType: dataType,
			success: function(a){
				var error = '',px = $(x).width();
				switch(type)
				{
					case 'google':
						error = a.error ? a.error : '' ;
						a = a.items;
					break;
					case 'flickr':
						a = a.items;
					break;
					case 'instagram':
						a = a.data;
					break;
					case 'twitter':
						error = a.errors ? a.errors : '' ;
						var cq = id.split('#');
						if(cq.length > 1){a = a.statuses} ;
					break;
					case 'dribbble':
						a = a.shots;
					break;
					case 'tumblr':
						a = a.posts;
					break;
					case 'delicious':
					break;
					case 'vimeo':
					break;
					default:
						if(a.responseStatus == 200){
							a = a.responseData.feed.entries;
						} else {
							error = a.responseDetails;
						}
					break;
				}
				if(error == ''){
					$.each(a, function(i,item){
						if(i < o.limit){
							d = item.publishedDate;
							var x = '<a href="'+item.link+'" class="title">'+item.title+'</a>', sq = item.link, st = item.title, s = '';
							html.push('<li>');
							switch(type)
							{
								case 'twitter':
								d = parseTwitterDate(item.created_at);
								var un = item.user.screen_name, ua = item.user.profile_image_url_https ;
								html.push('<a href="https://www.twitter.com/'+un+'" class="thumb"><img src="'+ua+'" alt="" /></a>');
								html.push('<span class="twitter-user"><a href="https://www.twitter.com/'+un+'"><strong>'+item.user.name+'</strong> @'+un+'</a></span><br />');
								html.push(linkify(item.text));
								if(o.images != '' && item.entities.media){
									$.each(item.entities.media, function(i,media){
										html.push('<a href="'+media.media_url_https+'" class="twitter-image"><img src="'+media.media_url_https+':'+o.images+'" alt="" /></a>');
									});
								}
								sq = item.id_str;
								break;
								
								case 'facebook':
								html.push(x);
								html.push(item[o.text]);
								break;
								
								case 'delicious':
								d = item.dt;
								html.push('<a href="' + item.u + '" class="title">'+item.d+'</a>');
								html.push('<span class="text">'+item.n+'</span>');
								sq = item.u;
								st = item.d;
								break;
								
								case 'rss':
								html.push(x+item[o.text]);
								break;
								
								case 'pinterest':
								var img=$('img',item.content).attr('src') ? '<a href="'+item.link+'"><img src="'+$('img',item.content).attr('src')+'" alt="" /></a>':'';
								html.push(img);
								html.push(item.contentSnippet);
								st = item.contentSnippet;
								break;
								
								case 'youtube':
								html.push(x);
								var v = [];
								v = parseQ(item.link);
								html.push('<a href="' + item.link + '" class="thumb"><img src="https://i.ytimg.com/vi/'+v['v']+'/default.jpg" alt="" /></a>');
								html.push(item.contentSnippet);
								break;
								
								case 'flickr':
								d = '';
								html.push('<a href="' + item.link + '" class="thumb" title="'+ item.title +'"><img src="' + item.media.m + '" alt="" /></a>');
								break;
								
								case 'lastfm':
								html.push('<a href="'+item.content+'" class="title">'+item.title+'</a>');
								break;
								
								case 'dribbble':
								d = item.created_at;
								html.push('<a href="'+item.url+'" class="thumb"><img src="' + item.image_teaser_url + '" alt="' + item.title + '" /></a>');
								html.push('<span class="meta"><span class="views">'+num(item.views_count)+'</span><span class="likes">'+num(item.likes_count)+'</span><span class="comments">'+num(item.comments_count)+'</span></span>');
								sq = item.url;
								break;
								
								case 'deviantart':
								html.push(x+item.content);
								break;
								
								case 'tumblr':
								d = item.date;
								var x = '<a href="'+item['url-with-slug']+'">', z = '';
								switch(item.type)
								{
									case 'photo':
									x += item['photo-caption']+'</a>';
									z += '<img src="'+item['photo-url-'+o.thumb]+'" alt="" />';
									st = item['photo-caption'];
									break;
									case 'video':
									x += item['video-caption']+'</a>';
									z += o.video != '400' ? item['video-player-'+o.video] : item['video-player'] ;
									st = item['video-caption'];
									break;
									case 'regular':
									x += item['regular-title'].replace(/./g, '')+'</a>';
									z += item['regular-body'];
									st = item['regular-title'].replace(/./g, '');
									break;
									case 'quote':
									x += item['quote-source']+'</a>';
									z += item['quote-text'];
									st = item['quote-source'];
									break;
									case 'audio':
									x = item['id3-artist'] ? '<a href="'+item['url-with-slug']+'">'+item['id3-artist']+' - '+item['id3-album']+'</a>' : '' ;
									x += item['id3-title'] ? '<a href="'+item['url-with-slug']+'" class="track">'+item['id3-title']+'</a>' : '' ;
									z += item['audio-caption'] ? item['audio-caption'] : '' ;
									z += item['audio-player'] ? item['audio-player'] : '' ;
									st = item['id3-artist']+' - '+item['id3-album']+' - '+item['id3-title'];
									break;
									case 'conversation':
									x += item['conversation-title']+'</a>';
									z += item['conversation-text'];
									st = item['conversation-title'];
									break;
									case 'link':
									var ltxt = item['link-text'].replace(/:/g, '').replace(/\?/g, '').replace(/\!/g, '').replace(/\./g, '').replace(/\'/g, '');
									x = '<a href="'+item['link-url']+'">'+ltxt+'</a>';
									z += item['link-description'];
									st = ltxt;
									break;
								}
								html.push(x);
								html.push(z);
								st = stripHtml(st);
								sq = item['url-with-slug'];
								break;

								case 'vimeo':
								d = '', f = o.feed, at = item.name, tx = item.description;
								if(f == 'channels'){
									if(item.logo != ''){
										html.push('<a href="'+item.url+'" class="logo"><img src="'+item.logo+'" alt="" width="'+px+'" /></a>');
									}
								} else if(f == 'groups'){
									html.push('<a href="'+item.url+'" class="thumb"><img src="'+item.thumbnail+'" alt="" /></a>');
								} else {
									var thumb = 'thumbnail_'+o.thumb, at = item.title, tx = f != 'albums' ? item.duration+' secs' : item.description ;
									html.push('<a href="'+item.url+'" class="thumb"><img src="'+item[thumb]+'" alt="" /></a>');
								}
								html.push('<a href="'+item.url+'" class="title">'+at+'</a>');
								html.push('<span class="text">'+tx+'</span>');
								if(o.stats == true){
									var m = '';
									m += f == 'albums' || f == 'channels' || f == 'groups' ? '<span class="videos">'+num(item.total_videos)+'</span>' : '' ;
									if(f == 'channels'){
										m += '<span class="users">'+num(item.total_subscribers)+'</span>';
									} else if(f == 'groups'){
										m += '<span class="users">'+num(item.total_members)+'</span>';
									} else if(f != 'albums'){
										m += '<span class="likes">'+num(item.stats_number_of_likes)+'</span><span class="views">'+num(item.stats_number_of_plays)+'</span><span class="comments">'+num(item.stats_number_of_comments)+'</span>';
									}
									html.push('<span class="meta">'+m+'</span>');
								}
								var dt = item.upload_date;
								if(f == 'likes'){
									dt = item.liked_on;
								} else if(f == 'albums' || f == 'channels' || f == 'groups'){
									dt = item.created_on;
								}
								html.push('<span class="date">'+dt+'</span>');
								sq = item.url;
								st = at;
								break;	
								
								case 'stumbleupon':
								var src = $('img',item.content).attr('src');
								if(src && o.feed == 'favorites'){
									html.push('<a href="'+item.link+'" class="thumb"><img src="'+src+'" alt="" /></a>');
								}
								html.push(x+item.contentSnippet);
								break;
								
								case 'instagram':
								d = parseInt(item.created_time * 1000,10);
								html.push('<a href="'+item.link+'" class="thumb"><img src="' + item.images[o.thumb].url + '" alt="" /></a>');
								if(item.caption !=null){
									html.push(item.caption.text);
								}
								if(item.comments.count > 0 && o.comments > 0){
									i = 0;
									html.push('<span class="meta"><span class="comments">'+num(item.comments.count)+' comments</span></span>');
									$.each(item.comments.data, function(i,cmt){
										if(o.comments > i){
											html.push('<span class="meta item-comments"><img src="'+cmt.from.profile_picture+'" />'+cmt.from.full_name+' - '+cmt.text+'</span>');
											i++;
										} else {
											return false;
										}
									});
								}
								if(item.likes.count > 0 && o.likes > 0){
									i = 0;
									html.push('<span class="meta"><span class="likes">'+num(item.likes.count)+' likes</span></span>');
									html.push('<span class="meta item-likes">');
									$.each(item.likes.data, function(i,lk){
										if(o.likes > i){
											html.push('<img src="'+lk.profile_picture+'" />');
											i++;
										} else {
											return false;
										}
									});
									html.push('</span>');
								}
								st = item.caption !=null ? item.caption.text : '' ;
								break;
								
								case 'google':
								var g = item.object.replies ? num(item.object.replies.totalItems) : 0, m = item.object.plusoners ? num(item.object.plusoners.totalItems) : 0, p = item.object.resharers ? num(item.object.resharers.totalItems) : 0, dl;
								d = item.published;
								dl = {src: "",imgLink: "",useLink: "",useTitle: ""};
								var k = item.object.attachments;
								if (k) if (k.length){
									for (var l = 0; l < k.length; l++) {
										var h = k[l];
										if (h.image) {
											dl.src = h.image.url;
											dl.imgLink = h.url;
											if (h.fullImage) {
												dl.w = h.fullImage.width || 0;
												dl.h = h.fullImage.height || 0
											}
										}
										if (h.objectType == "article") dl.useLink = h.url;
										if (h.displayName) dl.useTitle = h.displayName
									}
									if (!dl.useLink) dl.useLink = dl.imgLink;
									var img_h = o.image_height ? o.image_height : 75 ;
									var img_w = o.image_width ? o.image_width : 75 ;
									if (dl.src.indexOf("resize_h") >= 0) dl.src = dl.w >= dl.h ? dl.src.replace(/resize_h=\d+/i, "resize_h=" + img_h) : dl.src.replace(/resize_h=\d+/i, "resize_w=" + img_w)
								}
								dl = dl;
								html.push((dl.src ? (dl.useLink ? '<a href="' + dl.useLink + '" class="thumb">' : '')+'<img src="' + dl.src + '" />'+(dl.useLink ? '</a>' : '') : ''));
								var t1 = px/(dl.w/dl.h) < px/3 ? ' clear' : '' ;
								html.push((dl.useLink ? '<a href="' + dl.useLink + '" class="title'+t1+'">' : '')+(item.title ? item.title : dl.useTitle)+(dl.useLink ? '</a>' : ''));
								if(o.shares){
									html.push('<span class="meta"><span class="plusones">+1s '+m+'</span><span class="shares">'+p+'</span><span class="comments">'+g+'</span></span>');
								}
								sq = dl.useLink;
								st = dl.useTitle;
								break;
							}
							if(share == true){
								s = shareLink(st,sq,tweetId,type);
								html.push('<span class="section-share">'+s+'</span>');
							}
							if(type == 'twitter'){
								d = d != '' ? html.push('<span class="date"><a href="https://twitter.com/'+un+'/status/'+item.id_str+'">'+nicetime(new Date(d).getTime())+'</a></span></li>') : '' ;
							} else {
								d = d != '' ? html.push('<span class="date">'+nicetime(new Date(d).getTime())+'</span></li>') : '' ;
							}
						}
					});
				} else {
					html.push('<li class="dcsmt-error">Error. '+error+'</li>');
				}
				$(x).html(html.join(''));
			}
		});
	};
		
	function getFrame(src,w,h){
		html = '<iframe src="'+src+'" scrolling="no" frameborder="0" style="border: none; background: #fff; overflow: hidden; width: '+w+'px; height: '+h+'px;" allowTransparency="true"></iframe>';
		return html;
	};
	
	function getLinkedIn(id,a,b,w){
		id = a == 'JYMBII' ? 'data-companyid="'+id+'"' : 'data-id="'+id+'"' ;
		out = '<script type="IN/'+a+'" data-width="'+w+'" '+id+b+' data-format="inline"></script>';
		return out;
	};
		
	function linkify(text){
		text = text.replace(
			/((https?\:\/\/)|(www\.))(\S+)(\w{2,4})(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?/gi,
			function(url){
				var full_url = !url.match('^https?:\/\/') ? 'http://' + url : url ;
				return '<a href="' + full_url + '">' + url + '</a>';
			}
		);
		text = text.replace(/(^|\s)@(\w+)/g, '$1@<a href="https://www.twitter.com/$2">$2</a>');
		text = text.replace(/(^|\s)#(\w+)/g, '$1#<a href="https://twitter.com/search/%23$2">$2</a>');
		return text;
	}

	function parseTwitterDate(a){
		var out = navigator.userAgent.match(/msie [6]/i) ? a.replace(/(\+\S+) (.*)/, '$2 $1') : a ;
		return out;
	}
	
	function stripHtml(v){
		var $html = $(v);
		return $html.text();
	}
	
	function shareLink(st,sq,tweetId,type){
		var s = '';
		if(type == 'twitter'){
			var intent = 'https://twitter.com/intent/';
			s = '<a href="'+intent+'tweet?in_reply_to='+sq+'&via='+tweetId+'" class="share-reply"></a>';
			s += '<a href="'+intent+'retweet?tweet_id='+sq+'&via='+tweetId+'" class="share-retweet"></a>';
			s += '<a href="'+intent+'favorite?tweet_id='+sq+'" class="share-favorite"></a>';
		} else {
			var sq = encodeURIComponent(sq), st = encodeURIComponent(st);
			s = '<a href="http://www.facebook.com/sharer.php?u='+sq+'&t='+st+'" class="share-facebook"></a>';
			s += '<a href="https://twitter.com/share?url='+sq+'&text='+st+'&via='+tweetId+'" class="share-twitter"></a>';
			s += '<a href="https://plus.google.com/share?url='+sq+'" class="share-google"></a>';
			s += '<a href="http://www.linkedin.com/shareArticle?mini=true&url='+sq+'&title='+st+'" class="share-linkedin"></a>';
			return s;
		}
		return s;
    }
	
		function nicetime(a){
			var d = Math.round((+new Date - a) / 1000), fuzzy = '';
			var chunks = new Array();
                    chunks[0] = [60 * 60 * 24 * 365 , 'year', 'years'];
                    chunks[1] = [60 * 60 * 24 * 30 , 'month', 'months'];
                    chunks[2] = [60 * 60 * 24 * 7, 'week', 'weeks'];
                    chunks[3] = [60 * 60 * 24 , 'day', 'days'];
                    chunks[4] = [60 * 60 , 'hr', 'hrs'];
                    chunks[5] = [60 , 'min', 'mins'];
                    var i = 0, j = chunks.length;
                    for (i = 0; i < j; i++) {
                        s = chunks[i][0];
                        if ((xj = Math.floor(d / s)) != 0)
                        {
                            n = xj == 1 ? chunks[i][1] : chunks[i][2] ;
                            break;
                        }
                    }
                    fuzzy += xj == 1 ? '1 '+n : xj+' '+n ;
                    if (i + 1 < j) {
                        s2 = chunks[i + 1][0];
                        if ( ((xj2 = Math.floor((d - (s * xj)) / s2)) != 0) )
                        {
                            n2 = (xj2 == 1) ? chunks[i + 1][1] : chunks[i + 1][2] ;
                            fuzzy += (xj2 == 1) ? ' + 1 '+n2 : ' + '+xj2+' '+n2 ;
                        }
                    }
					fuzzy += ' ago';
            return fuzzy;
		}

		function num(a){
            var b = a;
            if (a > 999999) b = Math.floor(a / 1E6) + "M";
            else if (a > 9999) b = Math.floor(a / 1E3) + "K";
            else if (a > 999) b = Math.floor(a / 1E3) + "," + a % 1E3;
            return b
        };
		
		function parseQ(url){
			var v = [], hash, q = url.split('?')[1];
			if(q != undefined){
				q = q.split('&');
				for(var i = 0; i < q.length; i++){
					hash = q[i].split('=');
					v.push(hash[1]);
					v[hash[0]] = hash[1];
				}
			}
			return v;
		};
		
		function ticker(s,b,speed){
			var $a = $('li:last',s),$b = $('li:first',s),$gx;
			if(b == 'next'){
				$gx = $a.clone().hide().css({opacity: 0});
				$b.before($gx);
				$a.remove();
				$gx.slideDown(speed,'linear',function(){
					$(this).animate({opacity: 1},speed);
				});
			} else {
				var bh = $b.outerHeight(true);
				$gx = $b.clone();
				$b.animate({marginTop: -bh+'px',opacity: 0},speed,'linear',function(){
					$a.after($gx);
					$b.remove();
				});
			}
		};
		
		function fbLink(id,obj){
			var link = '';
			jQuery.ajax({
				url: 'https://graph.facebook.com/'+id,
				cache: true,
				dataType: 'jsonp',
				async: false,
				success: function(a){
					obj.attr('href',a.link);	
				}
			});
		};
		
})(jQuery);

jQuery(window).load(function(){
	jQuery.getScript("//platform.twitter.com/widgets.js", function(){});
});