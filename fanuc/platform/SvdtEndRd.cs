using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> SvdtEndRdAsync()
        {
            return await Task.FromResult(SvdtEndRd());
        }
        
        public dynamic SvdtEndRd()
        {
            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas.focas_ret) Focas.cnc_svdtendrd(_handle);
            });

            var nr = new
            {
                method = "cnc_svdtendrd",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = "https://www.inventcom.net/fanuc-focas-library/servo/cnc_svdtendrd",
                success = ndr.RC == Focas.EW_OK,
                rc = ndr.RC,
                request = new {cnc_svdtendrd = new {}},
                response = new {cnc_svdtendrd = new {}}
            };
            
            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");

            return nr;
        }
    }
}