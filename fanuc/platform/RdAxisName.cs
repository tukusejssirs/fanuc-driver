using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> RdAxisNameAsync(short data_num = 8)
        {
            return await Task.FromResult(RdAxisName(data_num));
        }

        public dynamic RdAxisName(short data_num = 8)
        {
            short data_num_out = data_num;
            Focas1.ODBAXISNAME axisname = new Focas1.ODBAXISNAME();

            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas1.focas_ret) Focas1.cnc_rdaxisname(_handle, ref data_num_out, axisname);
            });

            var nr = new
            {
                method = "cnc_rdaxisname",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = "https://www.inventcom.net/fanuc-focas-library/position/cnc_rdaxisname",
                success = ndr.RC == Focas1.EW_OK,
                rc = ndr.RC,
                request = new {cnc_rdaxisname = new {data_num}},
                response = new {cnc_rdaxisname = new {data_num = data_num_out, axisname}}
            };

            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");

            return nr;
        }
    }
}