using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> SvdtStartRdAsync(short axis)
        {
            return await Task.FromResult(SvdtStartRd(axis));
        }

        public dynamic SvdtStartRd(short axis)
        {
            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas1.focas_ret) Focas1.cnc_svdtstartrd(_handle, axis);
            });

            var nr = new
            {
                method = "cnc_svdtstartrd",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = "https://www.inventcom.net/fanuc-focas-library/servo/cnc_svdtstartrd",
                success = ndr.RC == Focas1.EW_OK,
                rc = ndr.RC,
                request = new {cnc_svdtstartrd = new {axis}},
                response = new {cnc_svdtstartrd = new {}}
            };

            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");

            return nr;
        }
    }
}