using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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

            var roboDexerUser = FindLoggedInRoboDexer();
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
            if(id == 0)
            {
                var roboDexerUser = FindLoggedInRoboDexer();
                return View(roboDexerUser);
            }

            var item = _repo.Items.FindByCondition(i => i.ItemId == id).FirstOrDefault();
            var inventory = _repo.Inventory.FindByCondition(l => l.ItemId == item.ItemId).SingleOrDefault();
            var roboDexer = _repo.RoboDexer.FindByCondition(r => r.RoboDexerId == inventory.RoboDexerId).SingleOrDefault();
                   
            return View(roboDexer);
        }

        public ActionResult ItemDetails (int id)
        {
            var roboDexerUser = FindLoggedInRoboDexer();
            var item = _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefault();
            var inventory = _repo.Inventory.FindByCondition(i => i.ItemId == id).SingleOrDefault();
            var itemOwner = _repo.RoboDexer.FindByCondition(r => r.RoboDexerId == inventory.RoboDexerId).FirstOrDefault();

            var itemToReturn = ConvertItemToItemTagsLocation(item);
            if (roboDexerUser.RoboDexerId == itemOwner.RoboDexerId)
            {

                return View(itemToReturn);
            }
            else
            {
                return  RedirectToAction("SellerItemDetails", new { id });
            }

        }

        public ActionResult SellerItemDetails(int id)
        {
            var item = _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefault();
            var itemToReturn = ConvertItemToItemTagsLocation(item);
            return View(itemToReturn);
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
        public ActionResult EditItem(int id)
        {
            ItemTagsLocation itemToEnterIntoView = new ItemTagsLocation();

            var itemToEdit = _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefault();
            itemToEnterIntoView.Items = itemToEdit;

            var locationToEdit = _repo.LocationPlace.FindByCondition(l => l.LocationId == itemToEdit.LocationId).SingleOrDefault();
            itemToEnterIntoView.LocationPlace = locationToEdit;

            var tagsToEdit = _repo.Tags.FindByCondition(t => t.TagId == itemToEdit.TagId).SingleOrDefault();
            itemToEnterIntoView.Tags = tagsToEdit;

            return View(itemToEnterIntoView);
        }

        // POST: RoboDexerController/Edit/5
        [HttpPost, ActionName("EditItem")]
        [ValidateAntiForgeryToken]
        public ActionResult EditTheItem(int id, ItemTagsLocation itemTagsLocation)
        {
            var itemToEdit = _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefault();
            itemToEdit = itemTagsLocation.Items;

            var locationToEdit = _repo.LocationPlace.FindByCondition(l => l.LocationId == itemToEdit.LocationId).SingleOrDefault();
            locationToEdit = itemTagsLocation.LocationPlace;

            var tagsToEdit = _repo.Tags.FindByCondition(t => t.TagId == itemToEdit.TagId).SingleOrDefault();
            tagsToEdit = itemTagsLocation.Tags;

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

        public ActionResult SubmitMessage(int id)
        {
            var cart = _repo.ShoppingCart.FindByCondition(s => s.Id == id).SingleOrDefault();
            var item = _repo.Items.FindByCondition(i => i.ItemId == cart.ItemId).SingleOrDefault();
            var inventory = _repo.Inventory.FindByCondition(i => i.ItemId == item.ItemId).SingleOrDefault();
            var itemOwner = _repo.RoboDexer.FindByCondition(r => r.RoboDexerId == inventory.RoboDexerId).SingleOrDefault();

            Inbox inbox = new Inbox();
            inbox.InboxId = itemOwner.InboxId;


            return View(inbox);
        }

        [HttpPost]
        public IActionResult SubmitMessage(Inbox messageToAdd)
        {
            _repo.Inbox.Create(messageToAdd);
            _repo.Save();

            return View();
            //return RedirectToAction("AddedToCart", new { cartId });
        }

        public ActionResult AddItemToCart(int id)
        {
            var loggedInRoboDexer = FindLoggedInRoboDexer();

            var itemToAdd = _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefault();
            ShoppingCart shoppingCart = new ShoppingCart();
            

            shoppingCart.ItemId = itemToAdd.ItemId;
            shoppingCart.ShoppingCartId = loggedInRoboDexer.ShoppingCartId;
            _repo.ShoppingCart.Create(shoppingCart);
            _repo.Save();

            //**Possibly add a success meessage after item is added**
            var cartItem = _repo.ShoppingCart.FindByCondition(s => s.Id == shoppingCart.Id).SingleOrDefault();
            var cartId = cartItem.Id;

            return RedirectToAction("SubmitMessage", new { id = cartId });
            //SubmitMessage(cartId);
            //return RedirectToAction("AddedToCart",  new { cartId} );
            //return View(shoppingCart);

        }

        public ActionResult AddedToCart(int cartId)
        {
            var cartItem = _repo.ShoppingCart.FindByCondition(s => s.Id == cartId).FirstOrDefault();
            var shoppingCart = _repo.Items.FindByCondition(i => i.ItemId == cartItem.ItemId).FirstOrDefault();
            return View(shoppingCart);
        }

        public ActionResult ShoppingCart()
        {
            var loggedInRoboDexer = FindLoggedInRoboDexer();

            var shoppingCart = _repo.ShoppingCart.FindByCondition(s => s.ShoppingCartId == loggedInRoboDexer.ShoppingCartId).FirstOrDefault();

            var items = _repo.ShoppingCart.FindByCondition(s => s.ItemId == shoppingCart.ItemId).ToList();
            var roboSeller = _repo.RoboDexer.FindByCondition(r => r.ShoppingCartId == shoppingCart.ShoppingCartId).SingleOrDefault();
            List<ShoppingCartItemsDetails> shoppingCartItemsDetails = new List<ShoppingCartItemsDetails>();
            foreach (ShoppingCart item in items)
            {
                ShoppingCartItemsDetails placeHolder = new ShoppingCartItemsDetails();
                var itemToAdd = _repo.Items.FindByCondition(i => i.ItemId == item.ItemId).SingleOrDefault();
                placeHolder.Items = itemToAdd;
                placeHolder.ShoppingCart = shoppingCart;
                placeHolder.RoboDexer = roboSeller;
                shoppingCartItemsDetails.Add(placeHolder);

            }
           
            return View(shoppingCartItemsDetails);
        }
        public ActionResult Follow(int id)
        {
            var loggedInRoboDexer = FindLoggedInRoboDexer();

            var item = _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefault();
            var inventory = _repo.Inventory.FindByCondition(i => i.ItemId == item.ItemId).SingleOrDefault();
            var followedDexer = _repo.Inventory.FindByCondition(i => i.RoboDexerId == inventory.RoboDexerId).FirstOrDefault();
            //var help = _repo.Items.FindByCondition(i => i.ItemId == id).FirstOrDefault();
            //var selectedInventory = _repo.Inventory.FindByCondition(i => i.RoboDexerId == help.ItemId).SingleOrDefault();
            //var roboDexerToFollow = _repo.RoboDexer.FindByCondition(r => r.InventoryId == selectedInventory.InventoryId).SingleOrDefault();

            Followers follower = new Followers();

            follower.RoboDexerId = followedDexer.RoboDexerId;
            follower.FollowerId = loggedInRoboDexer.RoboDexerId;

            _repo.Followers.Create(follower);
            _repo.Save();

            return RedirectToAction(nameof(FollowedRoboDexers));
        }

        public ActionResult FollowedRoboDexers()
        {
            var loggedInRoboDexer = FindLoggedInRoboDexer();

            var listOfFollowers = _repo.Followers.FindByCondition(f => f.FollowerId == loggedInRoboDexer.RoboDexerId).ToList();
            List<RoboDexer> followedRoboDexers = new List<RoboDexer>();

            foreach(Followers follower in listOfFollowers)
            {
                var followedRoboDexer = _repo.RoboDexer.FindByCondition(r => r.RoboDexerId == follower.RoboDexerId).SingleOrDefault();
                followedRoboDexers.Add(followedRoboDexer);
            }
            return View(followedRoboDexers);
        }

        public ActionResult Followers()
        {
            var loggedInRoboDexer = FindLoggedInRoboDexer();
            List<RoboDexer> roboDexersThatFollow = new List<RoboDexer>();
            var dexersFollow = _repo.Followers.FindByCondition(f => f.RoboDexerId == loggedInRoboDexer.RoboDexerId).ToList();
            foreach(Followers follower in dexersFollow)
            {
                var followedRoboDexer = _repo.RoboDexer.FindByCondition(r => r.RoboDexerId == follower.RoboDexerId).SingleOrDefault();
                roboDexersThatFollow.Add(followedRoboDexer);
            }
            return View(roboDexersThatFollow);
        }

        public ActionResult Inventory(int? id)
        {
            var loggedInRoboDexer = FindLoggedInRoboDexer();
            List<ItemTagsLocation> myItemsList = new List<ItemTagsLocation>();
            List<ItemsTagsInfo> itemsTagsInfo = new List<ItemsTagsInfo>();
            if (id == null)
            {
                return NotFound();
            }
            
            if(loggedInRoboDexer.RoboDexerId == id)
            {
                myItemsList = FindMyInventory(loggedInRoboDexer);
                return View(myItemsList);
            }
            else
            {   
                return RedirectToAction("SellerInventory", new { id });               
            }
            
        }

        public ActionResult SellerInventory(int? id)
        {          
            List<ItemsTagsInfo> myItemsList = new List<ItemsTagsInfo>();
            if (id == null)
            {
                return NotFound();
            }
         
            myItemsList = FindAnotherUserInventory(id);
            return View(myItemsList);           
        }

        public IActionResult Search(string searchTerm)
        {
            var allTags =  _repo.Tags.FindAll();
            var allItems = _repo.Items.FindAll();
                       
            if (!String.IsNullOrEmpty(searchTerm))
            {
                allItems = allItems.Where(a => a.Name.Contains(searchTerm));
            }

            if (!String.IsNullOrEmpty(searchTerm))
            {
                allTags = allTags.Where(a => a.Name.Contains(searchTerm));
            }
            List<ItemsTagsInfo> listOfAllItems = new List<ItemsTagsInfo>();

            //Here I am adding the item, and tag name and tag id into a single ItemTagsInfo object.
            foreach (Tags tag in allTags)
            {
                ItemsTagsInfo itemTagsInfo = new ItemsTagsInfo();

                itemTagsInfo.TagName = tag.Name;
                itemTagsInfo.TagId = tag.TagId;
                var doodly = allItems.Where(a => a.TagId == tag.TagId).SingleOrDefault();
                itemTagsInfo.Item = doodly;
                listOfAllItems.Add(itemTagsInfo);                              
            }
            return View(listOfAllItems);
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
            var loggedInRoboDexer = FindLoggedInRoboDexer();

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

        private List<ItemTagsLocation> FindMyInventory(RoboDexer loggedInRoboDexer)
        {
            var allItems =  _repo.Inventory.FindAll().ToList();
            List<ItemTagsLocation> myItemsList = new List<ItemTagsLocation>();
            foreach (Inventory item in allItems)
            {
                ItemTagsLocation itemTagsLocation = new ItemTagsLocation();

                var roboDexerItems = _repo.Items.FindByCondition(i => i.ItemId == item.ItemId).SingleOrDefault();

                if (roboDexerItems == null)
                {
                    continue;
                }

                if (item.RoboDexerId == loggedInRoboDexer.RoboDexerId)
                {                   
                    var allInventory =  _repo.Inventory.FindByCondition(i => i.ItemId == roboDexerItems.ItemId).SingleOrDefault();

                    var tags = _repo.Tags.FindByCondition(i => i.TagId == roboDexerItems.TagId).SingleOrDefault();

                    var location = _repo.LocationPlace.FindByCondition(l => l.LocationId == roboDexerItems.LocationId).SingleOrDefault();
                    
                    itemTagsLocation.Inventory = item;
                    itemTagsLocation.Items = roboDexerItems;
                    itemTagsLocation.Tags = tags;
                    itemTagsLocation.LocationPlace = location;
                    
                    myItemsList.Add(itemTagsLocation);
                }
            }
            return myItemsList;
        }

        private List<ItemsTagsInfo> FindAnotherUserInventory(int? id)
        {
            var inventoryToView = _repo.Inventory.FindByCondition(i => i.InventoryId == id).SingleOrDefault();

            var allItems = _repo.Inventory.FindAll().ToList();
            List<ItemsTagsInfo> myItemsList = new List<ItemsTagsInfo>();
            foreach (Inventory item in allItems)
            {
                ItemsTagsInfo itemTagsLocation = new ItemsTagsInfo();
                
                if (item.RoboDexerId == id)
                {
                    var roboDexerItems =  _repo.Items.FindByCondition(i => i.ItemId == item.ItemId).SingleOrDefault();

                    if(roboDexerItems == null)
                    {
                        continue;
                    }

                    var allInventory =  _repo.Inventory.FindByCondition(i => i.ItemId == inventoryToView.ItemId).SingleOrDefault();

                    var tags = _repo.Tags.FindByCondition(i => i.TagId == roboDexerItems.TagId).SingleOrDefault();
                    
                    itemTagsLocation.Item = roboDexerItems;
                    itemTagsLocation.TagName = tags.Name;

                    myItemsList.Add(itemTagsLocation);
                }                
            }
            return myItemsList;
        }

        private RoboDexer FindLoggedInRoboDexer()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInRoboDexer =  _repo.RoboDexer.FindByCondition(r => r.IdentityUserId == userId).SingleOrDefault();
            return loggedInRoboDexer;
        }

        private ItemTagsLocation ConvertItemToItemTagsLocation(Items item)
        {

            var tag = _repo.Tags.FindByCondition(t => t.TagId == item.TagId).SingleOrDefault();
            var location = _repo.LocationPlace.FindByCondition(l => l.LocationId == item.LocationId).SingleOrDefault();

            ItemTagsLocation itemTagsLocation = new ItemTagsLocation();

            itemTagsLocation.LocationPlace = location;
            itemTagsLocation.Tags = tag;
            itemTagsLocation.Items = item;

            return (itemTagsLocation);
        }
    }
}
