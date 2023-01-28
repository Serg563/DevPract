using DevPract.AllData.Base;
using DevPract.Models.Domain;

namespace DevPract.AllData.Services
{
    public class ActorService : EntityBaseRepository<Actor>, IActorsService
    {
        public ActorService(MainContext context) : base(context) { }
    }
}
