using UnityEngine;
using RosSharp;
using RosTwist = RosSharp.RosBridgeClient.MessageTypes.Geometry.Twist;
using RosVector3 = RosSharp.RosBridgeClient.MessageTypes.Geometry.Vector3;

namespace Roborts
{
    public sealed class RobortsVelocitySubscriber : RobortsSubscriber<RosTwist>
    {
        public Vector3 linearVelocity;
        public Vector3 angularVelocity;
        public Rigidbody subscribedRigidbody;
        public Vector3 m_EulerAngleVelocity;
        private bool messageReceived;

        public RobortsVelocitySubscriber()
        {
            topic = "/cmd_vel";
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void ReceiveMessage(RosTwist message)
        {
            Debug.Log("Message received");
            linearVelocity = LinearVelocityToVector3(message.linear).Ros2Unity();
            angularVelocity = AngularVelocityToVector3(message.angular).Ros2Unity();
            messageReceived = true;
        }

        private static Vector3 LinearVelocityToVector3(RosVector3 geometryVector3)
        {
            return new Vector3((float) geometryVector3.y, (float) -geometryVector3.x, 0);
        }

        private static Vector3 AngularVelocityToVector3(RosVector3 geometryVector3)
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
            subscribedRigidbody.velocity = linearVelocity;
            subscribedRigidbody.angularVelocity = angularVelocity;
            messageReceived = false;
        }
    }
}
