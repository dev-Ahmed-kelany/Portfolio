using DataAccess;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace Portfolio.DataAccess
{
    public class PersonDTO
    {
        public long ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public interface IPerson : IRepository<PersonDTO>
    {
    }

    public class clsPersonDA : IPerson
    {
        private readonly NpgsqlDataSource __DataSource;
        private readonly ILogger<clsPersonDA>? __Logger;

        /// <summary>
        /// Initializes a new instance of the clsPersonDA class.
        /// </summary>
        /// <param name="DataSource">The Npgsql data source for database connections.</param>
        /// <param name="Logger">Optional logger for debugging and error tracking.</param>
        /// <exception cref="ArgumentNullException">Thrown when DataSource is null.</exception>
        public clsPersonDA(NpgsqlDataSource DataSource, ILogger<clsPersonDA>? Logger = null)
        {
            __DataSource = DataSource ?? throw new ArgumentNullException(nameof(DataSource), "DataSource cannot be null.");
            __Logger = Logger;
        }

        /// <summary>
        /// Retrieves all people from the database asynchronously.
        /// </summary>
        /// <returns>A list of PersonDTO objects. Returns an empty list if no data is found.</returns>
        /// <exception cref="InvalidOperationException">Thrown when database connection fails.</exception>
        /// <exception cref="NpgsqlException">Thrown when a PostgreSQL-specific error occurs.</exception>
        /// <exception cref="Exception">Thrown for unexpected errors during data retrieval.</exception>
        public async Task<List<PersonDTO>> getAll()
        {
            var list = new List<PersonDTO>();

            try
            {
                __Logger?.LogInformation("Attempting to retrieve all people from database.");

                // Validate DataSource
                if (__DataSource == null)
                {
                    __Logger?.LogError("DataSource is null. Cannot proceed with database operation.");
                    throw new InvalidOperationException("DataSource is not initialized. Please ensure proper dependency injection configuration.");
                }

                // Open connection with timeout handling
                await using var connection = await __DataSource.OpenConnectionAsync();

                if (connection == null || connection.State != System.Data.ConnectionState.Open)
                {
                    __Logger?.LogError("Failed to establish database connection. Connection state: {ConnectionState}", connection?.State);
                    throw new InvalidOperationException("Unable to establish a connection to the database. Please check your connection settings.");
                }

                // Create and execute command
                await using var command = new NpgsqlCommand("SELECT * FROM getAllPeople()", connection);

                if (command == null)
                {
                    __Logger?.LogError("Failed to create NpgsqlCommand.");
                    throw new InvalidOperationException("Failed to create database command.");
                }

                // Execute query and read results
                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                // Read all rows BEFORE leaving scope (to prevent reader disposal issues)
                while (await reader.ReadAsync())
                {
                    try
                    {
                        // Safely extract column values with null checks
                        if (reader.HasRows && reader.FieldCount >= 4)
                        {
                            var personDTO = new PersonDTO
                            {
                                ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                                Name = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                                Description = !reader.IsDBNull(2) ? reader.GetString(2) : null,
                                ImageUrl = !reader.IsDBNull(3) ? reader.GetString(3) : null
                            };

                            // Additional validation for required fields
                            if (personDTO.ID <= 0)
                            {
                                __Logger?.LogWarning("Skipping record with invalid ID: {ID}", personDTO.ID);
                                continue;
                            }

                            if (string.IsNullOrWhiteSpace(personDTO.Name))
                            {
                                __Logger?.LogWarning("Skipping record with empty Name for ID: {ID}", personDTO.ID);
                                continue;
                            }

                            list.Add(personDTO);
                        }
                    }
                    catch (InvalidOperationException ioEx)
                    {
                        __Logger?.LogWarning("Error reading row data: {Message}", ioEx.Message);
                        continue; // Skip this row and continue with the next
                    }
                    catch (Exception rowEx)
                    {
                        __Logger?.LogError("Unexpected error while reading row: {Message}", rowEx.Message);
                        continue; // Skip this row and continue with the next
                    }
                }

                __Logger?.LogInformation("Successfully retrieved {Count} people from database.", list.Count);
                return list;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving all people. Error Code: {ErrorCode}", npgsqlEx.ErrorCode);
                throw new InvalidOperationException("A database error occurred while retrieving people. Please try again later.", npgsqlEx);
            }
            catch (TimeoutException timeoutEx)
            {
                __Logger?.LogError(timeoutEx, "Database operation timed out while retrieving all people.");
                throw new InvalidOperationException("The database operation timed out. Please try again later.", timeoutEx);
            }
            catch (InvalidOperationException invalidOpEx)
            {
                __Logger?.LogError(invalidOpEx, "Invalid operation error: {Message}", invalidOpEx.Message);
                throw; // Re-throw InvalidOperationException as is
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving all people: {Message}", ex.Message);
                throw new Exception("An unexpected error occurred while retrieving people. Please contact support.", ex);
            }
        }

        public async Task<PersonDTO> getById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to retrieve person with ID: {ID}", ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM getPersonById(@p_id)", connection);
                command.Parameters.AddWithValue("@p_id", ID);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync())
                {
                    if (reader.HasRows && reader.FieldCount >= 4)
                    {
                        var personDTO = new PersonDTO
                        {
                            ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                            Name = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                            Description = !reader.IsDBNull(2) ? reader.GetString(2) : null,
                            ImageUrl = !reader.IsDBNull(3) ? reader.GetString(3) : null
                        };

                        __Logger?.LogInformation("Successfully retrieved person with ID: {ID}", ID);
                        return personDTO;
                    }
                }

                __Logger?.LogWarning("Person with ID: {ID} not found.", ID);
                return new PersonDTO();
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving person with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving person with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<long> addNew(PersonDTO person)
        {
            try
            {
                __Logger?.LogInformation("Attempting to add new person: {Name}", person.Name);

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

                await using var command = new NpgsqlCommand("SELECT * FROM addNewPerson(@p_name, @p_description, @p_imageurl)", connection);
                command.Parameters.AddWithValue("@p_name", person.Name ?? string.Empty);
                command.Parameters.AddWithValue("@p_description", person.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@p_imageurl", person.ImageUrl ?? (object)DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync() && !reader.IsDBNull(0))
                {
                    var newId = reader.GetInt64(0);
                    __Logger?.LogInformation("Successfully added new person with ID: {ID}", newId);
                    return newId;
                }

                __Logger?.LogWarning("Failed to add new person.");
                return 0;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while adding new person.");
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while adding new person.");
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> updateById(PersonDTO person)
        {
            try
            {
                __Logger?.LogInformation("Attempting to update person with ID: {ID}", person.ID);

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

                await using var command = new NpgsqlCommand("SELECT updatePersonById(@p_id, @p_name, @p_description, @p_imageurl)", connection);
                command.Parameters.AddWithValue("@p_id", person.ID);
                command.Parameters.AddWithValue("@p_name", person.Name ?? string.Empty);
                command.Parameters.AddWithValue("@p_description", person.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@p_imageurl", person.ImageUrl ?? (object)DBNull.Value);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully updated person with ID: {ID}", person.ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to update person with ID: {ID}", person.ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while updating person with ID: {ID}", person.ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while updating person with ID: {ID}", person.ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> deleteById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to delete person with ID: {ID}", ID);

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

                await using var command = new NpgsqlCommand("SELECT deletePersonById(@p_id)", connection);
                command.Parameters.AddWithValue("@p_id", ID);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully deleted person with ID: {ID}", ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to delete person with ID: {ID}", ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while deleting person with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while deleting person with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
    }
}
