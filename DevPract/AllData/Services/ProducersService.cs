using DevPract.AllData.Base;
using DevPract.Models.Domain;
using System;

namespace DevPract.AllData.Services
{
    public class ProducersService : EntityBaseRepository<Producer>, IProducersService
    {
        public ProducersService(MainContext context) : base(context)
        {
        }
    }
}
