using Snowflake.Core;

namespace CPTech.EFCore
{
    public class SnowFlake
    {
        private static readonly IdWorker worker = new IdWorker(1, 1);

        public static long NextId() => worker.NextId();
    }
}
