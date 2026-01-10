using Microsoft.AspNetCore.SignalR;

namespace SignalRSocketServer.Hubs
{
    /// <summary>
    /// SignalR Hub for managing client connections and push notifications
    /// Clients connect and register with a value (1 or 2) to determine authorization
    /// </summary>
    public class NotificationHub : Hub
    {
        private const string ALLOWED_GROUP = "allowed";

        /// <summary>
        /// Called when a client connects to the hub
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects from the hub
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Register a client with authorization value
        /// Value = 1: Client is ALLOWED to receive push messages (added to "allowed" group)
        /// Value = 2: Client is NOT allowed to receive push messages (not added to any group)
        /// </summary>
        /// <param name="value">Authorization value (1 or 2)</param>
        public async Task RegisterClient(int value)
        {
            var connectionId = Context.ConnectionId;
            
            if (value == 1)
            {
                // Add client to the "allowed" group - they will receive push messages
                await Groups.AddToGroupAsync(connectionId, ALLOWED_GROUP);
                await Clients.Caller.SendAsync("RegistrationResult", new 
                { 
                    success = true, 
                    message = "Client registered successfully - WILL receive push messages",
                    groupId = ALLOWED_GROUP,
                    value = value
                });
            }
            else if (value == 2)
            {
                // Client is not added to any group - they will NOT receive push messages
                await Clients.Caller.SendAsync("RegistrationResult", new 
                { 
                    success = true, 
                    message = "Client registered successfully - WILL NOT receive push messages",
                    groupId = "none",
                    value = value
                });
            }
            else
            {
                // Invalid value
                await Clients.Caller.SendAsync("RegistrationResult", new 
                { 
                    success = false, 
                    message = "Invalid value. Must be 1 (allowed) or 2 (not allowed)",
                    groupId = "none",
                    value = value
                });
            }
        }
    }
}
