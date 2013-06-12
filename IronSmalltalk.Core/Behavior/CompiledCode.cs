using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.Runtime.Behavior
{
    public abstract class CompiledCode : IAnnotetable
    {
        #region Annotations

        /// <summary>
        /// Annotations that may be added to the binding.
        /// </summary>
        protected Dictionary<string, string> _annotations;

        /// <summary>
        /// The annotation pairs associated with the annotetable object.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Annotations
        {
            get
            {
                if (this._annotations == null)
                    return AnnotationsHelper.Empty;
                return this._annotations;
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
                if (this._annotations == null)
                    return;
                this._annotations.Remove(key);
            }
            else
            {
                if (this._annotations == null)
                    this._annotations = new Dictionary<string, string>();
                this._annotations[key] = value;
            }
        }

        #endregion
    }
}
