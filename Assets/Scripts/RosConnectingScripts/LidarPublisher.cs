using UnityEngine;
using RosLaserScan = RosSharp.RosBridgeClient.MessageTypes.Sensor.LaserScan;

namespace Roborts
{
    public sealed class LidarPublisher : RobortsPublisher<RosLaserScan>
    {
        private static readonly Vector3 lidarOffset = new Vector3(0.18f, 0.0f, 0.0f);

        public float lidarMinRange;
        public float lidarMaxRange;
        public float angularResolutionDeg;
        public float sampleFrequencyHz;
        private float currentAngle = 0;

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
            // calculate the scan region
            uint n_sample = (uint) Mathf.Ceil(sampleFrequencyHz * Time.deltaTime);
            float minAngle = currentAngle;
            float maxAngle = currentAngle + n_sample * angularResolutionDeg;
            currentAngle = (maxAngle >= 360.0f) ? (maxAngle - 360.0f) : maxAngle;

            // perform the scan
            Vector3 lidarPos = gameObject.transform.position + lidarOffset;
            float[] ranges = new float[n_sample];
            for (uint i = 0; i < n_sample; i++)
            {
                float angle = minAngle + i * angularResolutionDeg;
                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                RaycastHit hitInfo;

                bool hit = Physics.Raycast(
                    lidarPos,
                    rotation * gameObject.transform.forward,
                    out hitInfo,
                    lidarMaxRange);

                if (hit && (hitInfo.distance >= lidarMinRange))
                {
                    ranges[i] = hitInfo.distance;
                }
                else
                {
                    ranges[i] = 0;
                }
            }

            // send msg to ROS
            RosLaserScan msg = new RosLaserScan();
            msg.header.seq = (uint) Time.frameCount; // TODO: is this correct?
            msg.header.frame_id = "scan";
            msg.angle_min = minAngle * Mathf.Deg2Rad;
            msg.angle_max = maxAngle * Mathf.Deg2Rad;
            msg.angle_increment = angularResolutionDeg * Mathf.Deg2Rad;
            msg.scan_time = Time.deltaTime;
            msg.range_min = lidarMinRange;
            msg.range_max = lidarMaxRange;
            msg.ranges = ranges;
            Publish(msg);
        }
    }
}
