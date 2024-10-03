using Http.Controllers;
using Http.HTTP;

namespace Demo.Controllers
{
    public class UserController : Controller
    {
        private const string LoginForm = @"<form action='/Login' method='POST'>
                                           Username: <input type='text' name='Username'/>
                                           Password: <input type='password' name='Pasword'/>
                                           <input type='submit' value='Log In'/>
                                           </form>";

        private const string Username = "user";
        private const string Password = "user123";


        public UserController(Request request) : base(request)
        {
        }

        public Response LoginUser()
        {
            Request.Session.Clear();

            var bodyText = "";

            var usernameMatches = Request.Form["Username"] == Username;
            var passwordMatches = Request.Form["Pasword"] == Password;

            if (usernameMatches && passwordMatches)
            {
                Request.Session[Session.SessionUserKey] = "MyUserId";
                CookieCollection cookies = new CookieCollection();
                cookies.Add(Session.SessionCookieName, Request.Session.Id);

                bodyText = "<h3>Login successfully!</h3>";

                return Html(bodyText, cookies);
            }
            else
            {
                bodyText = LoginForm;
            }
            return Html(bodyText);
        }

        public Response Login() => Html(LoginForm);

        public Response Logout()
        {
            Request.Session.Clear();
       
           return Html("<h3>Logouted successfully!</h3>");
        }

        internal Response GetUserData()
        {
            if (Request.Session.ContainsKey(Session.SessionUserKey))
            {
                return Html( $"<h3>Currently logged-in user " +
                    $"is with username '{Username}'</h3>");
            }
            return Redirect("/Login");
        }
    }
}
