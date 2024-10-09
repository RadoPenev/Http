using Http.HTTP;

namespace Http.Attributes
{
    internal class HttpPostAttribute : HttpMethodAttribute
    {
        public HttpPostAttribute() : base(Method.POST)
        {
        }
    }
}
