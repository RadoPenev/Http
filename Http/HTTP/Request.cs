using Http.Common;
using System.Threading.Tasks.Sources;
using System.Web;

namespace Http.HTTP
{
    public class Request
    {
        private static Dictionary<string, Session> Sessions = new(); 

        public Method Method { get; private set; }
        public string URL { get; private set; }
        public HeaderCollection Headers { get; private set; }
        public CookieCollection Cookies { get; private set; }
        public string Body { get; private set; }

        public Session Session { get; private set; }

        public IReadOnlyDictionary<string, string> Form { get; private set; }
        public IReadOnlyDictionary<string, string> Query { get; private set; }
        public static IServiceCollection ServiceCollection { get; private set; }

        public static Request Parse(string request,IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
            var lines = request.Split("\r\n");
            var firstLine = lines.First().Split(' ');
            var method = ParseMethod(firstLine[0]);
            (string url, Dictionary<string, string> query) = ParseUrl(firstLine[1]);
            
            HeaderCollection headers = ParseHeaders(lines.Skip(1));
            var cookies = ParseCookies(headers);
            var session=GetSession(cookies);
            var bodyLines = lines.Skip(headers.Count + 2);
            string body = string.Join("\r\n", bodyLines);
            var form = ParseForm(headers, body);
            return new Request()
            {
                Method = (Method)method,
                URL = url,
                Headers = headers,
                Cookies = cookies,
                Session=session,
                Body = body,
                Form = form,
                Query = query
        };
    }

        private static (string url, Dictionary<string, string> query) ParseUrl(string queryString)
        {
            string url=String.Empty;
            Dictionary<string, string> query = new();
            var parts= queryString.Split("?",2);

            if (parts.Length==1)
            {
                url = parts[0];
            }
            else
            {
                var queryParams= parts[1].Split("&");

                foreach (var pair in queryParams)
                {
                    var param=pair.Split("=");
                    if (param.Length == 2)
                    {
                        query.Add(param[0], param[1]);
                    }
                }
            }

            return (url,query);
        }

        private static CookieCollection ParseCookies(HeaderCollection headers)
        {
            var cookieCollection=new CookieCollection();

            if (headers.Contains(Header.Cookie))
            {
                var cookieHeader = headers[Header.Cookie];
                var allCookies = cookieHeader.Split(";", StringSplitOptions.RemoveEmptyEntries);

                foreach (var cookieText in allCookies)
                {
                    var cookieParts = cookieText.Split('=', StringSplitOptions.RemoveEmptyEntries);

                    var cookieName = cookieParts[0];
                    var cookieValue = cookieParts[1];

                    cookieCollection.Add(cookieName?.Trim(), cookieValue?.Trim());
                }
            }

            return cookieCollection;
        }

    private static HeaderCollection ParseHeaders(IEnumerable<string> lines)
    {
        var headers = new HeaderCollection();

        foreach (var line in lines)
        {
            if (line == string.Empty)
            {
                break;
            }

            var parts = line.Split(": ");
            if (parts.Length != 2)
            {
                throw new InvalidOperationException("Request headers invalid");

            }

            headers.Add(parts[0], parts[1].Trim());
        }
        return headers;
    }

    private static object ParseMethod(string method)
    {
        try
        {
            return Enum.Parse<Method>(method);
        }
        catch (Exception)
        {
            throw new InvalidOperationException($"Method {method} is not supported");
        }
    }



    private static Dictionary<string, string> ParseForm(HeaderCollection headers, string body)
    {
        var formCollection = new Dictionary<string, string>();

        if (headers.Contains(Header.ContentType) && headers[Header.ContentType] == ContentType.FormUrlEncoded)
        {
            Dictionary<string, string> parsedFormData = ParseFormData(body);

            foreach (KeyValuePair<string, string> pair in parsedFormData)
                formCollection.Add(pair.Key, pair.Value);
        }

        return formCollection;
    }

    private static Dictionary<string, string> ParseFormData(string bodyLines)
=> HttpUtility.UrlDecode(bodyLines)
    .Split('&')
    .Select(part => part.Split('='))
    .Where(part => part.Length == 2)
    .ToDictionary(
    part => part[0],
    part => part[1],
    StringComparer.InvariantCultureIgnoreCase);

        private static Session GetSession(CookieCollection cookies)
        {
            var sessionId = cookies.Contains(Session.SessionCookieName)
            ? cookies[Session.SessionCookieName]
            :Guid.NewGuid().ToString();

            if (!Sessions.ContainsKey(sessionId))
            {
                Sessions[sessionId]=new Session(sessionId);
            }

            return Sessions[sessionId];
        }   

        
   }
}

