using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> RdActPtAsync()
        {
            return await Task.FromResult(RdActPt());
        }

        public dynamic RdActPt()
        {
            int prog_no = 0;
            int blk_no = 0;

            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas1.focas_ret) Focas1.cnc_rdactpt(_handle, out prog_no, out blk_no);
            });

            var nr = new
            {
                method = "cnc_rdactpt",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = "https://www.inventcom.net/fanuc-focas-library/program/cnc_rdactpt",
                success = ndr.RC == Focas1.EW_OK,
                rc = ndr.RC,
                request = new {cnc_rdactpt = new { }},
                response = new {cnc_rdactpt = new {prog_no, blk_no}}
            };

            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");

            return nr;
        }
    }
}