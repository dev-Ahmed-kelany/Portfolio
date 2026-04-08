using Npgsql;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using DataAccess;

namespace Portfolio.DataAccess
{
    public class ContactInfoDTO
    {
        public long ID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public long PersonID { get; set; }
    }

    public interface IContactInfo : IRepository<ContactInfoDTO>
    {
        Task<ContactInfoDTO> getByPerson(long PersonID);
    }

    public class clsContactInfoDA : IContactInfo
    {
        private readonly NpgsqlDataSource __DataSource;
        private readonly ILogger<clsContactInfoDA>? __Logger;

        public clsContactInfoDA(NpgsqlDataSource DataSource, ILogger<clsContactInfoDA>? Logger = null)
        {
            __DataSource = DataSource ?? throw new ArgumentNullException(nameof(DataSource), "DataSource cannot be null.");
            __Logger = Logger;
        }

        public async Task<List<ContactInfoDTO>> getAll()
        {
            var list = new List<ContactInfoDTO>();

            try
            {
                __Logger?.LogInformation("Attempting to retrieve all contact info records from database.");

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

                await using var command = new NpgsqlCommand("SELECT * FROM getAllContactInfo()", connection);

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
                        if (reader.HasRows && reader.FieldCount >= 5)
                        {
                            var contactInfoDTO = new ContactInfoDTO
                            {
                                ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                                Email = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                                Phone = !reader.IsDBNull(2) ? (reader.GetString(2) ?? string.Empty) : string.Empty,
                                Location = !reader.IsDBNull(3) ? (reader.GetString(3) ?? string.Empty) : string.Empty,
                                PersonID = !reader.IsDBNull(4) ? reader.GetInt64(4) : 0
                            };

                            if (contactInfoDTO.ID <= 0)
                            {
                                __Logger?.LogWarning("Skipping record with invalid ID: {ID}", contactInfoDTO.ID);
                                continue;
                            }

                            list.Add(contactInfoDTO);
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

                __Logger?.LogInformation("Successfully retrieved {Count} contact info records from database.", list.Count);
                return list;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving all contact info records. Error Code: {ErrorCode}", npgsqlEx.ErrorCode);
                throw new InvalidOperationException("A database error occurred while retrieving contact info records. Please try again later.", npgsqlEx);
            }
            catch (TimeoutException timeoutEx)
            {
                __Logger?.LogError(timeoutEx, "Database operation timed out while retrieving all contact info records.");
                throw new InvalidOperationException("The database operation timed out. Please try again later.", timeoutEx);
            }
            catch (InvalidOperationException invalidOpEx)
            {
                __Logger?.LogError(invalidOpEx, "Invalid operation error: {Message}", invalidOpEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving all contact info records: {Message}", ex.Message);
                throw new Exception("An unexpected error occurred while retrieving contact info records. Please contact support.", ex);
            }
        }

        public async Task<ContactInfoDTO> getById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to retrieve contact info record with ID: {ID}", ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM getContactInfoById(@p_id)", connection);
                command.Parameters.AddWithValue("@p_id", ID);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync())
                {
                    if (reader.HasRows && reader.FieldCount >= 5)
                    {
                        var contactInfoDTO = new ContactInfoDTO
                        {
                            ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                            Email = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                            Phone = !reader.IsDBNull(2) ? (reader.GetString(2) ?? string.Empty) : string.Empty,
                            Location = !reader.IsDBNull(3) ? (reader.GetString(3) ?? string.Empty) : string.Empty,
                            PersonID = !reader.IsDBNull(4) ? reader.GetInt64(4) : 0
                        };

                        __Logger?.LogInformation("Successfully retrieved contact info record with ID: {ID}", ID);
                        return contactInfoDTO;
                    }
                }

                __Logger?.LogWarning("Contact info record with ID: {ID} not found.", ID);
                return new ContactInfoDTO();
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving contact info record with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving contact info record with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<long> addNew(ContactInfoDTO contactInfo)
        {
            try
            {
                __Logger?.LogInformation("Attempting to add new contact info record.");

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

                await using var command = new NpgsqlCommand("SELECT * FROM addNewContactInfo(@p_email, @p_phone, @p_location, @p_personid)", connection);
                command.Parameters.AddWithValue("@p_email", contactInfo.Email ?? string.Empty);
                command.Parameters.AddWithValue("@p_phone", contactInfo.Phone ?? string.Empty);
                command.Parameters.AddWithValue("@p_location", contactInfo.Location ?? string.Empty);
                command.Parameters.AddWithValue("@p_personid", contactInfo.PersonID);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync() && !reader.IsDBNull(0))
                {
                    var newId = reader.GetInt64(0);
                    __Logger?.LogInformation("Successfully added new contact info record with ID: {ID}", newId);
                    return newId;
                }

                __Logger?.LogWarning("Failed to add new contact info record.");
                return 0;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while adding new contact info record.");
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while adding new contact info record.");
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> updateById(ContactInfoDTO contactInfo)
        {
            try
            {
                __Logger?.LogInformation("Attempting to update contact info record with ID: {ID}", contactInfo.ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM updateContactInfoById(@p_id, @p_email, @p_phone, @p_location, @p_personid)", connection);
                command.Parameters.AddWithValue("@p_id", contactInfo.ID);
                command.Parameters.AddWithValue("@p_email", contactInfo.Email ?? string.Empty);
                command.Parameters.AddWithValue("@p_phone", contactInfo.Phone ?? string.Empty);
                command.Parameters.AddWithValue("@p_location", contactInfo.Location ?? string.Empty);
                command.Parameters.AddWithValue("@p_personid", contactInfo.PersonID);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully updated contact info record with ID: {ID}", contactInfo.ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to update contact info record with ID: {ID}", contactInfo.ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while updating contact info record with ID: {ID}", contactInfo.ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while updating contact info record with ID: {ID}", contactInfo.ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> deleteById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to delete contact info record with ID: {ID}", ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM deleteContactInfoById(@p_id)", connection);
                command.Parameters.AddWithValue("@p_id", ID);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully deleted contact info record with ID: {ID}", ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to delete contact info record with ID: {ID}", ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while deleting contact info record with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while deleting contact info record with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<ContactInfoDTO> getByPerson(long PersonID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to retrieve contact info record for person with ID: {PersonID}", PersonID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM getContactInfoByPerson(@p_personid)", connection);
                command.Parameters.AddWithValue("@p_personid", PersonID);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync())
                {
                    if (reader.HasRows && reader.FieldCount >= 5)
                    {
                        var contactInfoDTO = new ContactInfoDTO
                        {
                            ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                            Email = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                            Phone = !reader.IsDBNull(2) ? (reader.GetString(2) ?? string.Empty) : string.Empty,
                            Location = !reader.IsDBNull(3) ? (reader.GetString(3) ?? string.Empty) : string.Empty,
                            PersonID = !reader.IsDBNull(4) ? reader.GetInt64(4) : 0
                        };

                        __Logger?.LogInformation("Successfully retrieved contact info record for person with ID: {PersonID}", PersonID);
                        return contactInfoDTO;
                    }
                }

                __Logger?.LogWarning("Contact info record for person with ID: {PersonID} not found.", PersonID);
                return new ContactInfoDTO();
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving contact info record for person with ID: {PersonID}", PersonID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving contact info record for person with ID: {PersonID}", PersonID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
    }
}
