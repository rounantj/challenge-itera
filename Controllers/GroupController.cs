using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IteraCompanyGroups.Services;
using IteraCompanyGroups.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace IteraCompanyGroups.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }


        // Endpoints específicos do desafio

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroupsByDate([FromQuery] string date)
        {
            DateTime parsedDate;
            if (!DateTime.TryParse(date, out parsedDate))
            {
                return BadRequest();
            }

            var groups = await _groupService.GetGroupsByDateAsync(parsedDate);

            if (groups == null || groups.Count() == 0)
            {
                return NotFound();
            }

            return Ok(groups);
        }


        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Group>>> GetAllGroups()
        {
            var groups = await _groupService.GetAllGroupsAsync();
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCostsByGroupIdAsync(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
            {
                return NotFound("Group not found");
            }

            try
            {
                var costs = await _groupService.GetCostsByGroupIdAsync(id);
                return Ok(costs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Group>> CreateGroup([FromBody] GroupRequest groupRequest)
        {
            // Converter GroupRequest em Group
            var group = new Group
            {
                Name = groupRequest.Name,
                Id = groupRequest.Id,
                Category = groupRequest.Category,
                DateIngestion = groupRequest.DateIngestion,
                LastUpdate = groupRequest.LastUpdate,
            };

            try
            {
                var createdGroup = await _groupService.CreateGroupAsync(group);
                return CreatedAtAction(nameof(_groupService.GetGroupByIdAsync), new { id = createdGroup.Id }, createdGroup);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] GroupRequest groupRequest)
        {
            if (id != groupRequest.Id)
            {
                return BadRequest();
            }

            var existingGroup = await _groupService.GetGroupByIdAsync(id);
            if (existingGroup == null)
            {
                return NotFound();
            }

            // Converter GroupRequest em Group
            var group = new Group
            {
                Id = groupRequest.Id,
                Name = groupRequest.Name,
                Category = groupRequest.Category,
                DateIngestion = groupRequest.DateIngestion,
                LastUpdate = groupRequest.LastUpdate,
            };

            await _groupService.UpdateGroupAsync(group);

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var existingGroup = await _groupService.GetGroupByIdAsync(id);
            if (existingGroup == null)
            {
                return NotFound();
            }

            await _groupService.DeleteGroupAsync(id);

            return NoContent();
        }
    }
}
