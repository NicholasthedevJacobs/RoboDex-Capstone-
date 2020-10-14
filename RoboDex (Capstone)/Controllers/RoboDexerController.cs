using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoboDex__Capstone_.Contracts;
using RoboDex__Capstone_.Models;
using RoboDex__Capstone_.Models.ViewModels;

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
                

                _repo.RoboDexer.Create(robodexer);
                _repo.Save();
            }


            robodexer.InboxId = robodexer.RoboDexerId;
            robodexer.InventoryId = robodexer.RoboDexerId;
            robodexer.ShoppingCartId = robodexer.RoboDexerId;
            _repo.Save();
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

        public IActionResult Inventory(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Items> myItemsList = new List<Items>();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInRoboDexer = _repo.RoboDexer.FindByCondition(r => r.IdentityUserId == userId).SingleOrDefault();
            var loggedInRoboDexerId = loggedInRoboDexer.RoboDexerId;
            myItemsList = _repo.Items.FindByCondition(i => i.ItemId == loggedInRoboDexerId).ToList();

            return View(myItemsList);
        }

        public IActionResult AddItem()
        {
            ItemTagsLocation itemsTagsLocation = new ItemTagsLocation();
            return View(itemsTagsLocation);     
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(ItemTagsLocation itemTagsLocation)
        {
            //finds the logged in user
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInRoboDexer = _repo.RoboDexer.FindByCondition(r => r.IdentityUserId == userId).SingleOrDefault();

            //adds the item to the items table
            var itemObject = itemTagsLocation.Items;
            _repo.Items.Create(itemObject);
            _repo.Save();

            //adds the created item to the inventory table
            Inventory inventory = new Inventory();
            inventory.ItemId = itemObject.ItemId;
            inventory.RoboDexerId = loggedInRoboDexer.RoboDexerId;
            _repo.Inventory.Create(inventory);
            _repo.Save();

            //adds the input tag into the tags table
            Tags tags = new Tags();
            tags.Name = itemTagsLocation.Tags.Name;
            _repo.Tags.Create(tags);
            _repo.Save();

            LocationPlace locationPlace = new LocationPlace();
            locationPlace.MainLocation = itemTagsLocation.LocationPlace.MainLocation;
            locationPlace.SecondaryLocation = itemTagsLocation.LocationPlace.SecondaryLocation;

            _repo.LocationPlace.Create(locationPlace);

            return RedirectToAction("Index");
         /*   return View(itemTag)*/;
        }


    }
}
