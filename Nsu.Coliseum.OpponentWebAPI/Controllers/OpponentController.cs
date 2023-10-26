using System.Net;
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
        public Results<Ok<int>, BadRequest, StatusCodeHttpResult> UseStrategy([FromBody] WebDeck webDeck,
            [FromServices] WebStrategy webStrategy)
        {
            if (null == webStrategy.Strategy) return TypedResults.StatusCode((int)HttpStatusCode.InternalServerError);
            return TypedResults.Ok(webStrategy.Strategy.PickCard(webDeck.Cards));
        }

        [HttpPost(template: "UseStrategyAsync")]
        public async Task<Results<Ok<int>, BadRequest, StatusCodeHttpResult>> UseStrategyAsync(
            [FromBody] WebDeck webDeck,
            [FromServices] WebStrategy webStrategy)
        {
            if (null == webStrategy.Strategy) return TypedResults.StatusCode((int)HttpStatusCode.InternalServerError);
            int cardNumber = await webStrategy.Strategy.PickCardAsync(webDeck.Cards);
            return TypedResults.Ok(cardNumber);
        }
    }
}