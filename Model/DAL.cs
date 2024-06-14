using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SampleWebApp.Model
{
    public class DAL
    {
        private readonly IConfiguration _configuration;

        public DAL(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<string> GetConnectionStringAsync()
        {
            try
            {
                string keyVaultUrl = _configuration["KeyVaultUrl"];
                string secretName = _configuration["ConnectionStringSecretName"];

                var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
                KeyVaultSecret secret = await client.GetSecretAsync(secretName);

                return secret.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving connection string: {ex.Message}");
                throw;
            }
        }

        public async Task<List<User>> GetUsersAsync()
        {
            List<User> users = new List<User>();
            string connectionString = await GetConnectionStringAsync();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TblUsers", con);
                DataTable dt = new DataTable();
                await Task.Run(() => da.Fill(dt));
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        User user = new User();
                        user.Id = Convert.ToString(dt.Rows[i]["Id"]);
                        user.FirstName = Convert.ToString(dt.Rows[i]["FirstName"]);
                        user.LastName = Convert.ToString(dt.Rows[i]["LastName"]);
                        users.Add(user);
                    }
                }
            }
            return users;
        }

        public async Task<int> AddUserAsync(User user)
        {
            int i = 0;
            string connectionString = await GetConnectionStringAsync();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO TblUsers (FirstName, LastName) VALUES(@FirstName, @LastName)", con);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);

                con.Open();
                i = await cmd.ExecuteNonQueryAsync();
                con.Close();
            }
            return i;
        }

        public async Task<User> GetUserAsync(string id)
        {
            User user = new User();
            string connectionString = await GetConnectionStringAsync();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TblUsers WHERE ID = @ID", con);
                da.SelectCommand.Parameters.AddWithValue("@ID", id);
                DataTable dt = new DataTable();
                await Task.Run(() => da.Fill(dt));
                if (dt.Rows.Count > 0)
                {
                    user.Id = Convert.ToString(dt.Rows[0]["Id"]);
                    user.FirstName = Convert.ToString(dt.Rows[0]["FirstName"]);
                    user.LastName = Convert.ToString(dt.rows[0]["LastName"]);
                }
            }
            return user;
        }

        public async Task<int> UpdateUserAsync(User user)
        {
            int i = 0;
            string connectionString = await GetConnectionStringAsync();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE TblUsers SET FirstName = @FirstName, LastName = @LastName WHERE ID = @ID", con);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@ID", user.Id);

                con.Open();
                i = await cmd.ExecuteNonQueryAsync();
                con.Close();
            }
            return i;
        }

        public async Task<int> DeleteUserAsync(string id)
        {
            int i = 0;
            string connectionString = await GetConnectionStringAsync();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM TblUsers WHERE ID = @ID", con);
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                i = await cmd.ExecuteNonQueryAsync();
                con.Close();
            }
            return i;
        }
    }
}
