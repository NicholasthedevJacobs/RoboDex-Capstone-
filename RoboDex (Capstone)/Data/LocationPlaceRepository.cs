using Microsoft.CodeAnalysis;
using RoboDex__Capstone_.Contracts;
using RoboDex__Capstone_.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Data
{
    public class LocationPlaceRepository : RepositoryBase<LocationPlace>, ILocationPlaceRepository
    {
        public LocationPlaceRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
        {
        }
    }
}
