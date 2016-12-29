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
    public class PerformanceObserver
    {
        public static long NanoTime
        {
            get
            {
#if MONO
                return PerformanceObserverMono.NanoTime;
#else
                return PerformanceObserverWin.NanoTime;
#endif
            }
        }

        public static long MicroTime
        {
            get
            {
#if MONO
                return PerformanceObserverMono.MicroTime;
#else
                return PerformanceObserverWin.MicroTime;
#endif
            }
        }

        public static long MilliTime
        {
            get
            {
#if MONO
                return PerformanceObserverMono.MilliTime;
#else
                return PerformanceObserverWin.MilliTime;
#endif
            }
        }

        public static long TimeNano(Action action)
        {
#if MONO
            return PerformanceObserverMono.TimeNano(action);
#else
            return PerformanceObserverWin.TimeNano(action);
#endif
        }

        public static long TimeMicro(Action action)
        {
#if MONO
            return PerformanceObserverMono.TimeMicro(action);
#else
            return PerformanceObserverWin.TimeMicro(action);
#endif
        }

        public static long TimeMillis(Action action)
        {
#if MONO
            return PerformanceObserverMono.TimeMillis(action);
#else
            return PerformanceObserverWin.TimeMillis(action);
#endif
        }

        public static long GetTimeMillis()
        {
            return MilliTime;
        }

        public static long GetTimeMicros()
        {
            return MicroTime;
        }

        public static long GetTimeNanos()
        {
            return NanoTime;
        }
    }
}
