using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.IO;
using System.Linq;
using System.Dynamic;
using System.Collections;
using System.Text;

//you can include this in your WebMatrix app and run from there
//instead of the console runner
namespace QuixoteWeb{
    public static class Runner {
        /// <summary>
        /// This is a helper method that will decipher the root URL of your site
        /// </summary>
        public static string SiteRoot(bool includeAppPath=true) {
            var context = HttpContext.Current;
            var Port = context.Request.ServerVariables["SERVER_PORT"];
            if (Port == null || Port == "80" || Port == "443")
                Port = "";
            else
                Port = ":" + Port;
            var Protocol = context.Request.ServerVariables["SERVER_PORT_SECURE"];
            if (Protocol == null || Protocol == "0")
                Protocol = "http://";
            else
                Protocol = "https://";

            var appPath = "";
            if (includeAppPath) {
                appPath = context.Request.ApplicationPath;
                if (appPath == "/")
                    appPath = "";
            }
            var sOut = Protocol + context.Request.ServerVariables["SERVER_NAME"] + Port + appPath;
            return sOut;

        }
        /// <summary>
        /// A bit verbose, but since we're using dynamics you can't attach an extension method - which is sad.
        /// This includes core system LINQ stuff, like "FirstOrDefault".
        /// </summary>
        static string FindMatch(string source, string find) {
            Regex reg = new Regex(find, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
            return reg.Match(source).Value;

        }
        /// <summary>
        /// Matching core for loading up stuff on a web page
        /// </summary>
        static List<string> FindMatches(string source, string find) {
            Regex reg = new Regex(find, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

            List<string> result = new List<string>();
        
            foreach (Match m in reg.Matches(source))
                result.Add(m.Value);
            return result;
        }

        /// <summary>
        /// This method parses the Get request into digestable little chunks so you can query the response's title, links,
        /// headings, body, body links, etc.
        /// </summary>
        static void ParseResult(dynamic result) {
            string bodyPattern = "(?<=<body>).*?(?=</body>)";
            string titlePattern = "(?<=<title>).*?(?=</title>)";
            string linkPattern = "<a.*?>.*?</a>";
            string headPattern = @"(?<=<h\d\w*?>).*?(?=</h\d>)";

            //have to cast this ToString() as result.Html is a dynamic (which is actually an HtmlString)
            var html = result.Html.ToString();

            result.Title = FindMatch(html, titlePattern);
            result.Links = FindMatches(html, linkPattern);
            result.Headings = FindMatches(html, headPattern);
            result.Body = FindMatch(html, bodyPattern);
            result.BodyLinks = FindMatches(result.Body.ToString(), linkPattern);
        }

        /// <summary>
        /// Resets the virtual URL to an absolute so the web request knows what to do
        /// Won't accept URLs that aren't virtual
        /// </summary>
        static string DecipherLocalUrl(string url) {
            //this URL needs to be local
            if (url.Contains("http")) {
                throw new InvalidOperationException("Get() only supports local URLs. You can use absolute or relative");
            }

            //convert it...
            var relUrl = VirtualPathUtility.ToAbsolute(url);
            var absUrl = SiteRoot(false) + relUrl;
            return absUrl;

        }
        /// <summary>
        /// Reads the Response and pops the result into a WebResponse
        /// </summary>
        static WebResponse ReadAndLoadResponse(HttpWebRequest request, Cookie cookie) {
            var result = new WebResponse();
            //add the cookie if needed
            if (cookie != null) {
                request.CookieContainer.Add(cookie);
            }
            // Execute the query
            var response = (HttpWebResponse)request.GetResponse();
            result.Response = response;
            result.Url = result.Response.ResponseUri.AbsoluteUri;
            result.Code = result.Response.StatusCode;
        
            using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
                result.Html = sr.ReadToEnd();
                sr.Close();
            }
            ParseResult(result);
            return result;

        }

        /// <summary>
        /// Sends a POST request to your app, with the postData being an Anonymous object
        /// </summary>
        public static WebResponse Post(string url, object postData, Cookie cookie=null) {
        
            //this URL needs to be local
            var absUrl = DecipherLocalUrl(url);

            //deserialize the form values
            var list = new List<string>();
            var postParams = "";
            //this will be used later on
            var formValues = new Dictionary<string,object>();

            if(postData != null){
                var props = postData.GetType().GetProperties();
                var d = (IDictionary)formValues;
                foreach (var prop in props)
	            {
		            var key = prop.Name;
                    var val = prop.GetValue(postData,null);
                    d.Add(key, val);
                    list.Add(string.Format("{0}={1}", key, HttpUtility.UrlEncode(val.ToString())));
	            }
                postParams = String.Join("&", list.ToArray());
            }

            // Set the encoding type
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(absUrl);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            // Build a string containing all the parameters
            request.ContentLength = postParams.Length;

            // We write the parameters into the request
            using (StreamWriter sw = new StreamWriter(request.GetRequestStream())) {
                sw.Write(postParams);
                sw.Close();
            }
            var result = ReadAndLoadResponse(request,cookie);
            result.FormValues = formValues;
            return result;
        }

        /// <summary>
        /// Will run a GET request on your site
        /// </summary>
        public static WebResponse Get(string url, Cookie cookie=null) {
            var absUrl = DecipherLocalUrl(url);
            var result = new WebResponse();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(absUrl);

            return ReadAndLoadResponse(request,cookie);
        }
    
    }

