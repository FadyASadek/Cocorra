using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace Cocorra.DAL.Models
{
    [Index(nameof(SenderId), nameof(Status))]
    [Index(nameof(ReceiverId), nameof(Status))]
    public class FriendRequest : BaseEntity
    {

        public Guid SenderId { get; set; }
        public ApplicationUser? Sender { get; set; }

        public Guid ReceiverId { get; set; }
        public ApplicationUser? Receiver { get; set; }

        public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;

    }
}
