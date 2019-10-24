namespace NRToolkit.Record
{
    using System;

    public class DispatchUtility
    {
        public static event Action<UInt64> onFrame;
        private static readonly DispatchUtility instance;

        static DispatchUtility()
        {
            instance = new DispatchUtility();
        }

        public void Update(UInt64 timestamp)
        {
            if (onFrame != null)
            {
                onFrame(timestamp);
            }
        }
    }
}