    public static class TheFollowing {
        /// <summary>
        /// Helps readability if you describe the context of your tests
        /// </summary>
        public static dynamic Describes(string context) {
            return new HtmlString(string.Format("<h2>{0}</h2>", context));
        }
    }
    /// <summary>
    /// Syntactic Sugar
    /// </summary>
    public static class They {
        public static dynamic Should(string name,Func<dynamic> action){
            return It.Should(name,action);
        }
        public static dynamic ShouldNot(string name, Func<dynamic> action) {
            return It.Should(name, action);
        }
        public static dynamic Should(string name) {
            return It.Should(name);
        }
        public static dynamic ShouldNot(string name) {
            return It.Should(name);
        }
    }
    /// <summary>
    /// The core execution class of Quixote
    /// </summary>
    public static class It{

        /// <summary>
        /// Executes the test call
        /// </summary>
        public static dynamic ExecuteIt(Func<dynamic> action) {
            dynamic result = new System.Dynamic.ExpandoObject();
            try{
                result = action.Invoke();
                result.CSS = result.Passed ? "pass" : "fail";
            }catch(Exception x){
                result.Passed = false;
                result.CSS =  "fail";
                result.Result = new HtmlString(BuildExceptionMessage(x).Trim());
                result.Message = x.Message;
                result.Threw = true;
            }
        
            return result;
        }

        public static string BuildExceptionMessage(Exception x) {

            Exception logException = x;
            if (x.InnerException != null)
                logException = x.InnerException;
            var sb = new StringBuilder();
            sb.AppendLine("Error in Path :" + HttpContext.Current.Request.Path);

            // Get the QueryString along with the Virtual Path
            sb.AppendLine("Raw Url :" + HttpContext.Current.Request.RawUrl);


            // Get the error message
            sb.AppendLine("Message :" + logException.Message);

            // Source of the message
            sb.AppendLine("Source :" + logException.Source);

            // Stack Trace of the error

            sb.AppendLine("Stack Trace :" + logException.StackTrace);

            // Method where the error occurred
            sb.AppendLine("TargetSite :" + logException.TargetSite);
            return sb.ToString();
        }


        /// <summary>
        /// There is no difference between Should/ShouldNot. Just readability
        /// </summary>
        public static dynamic ShouldNot(string name) {
            return Should(name);
        }
        public static dynamic ShouldNot(string name, Func<dynamic> action) {
            return Should(name, action);
        }
        /// <summary>
        /// Stubs out a test and marks it as "pending"
        /// </summary>
        public static dynamic Should(string name) {
            var result = @"<div class='result'>
                <div class='pending'>Should <b>{0}</b> - pending</div>
            </div> ";
            return new HtmlString(string.Format(result, name));
        }
        /// <summary>
        /// A method that prepares a test for execution
        /// </summary>
        public static dynamic Should(string name, Func<dynamic> action) {
            var test = ExecuteIt(action);
            var result = "";
            if (test.Threw) {
                result = @"<div class='result'>
                <div class='fail'>Should {0} - Exception Thrown: {1}</div>
                    <div class='exception'>
                        <textarea class = 'exception-box'>{2}</textarea>
                    </div>
                </div>";
                result = string.Format(result, name, test.Message, test.Result);
            }
            else
            {
                result = @"<div class='result'>
                    <div class='{0}'>
                        Should 
                        {1}
                    </div>
                </div>";
                if (test.Passed) {
                    result = string.Format(result,test.CSS,name);
                }else{
                    result = string.Format(result, test.CSS, name + ": " + test.Message);
                }
            }
            return new HtmlString(result);
        }
    }

