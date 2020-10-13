using Microsoft.CodeAnalysis;
using RoboDex__Capstone_.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Data
{
    public class LocationRepository : RepositoryBase<Location>, ILocationRepository
    {
        public LocationRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
        {
        }
    }
}
