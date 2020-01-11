using UnityEngine;
using RosSharp;

namespace Roborts
{
    public class RobortsVelocitySubscriber :
        RobortsSubscriber<RosSharp.RosBridgeClient.MessageTypes.Geometry.Twist>
    {
        public Vector3 linearVelocity;
        public Vector3 angularVelocity;
        public Rigidbody SubscribedRigidbody;
        public Vector3 m_EulerAngleVelocity;
        private bool messageReceived;

        public RobortsVelocitySubscriber()
        {
            Topic = "/cmd_vel";
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void ReceiveMessage(RosSharp.RosBridgeClient.MessageTypes.Geometry.Twist message)
        {
            Debug.Log("Message received");
            linearVelocity = linearVelocityToVector3(message.linear).Ros2Unity();
            angularVelocity = angularVelocityToVector3(message.angular).Ros2Unity();
            messageReceived = true;
        }

        private static Vector3 linearVelocityToVector3(RosSharp.RosBridgeClient.MessageTypes.Geometry.Vector3 geometryVector3)
        {
            return new Vector3((float) geometryVector3.y, (float) -geometryVector3.x, 0);
        }

        private static Vector3 angularVelocityToVector3(RosSharp.RosBridgeClient.MessageTypes.Geometry.Vector3 geometryVector3)
        {
            return new Vector3(0, 0, (float) -geometryVector3.z);
        }

        private void FixedUpdate()
        {
            if (messageReceived)
            {
                ProcessMessage();
            }
        }

        private void ProcessMessage()
        {
            SubscribedRigidbody.velocity = linearVelocity;
            SubscribedRigidbody.angularVelocity = angularVelocity;
            messageReceived = false;
        }
    }
}