    /// <summary>
    /// Wish I could use dynamic for this - but I need to have a higher order of logic here, and I also need to be able
    /// to attach extension methods to it for the assertions. Dynamics can't have extension methods :( and I want you to 
    /// be able to run "response.Title.ShouldContain("Sex Appeal");
    /// </summary>
    public class WebResponse {

        public string Body { get; set; }
        public HttpStatusCode Code { get; set; }
        public string Url { get; set; }
        public HttpWebResponse Response { get; set; }
        public HttpWebRequest Request { get; set; }
        public string Html { get; set; }
        //Html Stuff
        public string Title { get; set; }
        public List<string> Links { get; set; }
        public List<string> BodyLinks { get; set; }
        public List<string> Headings { get; set; }
        public Dictionary<string,object> FormValues { get; set; }
    }

    /// <summary>
    /// This is the assertion library. These are all Extension methods except for ShouldThrow and ShouldNotThrow
    /// Add to it as you will :)
    /// </summary>
    public static class Assert {

        static dynamic InitResult() {
            dynamic result = new ExpandoObject();
            result.Passed = "false";
            result.CSS = "fail";
            result.Threw = false;
            result.Message = "";
            return result;
        }

        public static dynamic ShouldBeTrue(this bool condition) {

            dynamic result = InitResult();
            result.Passed = condition;
            return result;
        }
        public static dynamic ShouldBeFalse(this bool condition) {

            dynamic result = InitResult();
            result.Passed = !condition;
            return result;
        }
        public static dynamic ShouldBeGreaterThan(this int o, int item) {

            dynamic result = InitResult();
            result.Passed = o > item;
            if (!result.Passed) {
                result.Message = o.ToString() + " is less than " + item.ToString();
            }

            return result;
        }
        public static dynamic ShouldBeGreaterThan(this decimal o, decimal item) {

            dynamic result = InitResult();
            result.Passed = o > item;
            if (!result.Passed) {
                result.Message = o.ToString() + " is less than " + item.ToString();
            }

            return result;
        }
        public static dynamic ShouldBeGreaterThan(this double o, double item) {

            dynamic result = InitResult();
            result.Passed = o > item;
            if (!result.Passed) {
                result.Message = o.ToString() + " is less than " + item.ToString();
            }

            return result;
        }
        public static dynamic ShouldBeGreaterThan(this float o, float item) {

            dynamic result = InitResult();
            result.Passed = o > item;
            if (!result.Passed) {
                result.Message = o.ToString() + " is less than " + item.ToString();
            }

            return result;
        }


        public static dynamic ShouldBeLessThan(this int o, int item) {

            dynamic result = InitResult();
            result.Passed = o < item;
            if (!result.Passed) {
                result.Message = o.ToString() + " is greater than " + item.ToString();
            }

            return result;
        }
        public static dynamic ShouldBeLessThan(this decimal o, decimal item) {

            dynamic result = InitResult();
            result.Passed = o < item;
            if (!result.Passed) {
                result.Message = o.ToString() + " is greater than " + item.ToString();
            }
            return result;
        }
        public static dynamic ShouldBeLessThan(this double o, double item) {

            dynamic result = InitResult();
            result.Passed = o < item;
            if (!result.Passed) {
                result.Message = o.ToString() + " is greater than " + item.ToString();
            }
            return result;
        }
        public static dynamic ShouldBeLessThan(this float o, float item) {

            dynamic result = InitResult();
            result.Passed = o < item;
            if (!result.Passed) {
                result.Message = o.ToString() + " is greater than " + item.ToString();
            }
            return result;
        }


        public static dynamic ShouldBeNull(this object item) {
            dynamic result = InitResult();

            result.Passed = item == null;
            if (!result.Passed) {
                result.Message = item.ToString() + " isn't null...";
            }
            return result;
        }

        public static dynamic ShouldNotBeNull(this object item) {
            dynamic result = InitResult();

            result.Passed = item != null;
            if (!result.Passed) {
                result.Message = "Nope - it's null...";
            }

            return result;
        }

