using AstroDroids.Coroutines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AstroDroids.Managers
{
    public class CoroutineManager
    {
        public List<CoroutineInstance> Coroutines = new();
        public List<CoroutineInstance> CoroutinesToRemove = new();

        bool iterating = false;

        public void Update()
        {
            iterating = true;
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
            iterating = false;

            foreach (var item in CoroutinesToRemove)
            {
                Coroutines.Remove(item);
            }
            CoroutinesToRemove.Clear();
        }

        public CoroutineInstance StartCoroutine(IEnumerator coroutine)
        {
            CoroutineInstance instance = new();
            instance.Stack.Push(coroutine);

            Coroutines.Add(instance);

            return instance;
        }

        public void StopAllCoroutines()
        {
            if(iterating)
            {
                CoroutinesToRemove.Clear();
                Coroutines.AddRange(Coroutines);
            }
            else
            {
                Coroutines.Clear();
            }
        }

        public void StopCoroutine(CoroutineInstance instance)
        {
            if(iterating)
            {
                CoroutinesToRemove.Add(instance);
            }
            else
            {
                Coroutines.Remove(instance);
            }
        }
    }
}
