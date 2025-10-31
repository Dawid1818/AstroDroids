using System;

namespace AstroDroids.Coroutines
{
    public class WaitUntil : Coroutine
    {
        Func<bool> condition;

        public WaitUntil(Func<bool> condition)
        {
            this.condition = condition;
        }
        public override bool Execute()
        {
            return condition();
        }
    }
}
