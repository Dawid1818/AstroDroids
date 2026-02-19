using System;

namespace AstroDroids.Coroutines
{
    public class WaitForSeconds : Coroutine
    {
        DateTime startTime;
        TimeSpan waitTime;

        public WaitForSeconds(double seconds)
        {
            waitTime = TimeSpan.FromSeconds(seconds);
            startTime = DateTime.Now;
        }

        public override bool Execute()
        {
            if(DateTime.Now - startTime >= waitTime)
            {
                return true;
            }

            return false;
        }
    }
}
