using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> RdPmcRngAsync(short adr_type, short data_type, ushort s_number, ushort e_number, ushort length,
            int IODBPMC_type)
        {
            return await Task.FromResult(RdPmcRng(adr_type, data_type, s_number, e_number, length, IODBPMC_type));
        }

        public dynamic RdPmcRng(short adr_type, short data_type, ushort s_number, ushort e_number, ushort length,
            int IODBPMC_type)
        {
            dynamic buf = new object();

            switch (IODBPMC_type)
            {
                case 0:
                    buf = new Focas1.IODBPMC0();
                    break;
                case 1:
                    buf = new Focas1.IODBPMC1();
                    break;
                case 2:
                    buf = new Focas1.IODBPMC2();
                    break;
            }

            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas1.focas_ret) Focas1.pmc_rdpmcrng(_handle, adr_type, data_type, s_number, e_number,
                    length, buf);
            });

            var nr = new
            {
                method = "pmc_rdpmcrng",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = "https://www.inventcom.net/fanuc-focas-library/pmc/pmc_rdpmcrng",
                success = ndr.RC == Focas1.EW_OK,
                rc = ndr.RC,
                request = new {pmc_rdpmcrng = new {adr_type, data_type, s_number, e_number, length, IODBPMC_type}},
                response = new {pmc_rdpmcrng = new {buf, IODBPMC_type = buf.GetType().Name}}
            };

            _logger.Trace($"[{_machine.Id}] Platform invocation result:\n{JObject.FromObject(nr).ToString()}");

            return nr;
        }
    }
}