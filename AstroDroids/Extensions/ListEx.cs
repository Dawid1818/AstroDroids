using System.Collections.Generic;

namespace AstroDroids.Extensions
{
    public static class ListEx
    {
        public static void MoveItemUp<T>(this IList<T> list, T item)
        {
            int index = list.IndexOf(item);
            if (index <= 0)
                return;
            list.RemoveAt(index);
            list.Insert(index - 1, item);
        }

        public static void MoveItemDown<T>(this IList<T> list, T item)
        {
            int index = list.IndexOf(item);
            if (index < 0 || index >= list.Count - 1)
                return;
            list.RemoveAt(index);
            list.Insert(index + 1, item);
        }

        public static void MoveItemUp<T>(this List<T> list, int index)
        {
            if (index <= 0 || index >= list.Count)
                return;
            T item = list[index];
            list.RemoveAt(index);
            list.Insert(index - 1, item);
        }

        public static void MoveItemDown<T>(this List<T> list, int index)
        {
            if (index < 0 || index >= list.Count - 1)
                return;
            T item = list[index];
            list.RemoveAt(index);
            list.Insert(index + 1, item);
        }
    }
}
