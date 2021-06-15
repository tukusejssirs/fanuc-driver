using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> RdMacroAsync(short number = 1, short length = 10)
        {
            return await Task.FromResult(RdMacro(number, length));
        }

        public dynamic RdMacro(short number = 1, short length = 10)
        {
            Focas1.ODBM macro = new Focas1.ODBM();

            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas1.focas_ret) Focas1.cnc_rdmacro(_handle, number, length, macro);
            });

            var nr = new
            {
                method = "cnc_rdmacro",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = "https://www.inventcom.net/fanuc-focas-library/ncdata/cnc_rdmacro",
                success = ndr.RC == Focas1.EW_OK,
                rc = ndr.RC,
                request = new {cnd_rdmacro = new {number, length}},
                response = new {cnd_rdmacro = new {macro}}
            };

            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");

            return nr;
        }
    }
}