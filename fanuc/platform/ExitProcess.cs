using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> ExitProcessAsync()
        {
            return await Task.FromResult(ExitProcess());
        }

        public dynamic ExitProcess()
        {
#if ARMV7 || LINUX64 || LINUX32
            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas1.focas_ret)Focas1.cnc_exitprocess();
            });

            var nr = new
            {
                method = "cnc_exitprocess",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = "",
                success = ndr.RC == Focas1.EW_OK,
                rc = ndr.RC,
                request = new { cnc_exitprocess = new {  } },
                response = new { cnc_exitprocess = new { } }
            };

            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");

            return nr;
#else
            var nr = new
            {
                method = "cnc_exitprocess",
                invocationMs = -1,
                doc = "",
                success = true,
                Focas1.EW_OK,
                request = new {cnc_exitprocess = new { }},
                response = new {cnc_exitprocess = new { }}
            };


            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");

            return nr;
#endif
        }
    }
}