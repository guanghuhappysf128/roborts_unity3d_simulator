using System.Threading;
using UnityEngine;
using RosSharp;
using RosTfMessage = RosSharp.RosBridgeClient.MessageTypes.Tf2.TFMessage;
using RosTransformStamped = RosSharp.RosBridgeClient.MessageTypes.Geometry.TransformStamped;
using RosQuaternion = RosSharp.RosBridgeClient.MessageTypes.Geometry.Quaternion;
using RosVector3 = RosSharp.RosBridgeClient.MessageTypes.Geometry.Vector3;

namespace Roborts
{
    public sealed class TfPublisher : RobortsPublisher<RosTfMessage>
    {
        private static uint msgSeq = 0;

        public GameObject robot;
        private Rigidbody robotRigidBody;

        public TfPublisher()
        {
            topic = "/tf";
        }

        protected override void Start()
        {
            nameSpace = ""; // tf messages are published without namespace
            base.Start();

            // initialization has to be done here because values
            // set in the unity editor aren't visible in the constructor
            robotRigidBody = (Rigidbody) robot.GetComponent(typeof(Rigidbody));
        }

        private void Update()
        {
            RosTfMessage msg = new RosTfMessage();

            msg.transforms = new RosTransformStamped[1] { new RosTransformStamped() };
            msg.transforms[0].header.frame_id = "/odom";
            msg.transforms[0].header.seq = msgSeq;
            msg.transforms[0].child_frame_id = "/base_link";
            msg.transforms[0].transform.translation = GetRobotPositionRos();
            msg.transforms[0].transform.rotation = GetRobotOrientationRos();
            msgSeq += 1;

            // publish on a separate thread to avoid blocking
            ThreadPool.QueueUserWorkItem((m) => this.Publish((RosTfMessage) m), msg);
        }

        private RosVector3 GetRobotPositionRos()
        {
            Vector3 pos = robot.transform.position.Unity2Ros();
            return new RosVector3(pos.x, pos.y, 0);
        }

        private RosQuaternion GetRobotOrientationRos()
        {
            Quaternion pos = robot.transform.rotation.Unity2Ros();
            return new RosQuaternion(pos.x, pos.y, pos.z, pos.w);
        }
    }
}
