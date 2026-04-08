using Npgsql;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using DataAccess;

namespace Portfolio.DataAccess
{
    public class UserDTO
    {
        public long ID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public long Permissions { get; set; }
        public long PersonID { get; set; }
    }

    public class UserWithoutPasswordDTO
    {
        public long ID { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public long Permissions { get; set; }
        public long PersonID { get; set; }
    }

    public interface IUser
    {
        Task<long> addNew(UserDTO User);
        Task<bool> updateById(UserDTO User);
        Task<bool> deleteById(long ID);
        Task<UserWithoutPasswordDTO> getById(long ID);
        Task<List<UserWithoutPasswordDTO>> getAll();
        Task<UserDTO> getUserByCredentials(string Username, string Password);
    }

    public class clsUserDA : IUser
    {
        private readonly NpgsqlDataSource __DataSource;
        private readonly ILogger<clsUserDA>? __Logger;

        public clsUserDA(NpgsqlDataSource DataSource, ILogger<clsUserDA>? Logger = null)
        {
            __DataSource = DataSource ?? throw new ArgumentNullException(nameof(DataSource), "DataSource cannot be null.");
            __Logger = Logger;
        }

        public async Task<List<UserWithoutPasswordDTO>> getAll()
        {
            var list = new List<UserWithoutPasswordDTO>();

            try
            {
                __Logger?.LogInformation("Attempting to retrieve all user records from database.");

                if (__DataSource == null)
                {
                    __Logger?.LogError("DataSource is null. Cannot proceed with database operation.");
                    throw new InvalidOperationException("DataSource is not initialized. Please ensure proper dependency injection configuration.");
                }

                await using var connection = await __DataSource.OpenConnectionAsync();

                if (connection == null || connection.State != System.Data.ConnectionState.Open)
                {
                    __Logger?.LogError("Failed to establish database connection. Connection state: {ConnectionState}", connection?.State);
                    throw new InvalidOperationException("Unable to establish a connection to the database. Please check your connection settings.");
                }

                await using var command = new NpgsqlCommand("SELECT * FROM getAllUsers()", connection);

                if (command == null)
                {
                    __Logger?.LogError("Failed to create NpgsqlCommand.");
                    throw new InvalidOperationException("Failed to create database command.");
                }

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                while (await reader.ReadAsync())
                {
                    try
                    {
                        if (reader.HasRows && reader.FieldCount >= 6)
                        {
                            var userDTO = new UserWithoutPasswordDTO
                            {
                                ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                                Username = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                                IsActive = !reader.IsDBNull(2) ? reader.GetBoolean(2) : false,
                                Permissions = !reader.IsDBNull(3) ? reader.GetInt64(3) : 0,
                                PersonID = !reader.IsDBNull(4) ? reader.GetInt64(4) : 0
                            };

                            if (userDTO.ID <= 0)
                            {
                                __Logger?.LogWarning("Skipping record with invalid ID: {ID}", userDTO.ID);
                                continue;
                            }

                            list.Add(userDTO);
                        }
                    }
                    catch (InvalidOperationException ioEx)
                    {
                        __Logger?.LogWarning("Error reading row data: {Message}", ioEx.Message);
                        continue;
                    }
                    catch (Exception rowEx)
                    {
                        __Logger?.LogError("Unexpected error while reading row: {Message}", rowEx.Message);
                        continue;
                    }
                }

                __Logger?.LogInformation("Successfully retrieved {Count} user records from database.", list.Count);
                return list;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving all user records. Error Code: {ErrorCode}", npgsqlEx.ErrorCode);
                throw new InvalidOperationException("A database error occurred while retrieving user records. Please try again later.", npgsqlEx);
            }
            catch (TimeoutException timeoutEx)
            {
                __Logger?.LogError(timeoutEx, "Database operation timed out while retrieving all user records.");
                throw new InvalidOperationException("The database operation timed out. Please try again later.", timeoutEx);
            }
            catch (InvalidOperationException invalidOpEx)
            {
                __Logger?.LogError(invalidOpEx, "Invalid operation error: {Message}", invalidOpEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving all user records: {Message}", ex.Message);
                throw new Exception("An unexpected error occurred while retrieving user records. Please contact support.", ex);
            }
        }

        public async Task<UserWithoutPasswordDTO> getById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to retrieve user record with ID: {ID}", ID);

                if (__DataSource == null)
                {
                    __Logger?.LogError("DataSource is null. Cannot proceed with database operation.");
                    throw new InvalidOperationException("DataSource is not initialized.");
                }

                await using var connection = await __DataSource.OpenConnectionAsync();

                if (connection == null || connection.State != System.Data.ConnectionState.Open)
                {
                    __Logger?.LogError("Failed to establish database connection.");
                    throw new InvalidOperationException("Unable to establish a connection to the database.");
                }

                await using var command = new NpgsqlCommand("SELECT * FROM getUserById(@p_id)", connection);
                command.Parameters.AddWithValue("@p_id", ID);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync())
                {
                    if (reader.HasRows && reader.FieldCount >= 6)
                    {
                        var userDTO = new UserWithoutPasswordDTO
                        {
                            ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                            Username = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                            IsActive = !reader.IsDBNull(2) ? reader.GetBoolean(2) : false,
                            Permissions = !reader.IsDBNull(3) ? reader.GetInt64(3) : 0,
                            PersonID = !reader.IsDBNull(4) ? reader.GetInt64(4) : 0
                        };

                        __Logger?.LogInformation("Successfully retrieved user record with ID: {ID}", ID);
                        return userDTO;
                    }
                }

