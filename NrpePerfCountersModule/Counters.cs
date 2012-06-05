// Copyright (c) 2012, XBRL Cloud Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer. Redistributions in
// binary form must reproduce the above copyright notice, this list of
// conditions and the following disclaimer in the documentation and/or
// other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
// IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NrpePerfCountersModule
{
    internal class PCounter : IDisposable
    {
        public PCounter()
        {
            _values = new Queue<float>();
            _values.Enqueue(0);
        }
        public PerfCounter Config { get; set; }
        public PerformanceCounter Counter { get; set; }
        public float Value
        {
            get
            {
                if (Config.CalcMethod == 0) // avg
                    return _values.Average();
                else if (Config.CalcMethod == 1) // min
                    return _values.Min();
                else if (Config.CalcMethod == 2) // max
                    return _values.Max();
                else // sum
                    return _values.Sum();
            }
        }
        public DateTime LastChecked { get; set; }

        private Queue<float> _values;

        public void Check()
        {
            if (Counter == null)
                return;
            float value = Counter.NextValue();
            int queueLength = (int)(Config.Duration < 5 ? 5 : Config.Duration) / 5; // 5 ss - minimum checking interval

            LastChecked = DateTime.Now;

            _values.Enqueue(value);
            while (_values.Count > queueLength)
            {
                _values.Dequeue();
            }
        }

        public void Dispose()
        {
            if (Counter != null)
            {
                Counter.Close();
                Counter.Dispose();
            }
        }
    }
}
