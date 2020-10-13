using RoboDex__Capstone_.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Data
{

    public class TagsRepository : RepositoryBase<TagsRepository>, ITagsRepository
    {
        public TagsRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
        }
    }
}
