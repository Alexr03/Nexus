using System.Threading.Tasks;
using FluentScheduler;

namespace Nexus.SDK.Modules
{
    public abstract class NexusScheduledTaskModule : IJob
    {
        public int RepeatEveryMilliseconds { get; set; }

        public void Execute()
        {
            Task.Run(async () => await DoAction());
        }

        public abstract Task DoAction();
    }
}