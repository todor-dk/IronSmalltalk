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
using IronSmalltalk.Common;

namespace IronSmalltalk.DefinitionInstaller
{
    /// <summary>
    /// Reference to a location in a source code.
    /// </summary>
    public class SourceReference : ISourceReference
    {
        private readonly SourceLocation _StartPosition;
        private readonly SourceLocation _StopPosition;
        private readonly ISourceCodeReferenceService SourceCodeReferenceService;

        public SourceReference(SourceLocation startPosition, SourceLocation stopPosition, ISourceCodeReferenceService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            this._StartPosition = startPosition;
            this._StopPosition = stopPosition;
            this.SourceCodeReferenceService = service;
        }

        /// <summary>
        /// Absolute start location in the source code that defines the value.
        /// </summary>
        public SourceLocation StartPosition
        {
            get
            {
                if (this.SourceCodeReferenceService == null)
                    return SourceLocation.Invalid;
                return this.SourceCodeReferenceService.TranslateSourcePosition(this._StartPosition);
            }
        }

        /// <summary>
        /// Absolute stop location in the source code that defines the value.
        /// </summary>
        public SourceLocation StopPosition
        {
            get
            {
                if (this.SourceCodeReferenceService == null)
                    return SourceLocation.Invalid;
                return this.SourceCodeReferenceService.TranslateSourcePosition(this._StopPosition);
            }
        }

        /// <summary>
        /// Return the source code service for this source code reference.
        /// The service can translate the locations of source references from relative to absolute positions.
        /// </summary>
        public ISourceCodeReferenceService Service 
        {
            get { return this.SourceCodeReferenceService; }
        }
    }

    /// <summary>
    /// Represents a reference to a source code.
    /// </summary>
    /// <remarks>
    /// References to interesting values are wrapped in instances of this class.
    /// This allows us to get the value using the Value property, but we also
    /// have indication where in the source code that value was generated, 
    /// and if error is encountered, we can tell the user which source code
    /// is the offending code.
    /// 
    /// Having reference to source location is optional. In other words, it is
    /// legal to let the source location attributes be empty and just set the Value.
    /// </remarks>
    /// <typeparam name="TValue">Type of the value we are wrapping.</typeparam>
    public class SourceReference<TValue> : SourceReference
    {
        /// <summary>
        /// Value being wrapped in the source reference.
        /// </summary>
        public TValue Value { get; private set; }

        /// <summary>
        /// Create a new source reference wrapped value and set the source locations.
        /// </summary>
        /// <param name="value">Value being wrapped.</param>
        /// <param name="startPosition">Start location in the source code that defines the value.</param>
        /// <param name="stopPosition">Stop location in the source code that defines the value.</param>
        /// <param name="service">Source reference service that translates the locations from relative to absolute positions.</param>
        public SourceReference(TValue value, SourceLocation startPosition, SourceLocation stopPosition, ISourceCodeReferenceService service)
            : base (startPosition, stopPosition, service)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            this.Value = value;
        }
    }
}
