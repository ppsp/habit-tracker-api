﻿using HyperTaskCore.Models;
using System;

namespace HyperTaskServices.Models.DTO
{
    public class DTOUser : IUser
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public UserConfig Config { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime InsertDate { get; set; }

        public DTOUser()
        {

        }

        public DTOUser(IUser user)
        {
            this.Id = user.Id;
            this.UserId = user.UserId;
            this.LastActivityDate = user.LastActivityDate;
            this.Config = user.Config;
            this.InsertDate = user.InsertDate;
        }

        public User ToUser()
        {
            var user = new User();
            user.Id = Id;
            user.UserId = this.UserId;
            user.LastActivityDate = this.LastActivityDate;
            user.Config = this.Config;
            user.InsertDate = this.InsertDate;
            return user;
        }
    }
}
