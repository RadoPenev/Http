using Http.Common;

namespace Http.HTTP
{
    public class Session
    {
        public const string SessionCookieName = "MyWebServerSID";
        public const string SessionCurrentDateKey = "CurrentDate";
        public const string SessionUserKey = "AuthenticatedUserId";

        public string Id { get; set; }

        private Dictionary<string, string> data;

        public Session(string id)
        {
            Guard.AgainstNull(id,nameof(id));
            Id = id;
            data=new Dictionary<string, string>();
        }

        public string this[string key]
        {
            get => data[key];
            set => data[key]=value;
        }

        public bool ContainsKey(string key)
            =>data.ContainsKey(key);

        public void Clear()=>this.data.Clear();
    }
}
