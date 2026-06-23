using System.Collections;
using System.Collections.Generic;

namespace AstroDroids.Coroutines
{
    public class CoroutineInstance
    {
        public Stack<IEnumerator> Stack { get; set; } = new Stack<IEnumerator>();
    }
}
