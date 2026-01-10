using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRSocketServer.Hubs;

namespace SignalRSocketServer.Controllers
{
    /// <summary>
    /// REST API Controller for triggering push notifications to SignalR clients
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PushController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private const string ALLOWED_GROUP = "allowed";

        public PushController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// POST endpoint to trigger a push message to authorized clients only
        /// Only clients in the "allowed" group (those who sent value = 1) will receive the message
        /// </summary>
        /// <param name="request">Push message request containing the message text</param>
        /// <returns>Success response with message details</returns>
        [HttpPost]
        public async Task<IActionResult> SendPush([FromBody] PushRequest request)
        {
            if (string.IsNullOrEmpty(request?.Message))
            {
                return BadRequest(new { error = "Message is required" });
            }

            // Create the push payload with message and server timestamp
            var pushPayload = new
            {
                message = request.Message,
                serverTimeUtc = DateTime.UtcNow,
                formattedMessage = $"Message received from server at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC"
            };

            // Send the message ONLY to clients in the "allowed" group
            // Clients that sent value = 2 are NOT in this group and will NOT receive the message
            await _hubContext.Clients.Group(ALLOWED_GROUP).SendAsync("ReceivePush", pushPayload);

            return Ok(new 
            { 
                success = true,
                message = "Push sent to authorized clients only",
                sentToGroup = ALLOWED_GROUP,
                payload = pushPayload
            });
        }
    }

    /// <summary>
    /// Request model for the push endpoint
    /// </summary>
    public class PushRequest
    {
        /// <summary>
        /// The message text to send to clients
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
