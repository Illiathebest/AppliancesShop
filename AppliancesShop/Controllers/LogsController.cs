using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppliancesShop_DataAccess.Repository.IRepository;
using AppliancesShop_Models.Enums;
using AppliancesShop_Utility;
using System.Linq;

namespace AppliancesShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class LogsController : Controller
    {
        private readonly ILogRepository _logRepository;

        public LogsController(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public IActionResult Index(string searchName = null, string searchEmail = null, string searchPhone = null, string searchLevel = null)
        {
            var logs = _logRepository.GetAll();

            if (!string.IsNullOrEmpty(searchName))
            {
                logs = logs.Where(u => u.FullName.ToLower().Contains(searchName.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                logs = logs.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                logs = logs.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchLevel) && searchLevel != "--Log Level--")
            {
                logs = logs.Where(u => u.LogLevel == (LogLevel)int.Parse(searchLevel));
            }

            return View(logs);
        }
    }
}
