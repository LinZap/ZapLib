using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ZapLib
{
    /// <summary>
    /// 具備 SignalR 擴充功能的 ApiController
    /// </summary>
    /// <typeparam name="T">自訂的 IHub 類別 T</typeparam>
    public abstract class ApiControllerSignalR<T> : ApiController where T : IHub
    {
        Lazy<IHubContext> hub = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<T>());

        /// <summary>
        /// 取得 IHubContext Hub 物件
        /// </summary>
        protected IHubContext Hub
        {
            get { return hub.Value; }
        }

        /// <summary>
        /// 判斷連線 Id 是否正常連線
        /// </summary>
        /// <param name="connectionId">連線 ID</param>
        /// <returns>是否正常連線</returns>
        [NonAction]
        public bool IsConnectionIdAlive(string connectionId)
        {
            var heartBeat = GlobalHost.DependencyResolver.Resolve<ITransportHeartbeat>();
            ITrackingConnection res = heartBeat.GetConnections().Where(trackConnection => trackConnection.ConnectionId == connectionId).FirstOrDefault();
            return res == null ? false : res.IsAlive;
        }

        /// <summary>
        /// 解析連線 Ids，回傳正常連線與已經失去連線的兩組資料
        /// </summary>
        /// <param name="connectionIds">連線 Id 列表</param>
        /// <param name="Alive">正常連線的列表</param>
        /// <param name="Dead">失去連線的列表</param>
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
