using System;
using System.Threading;
using Newtonsoft.Json;
using RosMessage = RosSharp.RosBridgeClient.Message;
using RosTime = RosSharp.RosBridgeClient.MessageTypes.Std.Time;

namespace Roborts
{
    public sealed class Clock : RosMessage
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        [JsonIgnore]
        public const string RosMessageName = "rosgraph_msgs/Clock";

        public RosTime clock;

        public static Clock Now()
        {
            Clock clock = new Clock();

            long elapsed = DateTime.Now.Ticks - Epoch.Ticks;
            clock.clock.secs = (uint) (elapsed / 10000000);
            clock.clock.nsecs = (uint) (elapsed % 10000000);

            // avoid jitters
            clock.clock.nsecs /= 1000000;
            clock.clock.nsecs *= 1000000;

            return clock;
        }

        public Clock()
        {
            clock = new RosTime();
        }
    }

    public sealed class ClockPublisher : RobortsPublisher<Clock>
    {
        public ClockPublisher()
        {
            topic = "/clock";
        }

        protected override void Start()
        {
            nameSpace = ""; // clock messages are published without namespace
            base.Start();
        }

        private void Update()
        {
            ThreadPool.QueueUserWorkItem((m) => this.Publish((Clock) m), Clock.Now());
        }
    }
}
