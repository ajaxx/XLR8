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

namespace XLR8.Performance
{
    /// <summary>
    /// Implementation of the performance observer tuned for use on Windows.
    /// </summary>

    public class PerformanceObserverMono
    {
        public static long NanoTime
        {
            get { return DateTime.Now.Ticks*100; }
        }

        public static long MicroTime
        {
            get { return DateTime.Now.Ticks / 10; }
        }

        public static long MilliTime
        {
            get { return DateTime.Now.Ticks / 10000; }
        }

        public static long TimeNano(Action action)
        {
            long timeA = DateTime.Now.Ticks;
            action.Invoke();
            long timeB = DateTime.Now.Ticks;
            return 100*(timeB - timeA);
        }

        public static long TimeMicro(Action action)
        {
            long timeA = DateTime.Now.Ticks;
            action.Invoke();
            long timeB = DateTime.Now.Ticks;
            return (timeB - timeA)/10;
        }

        public static long TimeMillis(Action action)
        {
            long timeA = DateTime.Now.Ticks;
            action.Invoke();
            long timeB = DateTime.Now.Ticks;
            return (timeB - timeA)/10000;
        }
    }
}
