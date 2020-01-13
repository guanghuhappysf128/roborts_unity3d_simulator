using System.Threading;
using UnityEngine;
using RosSharp;
using RosOdometry = RosSharp.RosBridgeClient.MessageTypes.Nav.Odometry;
using RosPoint = RosSharp.RosBridgeClient.MessageTypes.Geometry.Point;
using RosQuaternion = RosSharp.RosBridgeClient.MessageTypes.Geometry.Quaternion;
using RosVector3 = RosSharp.RosBridgeClient.MessageTypes.Geometry.Vector3;

namespace Roborts
{
    public sealed class TfPublisher : RobortsPublisher<RosOdometry>
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
            base.Start();

            // initialization has to be done here because values
            // set in the unity editor aren't visible in the constructor
            nameSpace = ""; // odom messages are published without namespace
            robotRigidBody = (Rigidbody) robot.GetComponent(typeof(Rigidbody));
        }

        private void Update()
        {
            RosOdometry msg = new RosOdometry();

            msg.header.frame_id = "odom";
            msg.header.seq = msgSeq;
            msg.child_frame_id = "base_link";
            msg.pose.pose.position = GetRobotPositionRos();
            msg.pose.pose.orientation = GetRobotOrientationRos();
            msg.pose.covariance = null; // TODO: replace this
            msg.twist.twist.linear = GetRobotLinearVelocity();
            msg.twist.twist.angular = GetRobotAngularVelocity();
            msg.twist.covariance = null; // TODO: replace this
            msgSeq += 1;

            // publish on a separate thread to avoid blocking
            ThreadPool.QueueUserWorkItem((msg) => this.Publish((RosOdometry) msg), msg);
        }

        private RosPoint GetRobotPositionRos()
        {
            Vector3 pos = robot.transform.position.Unity2Ros();
            return new RosPoint(pos.x, pos.y, pos.z);
        }

        private RosQuaternion GetRobotOrientationRos()
        {
            Quaternion pos = robot.transform.rotation.Unity2Ros();
            return new RosQuaternion(pos.x, pos.y, pos.z, pos.w);
        }

        private RosVector3 GetRobotLinearVelocity()
        {
            Vector3 vel = robotRigidBody.velocity.Unity2Ros();
            return new RosVector3(-vel.y, vel.x, 0);
        }

        private RosVector3 GetRobotAngularVelocity()
        {
            Vector3 vel = robotRigidBody.angularVelocity.Unity2Ros();
            return new RosVector3(0, 0, -vel.z);
        }
    }
}