        public static dynamic ShouldThrow(Action a) {
            dynamic result = InitResult();

            try {
                a.Invoke();
                result.Passed = false;
                result.Message = "Didnt' throw";
            } catch {
                result.Passed = true;
            }

            return result;
        }
        public static dynamic ShouldNotThrow(Action a) {
            dynamic result = InitResult();

            try {
                a.Invoke();
                result.Passed = true;
            } catch (Exception x) {
                result.Passed = false;
                result.Message = "BOOM - threw a " + x.GetType().Name;
            }

            return result;
        }
        public static dynamic ShouldEqual(this object first, object second) {
            dynamic result = InitResult();
            result.Passed = first.Equals(second);
            if (!result.Passed) {
                result.Message = "Expected " + first.ToString() + " but was " + second.ToString();
            }

            return result;
        }

        public static dynamic ShouldNotEqual(this object first, object second) {
            dynamic result = InitResult();
            result.Passed = !first.Equals(second);
            if (!result.Passed) {
                result.Message = first.ToString() + " is equal to  " + second.ToString();
            }

            return result;
        }
        public static dynamic ShouldContain(this string content, string item) {
            dynamic result = InitResult();
            result.Passed = content.Contains(item);
            if (!result.Passed) {
                result.Message = string.Format("'{0}' not found in sample", item);
            }
            return result;
        }
        public static dynamic ShouldNotContain(this string content, string item) {
            dynamic result = InitResult();
            result.Passed = !content.Contains(item);
            if (!result.Passed) {
                result.Message = string.Format("'{0}' found in sample", item);
            }
            return result;
        }
        public static dynamic ShouldContainItem<T>(this IEnumerable<T> list, object item) {
            dynamic result = InitResult();
            result.Passed = list.Any(x => x.Equals(item));
            if (!result.Passed) {
                result.Message = string.Format("'{0}' not found in list", item);
            }
            return result;
        }
        public static dynamic ShouldContainItem<T>(this T[] list, object item) {
            dynamic result = InitResult();
            result.Passed = list.Any(x => x.Equals(item));
            if (!result.Passed) {
                result.Message = string.Format("'{0}' not found in list", item);
            }
            return result;
        }
        public static dynamic ShouldNotContainItem<T>(this IEnumerable<T> list, object item) {
            dynamic result = InitResult();
            result.Passed = !list.Any(x => x.Equals(item));
            if (!result.Passed) {
                result.Message = string.Format("'{0}' found in list", item);
            }
            return result;
        }

        public static dynamic ShouldContainString<T>(this IEnumerable<T> list, string item) {
            dynamic result = InitResult();
            result.Passed = list.Any(x => x.ToString().Contains(item));
            if (!result.Passed) {
                result.Message = string.Format("'{0}' not found in list", item);
            }
            return result;
        }
        public static dynamic ShouldNotContainString<T>(this IEnumerable<T> list, string item) {
            dynamic result = InitResult();
            result.Passed = !list.Any(x => x.ToString().Contains(item));
            if (!result.Passed) {
                result.Message = string.Format("'{0}' found in list", item);
            }
            return result;
        }
        public static dynamic ShouldContainKey<TKey, TVal>(this IDictionary<TKey, TVal> list, TKey key) {
            dynamic result = InitResult();
            result.Passed = list.ContainsKey(key);
            if (!result.Passed) {
                result.Message = string.Format("'{0}' not found in keys", key);
            }
            return result;
        }
        public static dynamic ShouldNotContainKey<TKey, TVal>(this IDictionary<TKey, TVal> list, TKey key) {
            dynamic result = InitResult();
            result.Passed = !list.ContainsKey(key);
            if (!result.Passed) {
                result.Message = string.Format("'{0}' found in keys", key);
            }
            return result;
        }
        public static dynamic ShouldContainValue<TKey, TVal>(this IDictionary<TKey, TVal> list, TVal val) {
            dynamic result = InitResult();
            result.Passed = list.Values.Contains(val);
            if (!result.Passed) {
                result.Message = string.Format("'{0}' not found in values", val);
            }
            return result;
        }
        public static dynamic ShouldNotContainValue<TKey, TVal>(this IDictionary<TKey, TVal> list, TVal val) {
            dynamic result = InitResult();
            result.Passed = !list.Values.Contains(val);
            if (!result.Passed) {
                result.Message = string.Format("'{0}' found in values", val);
            }
            return result;
        }
        public static dynamic ShouldHaveACountOf<T>(this IEnumerable<T> list, int count) {
            dynamic result = InitResult();
            result.Passed = list.Count() == count;
            if (!result.Passed) {
                result.Message = string.Format("Expected {0} but was {1}", count, list.Count());
            }
            return result;
        }

    }
}