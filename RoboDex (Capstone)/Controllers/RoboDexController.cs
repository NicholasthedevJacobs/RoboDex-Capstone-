using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoboDex__Capstone_.Contracts;

namespace RoboDex__Capstone_.Controllers
{
    public class RoboDexController : Controller
    {
        private IRepositoryWrapper _repo;


        public RoboDexController(IRepositoryWrapper repo)
        {
            _repo = repo;

        }
        // GET: RoboDexController
        public ActionResult Index()
        {
            return View();
        }

        // GET: RoboDexController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RoboDexController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoboDexController/Create
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

        // GET: RoboDexController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RoboDexController/Edit/5
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

        // GET: RoboDexController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RoboDexController/Delete/5
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
