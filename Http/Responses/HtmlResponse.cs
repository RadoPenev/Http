using Http.HTTP;

namespace Http.Responses
{
    public class HtmlResponse : ContentResponse
    {
        public HtmlResponse(string text) : base(text, ContentType.Html)
        {
        }
    }
}
