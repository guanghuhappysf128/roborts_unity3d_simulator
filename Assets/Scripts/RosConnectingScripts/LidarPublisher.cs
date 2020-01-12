using UnityEngine;
using RosLaserScan = RosSharp.RosBridgeClient.MessageTypes.Sensor.LaserScan;

namespace Roborts
{
    public sealed class LidarPublisher : RobortsPublisher<RosLaserScan>
    {
        private static readonly Vector3 lidarOffset = new Vector3(0.18f, 0.0f, 0.0f);

        public float lidarRange;

        public LidarPublisher()
        {
            topic = "/scan";
        }

        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(
                gameObject.transform.position + lidarOffset,
                gameObject.transform.forward,
                out hitInfo,
                lidarRange);

            if (hit)
            {
                Debug.Log($"raycast from {gameObject.name} hit {hitInfo.collider.name} at {hitInfo.point}");
                RosLaserScan msg = new RosLaserScan();

                // TODO: set other msg fields?
                msg.header.seq = (uint) Time.frameCount; // TODO: is this correct?
                msg.header.frame_id = "1"; // TODO: is this correct?
                msg.range_max = lidarRange;
                msg.ranges = new float[1] { hitInfo.distance };

                Publish(msg);
            }
        }
    }
}
