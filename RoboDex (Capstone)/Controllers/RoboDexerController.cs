using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult> DeleteItem(int id)
        {
            var itemTODelete = await _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefaultAsync();
            

            if(itemTODelete == null)
            {
                return NotFound();
            }

            return View(itemTODelete);
        }

        // POST: RoboDexerController/Delete/5
        [HttpPost, ActionName("DeleteItem")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var itemToDelete = await _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefaultAsync();
            _repo.Items.Delete(itemToDelete);
            _repo.Save();

            
            return RedirectToAction(nameof(Index));
           
        }

        public async Task<IActionResult> Inventory(int? id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInRoboDexer =  await _repo.RoboDexer.FindByCondition(r => r.IdentityUserId == userId).SingleOrDefaultAsync();
            if (id == null)
            {
                return NotFound();
            }
          
            var allItems = await _repo.Inventory.FindAll().ToListAsync();
            List<ItemTagsLocation> myItemsList = new List<ItemTagsLocation>();
            foreach (Inventory item in allItems)
            {
                ItemTagsLocation itemTagsLocation = new ItemTagsLocation();
                

                if (item.RoboDexerId == loggedInRoboDexer.RoboDexerId)
                {
                    var roboDexerItems = await _repo.Items.FindByCondition(i => i.ItemId == item.ItemId).SingleOrDefaultAsync();

                    var allInventory = await _repo.Inventory.FindByCondition(i => i.ItemId == roboDexerItems.ItemId).SingleOrDefaultAsync();

                    itemTagsLocation.Inventory = item;
                    itemTagsLocation.Items = roboDexerItems;

                    myItemsList.Add(itemTagsLocation);
                }
               
            }

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
            var loggedInRoboDexer = await _repo.RoboDexer.FindByCondition(r => r.IdentityUserId == userId).SingleOrDefaultAsync();

            //adds the input tag into the tags table
            Tags tags = new Tags();
            tags.Name = itemTagsLocation.Tags.Name;
            _repo.Tags.Create(tags);
            _repo.Save();

            //adds the item's location to the location table
            LocationPlace locationPlace = new LocationPlace();
            locationPlace.MainLocation = itemTagsLocation.LocationPlace.MainLocation;
            locationPlace.SecondaryLocation = itemTagsLocation.LocationPlace.SecondaryLocation;

            _repo.LocationPlace.Create(locationPlace);
            _repo.Save();

            //adds the item to the items table
            var itemObject = itemTagsLocation.Items;
            itemObject.LocationId = locationPlace.LocationId;
            itemObject.TagId = tags.TagId;
            _repo.Items.Create(itemObject);
            _repo.Save();

            //adds the created item to the inventory table
            Inventory inventory = new Inventory();
            inventory.ItemId = itemObject.ItemId;
            inventory.RoboDexerId = loggedInRoboDexer.RoboDexerId;
            _repo.Inventory.Create(inventory);
            _repo.Save();
          
            return RedirectToAction("Index");

        }
    }
}
