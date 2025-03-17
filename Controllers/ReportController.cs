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
        private readonly string username = "ENERGYCO"; // اسم المستخدم لـ SSRS
        private readonly string password = "26988ENG"; // كلمة المرور لـ SSRS

        // إضافة معلمة `reportName` لتحديد التقرير
        [HttpGet]
        public async Task<IActionResult> GetReport(
            [FromQuery] string reportName, // اسم التقرير المطلوب
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            [FromQuery] string factoryUrl, // إضافة معلمة factoryUrl لتحديد عنوان الخادم
            [FromQuery] string filterValue = "1", // تعيين القيمة الافتراضية
            [FromQuery] string format = "PDF")
        {
            try
            {
                // التحقق من صحة التواريخ
                if (!DateTime.TryParse(startDate, out DateTime startDateTime) ||
                    !DateTime.TryParse(endDate, out DateTime endDateTime))
                {
                    return BadRequest("تنسيق التاريخ غير صحيح");
                }

                // تحديد مسار التقرير بناءً على اسم التقرير
                string reportPath = $"/{reportName}";

                // التأكد من أن factoryUrl غير فارغ
                if (string.IsNullOrEmpty(factoryUrl))
                {
                    return BadRequest("يجب تحديد عنوان الخادم (factoryUrl)"); // إذا لم يتم تمرير URL للخادم
                }

                // استخدام factoryUrl لتحديد الخادم
                string serverUrl = $"{factoryUrl}/ReportServer"; // استخدام الـ URL المرسل من العميل

                using (var client = new HttpClient(new HttpClientHandler { Credentials = new NetworkCredential(username, password) }))
                {
                    // بناء رابط التقرير
                    var reportUrl = $"{serverUrl}?{reportPath}&rs:Format={format}" +
                                    $"&StartDate={startDateTime:yyyy-MM-dd}" +
                                    $"&EndDate={endDateTime:yyyy-MM-dd}" +
                                    $"&FilterValue={filterValue}";

                    // طلب التقرير من خادم SSRS
                    var response = await client.GetAsync(reportUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsByteArrayAsync();
                        return File(content, "application/pdf", $"{reportName}_Report.pdf");
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
