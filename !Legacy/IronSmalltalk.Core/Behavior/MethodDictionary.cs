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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IronSmalltalk.Runtime.Behavior
{
    /// <summary>
    /// A dictionary containing methods.
    /// </summary>
    /// <remarks>
    /// Smalltalk classes have two method dictionaries; 
    /// one describing the instance behavior and one describing the class behavior.
    /// </remarks>
    public abstract class MethodDictionary :
        IDictionary<Symbol, CompiledMethod>, IDictionary<string, CompiledMethod>, // IDictionary,
        ICollection<CompiledMethod> //, ICollection // Also implements IEnumerable<CompiledMethod>, IEnumerable
    {
        private volatile Dictionary<Symbol, CompiledMethod> _contents;
        private readonly Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>> _LazyInitializer;
        private readonly SmalltalkRuntime _Runtime;
        private bool _readOnly = false;

        /// <summary>
        /// Create a new method dictionary.
        /// </summary>
        /// <param name="runtime">Smalltalk runtime that the methods in the dictionary belong to.</param>
        protected MethodDictionary(SmalltalkRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            this._Runtime = runtime;
            this._contents = new Dictionary<Symbol, CompiledMethod>();
        }

        /// <summary>
        /// Create a new method dictionary that's lazy initializable.
        /// </summary>
        /// <param name="runtime">Smalltalk runtime that the methods in the dictionary belong to.</param>
        /// <param name="lazyInitializer">Callback function for lazy initializing of the contents of the method dictionary.</param>
        /// <remarks
        /// >In rare multi-threaded conditions the lazyInitializer function may be called more than once. 
        /// Only one of the results will be stored, and the rest discarded! Therefore, the initializer 
        /// function should be robust without side-effects if run more than once.
        /// </remarks>
        protected MethodDictionary(SmalltalkRuntime runtime, Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>> lazyInitializer)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            if (lazyInitializer == null)
                throw new ArgumentNullException(nameof(lazyInitializer));
            this._Runtime = runtime;
            this._LazyInitializer = lazyInitializer;
            this._contents = null;
        }

        #region Accessing 

        private Dictionary<Symbol, CompiledMethod> Contents
        {
            get
            {
                // Lazy initializing of method dictionary
                if (this._contents == null)
                    System.Threading.Interlocked.CompareExchange(ref this._contents, this._LazyInitializer(this._Runtime), null);
                return this._contents;
            }
        }

        /// <summary>
        /// Gets or sets the method with the specified selector.
        /// </summary>
        /// <param name="selector">Selector of the method to get or set.</param>
        /// <returns>The method with the specified selector.</returns>
        public CompiledMethod this[string selector]
        {
            get
            {
                if (selector == null)
                    throw new ArgumentNullException(nameof(selector));
                return this[this.ToSymbol(selector)];
            }
            set
            {
                if (selector == null)
                    throw new ArgumentNullException(nameof(selector));
                this[this.ToSymbol(selector)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the method with the specified selector.
        /// </summary>
        /// <param name="selector">Selector of the method to get or set.</param>
        /// <returns>The method with the specified selector.</returns>
#pragma warning disable CA1043 // Use integral or string argument for indexers
        public CompiledMethod this[Symbol selector]
#pragma warning restore CA1043 // Use integral or string argument for indexers
        {
            get
            {
                if (selector == null)
                    throw new ArgumentNullException(nameof(selector));
                CompiledMethod value;
                if (this.TryGetValue(selector, out value))
                    return value;
                else
                    return MethodDictionary.ElementMissing(selector.Value);
            }
            set
            {
                if (selector == null)
                    throw new ArgumentNullException(nameof(selector));
                this.ValidateModification(value, selector);
                this.Contents[selector] = value;
            }
        }

        /// <summary>
        /// Gets the value associated with the specified selector.
        /// </summary>
        /// <param name="selector">Selector of the method to get.</param>
        /// <param name="method">
        /// When this method returns, the method associated with the specified selector, 
        /// if the selector is found; otherwise null. This parameter is passed uninitialized.
        /// </param>
        /// <returns>True if a method with the specified selector exists, otherwise, false.</returns>
        public bool TryGetValue(string selector, out CompiledMethod method)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            return this.TryGetValue(this.ToSymbol(selector), out method);
        }

        /// <summary>
        /// Gets the value associated with the specified selector.
        /// </summary>
        /// <param name="selector">Selector of the method to get.</param>
        /// <param name="method">
        /// When this method returns, the method associated with the specified selector, 
        /// if the selector is found; otherwise null. This parameter is passed uninitialized.
        /// </param>
        /// <returns>True if a method with the specified selector exists, otherwise, false.</returns>
        public bool TryGetValue(Symbol selector, out CompiledMethod method)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            return this.Contents.TryGetValue(selector, out method);
        }

        /// <summary>
        /// Gets the number of method contained in the method dictionary
        /// </summary>
        public int Count
        {
            get { return this.Contents.Count; }
        }

        /// <summary>
        /// Gets a collection containing the selectors of methods in the method dictionary.
        /// </summary>
        public ICollection<Symbol> Keys
        {
            get { return this.Contents.Keys; }
        }

        /// <summary>
        /// Gets a collection containing the methods in the method dictionary.
        /// </summary>
        public ICollection<CompiledMethod> Values
        {
            get { return this.Contents.Values; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the methods in the method dictionary.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CompiledMethod> GetEnumerator()
        {
            return this.Contents.Select(pair => pair.Value).GetEnumerator();
        }

        /// <summary>
        /// Copies the methods in the method dictionary to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional System.Array that is the destination of the methods
        /// copied from method dictionary. The Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(CompiledMethod[] array, int arrayIndex)
        {
            this.Values.CopyTo(array, arrayIndex);
        }

        #endregion

        #region Modifying

        /// <summary>
        /// Adds an method identified by its selector to the method dictionary.
        /// </summary>
        /// <param name="method">Method to be added to the method dictionary.</param>
        public void Add(CompiledMethod method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            this.Add(method.Selector, method);
        }

        /// <summary>
        /// Adds an method identified with the provided selector to the method dictionary.
        /// </summary>
        /// <param name="selector">Selector to identify the method.</param>
        /// <param name="method">Method to be added to the method dictionary.</param>
        /// <remarks>
        /// The method parameter may be null, in which case it behaves like removing it 
        /// from the behavior of the object. It also has the effect that the method lookup
        /// will stop and not look into superclasses and the message send for the given 
        /// selector will result in does-not-understand.
        /// </remarks>
        public void Add(string selector, CompiledMethod method)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            this.Add(this.ToSymbol(selector), method);
        }

        /// <summary>
        /// Adds an method identified with the provided selector to the method dictionary.
        /// </summary>
        /// <param name="selector">Selector to identify the method.</param>
        /// <param name="method">Method to be added to the method dictionary.</param>
        /// <remarks>
        /// The method parameter may be null, in which case it behaves like removing it 
        /// from the behavior of the object. It also has the effect that the method lookup
        /// will stop and not look into superclasses and the message send for the given 
        /// selector will result in does-not-understand.
        /// </remarks>
        public void Add(Symbol selector, CompiledMethod method)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            this.ValidateModification(method, selector);
            this.Contents.Add(selector, method);
            this._nativeMethodNameMap = null;
        }

        /// <summary>
        /// Adds the methods in the specified collection to the method dictionary.
        /// </summary>
        /// <param name="items">
        /// The collection whose methods should be added to the method dictionary.
        ///  The collection itself cannot be null, nor can it contain elements that are null.
        /// </param>
        public void AddRange(IEnumerable<CompiledMethod> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            foreach (CompiledMethod item in items)
                this.Add(item.Selector, item);
        }

        /// <summary>
        /// Removes the method with the given selector from the method dictionary.
        /// </summary>
        /// <param name="selector">Selector of the method to remove from the method dictionary.</param>
        /// <returns>True if the element is successfully removed; otherwise, false.</returns>
        public bool Remove(string selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            return this.Remove(this.ToSymbol(selector));
        }

        /// <summary>
        /// Removes the method with the given selector from the method dictionary.
        /// </summary>
        /// <param name="selector">Selector of the method to remove from the method dictionary.</param>
        /// <returns>True if the element is successfully removed; otherwise, false.</returns>
        public bool Remove(Symbol selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (this._readOnly)
                throw new InvalidOperationException("Dictionary is in read-only state.");
            bool removed = this.Contents.Remove(selector);
            if (removed)
                this._nativeMethodNameMap = null;
            return removed;
        }

        /// <summary>
        /// Removes the method that has the same selector as the selector of the given method from the method dictionary.
        /// </summary>
        /// <param name="method">Method who's selector to remove from the method dictionary.</param>
        /// <returns>True if the element is successfully removed; otherwise, false.</returns>
        public bool Remove(CompiledMethod method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            return this.Remove(method.Selector);
        }

        private void ValidateModification(CompiledMethod method, Symbol selector)
        {
            if (this._readOnly)
                throw new InvalidOperationException("Dictionary is in read-only state.");

            if ((selector == null) && (method != null))
                selector = method.Selector;

            if ((selector != null) && (selector.Manager.Runtime != this._Runtime))
                throw new ArgumentException("Method selector belongs to a different SmalltalkRuntime.", nameof(selector));

            if ((method != null) && (method.Class.Runtime != this._Runtime))
                throw new ArgumentException("Method selector belongs to a different SmalltalkRuntime.", nameof(method));

            // We could check (method.Selector != selector), but we allow to add a method with a different selector,
            // thus giving the same CompiledMethod behavior several names (selectors).

            if ((method != null) && (method.Type != this.MethodType))
                throw new ArgumentException($"Expected {this.MethodType} method but was given {method.Type} method.", nameof(method));
        }

        public abstract CompiledMethod.MethodType MethodType { get; }

        #endregion

        #region Testing

        /// <summary>
        /// Determines whether the method dictionary contains a method with the specified selector.
        /// </summary>
        /// <param name="selector">The selector to locate in the method dictionary.</param>
        /// <returns>True if the method dictionary contains a method with the given selector; otherwise, false.</returns>
        public bool ContainsKey(string selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            return this.ContainsKey(this.ToSymbol(selector));
        }

        /// <summary>
        /// Determines whether the method dictionary contains a method with the specified selector.
        /// </summary>
        /// <param name="selector">The selector to locate in the method dictionary.</param>
        /// <returns>True if the method dictionary contains a method with the given selector; otherwise, false.</returns>
        public bool ContainsKey(Symbol selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            CompiledMethod item;
            return this.TryGetValue(selector, out item) && (item != null);
        }

        /// <summary>
        /// Determines whether the method dictionary contains a method with a selector as the selector of the given method.
        /// </summary>
        /// <param name="method">Method who's selector to locate in the method dictionary.</param>
        /// <returns>True if the method dictionary contains a method with the given selector; otherwise, false.</returns>
        public bool Contains(CompiledMethod method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            CompiledMethod item;
            return this.TryGetValue(method.Selector, out item) && Object.ReferenceEquals(item, method);
        }

        #endregion

        #region Helpers

        internal void WriteProtect()
        {
            // Once write-protected, it cannot be unprotected to read-write state!
            this._readOnly = true;
        }

        private Symbol ToSymbol(string selector)
        {
            return this._Runtime.GetSymbol(selector);
        }

        private static CompiledMethod ElementMissing(string selector)
        {
            throw new KeyNotFoundException($"Method with the given selector #'{selector}' is not present in the dictionary.");
        }

        #endregion

        #region interface IDictionary<Symbol, CompiledMethod>

        void IDictionary<Symbol, CompiledMethod>.Add(Symbol selector, CompiledMethod value)
        {
            throw new NotSupportedException();
            //if (selector == null)
            //    throw new ArgumentNullException("selector");
            //if (value == null)
            //    throw new ArgumentNullException("value");
            //if (selector != value.Name)
            //    throw new ArgumentException("Key must be equal to Value.Name.");
            //this.Add(value);
        }

        bool IDictionary<Symbol, CompiledMethod>.ContainsKey(Symbol selector)
        {
            return this.ContainsKey(selector);
        }

        ICollection<Symbol> IDictionary<Symbol, CompiledMethod>.Keys
        {
            get { return this.Keys; }
        }

        bool IDictionary<Symbol, CompiledMethod>.Remove(Symbol selector)
        {
            throw new NotSupportedException();
            //return this.Remove(selector);
        }

        bool IDictionary<Symbol, CompiledMethod>.TryGetValue(Symbol selector, out CompiledMethod value)
        {
            return this.TryGetValue(selector, out value);
        }

        ICollection<CompiledMethod> IDictionary<Symbol, CompiledMethod>.Values
        {
            get { return this.Values; }
        }

        CompiledMethod IDictionary<Symbol, CompiledMethod>.this[Symbol selector]
        {
            get
            {
                return this[selector];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        void ICollection<KeyValuePair<Symbol, CompiledMethod>>.Add(KeyValuePair<Symbol, CompiledMethod> item)
        {
            throw new NotSupportedException();
            //if (item.Value == null)
            //    throw new ArgumentException("Item.Value must not be null.");
            //if (item.Key != item.Value.Name)
            //    throw new ArgumentException("Item.Key must be equal to Item.Value.Name.");
            //this.Add(item.Value);
        }

        void ICollection<KeyValuePair<Symbol, CompiledMethod>>.Clear()
        {
            throw new NotSupportedException();
            //this.Clear();
        }

        bool ICollection<KeyValuePair<Symbol, CompiledMethod>>.Contains(KeyValuePair<Symbol, CompiledMethod> item)
        {
            CompiledMethod candidate;
            return this.TryGetValue(item.Key, out candidate) && Object.ReferenceEquals(candidate, item.Value);
        }

        void ICollection<KeyValuePair<Symbol, CompiledMethod>>.CopyTo(KeyValuePair<Symbol, CompiledMethod>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "arrayIndex is less than 0.");
            if (array.Length < (this.Count + arrayIndex))
                throw new ArgumentException("The number of elements in the source ICollection <T> is greater than the available space from arrayIndex to the end of the destination array.");

            foreach (CompiledMethod item in this)
            {
                array[arrayIndex] = new KeyValuePair<Symbol, CompiledMethod>(item.Selector, item);
                arrayIndex++;
            }
        }

        int ICollection<KeyValuePair<Symbol, CompiledMethod>>.Count
        {
            get { return this.Count; }
        }

        bool ICollection<KeyValuePair<Symbol, CompiledMethod>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<Symbol, CompiledMethod>>.Remove(KeyValuePair<Symbol, CompiledMethod> item)
        {
            throw new NotSupportedException();
            //if (item.Value == null)
            //    return false; // Item.Value must not be null.
            //if (item.Key != item.Value.Name)
            //    return false; // Item.Key must be equal to Item.Value.Name.
            //return this.Remove(item.Value);
        }

        IEnumerator<KeyValuePair<Symbol, CompiledMethod>> IEnumerable<KeyValuePair<Symbol, CompiledMethod>>.GetEnumerator()
        {
            return this.Contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region interface IDictionary<string, CompiledMethod>

        void IDictionary<string, CompiledMethod>.Add(string selector, CompiledMethod value)
        {
            throw new NotSupportedException();
            //if (selector == null)
            //    throw new ArgumentNullException("selector");
            //if (value == null)
            //    throw new ArgumentNullException("value");
            //if (selector != value.Name.Value)
            //    throw new ArgumentException("Key must be equal to Value.Name.Value.");
            //this.Add(value);
        }

        bool IDictionary<string, CompiledMethod>.ContainsKey(string selector)
        {
            return this.ContainsKey(selector);
        }

        ICollection<string> IDictionary<string, CompiledMethod>.Keys
        {
            get { return this.Keys.Select(symbol => symbol.Value).ToArray(); }
        }

        bool IDictionary<string, CompiledMethod>.Remove(string selector)
        {
            throw new NotSupportedException();
            //return this.Remove(selector);
        }

        bool IDictionary<string, CompiledMethod>.TryGetValue(string selector, out CompiledMethod value)
        {
            return this.TryGetValue(selector, out value);
        }

        ICollection<CompiledMethod> IDictionary<string, CompiledMethod>.Values
        {
            get { return this.Values; }
        }

        CompiledMethod IDictionary<string, CompiledMethod>.this[string selector]
        {
            get
            {
                return this[selector];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        void ICollection<KeyValuePair<string, CompiledMethod>>.Add(KeyValuePair<string, CompiledMethod> item)
        {

            //if (item.Value == null)
            //    throw new ArgumentException("Item.Value must not be null.");
            //if (item.Key != item.Value.Name.Value)
            //    throw new ArgumentException("Item.Key must be equal to Item.Value.Name.Value.");
            //this.Add(item.Value);
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<string, CompiledMethod>>.Clear()
        {
            throw new NotSupportedException();
            //this.Clear();
        }

        bool ICollection<KeyValuePair<string, CompiledMethod>>.Contains(KeyValuePair<string, CompiledMethod> item)
        {
            CompiledMethod candidate;
            return this.TryGetValue(item.Key, out candidate) && Object.ReferenceEquals(candidate, item.Value);
        }

        void ICollection<KeyValuePair<string, CompiledMethod>>.CopyTo(KeyValuePair<string, CompiledMethod>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "arrayIndex is less than 0.");
            if (array.Length < (this.Count + arrayIndex))
                throw new ArgumentException("The number of elements in the source ICollection <T> is greater than the available space from arrayIndex to the end of the destination array.");

            foreach (CompiledMethod item in this)
            {
                array[arrayIndex] = new KeyValuePair<string, CompiledMethod>(item.Selector.Value, item);
                arrayIndex++;
            }
        }

        int ICollection<KeyValuePair<string, CompiledMethod>>.Count
        {
            get { return this.Count; }
        }

        bool ICollection<KeyValuePair<string, CompiledMethod>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<string, CompiledMethod>>.Remove(KeyValuePair<string, CompiledMethod> item)
        {
            throw new NotSupportedException();
            //if (item.Value == null)
            //    return false; // Item.Value must not be null.
            //if (item.Key != item.Value.Name.Value)
            //    return false; // Item.Key must be equal to Item.Value.Name.Value.
            //return this.Remove(item.Value);
        }

        IEnumerator<KeyValuePair<string, CompiledMethod>> IEnumerable<KeyValuePair<string, CompiledMethod>>.GetEnumerator()
        {
            return this.Contents.Select(pair => new KeyValuePair<string, CompiledMethod>(pair.Key.Value, pair.Value)).GetEnumerator();
        }

        #endregion

        #region interface IDictionary

        //void IDictionary.Add(object selector, object value)
        //{
        //    Symbol ckey;
        //    try
        //    {
        //        ckey = (Symbol)selector;
        //    }
        //    catch (InvalidCastException)
        //    {
        //        throw new ArgumentException(String.Format(
        //            "The value \"{0}\" is not of type \"{1}\" and cannot be used in this generic collection.", selector, typeof(Symbol)), "selector");
        //    }
        //    TItem cvalue;
        //    try
        //    {
        //        cvalue = (TItem)value;
        //    }
        //    catch (InvalidCastException)
        //    {
        //        throw new ArgumentException(String.Format(
        //            "The value \"{0}\" is not of type \"{1}\" and cannot be used in this generic collection.", selector, typeof(Symbol)), "value");
        //    }

        //    if (ckey == null)
        //        throw new ArgumentNullException("selector");
        //    if (cvalue == null)
        //        throw new ArgumentNullException("value");

        //    ((IDictionary<Symbol, TItem>)this).Add(ckey, cvalue);
        //}

        //bool IDictionary.Contains(object selector)
        //{
        //    if (selector == null)
        //        throw new ArgumentNullException("selector");
        //    if (selector is Symbol)
        //        return this.ContainsKey((Symbol)selector);
        //    if (selector is string)
        //        return this.ContainsKey((string)selector);
        //    return false;
        //}

        //IDictionaryEnumerator IDictionary.GetEnumerator()
        //{
        //    return this.GetEnumerator();
        //}

        //bool IDictionary.IsFixedSize
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //bool IDictionary.IsReadOnly
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //ICollection IDictionary.Keys
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //void IDictionary.Remove(object selector)
        //{
        //    throw new NotImplementedException();
        //}

        //ICollection IDictionary.Values
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //object IDictionary.this[object selector]
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        #endregion

        #region Interface ICollection<CompiledMethod>

        void ICollection<CompiledMethod>.Add(CompiledMethod item)
        {
            throw new NotSupportedException();
            //this.Add(item);
        }

        void ICollection<CompiledMethod>.Clear()
        {
            throw new NotSupportedException();
            //this.Clear();
        }

        bool ICollection<CompiledMethod>.Contains(CompiledMethod item)
        {
            return this.Contains(item);
        }

        void ICollection<CompiledMethod>.CopyTo(CompiledMethod[] array, int arrayIndex)
        {
            this.CopyTo(array, arrayIndex);
        }

        int ICollection<CompiledMethod>.Count
        {
            get { return this.Count; }
        }

        bool ICollection<CompiledMethod>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<CompiledMethod>.Remove(CompiledMethod item)
        {
            throw new NotSupportedException();
            //return this.Remove(item);
        }

        IEnumerator<CompiledMethod> IEnumerable<CompiledMethod>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region interface ICollection

        //void ICollection.CopyTo(Array array, int index)
        //{
        //    throw new NotImplementedException();
        //}

        //int ICollection.Count
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //bool ICollection.IsSynchronized
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //object ICollection.SyncRoot
        //{
        //    get { throw new NotImplementedException(); }
        //}

        #endregion

        private Dictionary<int, Dictionary<string, CompiledMethod>> _nativeMethodNameMap;

        private Dictionary<int, Dictionary<string, CompiledMethod>> NativeMethodNameMap
        {
            get
            {
                // Things are cached to speed-up lookups
                Dictionary<int, Dictionary<string, CompiledMethod>> mapping = this._nativeMethodNameMap;
                if (mapping == null)
                {
                    // Build the cache if not built ...
                    mapping = new Dictionary<int, Dictionary<string, CompiledMethod>>();
                    foreach (CompiledMethod mth in this)
                    {
                        var nativeName = mth.NativeName;
                        if (!String.IsNullOrWhiteSpace(nativeName))
                        {
                            int numArgs = mth.NumberOfArguments;
                            Dictionary<string, CompiledMethod> map = null;
                            mapping.TryGetValue(numArgs, out map);
                            if (map == null)
                            {
                                map = new Dictionary<string, CompiledMethod>();
                                mapping[numArgs] = map;
                            }

                            map[nativeName] = mth;
                        }
                    }
                    this._nativeMethodNameMap = mapping;
                }
                return mapping;
            }
        }

        /// <summary>
        /// This is a helper method that look-ups a compiled method by its
        /// native name and the number of arguments it accepts.
        /// </summary>
        /// <param name="name">Native name of the method, as defined in the "ist.runtime.native-name" annotation.</param>
        /// <param name="numberOfParameters">Number of arguments expected by the method.</param>
        /// <param name="ignoreCase">Determines if the method lookup is performed case-insensitively.</param>
        /// <param name="caseConflict">For case-insensitive method lookups, tells if multiple matches were fould.</param>
        /// <returns></returns>
        public CompiledMethod GetMethodByNativeName(string name, int numberOfParameters, bool ignoreCase, out bool caseConflict)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (numberOfParameters < 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfParameters));

            // Things are cached to speed-up lookups
            Dictionary<int, Dictionary<string, CompiledMethod>> mapping = this.NativeMethodNameMap;

            // Get the compiled method from the mapping (num-of-params / name).
            caseConflict = false;
            Dictionary<string, CompiledMethod> innerMap = null;
            mapping.TryGetValue(numberOfParameters, out innerMap);
            if (innerMap == null)
                return null;
            CompiledMethod method = null;
            if (ignoreCase)
            {
                foreach (var pair in innerMap)
                {
                    if (String.Equals(pair.Key, name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (method == null)
                        {
                            method = pair.Value;
                        }
                        else
                        {
                            caseConflict = true;
                            return null;
                        }
                    }
                }
            }
            else
            {
                innerMap.TryGetValue(name, out method);
            }
            return method;
        }

        /// <summary>
        /// Returns the names of the methods that are exported and visible to .Net.
        /// </summary>
        /// <returns>Collection of exported names.</returns>
        public IEnumerable<string> GetNativeMethodNames()
        {
            HashSet<string> result = new HashSet<string>();
            foreach (var p1 in this.NativeMethodNameMap)
            {
                foreach (var p2 in p1.Value)
                    result.Add(p2.Key);
            }
            return result;
        }
    }

    /// <summary>
    /// A dictionary containing class methods.
    /// </summary>
    public class ClassMethodDictionary : MethodDictionary
    {
        /// <summary>
        /// Create a new method dictionary.
        /// </summary>
        /// <param name="runtime">Smalltalk runtime that the methods in the dictionary belong to.</param>
        public ClassMethodDictionary(SmalltalkRuntime runtime)
            : base(runtime)
        {
        }

        /// <summary>
        /// Create a new method dictionary that's lazy initializable.
        /// </summary>
        /// <param name="runtime">Smalltalk runtime that the methods in the dictionary belong to.</param>
        /// <param name="lazyInitializer">Callback function for lazy initializing of the contents of the method dictionary.</param>
        /// <remarks
        /// >In rare multi-threaded conditions the lazyInitializer function may be called more than once. 
        /// Only one of the results will be stored, and the rest discarded! Therefore, the initializer 
        /// function should be robust without side-effects if run more than once.
        /// </remarks>
        public ClassMethodDictionary(SmalltalkRuntime runtime, Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>> lazyInitializer)
            : base(runtime, lazyInitializer)
        {
        }

        public override CompiledMethod.MethodType MethodType
        {
            get { return CompiledMethod.MethodType.Class; }
        }
    }

    /// <summary>
    /// A dictionary containing instance methods.
    /// </summary>
    public class InstanceMethodDictionary : MethodDictionary
    {
        /// <summary>
        /// Create a new method dictionary.
        /// </summary>
        /// <param name="runtime">Smalltalk runtime that the methods in the dictionary belong to.</param>
        public InstanceMethodDictionary(SmalltalkRuntime runtime)
            : base(runtime)
        {
        }

        /// <summary>
        /// Create a new method dictionary that's lazy initializable.
        /// </summary>
        /// <param name="runtime">Smalltalk runtime that the methods in the dictionary belong to.</param>
        /// <param name="lazyInitializer">Callback function for lazy initializing of the contents of the method dictionary.</param>
        /// <remarks
        /// >In rare multi-threaded conditions the lazyInitializer function may be called more than once. 
        /// Only one of the results will be stored, and the rest discarded! Therefore, the initializer 
        /// function should be robust without side-effects if run more than once.
        /// </remarks>
        public InstanceMethodDictionary(SmalltalkRuntime runtime, Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>> lazyInitializer)
            : base(runtime, lazyInitializer)
        {
        }

        public override CompiledMethod.MethodType MethodType
        {
            get { return CompiledMethod.MethodType.Instance; }
        }
    }
}
