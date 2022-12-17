using System.Data;
using System.Data.SqlClient;

namespace Sample.Ado;

public class Migrations : IDisposable
{
    private readonly string _connectionString;
    private SqlConnection _con;
    public Migrations(string connectionString)
    {
        _connectionString = connectionString;
        Apply();
    }

    private void Apply()
    {
        EnsureDbExists();
        CreateUserTable();
        CreateStoredProcedures();
    }

    private void EnsureDbExists()
    {
        try
        {
            _con = new SqlConnection(_connectionString);
            _con.Open();
            _con.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Unable to open a connection to db");
            throw;
        }
    }

    private void CreateUserTable()
    {
        string command =
            "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id=OBJECT_ID(N'[dbo].[Users]') AND type='U') " +
            "BEGIN " +
            "Create table Users( " +
            " [Id] [uniqueidentifier] NOT NULL, " +
            " [Name] nvarchar(200) NOT NULL, " +
            " [Age] smallint NOT NULL, " +
            " [Credit] float NOT NULL, " +
            " [CreateTime] [datetime2] NOT NULL " +
            ") " +
            "END ";

        _con.Open();
        SqlCommand cmd = new SqlCommand(command, _con);
        cmd.ExecuteReader();
        _con.Close();
    }

    private void CreateStoredProcedures()
    {
        string createUserSpName = "spCreateUser";
        string createUserSp =
            $"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{createUserSpName}]') AND type in (N'P', N'PC')) " +
            "BEGIN " +
            $"EXEC ('CREATE PROCEDURE {createUserSpName} " +
            "( " +
            " @Id [uniqueidentifier], " +
            " @Name nvarchar(200), " +
            " @Age smallint, " +
            " @Credit float, " +
            " @CreateTime [datetime2] " +
            ") " +
            "AS " +
            "BEGIN " +
            "INSERT INTO Users(Id, Name, Age, Credit, CreateTime) " +
            "VALUES(@Id, @Name, @Age, @Credit, @CreateTime) " +
            "END') " +
            "END ";

        _con.Open();
        SqlCommand createUserSpCmd = new SqlCommand(createUserSp, _con);
        createUserSpCmd.ExecuteReader();
        _con.Close();

        string updateUserSpName = "spUpdateUser";
        string updateSp =
            $"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{updateUserSpName}]') AND type in (N'P', N'PC')) " +
            "BEGIN " +
            $"EXEC ('CREATE PROCEDURE {updateUserSpName} " +
            "( " +
            " @Id [uniqueidentifier], " +
            " @Name nvarchar(200), " +
            " @Age smallint, " +
            " @Credit float " +
            ") " +
            "AS " +
            "BEGIN " +
            "UPDATE Users " +
            "SET Name = @Name, " +
            "Age = @Age, " +
            "Credit = @Credit " +
            "WHERE Id = @Id " +
            "END') " +
            "END ";

        _con.Open();
        SqlCommand updateUserSpCmd = new SqlCommand(updateSp, _con);
        updateUserSpCmd.ExecuteReader();
        _con.Close();

        string deleteUserSpName = "spDeleteUser";
        string deleteSp =
            $"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{deleteUserSpName}]') AND type in (N'P', N'PC')) " +
            "BEGIN " +
            $"EXEC ('CREATE PROCEDURE {deleteUserSpName} " +
            "( " +
            " @Id [uniqueidentifier] " +
            ") " +
            "AS " +
            "BEGIN " +
            "DELETE FROM Users WHERE Id=@Id " +
            "END') " +
            "END ";

        _con.Open();
        SqlCommand deleteUserSpCmd = new SqlCommand(deleteSp, _con);
        deleteUserSpCmd.ExecuteReader();
        _con.Close();
    }

    public void Dispose()
    {
        if (_con.State == ConnectionState.Open)
            _con.Close();

        _con.Dispose();
    }
}