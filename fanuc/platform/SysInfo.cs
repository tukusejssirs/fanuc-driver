using System.Threading.Tasks;

namespace l99.driver.fanuc
{
    public partial class Platform
    {
        public async Task<dynamic> SysInfoAsync()
        {
            return await Task.FromResult(SysInfo());
        }
        
        public dynamic SysInfo()
        {
            Focas1.ODBSYS sysinfo = new Focas1.ODBSYS();

            NativeDispatchReturn ndr = nativeDispatch(() =>
            {
                return (Focas1.focas_ret) Focas1.cnc_sysinfo(_handle, sysinfo);
            });

            return new
            {
                method = "cnc_sysinfo",
                invocationMs = ndr.ElapsedMilliseconds,
                doc = "https://www.inventcom.net/fanuc-focas-library/misc/cnc_sysinfo",
                success = ndr.RC == Focas1.EW_OK,
                rc = ndr.RC,
                request = new {cnc_sysinfo = new { }},
                response = new {cnc_sysinfo = new {sysinfo}}
            };
        }
    }
}