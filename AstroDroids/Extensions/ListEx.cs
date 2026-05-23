using System.Collections.Generic;

namespace AstroDroids.Extensions
{
    public static class ListEx
    {
        public static bool MoveItemUp<T>(this IList<T> list, T item)
        {
            int index = list.IndexOf(item);
            if (index <= 0)
                return false;
            list.RemoveAt(index);
            list.Insert(index - 1, item);
            return true;
        }

        public static bool MoveItemDown<T>(this IList<T> list, T item)
        {
            int index = list.IndexOf(item);
            if (index < 0 || index >= list.Count - 1)
                return false;
            list.RemoveAt(index);
            list.Insert(index + 1, item);
            return true;
        }

        public static bool MoveItemUp<T>(this List<T> list, int index)
        {
            if (index <= 0 || index >= list.Count)
                return false;
            T item = list[index];
            list.RemoveAt(index);
            list.Insert(index - 1, item);
            return true;
        }

        public static bool MoveItemDown<T>(this List<T> list, int index)
        {
            if (index < 0 || index >= list.Count - 1)
                return false;
            T item = list[index];
            list.RemoveAt(index);
            list.Insert(index + 1, item);
            return true;
        }
    }
}
