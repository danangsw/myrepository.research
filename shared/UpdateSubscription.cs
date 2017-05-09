using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyRepository
{
    public class UpdateSubscription : MarshalByRefObject
    {
        Action<string, object> _handler;

        public Action<string, object> Handler
        {
            get { return _handler; }
            set { _handler = value; }
        }

        public UpdateSubscription(Action<string, object> handler)
        {
            _handler = handler;
        }
    }
}
