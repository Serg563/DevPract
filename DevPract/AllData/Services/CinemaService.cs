using DevPract.AllData.Base;
using DevPract.Models.Domain;
using System;

namespace DevPract.AllData.Services
{
    public class CinemasService : EntityBaseRepository<Cinema>, ICinemasService
    {
        public CinemasService(MainContext context) : base(context)
        {
        }
    }
}
