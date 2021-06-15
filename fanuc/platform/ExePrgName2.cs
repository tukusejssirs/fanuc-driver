using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> ExePrgName2Async()
        {
            return await Task.FromResult(ExePrgName2());
        }

        public dynamic ExePrgName2()
        {
            char[] path_name = new char[256];

            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas1.focas_ret) Focas1.cnc_exeprgname2(_handle, path_name);
            });

            var nr= new
            {
                method = "cnc_exeprgname2",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = "https://www.inventcom.net/fanuc-focas-library/program/cnc_exeprgname2",
                success = ndr.RC == Focas1.EW_OK,
                rc = ndr.RC,
                request = new {cnc_exeprgname2 = new { }},
                response = new {cnc_exeprgname2 = new {path_name}}
            };

            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");

            return nr;
        }
    }
}