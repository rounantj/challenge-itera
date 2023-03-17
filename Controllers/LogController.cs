using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IteraEmpresaGrupos.Models;
using IteraEmpresaGrupos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IteraEmpresaGrupos.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly IteraLogService _logService;

        public LogsController(IteraLogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Log>>> Get()
        {
            try
            {
                var logs = await _logService.GetLogsAsync();
                return Ok(logs);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Log>> Get(int id)
        {
            try
            {
                var log = await _logService.GetLogByIdAsync(id);
                if (log == null)
                {
                    return NotFound();
                }
                return Ok(log);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

    }

    public interface LogService
    {
        Task<List<Log>> GetLogsAsync();
        Task<Log> GetLogByIdAsync(int id);
        Task CreateLogAsync(Log log);
        Task UpdateLogAsync(int id, Log log);
        Task DeleteLogAsync(int id);
    }
}
