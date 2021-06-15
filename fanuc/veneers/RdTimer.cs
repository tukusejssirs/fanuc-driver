using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using l99.driver.@base;

namespace l99.driver.fanuc.veneers
{
    public class RdTimer: Veneer
    {
        public RdTimer(string name = "", bool isInternal = false) : base(name, isInternal)
        {
            _lastChangedValue = new
            {
                minute = -1,
                msec = -1
            };
        }

        protected override async Task<dynamic> AnyAsync(dynamic input, params dynamic?[] additional_inputs)
        {
            if (input.success)
            {
                var current_value = new
                {
                    input.response.cnc_rdtimer.time.minute,
                    input.response.cnc_rdtimer.time.msec
                };

                await onDataArrivedAsync(input, current_value);

                if (!current_value.Equals(this._lastChangedValue))
                {
                    await onDataChangedAsync(input, current_value);
                }
            }
            else
            {
                await onErrorAsync(input);
            }

            return new { veneer = this };
        }
    }
}