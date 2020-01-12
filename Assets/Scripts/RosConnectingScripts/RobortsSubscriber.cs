using System.Threading;
using UnityEngine;
using RosSharp.RosBridgeClient;

namespace Roborts
{
    public abstract class RobortsSubscriber<T> : MonoBehaviour
        where T : Message
    {
        private static readonly int ConnectorTimeout = 1;

        public RosConnector rosConnector;
        public string nameSpace;
        public string topic;
        public float timeStep;

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
                nameSpace + topic, ReceiveMessage, (int) (timeStep * 1000));
        }

        protected abstract void ReceiveMessage(T message);
    }
}
