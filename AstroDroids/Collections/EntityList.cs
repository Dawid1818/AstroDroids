using AstroDroids.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroDroids.Collections
{
    public class EntityList<T> : IEnumerable<T>
    where T : Entity
    {
        List<T> items { get; } = new List<T>();
        List<T> itemsToRemove { get; } = new List<T>();

        public T this[int index] => items[index];

        public int Count => items.Count;

        public EntityList() { }

        public void Add(T item)
        {
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
            foreach (var item in items)
            {
                item.Update(gameTime);
            }

            foreach (var item in itemsToRemove)
            {
                RemoveImmediate(item);
            }

            itemsToRemove.Clear();
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var item in items)
            {
                item.Draw(gameTime);

                if (AstroDroidsGame.Debug)
                    item.DrawDebug(gameTime);
            }
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
