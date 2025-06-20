
using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using VillageRental.Components.Pages;
using VillageRental.Data;


namespace VillageRental.Services
{
    public class DatabaseService
    {
        string connectionVillageRental = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=VillageRental;Integrated Security=True";
        SqlConnection connection;

        //Constructor, to make sure connection is connected
        public DatabaseService()
        {
            try
            {
                // Creat connection
                connection = new SqlConnection(connectionVillageRental);
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", $"Failed to connect to db: {ex.Message}", "Ok");

            }

        }

        //Retrieve next available rentalId for inserting a new record
        public async Task<int> GetNextId()
        {
            connection.Open();
            string selectQuery = "Select MAX(id) from dbo.[user]";
            using (SqlCommand selectCmd = new SqlCommand(selectQuery, connection))
            {
                var result = await selectCmd.ExecuteScalarAsync();
                connection.Close();
                return result == DBNull.Value ? 1 : Convert.ToInt32(result) + 1;
            }
        }

        public async Task<bool> CheckValidUserName(string userName)
        {
            try
            {
                connection.Open();
                string selectQuery = "SELECT id FROM dbo.[user] WHERE userName = @userName";

                using (SqlCommand selectCmd = new SqlCommand(selectQuery, connection))
                {
                    selectCmd.Parameters.AddWithValue("@userName", userName);

                    var result = await selectCmd.ExecuteScalarAsync();
                    // If result is null, it means no match was found, so the username is valid
                    return result == null;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception if needed
                throw new Exception($"An error occurred while checking the username: {ex.Message}");
            }
            finally
            {
                connection.Close(); // Ensure the connection is always closed
            }
        }


        public async Task<bool> AddNewUser(User user)
        {
            connection.Open();
            // Do query
            SqlCommand cmd = new SqlCommand("INSERT into dbo.[user] VALUES(@id, @password, @userName, @firstName, @lastname, @email, @phoneNumber, @address, @status)", connection);

            // Adding when using parameters: PAIR of Name : Value
            cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.Parameters.AddWithValue("@password", user.Password);
            cmd.Parameters.AddWithValue("@userName", user.UserName);
            cmd.Parameters.AddWithValue("@firstName", user.FirstName);
            cmd.Parameters.AddWithValue("@lastName", user.LastName);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@address", user.Address);
            cmd.Parameters.AddWithValue("@phoneNumber", user.PhoneNumber);
            cmd.Parameters.AddWithValue("@status", user.Status);

            // Execute Non Querry
            await cmd.ExecuteNonQueryAsync();
            connection.Close();
            return true;
        }

        //Show all user info
        public async Task<List<User>> GetAllUsers()
        {
            connection.Open();
            var users = new List<User>();

            try
            {
                // Execute query
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.[user]", connection))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserName = reader.GetString(reader.GetOrdinal("userName")),
                                Password = reader.GetString(reader.GetOrdinal("password")),
                                FirstName = reader.GetString(reader.GetOrdinal("firstName")),
                                LastName = reader.GetString(reader.GetOrdinal("lastName")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                Address = reader.GetString(reader.GetOrdinal("address")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("phoneNumber")),
                                Status = reader.GetString(reader.GetOrdinal("status"))
                            };

                            users.Add(user);
                        }
                    }
                    // Alert for user
                    //await Application.Current.MainPage.DisplayAlert("Success", "Users Successfully Fetched", "Ok");
                    return users;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them, display an error message)
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch users: {ex.Message}", "Ok");
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<User> GetUser(int id)
        {
            connection.Open();
            User user = new();

            try
            {
                // Execute query
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.[user] WHERE id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            user = new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserName = reader.GetString(reader.GetOrdinal("userName")),
                                Password = reader.GetString(reader.GetOrdinal("password")),
                                FirstName = reader.GetString(reader.GetOrdinal("firstName")),
                                LastName = reader.GetString(reader.GetOrdinal("lastName")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                Address = reader.GetString(reader.GetOrdinal("address")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("phoneNumber")),
                                Status = reader.GetString(reader.GetOrdinal("status"))
                            };
                        }
                    }
                    // Alert for user
                    //await Application.Current.MainPage.DisplayAlert("Success", "Users Successfully Fetched", "Ok");
                    return user;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them, display an error message)
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch user Id {id}: {ex.Message}", "Ok");
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<bool> UpdateUser(int id, User user)
        {
            connection.Open();
            try
            {
                // Do query
                string sql = "Update dbo.[user] set status=@status WHERE id=@id ";
                SqlCommand cmd = new SqlCommand(sql, connection);
                using (cmd)
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@status", user.Status);
                    await cmd.ExecuteNonQueryAsync();

                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch categories: {e.Message}", "Ok");
            }
            finally
            {
                connection.Close();
            }
            return true;
        }


