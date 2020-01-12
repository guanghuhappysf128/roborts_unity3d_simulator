using UnityEngine;
using RosSharp.RosBridgeClient;

namespace Roborts
{
    public abstract class RobortsPublisher<T> : MonoBehaviour
        where T : Message
    {
        public RosConnector rosConnector;
        public string topic;
        private string publicationId;

        protected virtual void Start()
        {
            publicationId = rosConnector.RosSocket.Advertise<T>(topic);
        }

        protected void Publish(T message)
        {
            rosConnector.RosSocket.Publish(publicationId, message);
        }
    }
}
