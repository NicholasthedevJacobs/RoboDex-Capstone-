using RoboDex__Capstone_.Contracts;
using RoboDex__Capstone_.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Data
{
    public class ItemTagsRepository : RepositoryBase<ItemTags>, IItemTagsRepository
    {
        public ItemTagsRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
        }
    }
    
}
