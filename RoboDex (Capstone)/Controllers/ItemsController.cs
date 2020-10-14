using System;
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
    public class ItemsController : Controller
    {
        private readonly IRepositoryWrapper _repo;

        public ItemsController(IRepositoryWrapper repo)
        {
            _repo = repo;
        }
        // GET: ItemsController
        public ActionResult Index()
        {
            List<Items> myItemsList = new List<Items>();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInRoboDexer = _repo.RoboDexer.FindByCondition(r => r.IdentityUserId == userId).SingleOrDefault();
            var loggedInRoboDexerId = loggedInRoboDexer.RoboDexerId;
            myItemsList = _repo.Items.FindByCondition(i => i.ItemId == loggedInRoboDexerId).ToList();

            return View(myItemsList);
        }

        // GET: ItemsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ItemsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ItemsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: ItemsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ItemsController/Edit/5
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

        // GET: ItemsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ItemsController/Delete/5
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
    }
}
