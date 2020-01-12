using UnityEngine;
using RosLaserScan = RosSharp.RosBridgeClient.MessageTypes.Sensor.LaserScan;

namespace Roborts
{
    public sealed class LidarPublisher : RobortsPublisher<RosLaserScan>
    {
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
            // TODO: ignore lower_box?
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(
                gameObject.transform.position,
                gameObject.transform.forward,
                out hitInfo,
                lidarRange);

            if (hit)
            {
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
