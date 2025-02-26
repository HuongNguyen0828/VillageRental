using VillageRental.Services;
using VillageRental.Data;


namespace VillageRental.Services
{
    public class AppService
    {
        private LoginModel _adminModel = new LoginModel()
        {
            UserName = "admin@gmail.com",
            Password = "password"

        };

        public AppService() { }
        public async Task<string> AuthenticateUser(LoginModel loginModel)
        {

            // If admin
            if (loginModel.UserName == _adminModel.UserName && loginModel.Password == _adminModel.Password)
            {
                return "Admin";
            }

            // If normal user
            if (await GetValidUser(loginModel) != null)
            {
                return "User";
            }
            else{
                // Invalid user
                return "Invalid";
            }
           
        }


        // Is Valid User
        public static async Task<User> GetValidUser(LoginModel loginModel)
        {
            User loginUser = new();
            try
            {
                // Get all users from the database
                DatabaseService databaseService = new DatabaseService();
                List<User> users = await databaseService.GetAllUsers();

                // Validate user credentials against the list of users
  
                 loginUser =  users.FirstOrDefault(user => user.UserName.Equals(loginModel.UserName, StringComparison.OrdinalIgnoreCase) && user.Password == loginModel.Password);

                // Return true if the user is found, otherwise fals
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to validate user: {ex.Message}", "Ok");
            }
            return loginUser;
        }


    }
}
