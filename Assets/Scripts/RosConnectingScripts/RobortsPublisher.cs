using UnityEngine;
using RosSharp.RosBridgeClient;

namespace Roborts
{
    public abstract class UnityRobortsPublisher<T> : MonoBehaviour
        where T : Message
    {
        public RosConnector rosConnector;
        public string Topic;
        private string publicationId;

        protected virtual void Start()
        {
            publicationId = rosConnector.RosSocket.Advertise<T>(Topic);
        }

        protected void Publish(T message)
        {
            rosConnector.RosSocket.Publish(publicationId, message);
        }
    }
}
