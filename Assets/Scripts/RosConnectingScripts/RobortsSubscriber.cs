using System.Threading;
using UnityEngine;
using RosSharp.RosBridgeClient;

namespace Roborts
{
    public abstract class RobortsSubscriber <T> : MonoBehaviour
        where T : Message
    {
        public RosConnector rosConnector;
        public string Namespace;
        public string Topic;
        public float TimeStep;
        private readonly int ConnectorTimeout = 1;

        protected virtual void Start()
        {
            new Thread(Subscribe).Start();
        }

        private void Subscribe()
        {
            if (!rosConnector.IsConnected.WaitOne(ConnectorTimeout * 1000))
            {
                Debug.LogWarning("Failed to subscribe: RosConnector not connected");
            }

            // 2nd param: the rate (in ms in between messages) at which
            // to throttle the topics
            rosConnector.RosSocket.Subscribe<T>(
                Namespace + Topic, ReceiveMessage, (int) (TimeStep * 1000));
        }

        protected abstract void ReceiveMessage(T message);
    }
}
