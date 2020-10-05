using System;
using System.Collections.Generic;
using System.Text;

namespace HyperTaskCore.Models
{
    public interface IUser
    {
        string Id { get; set; }
        string UserId { get; set; }
        public UserConfig Config { get; set; }
        public DateTime? LastActivityDate { get; set; }
    }
}
