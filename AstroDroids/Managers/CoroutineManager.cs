using AstroDroids.Coroutines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AstroDroids.Managers
{
    public class CoroutineManager
    {
        public List<CoroutineInstance> Coroutines = new();

        public void Update()
        {
            foreach (var instance in Coroutines.ToList())
            {
                while (instance.Stack.Count > 0)
                {
                    var current = instance.Stack.Peek();

                    if (current.Current is Coroutine wait)
                    {
                        if (!wait.Execute())
                            break;
                    }

                    if (!current.MoveNext())
                    {
                        instance.Stack.Pop();
                        continue;
                    }

                    if (current.Current is IEnumerator nested)
                    {
                        //nested.MoveNext();
                        instance.Stack.Push(nested);
                        continue;
                    }

                    break;
                }

                if (instance.Stack.Count == 0)
                    Coroutines.Remove(instance);
            }
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            CoroutineInstance instance = new();
            instance.Stack.Push(coroutine);

            Coroutines.Add(instance);
        }
    }
}
