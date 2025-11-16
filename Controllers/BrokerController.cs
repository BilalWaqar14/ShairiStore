using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;


[Route("api/[controller]")]
[ApiController]
public class BrokerInfoController : ControllerBase
{
    private readonly IBrokerRepository _brokerRepo;

    public BrokerInfoController(IBrokerRepository brokerRepo)
    {
        _brokerRepo = brokerRepo;
    }

    [HttpGet("getallbrokers")]
    public async Task<ActionResult<IEnumerable<BrokerInfo>>> GetAllBrokers()
    {
        var brokers = await _brokerRepo.ListAllBrokersAsync();
        return Ok(brokers);
    }

    [HttpGet("getbrokerbyid/{id:int}")]
    public async Task<ActionResult<BrokerInfo>> GetBrokerById(int id)
    {
        var broker = await _brokerRepo.GetByIdAsync(id);
        if (broker == null)
            return NotFound();

        return Ok(broker);
    }

    [HttpPost("createbroker")]
    public async Task<ActionResult<BrokerInfo>> CreateBroker(BrokerInfo broker)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _brokerRepo.AddAsync(broker);
        return CreatedAtAction(nameof(GetBrokerById), new { id = created.BrokerId }, created);
    }

    [HttpPut("updatebroker/{id:int}")]
    public async Task<ActionResult<BrokerInfo>> UpdateBroker(int id, BrokerInfo broker)
    {
        if (id != broker.BrokerId)
            return BadRequest("ID mismatch");

        var updated = await _brokerRepo.UpdateAsync(broker);
        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("deletebroker/{id:int}")]
    public async Task<IActionResult> DeleteBroker(int id)
    {
        var deleted = await _brokerRepo.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}