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
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.Runtime.Behavior
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CompiledCode : IAnnotetable
    {
        #region Annotations

        /// <summary>
        /// Annotations that may be added to the binding.
        /// </summary>
        protected Dictionary<string, string> _Annotations;

        /// <summary>
        /// The annotation pairs associated with the annotetable object.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Annotations
        {
            get
            {
                if (this._Annotations == null)
                    return AnnotationsHelper.Empty;
                return this._Annotations;
            }
        }

        /// <summary>
        /// Set (or overwrite) an annotation on the annotetable object.
        /// </summary>
        /// <param name="key">Key of the annotation.</param>
        /// <param name="value">Value or null to remove the annotation.</param>
        public void Annotate(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException();
            if (value == null)
            {
                if (this._Annotations == null)
                    return;
                this._Annotations.Remove(key);
            }
            else
            {
                if (this._Annotations == null)
                    this._Annotations = new Dictionary<string, string>();
                this._Annotations[key] = value;
            }
        }

        #endregion
    }
}
