using Http.HTTP;

namespace Http.Attributes
{
    internal class HttpGetAttribute : HttpMethodAttribute
    {
        public HttpGetAttribute() : base(Method.GET)
        {
        }
    }
}
