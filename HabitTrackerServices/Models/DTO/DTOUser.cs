using HabitTrackerCore.Models;

namespace HabitTrackerServices.Models.DTO
{
    public class DTOUser : IUser
    {
        public string Id { get; set; }
        public eLanguage PreferedLanguage { get; set; }
        public DTOUser(IUser user)
        {
            this.Id = user.Id;
            this.PreferedLanguage = user.PreferedLanguage;
        }

        public User ToUser()
        {
            var user = new User();
            user.Id = this.Id;
            user.PreferedLanguage = this.PreferedLanguage;
            return user;
        }
    }
}
