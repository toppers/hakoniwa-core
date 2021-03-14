using Hakoniwa.Core.Asset;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hakoniwa.Core.Simulation
{
    public class RequestManager
    {
        private List<Request> req_list;

        public RequestManager()
        {
            this.req_list = new List<Request>();
        }

        public void PutEvent(CoreAssetNotificationEvent ev)
        {
            Request req = new Request(ev);
            this.req_list.Add(req);
        }

        public Request GetEvent()
        {
            if (this.req_list.Count > 0)
            {
                Request req = this.req_list[0];
                this.req_list.RemoveAt(0);
                return req;
            }
            else
            {
                return null;
            }
            
        }
    }

}
