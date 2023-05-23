using Microsoft.AspNetCore.Mvc;

namespace Chat.Filters
{
    public class BasicAuthAttribute
        : TypeFilterAttribute
    {
        public BasicAuthAttribute(Type type) : base(type)
        {
        }

        public BasicAuthAttribute(string realm = @"My Realm")
            : base(typeof(BasicAuthFilter))
        {
            Arguments = new object[] { realm };
        }
    }
}
