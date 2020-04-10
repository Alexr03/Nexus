﻿﻿using System.Threading.Tasks;
using DSharpPlus;
using Quartz;

namespace Nexus.SDK.Modules
{
    public class NexusScheduledTaskModule : NexusModule, IJob
    {
        public DiscordClient DiscordClient { get; private set; }
        
        public int RepeatEveryMilliseconds;
        
        public async Task Execute(IJobExecutionContext context)
        {
            DiscordClient = (DiscordClient) context.Scheduler.Context.Get("Client");
            await DoAction(context);
        }

        public virtual async Task DoAction(IJobExecutionContext context)
        {
            
        }
    }
}