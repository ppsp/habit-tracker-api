using HabitTrackerCore.Models;

namespace HabitTrackerServices.Models.DTO
{
    public class DTOUser : IUser
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public UserConfig Config { get; set; }

        public DTOUser()
        {

        }

        public DTOUser(IUser user)
        {
            this.Id = user.Id;
            this.UserId = user.UserId;
            this.Config = user.Config;
        }

        public User ToUser()
        {
            var user = new User();
            user.Id = Id;
            user.UserId = this.UserId;
            user.Config = this.Config;
            return user;
        }
    }
}
