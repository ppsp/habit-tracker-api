using HabitTrackerCore.Models;

namespace HabitTrackerServices.Models.DTO
{
    public class DTOUser : IUser
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public eLanguage PreferedLanguage { get; set; }
        public DTOUser(IUser user)
        {
            this.Id = user.Id;
            this.UserId = user.UserId;
            this.PreferedLanguage = user.PreferedLanguage;
        }

        public User ToUser()
        {
            var user = new User();
            user.Id = Id;
            user.UserId = this.UserId;
            user.PreferedLanguage = this.PreferedLanguage;
            return user;
        }
    }
}
