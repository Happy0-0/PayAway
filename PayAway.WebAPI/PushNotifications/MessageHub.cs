using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace PayAway.WebAPI.PushNotifications
{
    /// <summary>
    /// This is a simple SignalR hub implementation
    /// Implements the <see cref="Microsoft.AspNetCore.SignalR.Hub" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.SignalR.Hub" />
    public class MessageHub : Hub
    {
        // empty for now since we only need to send messages from the server to the client
        // we would add methods if clients needed to publish messages to other clients
    }
}
