using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IteraCompanyGroups.Models;
using Microsoft.AspNetCore.Mvc;

namespace IteraCompanyGroups.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Log>>> Get()
        {
            return await _logService.GetLogsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Log>> Get(int id)
        {
            var log = await _logService.GetLogByIdAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            return log;
        }

        [HttpPost]
        public async Task<ActionResult<Log>> Post(Log log)
        {
            await _logService.CreateLogAsync(log);
            return CreatedAtAction(nameof(Get), new { id = log.Id }, log);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Log log)
        {
            try
            {
                await _logService.UpdateLogAsync(id, log);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _logService.DeleteLogAsync(id);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return NoContent();
        }
    }

    public interface ILogService
    {
        Task<List<Log>> GetLogsAsync();
        Task<Log> GetLogByIdAsync(int id);
        Task CreateLogAsync(Log log);
        Task UpdateLogAsync(int id, Log log);
        Task DeleteLogAsync(int id);
    }
}