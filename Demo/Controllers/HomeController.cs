using Http.Controllers;
using Http.HTTP;
using System.Text;
using System.Web;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        private const string HtmlForm = @"<form action='/HTML' method='POST'>
                                          Name:<input type='text' name='Name'/>
                                          Age:<input type='number' name='Age'/>
                                        <input type='submit' value='Save'/>
                                        </form>";

        private const string DownloadForm = @"<form action='/Content' method='POST'>
                                              <input type='submit' value ='Download Sites Content' />
                                              </form>";

        private const string FileName = "content.txt";


        public HomeController(Request Request):base(Request)
        {
            
        }

        public Response Index() => Text("Hello from the server!");

        public Response Redirect() => Redirect("https://softuni.org");

        public Response Html()=> Html(HtmlForm);

        public Response HtmlFormPost()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var (key,value) in Request.Form)
            {
                sb.AppendLine($"{key} - {value}");
            }

            return Text(sb.ToString());
        }

        

        public Response Content()=>Html(DownloadForm);

        public Response DownloadContent() => File(FileName);

        public Response Cookies()
        {
            

            var RequestHasCookies = Request.Cookies.Any(c => c.Name != Http.HTTP.Session.SessionCookieName);
            var bodyText = "";

            if (RequestHasCookies)
            {
                var cookieText = new StringBuilder();
                cookieText.AppendLine("<h1>Cookies</h1>");

                cookieText.Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");

                foreach (var cookie in Request.Cookies)
                {
                    cookieText.Append("<tr>");
                    cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                    cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
                    cookieText.Append("</tr>");
                }
                cookieText.Append("</table>");
                bodyText = cookieText.ToString();

                return Html(bodyText);
            }
            else
            {
                bodyText = "<h1>Cookies set!</h1>";
            }

            var cookies = new CookieCollection();

            if (!RequestHasCookies)
            {
                cookies.Add("My-Cookie", "My-Value");
                cookies.Add("My-Second-Cookie", "My-Second-Value");
            }
           

            return Html(bodyText,cookies);
        }

        public Response Session()
        {
            var sessionExists = Request.Session
                   .ContainsKey(Http.HTTP.Session.SessionCurrentDateKey);

            var bodyText = "";

            if (sessionExists)
            {
                var currentDate = Request.Session[Http.HTTP.Session.SessionCurrentDateKey];
                bodyText = $"Stored date: {currentDate}!";
            }
            else
            {
                bodyText = "Current date stored";
            }

            return Text(bodyText);
        }
    }
}
