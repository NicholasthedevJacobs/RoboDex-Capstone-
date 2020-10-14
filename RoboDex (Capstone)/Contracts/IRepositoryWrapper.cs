using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Contracts
{
    public interface IRepositoryWrapper
    {
        IFollowersRepository Followers { get; }
        IInboxRepository Inbox { get; }
        IInventoryRepository Inventory { get; }
        IItemsRepository Items { get; }
        ILocationPlaceRepository LocationPlace { get; }
        IRoboDexerRepository RoboDexer { get; }
        IShoppingCartRepository ShoppingCart { get; }
        ITagsRepository Tags { get; }

        void Save();


    }
}
