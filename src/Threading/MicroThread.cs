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

using System.Threading;

namespace XLR8.Threading
{
    using Performance;

    public class MicroThread
    {
        public static void Sleep(long uSeconds)
        {
            var spins = PerformanceObserverWin.SpinIterationsPerMicro;
            var uHead = PerformanceObserver.MicroTime;
            var uTail = uHead + uSeconds;
            var uApprox = uSeconds/spins;

            do
            {
                Thread.SpinWait((int) uApprox);
                uHead = PerformanceObserver.MicroTime;
                if (uHead >= uTail)
                    return;

                uApprox = (uTail - uHead)/spins;
            } while (true);
        }

        public static void SleepNano(long nanoSeconds)
        {
            var spins = 1000*PerformanceObserverWin.SpinIterationsPerMicro;
            var uHead = PerformanceObserver.NanoTime;
            var uTail = uHead + nanoSeconds;
            var uApprox = nanoSeconds/spins;

            do
            {
                Thread.SpinWait((int)uApprox);
                uHead = PerformanceObserver.NanoTime;
                if (uHead >= uTail)
                    return;

                uApprox = (uTail - uHead)/spins;
            } while (true);
        }
    }
}
