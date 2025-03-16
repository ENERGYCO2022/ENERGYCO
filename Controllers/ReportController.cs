using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;

namespace YourNamespace.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly string reportServerUrl = "http://energyco/ReportServer"; // استخدم الـ Report Server URL الصحيح
        private readonly string reportPath = "/CUSTOMERREP"; // مسار التقرير داخل SSRS
        private readonly string username = "ENERGYCO"; // اسم المستخدم لـ SSRS
        private readonly string password = "26988ENG"; // كلمة المرور لـ SSRS

        [HttpGet("plc1")]
        public async Task<IActionResult> GetReport(
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            [FromQuery] string filterValue = "1", // 🔹 تعيين القيمة الافتراضية إلى 1
            [FromQuery] string format = "PDF")
        {
            try
            {
                if (!DateTime.TryParse(startDate, out DateTime startDateTime) ||
                    !DateTime.TryParse(endDate, out DateTime endDateTime))
                {
                    return BadRequest("تنسيق التاريخ غير صحيح");
                }

                using (var client = new HttpClient(new HttpClientHandler { Credentials = new NetworkCredential(username, password) }))
                {
                    // ✅ تعديل الرابط ليشمل `FilterValue`
                    var reportUrl = $"{reportServerUrl}?{reportPath}&rs:Format={format}" +
                                    $"&StartDate={startDateTime:yyyy-MM-dd}" +
                                    $"&EndDate={endDateTime:yyyy-MM-dd}" +
                                    $"&FilterValue={filterValue}"; // 🔹 إضافة `FilterValue`

                    var response = await client.GetAsync(reportUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsByteArrayAsync();
                        return File(content, "application/pdf", "CUSTOMER_REP.pdf");
                    }
                    else
                    {
                        return BadRequest($"خطأ في استرجاع التقرير من SSRS: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");
            }
        }
    }
}
