using RoboDex__Capstone_.Contracts;
using RoboDex__Capstone_.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Data
{
    public class FollowersRepository : RepositoryBase<Followers>, IFollowersRepository
    {
        public FollowersRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
        }
    }
}
