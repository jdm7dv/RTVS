using System;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.UnitTests.Core.Threading {
    public static class ControlledTaskSchedulerExtensions {
        public static void Link<T>(this ControlledTaskScheduler scheduler, IReceivableSourceBlock<T> sourceBlock, Action<T> action) {
            sourceBlock.LinkTo(new ActionBlock<T>(action, new ExecutionDataflowBlockOptions { TaskScheduler = scheduler }));
        }

        public static void Wait(this ControlledTaskScheduler scheduler, IDataflowBlock block) {
            scheduler.Wait();
            if (block.Completion.IsFaulted && block.Completion.Exception != null) {
                throw block.Completion.Exception;
            }
        }
    }
}