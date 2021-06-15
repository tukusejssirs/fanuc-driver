using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> RdPrgNumAsync()
        {
            return await Task.FromResult(RdPrgNum());
        }

        public dynamic RdPrgNum()
        {
            Focas1.ODBPRO prgnum = new Focas1.ODBPRO();

            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas1.focas_ret) Focas1.cnc_rdprgnum(_handle, prgnum);
            });

            var nr = new
            {
                method = "cnc_rdprgnum",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = "https://www.inventcom.net/fanuc-focas-library/program/cnc_rdprgnum",
                success = ndr.RC == Focas1.EW_OK,
                rc = ndr.RC,
                request = new {cnc_rdprgnum = new { }},
                response = new {cnc_rdprgnum = new {prgnum}}
            };

            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");

            return nr;
        }
    }
}