        public async Task<bool> DeleteUser(int id)
        {
            connection.Open();
            try
            {
                // Do query
                string sql = "Delete from dbo.[user] WHERE id=@id ";
                SqlCommand cmd = new SqlCommand(sql, connection);
                using (cmd)
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                    connection.Close();
                }
            }


            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch user: {e.Message}", "Ok");
            }
            return true;
        }

        //Retrieve next available rentalId for inserting a new record: 
        // NextID = 10 Or max(id) + 10;
        public async Task<int> GetNextCategoryId()
        {
            connection.Open();
            string selectQuery = "Select MAX(id) from dbo.[category]";
            using (SqlCommand selectCmd = new SqlCommand(selectQuery, connection))
            {
                var result = await selectCmd.ExecuteScalarAsync();
                connection.Close();
                return result == DBNull.Value ? 10 : Convert.ToInt32(result) + 10;
            }
        }

        // Add new category
        public async Task<bool> AddNewCategory(Category category)
        {
            connection.Open();
            // Do query
            SqlCommand cmd = new SqlCommand("INSERT into dbo.category VALUES(@id, @name)", connection);

            // Adding when using parameters: PAIR of Name : Value
            cmd.Parameters.AddWithValue("@id", category.Id);
            cmd.Parameters.AddWithValue("@name", category.Name);

            // Execute Non Querry
            await cmd.ExecuteNonQueryAsync();
            connection.Close();
            return true;
        }

        // // Get All Category
        public async Task<List<Category>> GetAllCategories()
        {
            connection.Open();
            var categories = new List<Category>();

            try
            {
                // Execute query
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.[category]", connection))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var category = new Category
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name"))
                            };

                            categories.Add(category);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them, display an error message)
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch categories: {ex.Message}", "Ok");
            }

            connection.Close();
            // Alert for user
            //await Application.Current.MainPage.DisplayAlert("Success", "Users Successfully Fetched", "Ok");
            return categories;
        }

        // Delete Category
        public async Task<bool> DeleteCategory(int id)
        {
            connection.Open();
            try
            {
                // Do query
                string sql = "Delete from dbo.[category] WHERE id=@id ";
                SqlCommand cmd = new SqlCommand(sql, connection);
                using (cmd)
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                    connection.Close();
                }
            }


            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch categories: {e.Message}", "Ok");
            }
            return true;
        }

        public async Task<bool> UpdateCategory(int id, Category category)
        {
            connection.Open();
            try
            {
                // Do query
                string sql = "Update dbo.[category] set name=@name WHERE id=@id ";
                SqlCommand cmd = new SqlCommand(sql, connection);
                using (cmd)
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", category.Name);
                    await cmd.ExecuteNonQueryAsync();
                    connection.Close();
                }
            }


            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch categories: {e.Message}", "Ok");
                return false;
            }
            return true;
        }

        // Get a Category
        public async Task<Category> GetCategory(int id)
        {
            connection.Open();
            Category category = new();

            try
            {
                // Execute query
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.[category] WHERE id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {

                            category.Id = reader.GetInt32(reader.GetOrdinal("id"));
                            category.Name = reader.GetString(reader.GetOrdinal("name"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them, display an error message)
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch category: {ex.Message}", "Ok");
            }
            connection.Close();
            return category;

        }

        //Checking the id Already exist
        public async Task<List<int>> GetIdList()
        {
            connection.Open();
            List<int> idList = new();

            using (SqlCommand cmd = new SqlCommand("Select id from dbo.[category]", connection))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("id"));
                        idList.Add(id);
                    }

                }
            }
            connection.Close();
            return idList;
        }


        //Show all Equipment info
        public async Task<List<EquipmentwithCategoryStr>> GetAllEquipment()
        {
            connection.Open();
            var equipmentList = new List<EquipmentwithCategoryStr>();

            try
            {
                // Execute query
                using (SqlCommand cmd = new SqlCommand("SELECT e.id, e.name, e.categoryId, e.quantity, e.DailyCost, e.description, c.name AS categoryStr FROM dbo.[equipment] e JOIN dbo.[category] c ON e.categoryId=c.id", connection))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var equipmentwithCategoryStr = new EquipmentwithCategoryStr
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                CategoryId = reader.GetInt32(reader.GetOrdinal("categoryId")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                                Description = reader.GetString(reader.GetOrdinal("description")),
                                DailyPrice = reader.GetDecimal(reader.GetOrdinal("dailyCost")),
                                CategoryStr = reader.GetString(reader.GetOrdinal("categoryStr"))

                            };

                            equipmentList.Add(equipmentwithCategoryStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them, display an error message)
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch Equipment List: {ex.Message}", "Ok");
            }

            connection.Close();
            // Alert for user
            //await Application.Current.MainPage.DisplayAlert("Success", "Users Successfully Fetched", "Ok");
            return equipmentList;
        }

        // Delete Equipment
        public async Task<bool> DeleteEquipment(int id)
        {
            connection.Open();
            try
            {
                // Do query
                string sql = "Delete from dbo.[equipment] WHERE id=@id ";
                SqlCommand cmd = new SqlCommand(sql, connection);
                using (cmd)
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                    connection.Close();
                }
            }


            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch equipment: {e.Message}", "Ok");
                return false;
            }
            return true;
        }


        //Retrieve next available rentalId for inserting a new record based on CategoryID
        // NextId = "categoryIdPart" + 1 Or "categoryIdPart" + "max(idPart)"
        public async Task<int> GetNextEquipmentId(int categoryId)
        {
            string nextIdStr;
            string categoryPart = categoryId.ToString();
            int rightPart;

            string selectQuery = "SELECT MAX(id) AS MaxId FROM dbo.[equipment] WHERE categoryId = @id";

            connection.Open();

            using (SqlCommand selectCmd = new SqlCommand(selectQuery, connection))
            {
                selectCmd.Parameters.AddWithValue("@id", categoryId);

                var result = await selectCmd.ExecuteScalarAsync();
                if (result == null || result == DBNull.Value)
                {
                    // No records found for this categoryId
                    rightPart = 1;
                }
                else
                {
                    // Parse the result to get the rightPart
                    string resultStr = result.ToString();
                    string rightPartStr = resultStr.Substring(categoryPart.Length); // Extract the right part
                    rightPart = int.TryParse(rightPartStr, out int parsedValue) ? parsedValue + 1 : 1;
                }

                nextIdStr = $"{categoryPart}{rightPart}"; // Format to ensure two digits for rightPart
            }

            connection.Close();
            return Convert.ToInt32(nextIdStr);
        }

        // Add new equipment
        public async Task<bool> AddNewEquipment(Equipment equipment)
        {
            connection.Open();
            // Do query
            SqlCommand cmd = new SqlCommand("INSERT into dbo.equipment VALUES(@id, @name, @categoryId, @quantity, @description, @dailyPrice)", connection);

            // Adding when using parameters: PAIR of Name : Value
            cmd.Parameters.AddWithValue("@id", equipment.Id);
            cmd.Parameters.AddWithValue("@name", equipment.Name);
            cmd.Parameters.AddWithValue("@categoryId", equipment.CategoryId);
            cmd.Parameters.AddWithValue("@quantity", equipment.Quantity);
            cmd.Parameters.AddWithValue("@description", equipment.Description);
            cmd.Parameters.AddWithValue("@dailyPrice", equipment.DailyPrice);

            // Execute Non Querry
            await cmd.ExecuteNonQueryAsync();
            connection.Close();
            return true;
        }

        public async Task<bool> UpdateEquipment(int id, Equipment equipment)
        {
            connection.Open();
            try
            {
                // Do query
                string sql = "Update dbo.[equipment] set name=@name, categoryId=@categoryId, quantity=@quantity, dailyCost=@dailyPrice, description=@description WHERE id=@id ";
                SqlCommand cmd = new SqlCommand(sql, connection);
                using (cmd)
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", equipment.Name);
                    cmd.Parameters.AddWithValue("@categoryId", equipment.CategoryId);
                    cmd.Parameters.AddWithValue("@quantity", equipment.Quantity);
                    cmd.Parameters.AddWithValue("@description", equipment.Description);
                    cmd.Parameters.AddWithValue("@dailyPrice", equipment.DailyPrice);

                    await cmd.ExecuteNonQueryAsync();
                }
                return true;
            }

            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch equipment: {e.Message}", "Ok");
                return false;
            }
            finally
            {
                connection.Close(); // Ensure that connection is closed regardless of success or failure
            }
        }

        // Get a Equipment
        public async Task<Equipment> GetEquipment(int id)
        {
            connection.Open();
            Equipment equipment = new();

            try
            {
                // Execute query
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.[equipment] WHERE id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {

                            equipment.Id = reader.GetInt32(reader.GetOrdinal("id"));
                            equipment.Name = reader.GetString(reader.GetOrdinal("name"));
                            equipment.CategoryId = reader.GetInt32(reader.GetOrdinal("categoryId"));
                            equipment.Quantity = reader.GetInt32(reader.GetOrdinal("quantity"));
                            equipment.Description = reader.GetString(reader.GetOrdinal("description"));
                            equipment.DailyPrice = reader.GetDecimal(reader.GetOrdinal("dailyCost"));

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them, display an error message)
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch equipment: {ex.Message}", "Ok");
            }
            finally
            {
                connection.Close(); // Ensure that connection is closed regardless of success or failure
            }
            return equipment;
        }

        // Get a Equipment with CategoryStr
        public async Task<EquipmentwithCategoryStr> GetEquipmentWithCategoryStr(int equipmentId)
        {
            connection.Open();
            EquipmentwithCategoryStr equipment = new();
            try
            {
                // Execute query
                using (SqlCommand cmd = new SqlCommand("SELECT e.id, e.name, e.categoryId, e.quantity,e.description, e.dailyCost, c.name AS categorystr FROM dbo.[category] c JOIN  dbo.[equipment] e ON e.categoryId = c.id WHERE e.id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", equipmentId);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            equipment.Id = reader.GetInt32(reader.GetOrdinal("id"));
                            equipment.Name = reader.GetString(reader.GetOrdinal("name"));
                            equipment.CategoryId = reader.GetInt32(reader.GetOrdinal("categoryId"));
                            equipment.Quantity = reader.GetInt32(reader.GetOrdinal("quantity"));
                            equipment.Description = reader.GetString(reader.GetOrdinal("description"));
                            equipment.DailyPrice = reader.GetDecimal(reader.GetOrdinal("dailyCost"));
                            equipment.CategoryStr = reader.GetString(reader.GetOrdinal("categorystr"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them, display an error message)
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch equipment: {ex.Message}", "Ok");
            }
            finally
            {
                connection.Close(); // Ensure that connection is closed regardless of success or failure
            }
            return equipment;
        }


        //Checking the id Already exist
        public async Task<List<int>> GetEquipmentIdList()
        {
            connection.Open();
            List<int> idList = new();

            using (SqlCommand cmd = new SqlCommand("Select id from dbo.[equipment]", connection))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("id"));
                        idList.Add(id);
                    }

                }
            }
            return idList;
        }
        // Add new rental
        public async Task<bool> AddNewRental(RentAEquipment rental)
        {
            try
            {
                // Open the connection
                connection.Open();

                // Define the SQL command
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO dbo.rental VALUES(@id, @customerId, @equipmentId, @quantity, @rentalDate, @startDate, @duration, @totalCost)",
                    connection
                );

                // Add parameters with values
                cmd.Parameters.AddWithValue("@id", rental.ID);
                cmd.Parameters.AddWithValue("@customerId", rental.CustomerId);
                cmd.Parameters.AddWithValue("@equipmentId", rental.EquipmentId);
                cmd.Parameters.AddWithValue("@quantity", rental.Quantity);
                cmd.Parameters.AddWithValue("@rentalDate", rental.RentalDate);
                cmd.Parameters.AddWithValue("@startDate", rental.StartDate);
                cmd.Parameters.AddWithValue("@duration", rental.Duration);
                cmd.Parameters.AddWithValue("@totalCost", rental.TotalCost);

                // Execute the query asynchronously
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                // Handle SQL-related exceptions
                Console.WriteLine($"SQL Error: {ex.Message}");
            }
            finally
            {
                // Ensure the connection is closed
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return true;
        }



        //Retrieve next available RenatlId for inserting a new record
        //Retrieve next available RenatlId for inserting a new record
        public async Task<int> GetRentalNextId()
        {
            connection.Open();
            string selectQuery = "Select MAX(id) from dbo.[rental]";
            try
            {
                using (SqlCommand selectCmd = new SqlCommand(selectQuery, connection))
                {
                    var result = await selectCmd.ExecuteScalarAsync();
                    connection.Close();
                    return result == DBNull.Value ? 1001 : Convert.ToInt32(result) + 1;
                }

            }
            catch (Exception e)
            {
                return 1001;
            }
            finally
            {
                connection.Close();
            }

        }

        //Search for equipment with matching str
        public async Task<List<EquipmentwithCategoryStr>> SearchEquipmentWithStr(string str)
        {
            connection.Open();
            var equipmentList = new List<EquipmentwithCategoryStr>();
            string sqlQuery = "SELECT e.id, e.name, e.categoryId, e.quantity, e.DailyCost, e.description, c.name AS categoryStr FROM dbo.[equipment] e JOIN dbo.[category] c ON e.categoryId=c.id WHERE LOWER(e.name) LIKE @strg OR LOWER(e.description) LIKE @strg OR LOWER(c.name) LIKE @strg";

            try
            {
                // Execute query
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    // Compare LOWER to LOWER
                    cmd.Parameters.AddWithValue("@strg", "%" + str.ToLower() + "%");


                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var equipmentwithCategoryStr = new EquipmentwithCategoryStr
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                CategoryId = reader.GetInt32(reader.GetOrdinal("categoryId")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                                Description = reader.GetString(reader.GetOrdinal("description")),
                                DailyPrice = reader.GetDecimal(reader.GetOrdinal("dailyCost")),
                                CategoryStr = reader.GetString(reader.GetOrdinal("categoryStr"))

                            };

                            equipmentList.Add(equipmentwithCategoryStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them, display an error message)
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch Equipment List: {ex.Message}", "Ok");
            }


            // Alert for user
            //await Application.Current.MainPage.DisplayAlert("Success", "Users Successfully Fetched", "Ok");
            return equipmentList;
        }

        public async Task<List<RentAEquipmentFullInfo>> GetAllRental()
        {
            connection.Open();
            var rentalList = new List<RentAEquipmentFullInfo>();

            try
            {
                // Execute query
                using (SqlCommand cmd = new SqlCommand("SELECT r.id, r.equipmentId, r.customerId, r.quantity, r.rentalDate, r.startDate, r.duration, r.totalCost, e.name as equipment, ca.name As category, cu.firstname as firstname, cu.LastName AS lastname FROM dbo.[rental] r JOIN dbo.[user] cu ON r.customerId = cu.id JOIN  dbo.[equipment] e ON e.id = r.equipmentId JOIN dbo.[category] ca ON e.categoryId=ca.id", connection))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var rentAEquipmentFullInfo = new RentAEquipmentFullInfo
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("id")),
                                EquipmentId = reader.GetInt32(reader.GetOrdinal("equipmentId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("customerId")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                                RentalDate = reader.GetDateTime(reader.GetOrdinal("RentalDate")),
                                StartDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("StartDate"))),
                                Duration = reader.GetInt32(reader.GetOrdinal("duration")),
                                TotalCost = reader.GetDecimal(reader.GetOrdinal("totalCost")),
                                EquipmentName = reader.GetString(reader.GetOrdinal("equipment")),
                                FirstName = reader.GetString(reader.GetOrdinal("firstname")),
                                LastName = reader.GetString(reader.GetOrdinal("lastname")),
                                CategoryName = reader.GetString(reader.GetOrdinal("category"))


                            };

                            rentalList.Add(rentAEquipmentFullInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them, display an error message)
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch Equipment List: {ex.Message}", "Ok");
            }

            connection.Close();
            // Alert for user
            //await Application.Current.MainPage.DisplayAlert("Success", "Users Successfully Fetched", "Ok");
            return rentalList;
        }


        public async Task<List<RentAEquipmentFullInfo>> GetAllRentalByYear( int year)
        {
            connection.Open();
            var rentalList = new List<RentAEquipmentFullInfo>();

            try
            {
                // Execute query
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT 
                        r.id, r.equipmentId,
                        r.customerId, r.quantity,
                        r.rentalDate, r.startDate,
                        r.duration, r.totalCost,
                        e.name AS equipment,
                        ca.name AS category,
                        cu.firstname AS firstname,
                        cu.lastname AS lastname
                    FROM dbo.[rental] r
                    JOIN dbo.[user] cu ON r.customerId = cu.id
                    JOIN dbo.[equipment] e ON e.id = r.equipmentId
                    JOIN dbo.[category] ca ON e.categoryId = ca.id
                    WHERE (YEAR(r.rentalDate) = @yearInput)", connection))
                {
                    // Pass null if no year is selected
                    cmd.Parameters.AddWithValue("@yearInput", year);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var rentAEquipmentFullInfo = new RentAEquipmentFullInfo
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("id")),
                                EquipmentId = reader.GetInt32(reader.GetOrdinal("equipmentId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("customerId")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                                RentalDate = reader.GetDateTime(reader.GetOrdinal("RentalDate")),
                                StartDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("StartDate"))),
                                Duration = reader.GetInt32(reader.GetOrdinal("duration")),
                                TotalCost = reader.GetDecimal(reader.GetOrdinal("totalCost")),
                                EquipmentName = reader.GetString(reader.GetOrdinal("equipment")),
                                FirstName = reader.GetString(reader.GetOrdinal("firstname")),
                                LastName = reader.GetString(reader.GetOrdinal("lastname")),
                                CategoryName = reader.GetString(reader.GetOrdinal("category"))


                            };

                            rentalList.Add(rentAEquipmentFullInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them, display an error message)
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch Equipment List: {ex.Message}", "Ok");
            }

            connection.Close();
            // Alert for user
            //await Application.Current.MainPage.DisplayAlert("Success", "Users Successfully Fetched", "Ok");
            return rentalList;
        }



        public async Task<List<RentalReportByMonthObject>> GetRentalReportByMonth()
        {
            connection.Open();
            var rentalList = new List<RentalReportByMonthObject>();

            try
            {
                // Execute query
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) AS #Rental, LEFT(FORMAT(rentalDate, 'MMMM dd, yyyy'), CHARINDEX(' ', FORMAT(rentalDate, 'MMMM dd, yyyy')) - 1) AS Month, SUM(totalCost) AS Revenue FROM dbo.[rental] GROUP BY LEFT(FORMAT(rentalDate, 'MMMM dd, yyyy'), CHARINDEX(' ', FORMAT(rentalDate, 'MMMM dd, yyyy')) - 1)", connection))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {

                        while (reader.Read())
                        {
                            var rental = new RentalReportByMonthObject
                            {
                                RentalCount = reader.GetInt32(reader.GetOrdinal("#Rental")),
                                TotalCost = reader.GetDecimal(reader.GetOrdinal("revenue")),
                                Month = reader.GetString(reader.GetOrdinal("Month"))
                            };

                            rentalList.Add(rental);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them, display an error message)
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch Equipment List: {ex.Message}", "Ok");
            }

            connection.Close();
            // Alert for user
            //await Application.Current.MainPage.DisplayAlert("Success", "Users Successfully Fetched", "Ok");
            return rentalList;
        }

        public async Task<List<RentalReportByMonthObject>> GetRentalReportByMonth(int year)
        {
            connection.Open();
            var rentalList = new List<RentalReportByMonthObject>();

            try
            {
                // Execute query
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT 
                        COUNT(*) AS [#Rental], 
                        LEFT(FORMAT(rentalDate, 'MMMM dd, yyyy'), CHARINDEX(' ', FORMAT(rentalDate, 'MMMM dd, yyyy')) - 1) AS [Month], 
                        SUM(totalCost) AS [Revenue]
                    FROM dbo.[rental]
                    WHERE  YEAR(rentalDate) = @yearInput
                    GROUP BY LEFT(FORMAT(rentalDate, 'MMMM dd, yyyy'), CHARINDEX(' ', FORMAT(rentalDate, 'MMMM dd, yyyy')) - 1)
                    ", connection))
                {
                    // Pass null if no year is selected
                    cmd.Parameters.AddWithValue("@yearInput", year);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {

                        while (reader.Read())
                        {
                            var rental = new RentalReportByMonthObject
                            {
                                RentalCount = reader.GetInt32(reader.GetOrdinal("#Rental")),
                                TotalCost = reader.GetDecimal(reader.GetOrdinal("revenue")),
                                Month = reader.GetString(reader.GetOrdinal("Month"))
                            };

                            rentalList.Add(rental);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them, display an error message)
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to fetch Equipment List: {ex.Message}", "Ok");
            }

            connection.Close();
            // Alert for user
            //await Application.Current.MainPage.DisplayAlert("Success", "Users Successfully Fetched", "Ok");
            return rentalList;
        }
    }
}
