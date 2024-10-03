using Demo.Controllers;
using Http.HTTP;
using Http.Routing;
using System.Text;
using System.Web;

namespace Http
{
    public class Startup
    {
        private const string LoginForm = @"<form action='/Login' method='POST'>
                                           Username: <input type='text' name='Username'/>
                                           Password: <input type='text' name='Pasword'/>
                                           <input type='submit' value='Log In'/>
                                           </form>";

        private const string Username = "user";
        private const string Password = "user123";
       

        public static async Task Main()
        {

            await new HttpServer(routes => routes
                 .MapGet<HomeController>("/", c => c.Index())
                 .MapGet<HomeController>("/Redirect", c => c.Redirect())
                 .MapGet<HomeController>("/HTML", c => c.Html())
                 .MapPost<HomeController>("/HTML", c => c.HtmlFormPost())
                 .MapGet<HomeController>("/Content", c => c.Content())
                 .MapPost<HomeController>("/Content", c => c.DownloadContent())
                 .MapGet<HomeController>("/Cookies", c => c.Cookies())
                 .MapGet<HomeController>("/Session", c => c.Session())
                 //.MapGet<HomeController>("/Login", new HtmlResponse(Startup.LoginForm))
                 //.MapPost<HomeController>("/Login", new HtmlResponse("", Startup.LoginAction))
                 //.MapGet<HomeController>("/Logout", new HtmlResponse("", Startup.LogoutAction))
                 //.MapGet<HomeController>("/UserProfile", new HtmlResponse("", Startup.GetUserDataAction))
                 ).Start();
        }

       



        private static void LoginAction(Request Request, Response response)
        {
            Request.Session.Clear();

            var bodyText = "";

            var usernameMatches = Request.Form["Username"] == Startup.Username;
            var passwordMatches = Request.Form["Pasword"] == Startup.Password;

            if (usernameMatches && passwordMatches)
            {
                Request.Session[Session.SessionUserKey] = "MyUserId";
                response.Cookies.Add(Session.SessionCookieName, Request.Session.Id);

                bodyText = "<h3>Login successfully!</h3>";
            }
            else
            {
                bodyText = Startup.LoginForm;
            }
            response.Body = "";
            response.Body += bodyText;
        }

        private static void LogoutAction(Request Request, Response response)
        {
            Request.Session.Clear();
            response.Body = "";
            response.Body += "<h3>Logouted successfully!</h3>";
        }

        private static void GetUserDataAction(Request Request, Response response)
        {
            if (Request.Session.ContainsKey(Session.SessionUserKey))
            {
                response.Body = "";
                response.Body += $"<h3>Currently logged-in user " +
                    $"is with username '{Username}'</h3>";
            }
            else
            {
                response.Body = "";
                response.Body += "<h3>You should first log in " +
                    "-<a href='/Login'>Login</a></h3>";
            }
        }

    }

}

