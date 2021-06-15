using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> RdSeqNumAsync()
        {
            return await Task.FromResult(RdSeqNum());
        }

        public dynamic RdSeqNum()
        {
            Focas1.ODBSEQ seqnum = new Focas1.ODBSEQ();

            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas1.focas_ret) Focas1.cnc_rdseqnum(_handle, seqnum);
            });

            var nr = new
            {
                method = "cnc_rdseqnum",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = "https://www.inventcom.net/fanuc-focas-library/program/cnc_rdseqnum",
                success = ndr.RC == Focas1.EW_OK,
                rc = ndr.RC,
                request = new {cnc_rdseqnum = new { }},
                response = new {cnc_rdseqnum = new {seqnum}}
            };

            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");

            return nr;
        }
    }
}