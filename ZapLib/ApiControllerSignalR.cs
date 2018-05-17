using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ZapLib
{
    public abstract class ApiControllerSignalR<T> : ApiController where T : IHub
    {
        Lazy<IHubContext> hub = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<T>());

        protected IHubContext Hub
        {
            get { return hub.Value; }
        }

        [NonAction]
        public bool IsConnectionIdAlive(string connectionId)
        {
            var heartBeat = GlobalHost.DependencyResolver.Resolve<ITransportHeartbeat>();
            ITrackingConnection res = heartBeat.GetConnections().Where(trackConnection => trackConnection.ConnectionId == connectionId).FirstOrDefault();
            return res == null ? false : res.IsAlive;
        }

        [NonAction]
        public void ResolveConnectionIds(IList<string> connectionIds, out IList<string> Alive, out IList<string> Dead)
        {
            Alive = new List<string>();
            Dead = new List<string>();
            if (connectionIds == null) return;
            var heardbeat = GlobalHost.DependencyResolver.Resolve<ITransportHeartbeat>();
            Dictionary<string, bool> connections = new Dictionary<string, bool>();

            foreach (var connection in heardbeat.GetConnections())
                connections.Add(connection.ConnectionId, connection.IsAlive);
            

            foreach(var id in connectionIds)
            {
                if (connections.ContainsKey(id)){
                    var isAlive = false;
                    if (connections.TryGetValue(id, out isAlive))
                    {
                        if (isAlive)
                        {
                            Alive.Add(id);
                            continue;
                        }
                    }
                }
                Dead.Add(id);
            }      
        }

    }
}
