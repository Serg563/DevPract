using DevPract.AllData.Roles;
using DevPract.AllData.Services;
using DevPract.Models.Domain;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DevPract.Controllers
{
    //[Authorize(Roles = UserRole.Admin)]
    public class ActorsController : Controller
    {
        private readonly IActorsService _service;
        private readonly MainContext _context;
        IDistributedCache cache;

        public ActorsController(IActorsService service,MainContext mainContext, IDistributedCache distributedCache)
        {
            _service = service;
            _context = mainContext;
            cache = distributedCache;

        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            //var data = await _service.GetAllAsync();
            var data =  await _context.Actors.ToListAsync();
            return View(data);
        }

        //Get: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(/*[Bind("FullName,ProfilePictureURL,Bio")] */Actor actor)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            await _service.AddAsync(actor);
            return RedirectToAction(nameof(Index));
        }

        //Get: Actors/Details/1
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
           
            var actorDetails = await _service.GetByIdAsync(id);
            //var actorDetails = await _context.Actors.Select(q => q.Id == id).ToListAsync();
            //var userString = await cache.GetStringAsync(id.ToString());
            //if (userString != null) actorDetails = JsonSerializer.Deserialize<Actor>(userString);

            if (actorDetails == null) return View("NotFound");
            return View(actorDetails);
        }
         
        //Get: Actors/Edit/1 
        public async Task<IActionResult> Edit(int id)
        {
            var actorDetails = await _service.GetByIdAsync(id);
            if (actorDetails == null) return View("NotFound");
            return View(actorDetails);
        } 

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,ProfilePictureURL,Bio")] Actor actor)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            await _service.UpdateAsync(id, actor);
            return RedirectToAction(nameof(Index));
        }

        //Get: Actors/Delete/1
        public async Task<IActionResult> Delete(int id)
        {
            var actorDetails = await _service.GetByIdAsync(id);
            if (actorDetails == null) return View("NotFound");
            return View(actorDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actorDetails = await _service.GetByIdAsync(id);
            if (actorDetails == null) return View("NotFound");

            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
