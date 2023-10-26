using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Nsu.Coliseum.Strategies;
using Nsu.Coliseum.StrategyInterface;

namespace OpponentWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpponentController : ControllerBase
    {
        [HttpPost(template: "SetStrategy")]
        public Results<Ok, BadRequest<string>> SetStrategy([FromQuery] string strategyName,
            [FromServices] WebStrategy webStrategy)
        {
            try
            {
                IStrategy strategy = StrategyResolverByName.ResolveStrategyByName(strategyName);
                webStrategy.Strategy ??= strategy;
                return TypedResults.Ok();
            }
            catch (ArgumentException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }

        [HttpPost(template: "UseStrategy")]
        public Results<Ok<int>, BadRequest> UseStrategy([FromBody] WebDeck webDeck,
            [FromServices] WebStrategy webStrategy)
        {
            if (null == webStrategy.Strategy) return TypedResults.BadRequest();
            return TypedResults.Ok(webStrategy.Strategy.PickCard(webDeck.Cards));
        }
    }
}