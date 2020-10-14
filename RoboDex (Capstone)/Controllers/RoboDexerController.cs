﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoboDex__Capstone_.Contracts;
using RoboDex__Capstone_.Models;

namespace RoboDex__Capstone_.Controllers
{
    public class RoboDexerController : Controller
    {
        private IRepositoryWrapper _repo;


        public RoboDexerController(IRepositoryWrapper repo)
        {
            _repo = repo;

        }
        // GET: RoboDexerController
        public ActionResult Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roboDexerUser = _repo.RoboDexer.FindByCondition(r => r.IdentityUserId == userId).SingleOrDefault();
            if (roboDexerUser == null)
            {
                return RedirectToAction("Create");
            }
            var userName = _repo.RoboDexer.FindByCondition(r => r.UserName == roboDexerUser.UserName).SingleOrDefault();

            return View(roboDexerUser);
        }

        // GET: RoboDexerController/Details/5
        public ActionResult Details(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roboDexerUser = _repo.RoboDexer.FindByCondition(r => r.IdentityUserId == userId).SingleOrDefault();
            return View(roboDexerUser);
        }

        // GET: RoboDexerController/Create
        public ActionResult Create()
        {
            RoboDexer roboDexer = new RoboDexer();
            return View(roboDexer);
        }

        // POST: RoboDexerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoboDexer robodexer)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                robodexer.IdentityUserId = userId;
                robodexer.InboxId = robodexer.RoboDexerId;
                robodexer.InventoryId = robodexer.RoboDexerId;
                robodexer.ShoppingCartId = robodexer.RoboDexerId;

                _repo.RoboDexer.Create(robodexer);
                _repo.Save();
            }
            


            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RoboDexerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RoboDexerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RoboDexerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RoboDexerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public IActionResult GetInventory(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInRoboDexer = _repo.RoboDexer.FindByCondition(r => r.IdentityUserId == userId).SingleOrDefault();
            var inventory = _repo.RoboDexer.FindByCondition(r => r.InventoryId == loggedInRoboDexer.InventoryId).SingleOrDefault();
            
            return View(inventory);
        }
    }
}
