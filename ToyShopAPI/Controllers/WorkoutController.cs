#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutAPI.Data;
using WorkoutAPI.Models;

namespace WorkoutAPI.Controllers
{
    [Route("api/workouts")]
    [ApiController]
    public class WorkoutsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkoutsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Produc
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkoutModel>>> GetActivities()
        {
            return await _context.Workouts.Include("Activity").ToListAsync();
        }

        // GET: api/Produc/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutModel>> GetActivityModel(int id)
        {
            var WorkoutModel = await _context.Workouts.Include("Activity").Where(x => x.ID == id).ToListAsync();

            if (WorkoutModel == null)
            {
                return NotFound();
            }

            return WorkoutModel.FirstOrDefault();
        }

        

        private bool ActivityModelExists(int id)
        {
            return _context.Workouts.Any(e => e.ID == id);
        }
    }
}
