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
