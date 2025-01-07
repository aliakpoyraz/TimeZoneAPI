using Microsoft.AspNetCore.Mvc;
using NodaTime;
using NodaTime.Text;
using System;

namespace TimeZone.Controllers
{
    [Route("api/time")]
    [ApiController]
    public class TimeController : ControllerBase
    {
        private readonly IDateTimeZoneProvider _timeZoneProvider;

        public TimeController()
        {
            _timeZoneProvider = DateTimeZoneProviders.Tzdb;
        }

        [HttpGet("{timezone}")]
        public IActionResult GetTime(string timezone)
        {
            var dateTimeZone = _timeZoneProvider.GetZoneOrNull(timezone);
            if (dateTimeZone == null)
            {
                return NotFound(new { Message = $"Geçersiz zaman dilimi: {timezone}" });
            }

            var now = SystemClock.Instance.GetCurrentInstant();
            var zonedDateTime = now.InZone(dateTimeZone);

            var pattern = LocalDateTimePattern.CreateWithInvariantCulture("yyyy-MM-dd HH:mm:ss");
            var formattedLocalTime = pattern.Format(zonedDateTime.LocalDateTime);

            return Ok(new
            {
                Timezone = timezone,
                LocalTime = formattedLocalTime,
                UtcOffset = zonedDateTime.Offset.ToString()
            });
        }

        [HttpGet("timezones")]
        public IActionResult GetTimezones()
        {
            var timeZones = _timeZoneProvider.Ids;
            return Ok(timeZones);
        }   
    }
}
