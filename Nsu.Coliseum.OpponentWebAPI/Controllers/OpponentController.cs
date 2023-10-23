using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.StrategyInterface;

namespace OpponentWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpponentController : ControllerBase
    {
        [HttpPost(template: "UseStrategy")]
        public Results<Ok<int>, BadRequest> UseStrategy([FromBody] Card[] cards, [FromServices] IStrategy strategy) =>
            TypedResults.Ok(strategy.PickCard(cards));

        [HttpGet(template: "Hello")]
        public string Hello() => "hello";
    }
}