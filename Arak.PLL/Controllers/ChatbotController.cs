using Arak.BLL.DTOs;
using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Arak.PLL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatbotController : ControllerBase
    {
        private readonly IEvaluationService _evaluationService;
        private readonly IAttendanceService _attendanceService;
        private readonly ITimetableService _timetableService;

        public ChatbotController(
            IEvaluationService evaluationService,
            IAttendanceService attendanceService,
            ITimetableService timetableService)
        {
            _evaluationService = evaluationService;
            _attendanceService = attendanceService;
            _timetableService = timetableService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
                return BadRequest(new { message = "Message is required." });

            var message = request.Message.ToLower();
            var response = new ChatResponse 
            { 
                Intent = "default",
                Confidence = 1.0 
            };

            // 1. Intent: studentgrade (درجات الطالب X)
            if (message.Contains("درجة") || message.Contains("درجات") || message.Contains("grade") || message.Contains("score"))
            {
                response.Intent = "studentgrade";
                var match = Regex.Match(message, @"\d+");
                if (match.Success && int.TryParse(match.Value, out int studentId))
                {
                    var evaluations = await _evaluationService.GetAllAsync();
                    var studentEvals = evaluations.Where(e => e.StudentId == studentId).ToList();

                    if (studentEvals.Any())
                    {
                        var reply = $"درجات الطالب {studentId}:\n";
                        foreach (var eval in studentEvals)
                        {
                            reply += $"✅ {eval.AssessmentType}: {eval.Marks}/{eval.MaxMarks}\n";
                        }
                        var avg = studentEvals.Average(e => (double)e.Marks / e.MaxMarks) * 100;
                        reply += $"المعدل: {avg:F2}%";
                        response.Reply = reply;
                    }
                    else
                    {
                        response.Reply = "عذراً، لم أجد درجات لهذا الطالب.";
                    }
                }
                else
                {
                    response.Reply = "من فضلك حدد رقم الطالب (مثال: درجات الطالب 5).";
                }
            }
            // 2. Intent: attendancequery (غياب الفصل Y)
            else if (message.Contains("غياب") || message.Contains("حضور") || message.Contains("attendance"))
            {
                response.Intent = "attendancequery";
                var match = Regex.Match(message, @"\d+");
                if (match.Success && int.TryParse(match.Value, out int classId))
                {
                    var today = DateOnly.FromDateTime(DateTime.Today);
                    var attendance = await _attendanceService.GetClassSummaryAsync(classId, today);
                    
                    response.Reply = $"تقرير حضور الفصل {classId} اليوم:\n" +
                                     $"✅ حاضر: {attendance.PresentCount}\n" +
                                     $"❌ غائب: {attendance.AbsentCount}\n" +
                                     $"⚠️ متأخر: {attendance.LateCount}";
                }
                else
                {
                    response.Reply = "من فضلك حدد رقم الفصل (مثال: غياب الفصل 1).";
                }
            }
            // 3. Intent: schedulequery (جدول الفصل Z)
            else if (message.Contains("جدول") || message.Contains("schedule") || message.Contains("timetable"))
            {
                response.Intent = "schedulequery";
                var match = Regex.Match(message, @"\d+");
                if (match.Success && int.TryParse(match.Value, out int classId))
                {
                    var timetable = await _timetableService.GetTimetableByClassId(classId);
                    if (timetable.Any())
                    {
                        var reply = $"جدول الفصل {classId}:\n";
                        var sorted = timetable.OrderBy(t => t.DayOfWeek).ThenBy(t => t.StartTime);
                        foreach (var slot in sorted)
                        {
                            var subjectName = slot.Subject != null ? slot.Subject.Name : $"مادة {slot.SubjectId}";
                            reply += $"- {slot.DayOfWeek}: {subjectName} ({slot.StartTime})\n";
                        }
                        response.Reply = reply;
                    }
                    else
                    {
                        response.Reply = "لا يوجد جدول مسجل لهذا الفصل.";
                    }
                }
                else
                {
                    response.Reply = "من فضلك حدد رقم الفصل (مثال: جدول الفصل 1).";
                }
            }
            // Default
            else
            {
                response.Reply = "أنا هنا للمساعدة. يمكنك الاستفسار عن الدرجات، الحضور، أو الجداول الدراسية. كيف يمكنني مساعدتك اليوم؟";
            }

            return Ok(response);
        }
    }
}
