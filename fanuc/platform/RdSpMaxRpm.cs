using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> RdSpMaxRpmAsync(short sp_no = 1)
        {
            return await Task.FromResult(RdSpMaxRpm(sp_no));
        }

        public dynamic RdSpMaxRpm(short sp_no = 1)
        {
            Focas1.ODBSPN serialspindle = new Focas1.ODBSPN();

            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas1.focas_ret) Focas1.cnc_rdspmaxrpm(_handle, sp_no, serialspindle);
            });

            var nr = new
            {
                method = "cnc_rdspmaxrpm",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = "https://www.inventcom.net/fanuc-focas-library/position/cnc_rdspmaxrpm",
                success = ndr.RC == Focas1.EW_OK,
                rc = ndr.RC,
                request = new {cnc_rdspmaxrpm = new {sp_no}},
                response = new {cnc_rdspmaxrpm = new {serialspindle}}
            };

            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");

            return nr;
        }
    }
}