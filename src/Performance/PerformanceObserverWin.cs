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
using System.Runtime.InteropServices;

namespace XLR8.Performance
{
    /// <summary>
    /// Implementation of the performance observer turned for use on Windows.
    /// </summary>

    public class PerformanceObserverWin
    {
        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);

        [DllImport("Kernel32.dll", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        public static extern Int32 GetCurrentWin32ThreadId();

        public static long Frequency;
        public static double MpMilli;
        public static double MpMicro;
        public static double MpNano;
        
        public static int SpinIterationsPerMicro;

        static PerformanceObserverWin()
        {
            Calibrate();
        }

        public static void Calibrate()
        {
            QueryPerformanceFrequency(out Frequency);
            MpMilli = 1000.0 / Frequency;
            MpMicro = 1000000.0 / Frequency;
            MpNano = 1000000000.0 / Frequency;
            
            // Our goal is to increase the iterations until we get at least 100 microseconds of
            // actual spin latency.
            
            long numCounter = (long) (Frequency / 1000.0);
            
            for( int nn = 2 ;; nn *= 2 ) {
            	long timeA;
            	long timeB;
            	QueryPerformanceCounter(out timeA);
            	System.Threading.Thread.SpinWait(nn);
            	QueryPerformanceCounter(out timeB);
            	
            	var measured = timeB - timeA;
            	if (measured >= numCounter) {
            		// We have achieved at least 1000 microseconds of delay, now computer
            		// the number of iterations per microsecond.
            		var numMicros = measured * MpMicro;
            		SpinIterationsPerMicro = (int) (((double) nn) / numMicros);
					break;
            	}
            }
        }

        public static long NanoTime
        {
            get
            {
                long time;
                QueryPerformanceCounter(out time);
                return (long)(time * MpNano);
            }
        }

        public static long MicroTime
        {
            get
            {
                long time;
                QueryPerformanceCounter(out time);
                return (long)(time * MpMicro);
            }
        }
        
        public static long MilliTime
        {
            get
            {
                long time;
                QueryPerformanceCounter(out time);
                return (long)(time * MpMilli);
            }
        }

        public static long TimeNano(Action r)
        {
            long timeA;
            long timeB;

            QueryPerformanceCounter(out timeA);
            r.Invoke();
            QueryPerformanceCounter(out timeB);
            return (long)((timeB - timeA) * MpNano);
        }

        public static long TimeMicro(Action r)
        {
            long timeA;
            long timeB;

            QueryPerformanceCounter(out timeA);
            r.Invoke();
            QueryPerformanceCounter(out timeB);
            return (long)((timeB - timeA) * MpMicro);
        }

        public static long TimeMillis(Action r)
        {
            long timeA;
            long timeB;

            QueryPerformanceCounter(out timeA);
            r.Invoke();
            QueryPerformanceCounter(out timeB);
            return (long)((timeB - timeA) * MpMilli);
        }
    }
}
