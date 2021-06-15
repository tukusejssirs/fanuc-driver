using System.Threading.Tasks;
using l99.driver.@base;

namespace l99.driver.fanuc.veneers
{
    public class SysInfo : Veneer
    {
        public SysInfo(string name = "", bool isInternal = false) : base(name, isInternal)
        {
            _lastChangedValue = new
            {
                max_axis = -1,
                cnc_type = string.Empty,
                mt_type = string.Empty,
                series = string.Empty,
                version = string.Empty,
                axes = string.Empty
            };
        }

        protected override async Task<dynamic> AnyAsync(dynamic input, params dynamic?[] additional_inputs)
        {
            if (input.success)
            {
                var current_value = new
                {
                    input.response.cnc_sysinfo.sysinfo.addinfo,
                    input.response.cnc_sysinfo.sysinfo.max_axis,
                    cnc_type = string.Join("", input.response.cnc_sysinfo.sysinfo.cnc_type),
                    mt_type = string.Join("", input.response.cnc_sysinfo.sysinfo.mt_type),
                    series = string.Join("", input.response.cnc_sysinfo.sysinfo.series),
                    version = string.Join("", input.response.cnc_sysinfo.sysinfo.version),
                    axes = string.Join("", input.response.cnc_sysinfo.sysinfo.axes)
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