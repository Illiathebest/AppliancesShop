using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppliancesShop_Models;
using AppliancesShop_Models.Enums;
using AppliancesShop_Utility;
using System;
using System.Collections.Generic;
using AppliancesShop_DataAccess.Repository.IRepository;

namespace AppliancesShop.Controllers
{

    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeRepository _appTypeRepo;
        private readonly ILogRepository _logRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IApplicationUserRepository _userRepository;

        public ApplicationTypeController(IApplicationTypeRepository appTypeRepo, ILogRepository logRepository, UserManager<IdentityUser> userManager, IApplicationUserRepository userRepository)
        {
            _appTypeRepo = appTypeRepo;
            _logRepository = logRepository;
            _userManager = userManager;
            _userRepository = userRepository;

        }


        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _appTypeRepo.GetAll();
            return View(objList);
        }


        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }


        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _appTypeRepo.Add(obj);
                _appTypeRepo.Save();
                TempData[WC.Success] = "Action completed successfully";

                var identityUser = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                var currentUser = _userRepository.FirstOrDefault(u => u.Id == identityUser.Id);
                _logRepository.Add(new LogEntry()
                {
                    Email = currentUser.Email,
                    FullName = currentUser.FullName,
                    PhoneNumber = currentUser.PhoneNumber,
                    LogLevel = LogLevel.Information,
                    Message = $"created ApplicationType {obj.Name}",
                    TimeStamp = DateTime.Now
                });

                _logRepository.Save();

                return RedirectToAction("Index");
            }
            return View(obj);

        }


        //GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _appTypeRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _appTypeRepo.Update(obj);
                _appTypeRepo.Save();
                TempData[WC.Success] = "Action completed successfully";

                var identityUser = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                var currentUser = _userRepository.FirstOrDefault(u => u.Id == identityUser.Id);

                _logRepository.Add(new LogEntry()
                {
                    Email = currentUser.Email,
                    FullName = currentUser.FullName,
                    PhoneNumber = currentUser.PhoneNumber,
                    LogLevel = LogLevel.Information,
                    Message = $"edited ApplicationType {obj.Name}",
                    TimeStamp = DateTime.Now
                });

                _logRepository.Save();

                return RedirectToAction("Index");


            }
            return View(obj);

        }

        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _appTypeRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _appTypeRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            _appTypeRepo.Remove(obj);
            _appTypeRepo.Save();
            TempData[WC.Success] = "Action completed successfully";

            var identityUser = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            var currentUser = _userRepository.FirstOrDefault(u => u.Id == identityUser.Id);

            _logRepository.Add(new LogEntry()
            {
                Email = currentUser.Email,
                FullName = currentUser.FullName,
                PhoneNumber = currentUser.PhoneNumber,
                LogLevel = LogLevel.Information,
                Message = $"deleted ApplicationType {obj.Name}",
                TimeStamp = DateTime.Now
            });
            _logRepository.Save();

            return RedirectToAction("Index");


        }

    }
}
