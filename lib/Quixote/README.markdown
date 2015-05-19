Quixote is a single-file, drop-in high-level test-runner for ASP.NET/WebMatrix using .NET 4.0
=================================================================================================

If you know Rails, you know RSpec and Cucumber. These high-level testing frameworks allow you to test how your app responds when people use your application (GET and POST requests, etc). These frameworks are pretty advanced - Cucumber allows you to simulate button and link clicks if you like - also entering data into a form field and checking the validations.

Quixote isn't quite that smart yet - but that's where I'd like to go. Right now is does basic assertions on logic with the typical "result.ShouldEqual(expectation)" as well as offering GET and POST functionality against your site - both with the option of sending in cookies.

It's much easier than Watin, prettier than NUnit, and helps people who need to test their WebMatrix app (since there currently isn't a way to test a WebMatrix app). That's the focus - if you need deep testing on your super-swell architected Domain Model From Space - you should push that to a new project and use NUnit or XUnit. Quixote is a bit more high level.


How To Install It?
------------------
Drop Quixote.cs into your App_Code directory (or a place where it can be compiled if you're not using WebMatrix). Then create a "tests" directory (which should be locked down!) and in there, add a page for your tests. Your test page is a simple Razor page - so call your first something like "happiness.cshtml".

At the very top of this new Razor page, add a using statement:

	@using Quixote;

Now you're ready to rock. Write a spec:

	@It.Should("rock me Amadeus")

Now run the page. You're rockin!
	
How Do You Use It?
------------------
You can use this for Acceptance Tests much easier than you can use something like Selenium or WatiN - and that's not a knock on those toolsets. They just take a bit to setup (Selenium requires Firefox and Java, WatiN relies on IE for everything). My goal with this is to make things drop-dead simple...

The simplest thing to do is fire  up a Console App in your project called "AcceptanceTests" and add the Quixote code file. You'll want access to the code ad you'll likely want to change a few things in there.

Then, write some tests:

    class Program {
        static void Main(string[] args) {
            Runner.SiteRoot = "http://localhost:1701/";
            TheFollowing.Describes("Home Page");

            It.Should("Have 'VidPub' in the title", () => {
                return Runner.Get("/").Title.ShouldContain("VidPugb");
            });

            It.Should("Log me in with correct credentials", () => {
                var post = Runner.Post("/account/logon", new { login = "rob@tekpub.com", password = "password" });
                return post.Title.ShouldContain("Welcome");
            });

            Console.Read();
        }
    }

POST and GET are available off of the Runner class and, pretty much do what you see here. You can also drop Quixote into your Web app (MVC or WebMatrix) and run your acceptance tests as a webpage.

A typical flow would be to decide what it is you're doing before you do it. Crazy Talk. Let's say you're going to have a party:
	
	@TheFollowing.Describes("Drinks for the party")
		@They.Should("be bubbly")
		@They.Should("include some type of beer")
		@They.ShouldNot("be Belgian")

When you run the page, the first line will output a nicely formatted title wrapped in an H2. Each spec under it will be marked as pending (and if you use the stylesheet inside of examples it will be yellow).

We have some specs to work with - let's write our Party Code.
(Presto Chango - DONE!)

Now we can go back and write some assertions:

	@TheFollowing.Describes("Drinks for the party")
		@They.Should("be bubbly", () => {
			return Drinks.Find(1).Categories.ShouldContainString("Bubbly");
		})
		
		@They.Should("include some type of beer", () => {
			return Drinks.Find(1).Categories.First().ShouldEqual("Beer");
		})
		
		@They.ShouldNot("be Belgian", () => {
			return Drinks.ShouldNotContainItem(new Category("Belgian"));
		})

Run the page again and your results will splash up on the screen. If you screw up and write bad code, the Exception will be wrapped up and tell you exactly where the problem is. If your test passes, it will be green, if it fails you will have a nice message telling you what the problem is.

Note two things: the first is the use of "Func<Action,dynamic>" - this funky C# shorthand that says "I'm passing in some code that will return a dynamic result". Thus the "return" statement there - that MUST be there or the test won't compile. That returns a bit of love to the renderer so it knows what to do with your code.

In the Examples directory are some examples of the Assertions - and you can just scroll down to the bottom of Quixote - they're all right there.

We can also get freaky and run some functional tests against our site:
	
	@TheFollowing.Describes("the home page")
		@It.Should("say hello to me", () => {
			return Runner.Get("~/").Title.ShouldEqual("Hello Rob");
		})
		@It.Should("have 5 links", () => {
			return Runner.Get("~/").Links.ShouldHaveACountOf(5);
		})
		@It.Should("show me a funny thing if I post 42", () => {
			return Runner.Post("~/",new{EasterEgg = 42}).Body.ShouldContain("1337");
		})

There's a bit more that's possible here - but I would probably start arm-waving. I'll build this out as time goes on (and as I use it) so I can extract usable things rather than what I think would be "neat". 
		
		