using System.Threading;
using UnityEngine;
using RosLaserScan = RosSharp.RosBridgeClient.MessageTypes.Sensor.LaserScan;

namespace Roborts
{
    public sealed class LidarPublisher : RobortsPublisher<RosLaserScan>
    {
        private static readonly Vector3 lidarOffset = new Vector3(0.18f, 0.0f, 0.0f);

        public GameObject robot;
        public float lidarMinRange;
        public float lidarMaxRange;
        public float angularResolutionDeg;
        public float sampleFrequencyHz;

        private uint samplePerMsg;
        private float[] samples;
        private uint numSample;
        private RosLaserScan msg;
        private uint msgSeq;


        public LidarPublisher()
        {
            topic = "/scan";
        }

        protected override void Start()
        {
            base.Start();

            // initialization has to be done here because values
            // set in the unity editor aren't visible in the constructor
            samplePerMsg = (uint) Mathf.Ceil(360.0f / angularResolutionDeg);
            samples = new float[samplePerMsg];
            numSample = 0;
            msgSeq = 0;
            InitializeMessage();
        }

        private void Update()
        {
            // # of samples to collect at this frame
            uint n = (uint) Mathf.Ceil(sampleFrequencyHz * Time.deltaTime);

            // collect samples
            Vector3 lidarPos = robot.transform.position + lidarOffset;
            for (uint i = 0; i < n; i++)
            {
                float angle = numSample * angularResolutionDeg;
                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                RaycastHit hitInfo;

                bool hit = Physics.Raycast(
                    lidarPos,
                    rotation * robot.transform.forward,
                    out hitInfo,
                    lidarMaxRange);

                if (hit && (hitInfo.distance >= lidarMinRange))
                {
                    samples[numSample] = hitInfo.distance;
                }
                else
                {
                    samples[numSample] = 0;
                }
                numSample += 1;

                // send msg if there are enough samples
                if (numSample == samplePerMsg)
                {
                    SendMessage();
                }
            }
        }

        private void InitializeMessage()
        {
            msg = new RosLaserScan();

            msg.header.seq = msgSeq;
            msg.header.stamp = Clock.Now().clock;
            msg.header.frame_id = "scan";
            msg.angle_min = 0;
            msg.angle_max = (360.0f - angularResolutionDeg) * Mathf.Deg2Rad;
            msg.angle_increment = angularResolutionDeg * Mathf.Deg2Rad;
            msg.scan_time = samplePerMsg / sampleFrequencyHz;
            msg.range_min = lidarMinRange;
            msg.range_max = lidarMaxRange;
            msg.ranges = samples;

            msgSeq += 1;
        }

        private void SendMessage()
        {
            // publish on a separate thread to avoid blocking
            ThreadPool.QueueUserWorkItem((msg) => this.Publish((RosLaserScan) msg), msg);

            // reset state
            samples = new float[samplePerMsg];
            numSample = 0;
            InitializeMessage();
        }
    }
}
