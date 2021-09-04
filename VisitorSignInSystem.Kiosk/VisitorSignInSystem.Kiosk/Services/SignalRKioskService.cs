using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using VisitorSignInSystem.Models;

namespace VisitorSignInSystem.Services
{
    public class SignalRKioskService
    {
        private readonly HubConnection _connection;

        public event Action<Visitor> VisitorMessageReceived;

        public SignalRKioskService(HubConnection connection)
        {
            _connection = connection;
            _connection.On<Visitor>("ReceiveMessage", (visitor) => VisitorMessageReceived?.Invoke(visitor));
        }

        public async Task SendAddVisitorMessage(Visitor visitor)
        {
            // hub method, groupName, visitor data
            await _connection.SendAsync("AddVisitor", visitor.Kiosk, visitor);
        }

    }


}
