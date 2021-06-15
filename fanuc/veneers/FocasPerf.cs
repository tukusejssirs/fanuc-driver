using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using l99.driver.@base;
using MoreLinq;

namespace l99.driver.fanuc.veneers
{
    public class FocasPerf : Veneer
    {
        public FocasPerf(string name = "", bool isInternal = false) : base(name, isInternal)
        {
            _lastChangedValue = new
            {
                invocation = new
                {
                    maxMethod = string.Empty,
                    maxMs = -1,
                    minMs = -1,
                    avgMs = -1,
                    failedMethods = new List<string>()
                }
            };
        }

        protected override async Task<dynamic> AnyAsync(dynamic input, params dynamic?[] additional_inputs)
        {
            var max = ((List<dynamic>)input.focas_invocations).MaxBy(o => o.invocationMs).First();
            var min = ((List<dynamic>)input.focas_invocations).MinBy(o => o.invocationMs).First();
            var avg = (int)((List<dynamic>)input.focas_invocations).Average(o => (int)o.invocationMs);
            var sum = ((List<dynamic>) input.focas_invocations).Sum(o => (int)o.invocationMs);
            var failedMethods = ((List<dynamic>) input.focas_invocations)
                .Where(o => o.rc != 0)
                .Select(o => new { o.method, o.rc });

            var current_value = new
            {
                input.sweepMs,
                invocation = new
                {
                    maxMethod = max.method,
                    maxMs = max.invocationMs,
                    minMs = min.invocationMs,
                    avgMs = avg,
                    sumMs = sum,
                    failedMethods
                }
            };;

            await onDataArrivedAsync(input, current_value);

            return new { veneer = this };
        }
    }
}