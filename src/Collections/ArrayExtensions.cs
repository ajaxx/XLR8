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

namespace XLR8.Collections
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Fills the specified array with the given value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayThis">The array this.</param>
        /// <param name="value">The value.</param>
        public static void Fill<T>(this T[] arrayThis, T value)
        {
            for (int ii = 0; ii < arrayThis.Length; ii++)
            {
                arrayThis[ii] = value;
            }
        }
    }
}
