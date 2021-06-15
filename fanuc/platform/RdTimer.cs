using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> RdTimerAsync(short type = 0)
        {
            return await Task.FromResult(RdTimer(type));
        }

        public dynamic RdTimer(short type = 0)
        {
            Focas1.IODBTIME time = new Focas1.IODBTIME();

            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas1.focas_ret) Focas1.cnc_rdtimer(_handle, type, time);
            });

            var nr = new
            {
                method = "cnc_rdtimer",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = "https://www.inventcom.net/fanuc-focas-library/misc/cnc_rdtimer",
                success = ndr.RC == Focas1.EW_OK,
                rc = ndr.RC,
                request = new {cnc_rdtimer = new {type}},
                response = new {cnc_rdtimer = new {time}}
            };

            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");

            return nr;
        }
    }
}