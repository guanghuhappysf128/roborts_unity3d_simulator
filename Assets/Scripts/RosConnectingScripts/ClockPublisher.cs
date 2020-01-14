using System.Threading;
using Newtonsoft.Json;
using RosMessage = RosSharp.RosBridgeClient.Message;
using RosTime = RosSharp.RosBridgeClient.MessageTypes.Std.Time;

namespace Roborts
{
    public sealed class Clock : RosMessage
    {
        [JsonIgnore]
        public const string RosMessageName = "rosgraph_msgs/Clock";

        public RosTime clock;

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
            ThreadPool.QueueUserWorkItem((m) => this.Publish((Clock) m), new Clock());
        }
    }
}
