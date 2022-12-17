using System.Data;
using System.Data.SqlClient;
using Sample.Domain;

namespace Sample.Ado
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;

        public UsersRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            await using SqlConnection con = new SqlConnection(_connectionString);
            
            string query = $"SELECT * FROM Users WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlParameter idParam = new SqlParameter();
            idParam.ParameterName = "@Id";
            idParam.Value = id.ToString();

            cmd.Parameters.Add(idParam);
            await con.OpenAsync();
            
            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows)
                throw new EntityNotFoundException(nameof(User));
            
            reader.Read();
            var user = new User
            {
                Id = id,
                Name = reader["Name"].ToString(),
                Age = Convert.ToByte(reader["Age"]),
                Credit = Convert.ToDouble(reader["Credit"]),
                CreateTime = Convert.ToDateTime(reader["CreateTime"])
            };

            await con.CloseAsync();

            return user;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            List<User> users = new List<User>();
            await using SqlConnection con = new SqlConnection(_connectionString);
            
            string query = $"SELECT * FROM Users";
            SqlCommand cmd = new SqlCommand(query, con);
            await con.OpenAsync();

            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var u = new User
                {
                    Id = Guid.Parse(reader["Id"].ToString()),
                    Name = reader["Name"].ToString(),
                    Age = Convert.ToByte(reader["Age"]),
                    Credit = Convert.ToDouble(reader["Credit"]),
                    CreateTime = Convert.ToDateTime(reader["CreateTime"])
                };

                users.Add(u);
            }

            await con.CloseAsync();

            return users;
        }

        public async Task InsertAsync(User user)
        {
            await using SqlConnection con = new SqlConnection(_connectionString);

            SqlCommand cmd = new SqlCommand("spCreateUser", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", user.Id);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Age", user.Age);
            cmd.Parameters.AddWithValue("@Credit", user.Credit);
            cmd.Parameters.AddWithValue("@CreateTime", user.CreateTime);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            await con.CloseAsync();
        }

        public async Task UpdateAsync(User user)
        {
            await using SqlConnection con = new SqlConnection(_connectionString);

            SqlCommand cmd = new SqlCommand("spUpdateUser", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", user.Id);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Age", user.Age);
            cmd.Parameters.AddWithValue("@Credit", user.Credit);
            
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            await con.CloseAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using SqlConnection con = new SqlConnection(_connectionString);

            SqlCommand cmd = new SqlCommand("spDeleteUser", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            await con.CloseAsync();
        }
    }
}