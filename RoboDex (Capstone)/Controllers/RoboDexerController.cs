using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
            NavLayout();
            var userName = _repo.RoboDexer.FindByCondition(r => r.UserName == roboDexerUser.UserName).SingleOrDefault();

            //This section finds the list to return to index view by multiple factors.
            var shoppingCart = FindItemsInShoppingCart();
            var tags = FindTagsFromItems(shoppingCart);

            var followers = CheckForPeopleDexerFollows();
            var followersItems = FindAllFollowersItems(followers);
            var tags2 = FindTagsInFollowersItems(followersItems);

            var finalListTags = MatchTagsFromCartWithTagsFromFollowersItems(tags, tags2);
            var items1 = FindFinalListOfItemsToReturnFromTags(finalListTags);//return this to master

            var newItems = FindNewItemsFromFollowers(followers);
            var finalListItems = CompareNewItemsWithItemsFromTags(newItems, items1);
            var itemsForView = CheckIfListOfItemsCountIsEnough(finalListItems);
            var listDevisibleByThree = MakeListDivisibleByThree(itemsForView);
            return View(listDevisibleByThree);
        }

        private List<Items> MakeListDivisibleByThree(List<Items> itemsForView)
        {
            var count = itemsForView.Count;
            var modulus = count % 3;
            List<Items> items = new List<Items>();
            for(int i = 0; i < count - modulus; i++)
            {
                items.Add(itemsForView[i]);
            }
              return items;
        }
       
        // GET: RoboDexerController/Details/5
        public ActionResult Details(int id)
        {
            NavLayout();
            var roboDexerUser = FindLoggedInRoboDexer();
            if (id == 0)
            {
                
                return View(roboDexerUser);
            }

            var item = _repo.Items.FindByCondition(i => i.ItemId == id).FirstOrDefault();
            var inventory = _repo.Inventory.FindByCondition(l => l.ItemId == item.ItemId).SingleOrDefault();
            var roboDexer = _repo.RoboDexer.FindByCondition(r => r.RoboDexerId == inventory.RoboDexerId).SingleOrDefault();
            
            if(roboDexer.RoboDexerId != roboDexerUser.RoboDexerId)
            {
                var dexerId = roboDexer.RoboDexerId;
                return RedirectToAction("SellerProfileInfo", new { id = dexerId });
            }
            return View(roboDexer);
        }

        public ActionResult EditProfile(int id)
        {
            var roboDexer = _repo.RoboDexer.FindByCondition(r => r.RoboDexerId == id).FirstOrDefault();

            return View(roboDexer);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(RoboDexer roboDexer)
        {
            _repo.RoboDexer.Update(roboDexer);
            _repo.Save();

            return RedirectToAction("Index");
        }

        public ActionResult SellerProfileInfo(int id)
        {
            var roboDexer = _repo.RoboDexer.FindByCondition(r => r.RoboDexerId == id).SingleOrDefault();
            return View(roboDexer);
        }

        public ActionResult ItemDetails (int id)
        {
            NavLayout();
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
            NavLayout();
            var item = _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefault();
           
            var itemToReturn = ConvertItemToItemTagsLocation(item);
            
            return View(itemToReturn);
        }      

        // GET: RoboDexerController/Create
        public ActionResult Create()
        {
            //NavLayout();
            RoboDexer roboDexer = new RoboDexer();
            return View(roboDexer);
        }

        // POST: RoboDexerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoboDexer robodexer)
        {
            //NavLayout();
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
            NavLayout();
            ItemTagsLocation itemToEnterIntoView = new ItemTagsLocation();

            var itemToEdit = _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefault();
            itemToEnterIntoView.Items = itemToEdit;

            var locationToEdit = _repo.LocationPlace.FindByCondition(l => l.LocationId == itemToEdit.LocationId).SingleOrDefault();
            itemToEnterIntoView.LocationPlace = locationToEdit;

            var itemTags = _repo.ItemTags.FindByCondition(i => i.ItemId == itemToEdit.ItemId).ToList();

            List<Tags> listOfTags = new List<Tags>();
            foreach(ItemTags itemtag in itemTags)
            {
                var tags = _repo.Tags.FindByCondition(t => t.TagId == itemtag.TagsId).SingleOrDefault();
                listOfTags.Add(tags);
            }
            itemToEnterIntoView.Tags = listOfTags;
            //itemToEnterIntoView.Tags = tagsToEdit;

            return View(itemToEnterIntoView);
        }

        // POST: RoboDexerController/Edit/5
        [HttpPost, ActionName("EditItem")]
        [ValidateAntiForgeryToken]
        public ActionResult EditTheItem(int id, ItemTagsLocation itemTagsLocation)
        {
            NavLayout();
            var itemToEdit = _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefault();
            itemToEdit = itemTagsLocation.Items;
            _repo.Items.Update(itemToEdit);

            var locationToEdit = _repo.LocationPlace.FindByCondition(l => l.LocationId == itemToEdit.LocationId).SingleOrDefault();
            locationToEdit = itemTagsLocation.LocationPlace;
            _repo.LocationPlace.Update(locationToEdit);

            ItemTags itemTags = new ItemTags();
            foreach(Tags tag in itemTagsLocation.Tags)
            {
                var items = _repo.ItemTags.FindByCondition(i => i.TagsId == tag.TagId).SingleOrDefault();
                var tagToEdit = _repo.Tags.FindByCondition(t => t.TagId == items.TagsId).SingleOrDefault();
                tagToEdit.Name = tag.Name;
                _repo.Tags.Update(tagToEdit);
            }

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
            NavLayout();
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
            NavLayout();
            var itemToDelete = await _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefaultAsync();
            _repo.Items.Delete(itemToDelete);
            _repo.Save();
         
            return RedirectToAction(nameof(Index));         
        }

        public async Task<IActionResult> MessageRead(int Id)
        {
            NavLayout();
            var message = await _repo.Inbox.FindByCondition(i => i.Id == Id).SingleOrDefaultAsync();
            message.isRead = true;
            _repo.Inbox.Update(message);
            _repo.Save();
            return RedirectToAction("UpdateInbox");
            //return View(message);
        }

        public ActionResult SubmitMessage(int cartId)
        {
            NavLayout();
            var cart = _repo.ShoppingCart.FindByCondition(s => s.Id == cartId).SingleOrDefault();
            var item = _repo.Items.FindByCondition(i => i.ItemId == cart.ItemId).SingleOrDefault();
            var inventory = _repo.Inventory.FindByCondition(i => i.ItemId == item.ItemId).SingleOrDefault();
            var itemOwner = _repo.RoboDexer.FindByCondition(r => r.RoboDexerId == inventory.RoboDexerId).SingleOrDefault();
           
            InboxCart inboxCart = new InboxCart();
           
            inboxCart.InboxId = itemOwner.InboxId;
            inboxCart.cartId = cartId;
            inboxCart.ItemId = item.ItemId;
            
            return View(inboxCart);
        }

        public async Task<IActionResult> UpdateInbox()
        {
            NavLayout();
            var loggedInRoboDexer = FindLoggedInRoboDexer();

            var inbox =  await _repo.Inbox.FindByCondition(i => i.InboxId == loggedInRoboDexer.InboxId).ToListAsync();

            List<Inbox> unreadMessages = new List<Inbox>();
            foreach (Inbox message in inbox)
            {
                if (message.isRead == false)
                {
                    unreadMessages.Add(message);
                }
                else
                {
                    continue;
                }
            }
            return View(unreadMessages);
        }
       
        public void NavLayout()
        {
            var loggedInRoboDexer = FindLoggedInRoboDexer();

            var inbox =  _repo.Inbox.FindByCondition(i => i.InboxId == loggedInRoboDexer.InboxId).ToList();

            List<Inbox> readMessages = new List<Inbox>();
            foreach (Inbox message in inbox)
            {
                if (message.isRead == false)
                {
                    readMessages.Add(message);
                }
                else
                {
                    continue;
                }
            }
            var messages = readMessages.Count;
            ViewBag.UserCount = messages.ToString();

        }

        public async Task<IActionResult> ReadInbox(int id)
        {
            NavLayout();
            var loggedInRoboDexer = FindLoggedInRoboDexer();
            
            var inbox = await _repo.Inbox.FindByCondition(i => i.InboxId == loggedInRoboDexer.InboxId).ToListAsync();

            List<Inbox> readMessages = new List<Inbox>();
            foreach(Inbox message in inbox)
            {
                if(message.isRead == true)
                {
                    readMessages.Add(message);
                }
                else
                {
                    continue;
                }
            }
            return View(readMessages);
        }

        [HttpPost]
        public IActionResult SubmitMessage(InboxCart inboxCart)
        {
            NavLayout();
            Inbox inbox = new Inbox();
            inbox = inboxCart.Inbox;
            inbox.InboxId = inboxCart.InboxId;
            inbox.ItemId = inboxCart.ItemId;
            _repo.Inbox.Create(inbox);
            _repo.Save();

            var cartId = inboxCart.cartId;
            return RedirectToAction("AddedToCart", new { cartId });
        }

        public ActionResult AddItemToCart(int id)
        {
            NavLayout();
            var loggedInRoboDexer = FindLoggedInRoboDexer();

            var itemToAdd = _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefault();
            ShoppingCart shoppingCart = new ShoppingCart();
            
            shoppingCart.ItemId = itemToAdd.ItemId;
            shoppingCart.ShoppingCartId = loggedInRoboDexer.ShoppingCartId;
            _repo.ShoppingCart.Create(shoppingCart);
            _repo.Save();

            var cartItem = _repo.ShoppingCart.FindByCondition(s => s.Id == shoppingCart.Id).SingleOrDefault();
            var cartId = cartItem.Id;

            return RedirectToAction("SubmitMessage", new { cartId = cartId });

        }

        public ActionResult AddedToCart(int cartId)
        {
            NavLayout();
            var cartItem = _repo.ShoppingCart.FindByCondition(s => s.Id == cartId).FirstOrDefault();
            var shoppingCart = _repo.Items.FindByCondition(i => i.ItemId == cartItem.ItemId).FirstOrDefault();
            return View(shoppingCart);
        }

        public ActionResult ShoppingCart()
        {
            NavLayout();
            var loggedInRoboDexer = FindLoggedInRoboDexer();

            var shoppingCart = _repo.ShoppingCart.FindByCondition(s => s.ShoppingCartId == loggedInRoboDexer.ShoppingCartId).FirstOrDefault();
            if(shoppingCart == null)
            {
                List<ShoppingCartItemsDetails> shoppingCartNull = new List<ShoppingCartItemsDetails>();
                return View(shoppingCartNull);
            }
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
            NavLayout();
            var loggedInRoboDexer = FindLoggedInRoboDexer();

            var item = _repo.Items.FindByCondition(i => i.ItemId == id).SingleOrDefault();
            var inventory = _repo.Inventory.FindByCondition(i => i.ItemId == item.ItemId).SingleOrDefault();
            var followedDexer = _repo.Inventory.FindByCondition(i => i.RoboDexerId == inventory.RoboDexerId).FirstOrDefault();

            Followers follower = new Followers();

            follower.RoboDexerId = followedDexer.RoboDexerId;
            follower.FollowerId = loggedInRoboDexer.RoboDexerId;

            _repo.Followers.Create(follower);
            _repo.Save();

            return RedirectToAction(nameof(FollowedRoboDexers));
        }

        

        public ActionResult FollowedRoboDexers()
        {
            //could probably call method that finds and returns people who they follow.
            NavLayout();
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

        public ActionResult Followers(int id)
        {
            NavLayout();
            var loggedInRoboDexer = FindLoggedInRoboDexer();
            List<RoboDexer> roboDexersThatFollow = new List<RoboDexer>();
            var dexersFollow = _repo.Followers.FindByCondition(f => f.RoboDexerId == loggedInRoboDexer.RoboDexerId).ToList();
            foreach(Followers follower in dexersFollow)
            {
                var followedRoboDexer = _repo.RoboDexer.FindByCondition(f => f.RoboDexerId == follower.FollowerId).SingleOrDefault();               
                roboDexersThatFollow.Add(followedRoboDexer);
            }
            return View(roboDexersThatFollow);
        }

        public ActionResult Inventory(int? id)
        {
            NavLayout();
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
            NavLayout();
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
            NavLayout();
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
                var items = _repo.ItemTags.FindByCondition(i => i.TagsId == tag.TagId).ToList();
                foreach(ItemTags item in items)
                {
                    var foundItem = _repo.Items.FindByCondition(i => i.ItemId == item.ItemId).SingleOrDefault();
                    itemTagsInfo.Item = foundItem;
                    listOfAllItems.Add(itemTagsInfo);
                }
                                            
            }
            return View(listOfAllItems);
        }
       
        public IActionResult AddItem()
        {
            NavLayout();
            ItemTagsLocation itemsTagsLocation = new ItemTagsLocation();
            return View(itemsTagsLocation);     
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(ItemTagsLocation itemTagsLocation)
        {
            NavLayout();
            //finds the logged in user
            var loggedInRoboDexer = FindLoggedInRoboDexer();

            //adds the item's location to the location table
            LocationPlace locationPlace = new LocationPlace();
            locationPlace.MainLocation = itemTagsLocation.LocationPlace.MainLocation;
            locationPlace.SecondaryLocation = itemTagsLocation.LocationPlace.SecondaryLocation;
            _repo.LocationPlace.Create(locationPlace);
            _repo.Save();

            //adds all info into the item object and saves
            var itemObject = itemTagsLocation.Items;
            itemObject.LocationId = locationPlace.LocationId;
            itemObject.Price = itemTagsLocation.Items.Price;
            itemObject.TimeAdded = itemTagsLocation.Items.TimeAdded;
            _repo.Items.Create(itemObject);
            _repo.Save();

            //adds the item to the user's inventory
            Inventory inventory = new Inventory();
            inventory.ItemId = itemObject.ItemId;
            inventory.RoboDexerId = loggedInRoboDexer.RoboDexerId;
            _repo.Inventory.Create(inventory);
            _repo.Save();
           

            //here the logic splits user input into spereate tags, adds tags to the table, and 
            //then populates the itemtags table with the tags/item ids
            var listOfTags = itemTagsLocation.TagsInitial.Name.Split(' ').ToList();
            foreach(string tag in listOfTags)
            {
                Tags tags = new Tags();
                tags.Name = tag;
                _repo.Tags.Create(tags);
                _repo.Save();

               
                ItemTags itemTags = new ItemTags();
                itemTags.ItemId = itemObject.ItemId;
                itemTags.TagsId = tags.TagId;
                _repo.ItemTags.Create(itemTags);
                _repo.Save();              
            }         
            return RedirectToAction("Index");
        }

        //******BELOW ARE ALL OF THE HELPER METHODS******
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

                    //var tags = _repo.Tags.FindByCondition(i => i.TagId == roboDexerItems.TagId).SingleOrDefault();

                    var location = _repo.LocationPlace.FindByCondition(l => l.LocationId == roboDexerItems.LocationId).SingleOrDefault();
                    
                    itemTagsLocation.Inventory = item;
                    itemTagsLocation.Items = roboDexerItems;
                    //itemTagsLocation.Tags = tags;
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
                    itemTagsLocation.Item = roboDexerItems;                    
                    
                    //itemTagsLocation.Item = roboDexerItems;
                    //itemTagsLocation.TagName = tags.Name;

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

            //var tag = _repo.Tags.FindByCondition(t => t.TagId == item.TagId).SingleOrDefault();
            var location = _repo.LocationPlace.FindByCondition(l => l.LocationId == item.LocationId).SingleOrDefault();

            ItemTagsLocation itemTagsLocation = new ItemTagsLocation();

            itemTagsLocation.LocationPlace = location;
            //itemTagsLocation.Tags = tag;
            itemTagsLocation.Items = item;

            return (itemTagsLocation);
        }

        private HashSet<int> GetVariousAmountsOfRandomNumbers(int itemCount)
        {
            HashSet<int> randomNumbers = new HashSet<int>();
            Random rand = new Random();

            while (randomNumbers.Count < itemCount)

            {
                randomNumbers.Add(rand.Next(0, itemCount));
            }

            return randomNumbers;
        }

        private List<Items> CheckIfListOfItemsCountIsEnough(List<Items> itemsThatMatch)
        {
            List<Items> finalList = new List<Items>();
            if (itemsThatMatch.Count >= 6)
            {
                var listOfNumbers = GetVariousAmountsOfRandomNumbers(itemsThatMatch.Count);


                foreach (int number in listOfNumbers)
                {
                    finalList.Add(itemsThatMatch[number]);
                }
                return finalList;
            }
            else
            {
                var allItems = PopulateListOfAllItems();
                foreach (Items item in allItems)
                {
                    finalList.Add(item);
                }

                var listOfExtraItems = CompareNewItemsWithItemsFromTags(allItems, itemsThatMatch);

                var listOfNumbers = GetVariousAmountsOfRandomNumbers(finalList.Count);
                List<Items> items = new List<Items>();
                foreach (int number in listOfNumbers)
                {
                    items.Add(listOfExtraItems[number]);
                }
                return items;
            }

        }

        private List<Items> PopulateListOfAllItems()
        {
            var allItems = _repo.Items.FindAll();
            List<Items> finalList = new List<Items>();
            foreach (Items item in allItems)
            {
                finalList.Add(item);
            }
            return finalList;
        }

        private List<Items> CompareNewItemsWithItemsFromTags(List<Items> newItems, List<Items> listOfItemsFromTags)
        {
            List<Items> items = new List<Items>();


            //This compares both list in the incoming method parameters.
            var dictionary = new Dictionary<Items, int>();
            foreach (Items item in newItems)
            {
                if (dictionary.ContainsKey(item))
                {
                    dictionary[item]++;
                }
                else
                {
                    dictionary.Add(item, 1);
                }
            }
            foreach (Items item in listOfItemsFromTags)
            {
                if (dictionary.ContainsKey(item))
                {
                    dictionary[item]--;
                }
                else
                {

                }
            }

            //This converts Keys from dictionary into a list
            for (int i = 0; i < dictionary.Count; i++)
            {
                items = dictionary.Keys.ToList();
            }


            return items;
        }

        private DateTime FindDateTime()
        {
            return DateTime.Now;
        }

        private List<Items> FindNewItemsFromFollowers(List<Followers> peopleWhoDexerFollows)
        {
            var dateTime = FindDateTime();
            var followersItems = FindAllFollowersItems(peopleWhoDexerFollows);

            DateTime sevenDaysAgo = dateTime.AddDays(-7);

            List<Items> newItems = new List<Items>();

            foreach (Items item in followersItems)
            {
                if (item.TimeAdded > sevenDaysAgo)
                {
                    newItems.Add(item);
                }
            }
            return newItems;
        }

        private List<Items> FindFinalListOfItemsToReturnFromTags(List<Tags> tagsThatMatch)
        {
            //#6
            List<Items> listOfItemsFromTags = new List<Items>();
            foreach (Tags tag in tagsThatMatch)
            {
                var itemTags = _repo.ItemTags.FindByCondition(i => i.TagsId == tag.TagId).SingleOrDefault();
                var itemToAdd = _repo.Items.FindByCondition(i => i.ItemId == itemTags.ItemId).SingleOrDefault();

                listOfItemsFromTags.Add(itemToAdd);
            }
            return listOfItemsFromTags;//retrun this to the master aggregator 
        }

        private List<Followers> CheckForPeopleDexerFollows()
        {
            //#4
            var roboDexerUser = FindLoggedInRoboDexer();

            var peopleWhoDexerFollows = _repo.Followers.FindByCondition(f => f.FollowerId == roboDexerUser.RoboDexerId).ToList();
            return peopleWhoDexerFollows;
        }

        private List<Items> FindAllFollowersItems(List<Followers> followers)
        {
            //#5
            List<Items> allFollowersItems = new List<Items>();
            foreach (Followers follower in followers)
            {
                //var followersItems = _repo.Items.FindAll();
                var inventory = _repo.Inventory.FindByCondition(i => i.RoboDexerId == follower.RoboDexerId).ToList();

                foreach (Inventory item in inventory)
                {
                    var theItem = _repo.Items.FindByCondition(i => i.ItemId == item.ItemId).SingleOrDefault();
                    allFollowersItems.Add(theItem);
                }
            }
            return allFollowersItems;
        }

        private List<Tags> FindTagsInFollowersItems(List<Items> allFollowersItems)
        {
            //#3
            var tagsOfFollowerItems = FindTagsFromItems(allFollowersItems);
            return tagsOfFollowerItems;
        }

        private List<Tags> MatchTagsFromCartWithTagsFromFollowersItems(List<Tags> tagsOfFollowerItems, List<Tags> tagsOfItemsInCart)
        {
            var allTags = _repo.Tags.FindAll();

            var tagsThatMatch = tagsOfFollowerItems.Concat(tagsOfItemsInCart)
                .GroupBy(x => x.TagId)
                    .Where(x => x.Count() == 1)
                    .Select(x => x.FirstOrDefault())
                    .ToList();

            return tagsThatMatch;
        }

        private List<Items> FindItemsInShoppingCart()
        {
            //#1
            var roboDexerUser = FindLoggedInRoboDexer();

            var shoppingcart = _repo.ShoppingCart.FindByCondition(s => s.ShoppingCartId == roboDexerUser.ShoppingCartId).ToList();

            List<Items> items = new List<Items>();
            foreach (ShoppingCart cart in shoppingcart)
            {
                var itemInCart = _repo.Items.FindByCondition(s => s.ItemId == cart.ItemId).SingleOrDefault();
                items.Add(itemInCart);
            }

            return items;//this goes into FindTagsFromItems parameter
        }

        private List<Tags> FindTagsFromItems(List<Items> items)
        {
            //#2
            List<Tags> tagsOfItemsInCart = new List<Tags>();
            foreach (Items item in items)
            {
                var itemTags = _repo.ItemTags.FindByCondition(i => i.ItemId == item.ItemId).ToList();
                foreach (ItemTags tags in itemTags)
                {
                    var tag = _repo.Tags.FindByCondition(t => t.TagId == tags.TagsId).SingleOrDefault();
                    tagsOfItemsInCart.Add(tag);
                }

            }
            return tagsOfItemsInCart;//this goes into MatchTagsFromCartWithTagsFromFollowersItems parameter
        }
    }

    
}
