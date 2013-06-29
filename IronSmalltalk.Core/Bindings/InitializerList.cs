/*
 * **************************************************************************
 *
 * Copyright (c) The IronSmalltalk Project. 
 *
 * This source code is subject to terms and conditions of the 
 * license agreement found in the solution directory. 
 * See: $(SolutionDir)\License.htm ... in the root of this distribution.
 * By using this source code in any fashion, you are agreeing 
 * to be bound by the terms of the license agreement.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * **************************************************************************
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Runtime.Behavior;

namespace IronSmalltalk.Runtime.Bindings
{
    public abstract class InitializerList<TItem> : IList<TItem>
    {
        private readonly List<TItem> Contents;
        private bool ReadOnly = false;

        protected InitializerList()
        {
            this.Contents = new List<TItem>();
        }

        protected InitializerList(int capacity)
        {
            this.Contents = new List<TItem>(capacity);
        }

        protected InitializerList(IEnumerable<TItem> items)
        {
            this.Contents = new List<TItem>(items);
        }

        public int IndexOf(TItem item)
        {
            return this.Contents.IndexOf(item);
        }

        public void Insert(int index, TItem item)
        {
            if (this.ReadOnly)
                throw new InvalidOperationException("List is in read-only state.");
            this.Contents.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            if (this.ReadOnly)
                throw new InvalidOperationException("List is in read-only state."); 
            this.Contents.RemoveAt(index);
        }

        public TItem this[int index]
        {
            get
            {
                return this.Contents[index];
            }
            set
            {
                if (this.ReadOnly)
                    throw new InvalidOperationException("List is in read-only state.");
                this.Contents[index] = value;
            }
        }

        public void Add(TItem item)
        {
            if (this.ReadOnly)
                throw new InvalidOperationException("List is in read-only state.");
            this.Contents.Add(item);
        }

        public void Clear()
        {
            if (this.ReadOnly)
                throw new InvalidOperationException("List is in read-only state.");
            this.Contents.Clear();
        }

        public bool Contains(TItem item)
        {
            return this.Contents.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            this.Contents.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.Contents.Count; }
        }

        public bool IsReadOnly
        {
            get { return this.ReadOnly; }
        }

        public bool Remove(TItem item)
        {
            if (this.ReadOnly)
                throw new InvalidOperationException("List is in read-only state.");
            return this.Contents.Remove(item);
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return this.Contents.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Contents.GetEnumerator();
        }

        public void AddRange(IEnumerable<TItem> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (this.ReadOnly)
                throw new InvalidOperationException("List is in read-only state.");

            foreach(TItem item in collection)
                this.Contents.Add(item);
        }

        internal void WriteProtect()
        {
            // Once write-protected, it cannot be unprotected to read-write state!
            this.ReadOnly = true;
        }
    }

    public class InitializerList : InitializerList<CompiledInitializer>
    {
        public InitializerList()
        {
        }
    }
}
