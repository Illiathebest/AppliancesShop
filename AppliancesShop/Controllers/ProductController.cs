﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppliancesShop_DataAccess;
using AppliancesShop_DataAccess.Repository.IRepository;
using AppliancesShop_Models;
using AppliancesShop_Models.Enums;
using AppliancesShop_Models.ViewModels;
using AppliancesShop_Utility;

namespace AppliancesShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _prodRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogRepository _logRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IApplicationUserRepository _userRepository;
        public ProductController(IProductRepository prodRepo, IWebHostEnvironment webHostEnvironment,
            ILogRepository logRepository, UserManager<IdentityUser> userManager, IApplicationUserRepository userRepository)
        {
            _prodRepo = prodRepo;
            _webHostEnvironment = webHostEnvironment;
            _logRepository = logRepository;
            _userManager = userManager;
            _userRepository = userRepository;
        }


        public IActionResult Index()
        {
            IEnumerable<Product> objList = _prodRepo.GetAll(includeProperties:"Category,ApplicationType");

            //foreach(var obj in objList)
            //{
            //    obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
            //    obj.ApplicationType = _db.ApplicationType.FirstOrDefault(u => u.Id == obj.ApplicationTypeId);
            //};

            return View(objList);
        }


        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {

            //IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.Id.ToString()
            //});

            ////ViewBag.CategoryDropDown = CategoryDropDown;
            //ViewData["CategoryDropDown"] = CategoryDropDown;

            //Product product = new Product();

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _prodRepo.GetAllDropdownList(WC.CategoryName),
                ApplicationTypeSelectList = _prodRepo.GetAllDropdownList(WC.ApplicationTypeName),
            };

            if (id == null)
            {
                //this is for create
                return View(productVM);
            }
            else
            {
                productVM.Product = _prodRepo.Find(id.GetValueOrDefault());
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
        }


        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    //Creating
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;

                    _prodRepo.Add(productVM.Product);

                    var identityUser = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    var currentUser = _userRepository.FirstOrDefault(u => u.Id == identityUser.Id);

                    _logRepository.Add(new LogEntry()
                    {
                        Email = currentUser.Email,
                        FullName = currentUser.FullName,
                        PhoneNumber = currentUser.PhoneNumber,
                        LogLevel = LogLevel.Information,
                        Message = $"created product {productVM.Product.Name}",
                        TimeStamp = DateTime.Now
                    });
                    _logRepository.Save();
                }
                else
                {
                    //updating
                    var objFromDb = _prodRepo.FirstOrDefault(u => u.Id == productVM.Product.Id,isTracking:false);

                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }

                    var identityUser = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                    var currentUser = _userRepository.FirstOrDefault(u => u.Id == identityUser.Id);

                    _logRepository.Add(new LogEntry()
                    {
                        Email = currentUser.Email,
                        FullName = currentUser.FullName,
                        PhoneNumber = currentUser.PhoneNumber,
                        LogLevel = LogLevel.Information,
                        Message = $"updated product {productVM.Product.Name}",
                        TimeStamp = DateTime.Now
                    });
                    _logRepository.Save();

                    _prodRepo.Update(productVM.Product);
                }
                TempData[WC.Success] = "Action completed successfully";

                _prodRepo.Save();
                return RedirectToAction("Index");
            }
            productVM.CategorySelectList = _prodRepo.GetAllDropdownList(WC.CategoryName);
            productVM.ApplicationTypeSelectList = _prodRepo.GetAllDropdownList(WC.ApplicationTypeName);
         
            return View(productVM);

        }


   
        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product product= _prodRepo.FirstOrDefault(u=>u.Id==id,includeProperties:"Category,ApplicationType");
            //product.Category = _db.Category.Find(product.CategoryId);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //POST - DELETE
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _prodRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }


            _prodRepo.Remove(obj);
                _prodRepo.Save();
            TempData[WC.Success] = "Action completed successfully";

            var identityUser = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            var currentUser = _userRepository.FirstOrDefault(u => u.Id == identityUser.Id);

            _logRepository.Add(new LogEntry()
            {
                Email = currentUser.Email,
                FullName = currentUser.FullName,
                PhoneNumber = currentUser.PhoneNumber,
                LogLevel = LogLevel.Information,
                Message = $"deleted product {obj.Name}",
                TimeStamp = DateTime.Now
            });
            _logRepository.Save();

            return RedirectToAction("Index");
            

        }

    }
}
