using HabitTrackerCore.Models;

namespace HabitTrackerServices.Models.DTO
{
    public class DTOUser : IUser
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public eLanguage PreferedLanguage { get; set; }
        public string EndOfDayTime { get; set; }

        public DTOUser()
        {

        }

        public DTOUser(IUser user)
        {
            this.Id = user.Id;
            this.UserId = user.UserId;
            this.PreferedLanguage = user.PreferedLanguage;
            this.EndOfDayTime = user.EndOfDayTime;
        }

        public User ToUser()
        {
            var user = new User();
            user.Id = Id;
            user.UserId = this.UserId;
            user.PreferedLanguage = this.PreferedLanguage;
            user.EndOfDayTime = this.EndOfDayTime;
            return user;
        }
    }
}
