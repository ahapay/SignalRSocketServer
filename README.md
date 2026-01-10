# SignalR Socket.IO Style Server

A .NET ASP.NET Core application that provides socket.io-style server functionality using SignalR with authorization logic based on client registration values.

## Features

- **SignalR Hub**: WebSocket-based real-time communication
- **Authorization Logic**: Clients register with value 1 (allowed) or 2 (not allowed)
- **Group Management**: Only authorized clients receive push messages
- **REST API**: POST endpoint to trigger push notifications
- **Timestamp**: Server UTC timestamp included in push messages

## Architecture

### Flow Overview

1. **Client Connection**: Clients connect to SignalR hub at `/hub`
2. **Registration**: Clients call `RegisterClient(value)` where:
   - `value = 1`: Client added to "allowed" group (receives pushes)
   - `value = 2`: Client not added to any group (no pushes)
3. **Push Trigger**: REST API call to `POST /api/push` sends message to "allowed" group only
4. **Message Delivery**: Only authorized clients receive the push with message + timestamp

### Components

- **NotificationHub**: SignalR hub managing client connections and groups
- **PushController**: REST API controller for triggering push messages
- **Program.cs**: Application configuration and middleware setup

## API Endpoints

### SignalR Hub
- **URL**: `/hub`
- **Method**: `RegisterClient(int value)`
  - Registers client with authorization value
  - Returns registration result with group assignment

### REST API
- **URL**: `POST /api/push`
- **Body**: `{ "message": "string" }`
- **Response**: 
  ```json
  {
    "success": true,
    "message": "Push sent to authorized clients only",
    "sentToGroup": "allowed",
    "payload": {
      "message": "original message",
      "serverTimeUtc": "2024-01-01T12:00:00Z",
      "formattedMessage": "Message received from server at 2024-01-01 12:00:00 UTC"
    }
  }
  ```

## Running the Application

### Prerequisites
- .NET 8.0 SDK
- Web browser for testing

### Steps

1. **Navigate to project directory**:
   ```bash
   cd "c:\Amir Data\Projects\Small Utility Projects\SignalRSocketServer"
   ```

2. **Run the application**:
   ```bash
   dotnet run
   ```

3. **Access the test client**:
   Open browser to: `https://localhost:XXXX` (check console for actual port)

4. **Test the functionality**:
   - Connect multiple browser tabs
   - Register some clients with value = 1 (allowed)
   - Register some clients with value = 2 (not allowed)
   - Send push messages via REST API
   - Observe that only value = 1 clients receive pushes

## Testing Scenarios

### Scenario 1: Authorized Client
1. Connect to hub
2. Register with value = 1
3. Call REST API: `POST /api/push`
4. **Result**: Client receives push message

### Scenario 2: Unauthorized Client
1. Connect to hub
2. Register with value = 2
3. Call REST API: `POST /api/push`
4. **Result**: Client does NOT receive push message

### Scenario 3: Multiple Clients
1. Connect 3 clients
2. Register: Client 1 (value=1), Client 2 (value=1), Client 3 (value=2)
3. Call REST API: `POST /api/push`
4. **Result**: Clients 1 & 2 receive push, Client 3 does not

## Project Structure

```
SignalRSocketServer/
├── Controllers/
│   └── PushController.cs          # REST API for push messages
├── Hubs/
│   └── NotificationHub.cs        # SignalR hub with authorization logic
├── wwwroot/
│   └── index.html                 # Test client interface
├── Program.cs                     # Application configuration
├── SignalRSocketServer.csproj     # Project file
└── README.md                      # This documentation
```

## Key Implementation Details

### Group Management
- Uses SignalR's `Groups.AddToGroupAsync()` for authorization
- "allowed" group contains only clients with value = 1
- Push messages sent using `Clients.Group("allowed")`

### Push Message Format
```json
{
  "message": "original message text",
  "serverTimeUtc": "2024-01-01T12:00:00Z",
  "formattedMessage": "Message received from server at 2024-01-01 12:00:00 UTC"
}
```

### Security Considerations
- CORS enabled for testing (restrict in production)
- Input validation on REST API endpoint
- Group-based authorization prevents unauthorized message delivery

## Dependencies

- Microsoft.AspNetCore.SignalR
- Microsoft.AspNetCore.App (ASP.NET Core)
