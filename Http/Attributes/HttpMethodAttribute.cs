using Http.HTTP;

namespace Http.Attributes
{
    public abstract class HttpMethodAttribute:Attribute
    {
        public Method HttpMethod { get; }
        protected HttpMethodAttribute(Method httpMethod) => HttpMethod = httpMethod;
    }
}
