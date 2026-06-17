using AstroDroids.Entities;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using System.Collections;
using System.Collections.Generic;

namespace AstroDroids.Collections
{
    public class EntityList<T> : IEnumerable<T>
    where T : Entity
    {
        List<T> items { get; } = new List<T>();
        List<T> itemsToRemove { get; } = new List<T>();
        List<T> itemsToAdd { get; } = new List<T>();

        bool currentlyEnumerating = false;

        public T this[int index] => items[index];

        public int Count => items.Count;

        public EntityList() { }

        public void Add(T item)
        {
            if (currentlyEnumerating)
            {
                itemsToAdd.Add(item);
                return;
            }
            items.Add(item);
        }

        public void Remove(T item)
        {
            itemsToRemove.Add(item);
        }

        public void RemoveImmediate(T item)
        {
            items.Remove(item);
        }

        public void Update(GameTime gameTime)
        {
            currentlyEnumerating = true;
            foreach (var item in items)
            {
                item.Update(gameTime);
            }

            foreach (var item in itemsToAdd)
            {
                items.Add(item);
            }
            itemsToAdd.Clear();

            foreach (var item in itemsToRemove)
            {
                RemoveImmediate(item);
            }
            currentlyEnumerating = false;

            itemsToRemove.Clear();
        }

        public void Draw(GameTime gameTime)
        {
            currentlyEnumerating = true;
            foreach (var item in items)
            {
                item.Draw(gameTime);

                if (AstroDroidsGame.Debug)
                    item.DrawDebug(gameTime);
            }
            currentlyEnumerating = false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }
    }
}
