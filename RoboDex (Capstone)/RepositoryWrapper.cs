using RoboDex__Capstone_.Contracts;
using RoboDex__Capstone_.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private ApplicationDbContext _context;
        private IFollowersRepository _followers;
        private IInboxRepository _inbox;
        private IInventoryRepository _inventory;
        private IItemsRepository _items;
        private ILocationPlaceRepository _locationPlace;
        private IRoboDexerRepository _roboDexer;
        private IShoppingCartRepository _shoppingCart;
        private ITagsRepository _tags;

        public IFollowersRepository Followers
        {
            get
            {
                if (_followers == null)
                {
                    _followers = new FollowersRepository(_context);
                }
                return _followers;
            }
        }
        public IInboxRepository Inbox
        {
            get
            {
                if (_inbox == null)
                {
                    _inbox = new InboxRepository(_context);
                }
                return _inbox;
            }
        }
        public IInventoryRepository Inventory
        {
            get
            {
                if (_inventory == null)
                {
                    _inventory = new InventoryRepository(_context);
                }
                return _inventory;
            }
        }
        public IItemsRepository Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ItemsRepository(_context);
                }
                return _items;
            }
        }
        public IRoboDexerRepository RoboDexer
        {
            get
            {
                if (_roboDexer == null)
                {
                    _roboDexer = new RoboDexerRepository(_context);
                }
                return _roboDexer;
            }
        }
        public ILocationPlaceRepository LocationPlace
        {
            get
            {
                if (_locationPlace == null)
                {
                    _locationPlace = new LocationPlaceRepository(_context);
                }
                return _locationPlace;
            }
        }
        
        public IShoppingCartRepository ShoppingCart
        {
            get
            {
                if (_shoppingCart == null)
                {
                    _shoppingCart = new ShoppingCartRepository(_context);
                }
                return _shoppingCart;
            }
        }
        public ITagsRepository Tags
        {
            get
            {
                if (_tags == null)
                {
                    _tags = new TagsRepository(_context);
                }
                return _tags;
            }
        }

        public RepositoryWrapper(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }

}
