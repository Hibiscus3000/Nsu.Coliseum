using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.Strategies;
using Nsu.Coliseum.StrategyInterface;
using ReposAndResolvers;

namespace OpponentWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpponentController : ControllerBase
    {
        private readonly ILogger<OpponentController> _logger;

        public OpponentController(ILogger<OpponentController> logger) => _logger = logger;

        [HttpPost(template: "SetStrategy")]
        public Results<Ok, BadRequest<string>> SetStrategy([FromQuery] string strategyName,
            [FromServices] WebStrategy webStrategy)
        {
            try
            {
                IStrategy strategy = StrategyResolverByName.ResolveStrategyByName(strategyName);
                webStrategy.Strategy ??= strategy;
                _logger.LogDebug($"Strategy received: {strategyName}");
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
            int cardNumber = webStrategy.Strategy.PickCard(webDeck.Cards!);
            _logger.LogDebug($"Card picked: {cardNumber}");
            return TypedResults.Ok(cardNumber);
        }

        [HttpPost(template: "UseStrategyAsync")]
        public async Task<Results<Ok<int>, BadRequest, StatusCodeHttpResult>> UseStrategyAsync(
            [FromBody] WebDeck webDeck,
            [FromServices] WebStrategy webStrategy)
        {
            if (null == webStrategy.Strategy) return TypedResults.StatusCode((int)HttpStatusCode.InternalServerError);
            int cardNumber = await webStrategy.Strategy.PickCardAsync(webDeck.Cards!);
            _logger.LogDebug($"Card picked async: {cardNumber}");
            return TypedResults.Ok(cardNumber);
        }

        [HttpGet(template: "GetCardColor")]
        public async Task<Results<Ok<CardColor>, BadRequest<string>>> GetCardColor([FromQuery] long experimentNum,
            [FromServices] IRepo<CardColor> cardColorRepo)
        {
            if (!cardColorRepo.ContainsExpNum(experimentNum))
            {
                _logger.LogWarning($"{experimentNum}: unable to find card color!");
                return TypedResults.BadRequest("No such EXP NUM");
            }

            CardColor cardColor = cardColorRepo.GetT(experimentNum);
            _logger.LogDebug($"{experimentNum}: CC sent");
            return TypedResults.Ok(cardColor);
        }
    }
}