                __Logger?.LogWarning("User record with ID: {ID} not found.", ID);
                return new UserWithoutPasswordDTO();
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving user record with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving user record with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<long> addNew(UserDTO user)
        {
            try
            {
                __Logger?.LogInformation("Attempting to add new user record.");

                if (__DataSource == null)
                {
                    __Logger?.LogError("DataSource is null. Cannot proceed with database operation.");
                    throw new InvalidOperationException("DataSource is not initialized.");
                }

                await using var connection = await __DataSource.OpenConnectionAsync();

                if (connection == null || connection.State != System.Data.ConnectionState.Open)
                {
                    __Logger?.LogError("Failed to establish database connection.");
                    throw new InvalidOperationException("Unable to establish a connection to the database.");
                }

                await using var command = new NpgsqlCommand("SELECT * FROM addNewUser(@p_username, @p_password, @p_isactive, @p_permissions, @p_personid)", connection);
                command.Parameters.AddWithValue("@p_username", user.Username ?? string.Empty);
                command.Parameters.AddWithValue("@p_password", user.Password ?? string.Empty);
                command.Parameters.AddWithValue("@p_isactive", user.IsActive);
                command.Parameters.AddWithValue("@p_permissions", user.Permissions);
                command.Parameters.AddWithValue("@p_personid", user.PersonID);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync() && !reader.IsDBNull(0))
                {
                    var newId = reader.GetInt64(0);
                    __Logger?.LogInformation("Successfully added new user record with ID: {ID}", newId);
                    return newId;
                }

                __Logger?.LogWarning("Failed to add new user record.");
                return 0;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while adding new user record.");
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while adding new user record.");
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> updateById(UserDTO user)
        {
            try
            {
                __Logger?.LogInformation("Attempting to update user record with ID: {ID}", user.ID);

                if (__DataSource == null)
                {
                    __Logger?.LogError("DataSource is null. Cannot proceed with database operation.");
                    throw new InvalidOperationException("DataSource is not initialized.");
                }

                await using var connection = await __DataSource.OpenConnectionAsync();

                if (connection == null || connection.State != System.Data.ConnectionState.Open)
                {
                    __Logger?.LogError("Failed to establish database connection.");
                    throw new InvalidOperationException("Unable to establish a connection to the database.");
                }

                await using var command = new NpgsqlCommand("SELECT * FROM updateUserById(@p_id, @p_username, @p_password, @p_isactive, @p_permissions, @p_personid)", connection);
                command.Parameters.AddWithValue("@p_id", user.ID);
                command.Parameters.AddWithValue("@p_username", user.Username ?? string.Empty);
                command.Parameters.AddWithValue("@p_password", user.Password ?? string.Empty);
                command.Parameters.AddWithValue("@p_isactive", user.IsActive);
                command.Parameters.AddWithValue("@p_permissions", user.Permissions);
                command.Parameters.AddWithValue("@p_personid", user.PersonID);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully updated user record with ID: {ID}", user.ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to update user record with ID: {ID}", user.ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while updating user record with ID: {ID}", user.ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while updating user record with ID: {ID}", user.ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> deleteById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to delete user record with ID: {ID}", ID);

                if (__DataSource == null)
                {
                    __Logger?.LogError("DataSource is null. Cannot proceed with database operation.");
                    throw new InvalidOperationException("DataSource is not initialized.");
                }

                await using var connection = await __DataSource.OpenConnectionAsync();

                if (connection == null || connection.State != System.Data.ConnectionState.Open)
                {
                    __Logger?.LogError("Failed to establish database connection.");
                    throw new InvalidOperationException("Unable to establish a connection to the database.");
                }

                await using var command = new NpgsqlCommand("SELECT * FROM deleteUserById(@p_id)", connection);
                command.Parameters.AddWithValue("@p_id", ID);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully deleted user record with ID: {ID}", ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to delete user record with ID: {ID}", ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while deleting user record with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while deleting user record with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<UserDTO> getUserByCredentials(string Username, string Password)
        {
            try
            {
                __Logger?.LogInformation("Attempting to retrieve user by credentials for username: {Username}", Username);

                if (__DataSource == null)
                {
                    __Logger?.LogError("DataSource is null. Cannot proceed with database operation.");
                    throw new InvalidOperationException("DataSource is not initialized.");
                }

                await using var connection = await __DataSource.OpenConnectionAsync();

                if (connection == null || connection.State != System.Data.ConnectionState.Open)
                {
                    __Logger?.LogError("Failed to establish database connection.");
                    throw new InvalidOperationException("Unable to establish a connection to the database.");
                }

                await using var command = new NpgsqlCommand("SELECT * FROM getUserByCredentials(@p_username, @p_password)", connection);
                command.Parameters.AddWithValue("@p_username", Username ?? string.Empty);
                command.Parameters.AddWithValue("@p_password", Password ?? string.Empty);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync())
                {
                    if (reader.HasRows && reader.FieldCount >= 6)
                    {
                        var userDTO = new UserDTO
                        {
                            ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                            Username = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                            Password = !reader.IsDBNull(2) ? (reader.GetString(2) ?? string.Empty) : string.Empty,
                            IsActive = !reader.IsDBNull(3) ? reader.GetBoolean(3) : false,
                            Permissions = !reader.IsDBNull(4) ? reader.GetInt64(4) : 0,
                            PersonID = !reader.IsDBNull(5) ? reader.GetInt64(5) : 0
                        };

                        __Logger?.LogInformation("Successfully retrieved user by credentials for username: {Username}", Username);
                        return userDTO;
                    }
                }

                __Logger?.LogWarning("User not found for username: {Username}", Username);
                return new UserDTO();
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving user by credentials for username: {Username}", Username);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving user by credentials for username: {Username}", Username);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
    }
}
