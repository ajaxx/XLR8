// --------------------------------------------------------------------------------
// Copyright (c) 2014, XLR8 Development
// --------------------------------------------------------------------------------
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// --------------------------------------------------------------------------------

using System;

namespace XLR8.Threading
{
	public class WeakReference<T> : WeakReference where T : class
	{
        private readonly int _hashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
		public WeakReference( T target )
			: base( target, false )
		{
		    _hashCode = target.GetHashCode();
		}

        /// <summary>
        /// Gets a value indicating whether this instance is dead.
        /// </summary>
        /// <value><c>true</c> if this instance is dead; otherwise, <c>false</c>.</value>
	    public bool IsDead
	    {
            get { return !IsAlive; }
	    }

        /// <summary>
        /// Gets or sets the object (the target) referenced by the current <see cref="T:System.WeakReference"></see> object.
        /// </summary>
        /// <value></value>
        /// <returns>null if the object referenced by the current <see cref="T:System.WeakReference"></see> object has been garbage collected; otherwise, a reference to the object referenced by the current <see cref="T:System.WeakReference"></see> object.</returns>
        /// <exception cref="T:System.InvalidOperationException">The reference to the target object is invalid. This can occur if the current <see cref="T:System.WeakReference"></see> object has been finalized.</exception>
		public new T Target
		{
			get { return (T) base.Target; }
		}

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return _hashCode;
        }
	}
}
