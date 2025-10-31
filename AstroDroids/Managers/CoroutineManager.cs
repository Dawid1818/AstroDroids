using AstroDroids.Coroutines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AstroDroids.Managers
{
    public class CoroutineManager
    {
        public List<IEnumerator> Coroutines = new List<IEnumerator>();

        public void Update()
        {
            foreach (var coroutine in Coroutines.ToList())
            {
                if(coroutine.Current is Coroutine coro)
                {
                    if(coro.Execute())
                    {
                        if (!coroutine.MoveNext())
                        {
                            Coroutines.Remove(coroutine);
                        }
                    }
                }
                else
                {
                    if (!coroutine.MoveNext())
                    {
                        Coroutines.Remove(coroutine);
                    }
                }
            }
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            Coroutines.Add(coroutine);
        }
    }
}
