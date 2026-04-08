using Npgsql;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using DataAccess;

namespace Portfolio.DataAccess
{
    public class PersonSkillDTO
    {
        public long ID { get; set; }
        public long PersonID { get; set; }
        public long SkillID { get; set; }
    }

    public interface IPersonSkill : IRepository<PersonSkillDTO>
    {
        Task<List<PersonSkillDTO>> getByPerson(long PersonID);
    }

    public class clsPersonSkillDA : IPersonSkill
    {
        private readonly NpgsqlDataSource __DataSource;
        private readonly ILogger<clsPersonSkillDA>? __Logger;

        public clsPersonSkillDA(NpgsqlDataSource DataSource, ILogger<clsPersonSkillDA>? Logger = null)
        {
            __DataSource = DataSource ?? throw new ArgumentNullException(nameof(DataSource), "DataSource cannot be null.");
            __Logger = Logger;
        }

        public async Task<List<PersonSkillDTO>> getAll()
        {
            var list = new List<PersonSkillDTO>();

            try
            {
                __Logger?.LogInformation("Attempting to retrieve all person skill records from database.");

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

                await using var command = new NpgsqlCommand("SELECT * FROM getAllPersonSkills()", connection);

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
                        if (reader.HasRows && reader.FieldCount >= 3)
                        {
                            var personSkillDTO = new PersonSkillDTO
                            {
                                ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                                PersonID = !reader.IsDBNull(1) ? reader.GetInt64(1) : 0,
                                SkillID = !reader.IsDBNull(2) ? reader.GetInt64(2) : 0
                            };

                            if (personSkillDTO.ID <= 0)
                            {
                                __Logger?.LogWarning("Skipping record with invalid ID: {ID}", personSkillDTO.ID);
                                continue;
                            }

                            list.Add(personSkillDTO);
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

                __Logger?.LogInformation("Successfully retrieved {Count} person skill records from database.", list.Count);
                return list;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving all person skill records. Error Code: {ErrorCode}", npgsqlEx.ErrorCode);
                throw new InvalidOperationException("A database error occurred while retrieving person skill records. Please try again later.", npgsqlEx);
            }
            catch (TimeoutException timeoutEx)
            {
                __Logger?.LogError(timeoutEx, "Database operation timed out while retrieving all person skill records.");
                throw new InvalidOperationException("The database operation timed out. Please try again later.", timeoutEx);
            }
            catch (InvalidOperationException invalidOpEx)
            {
                __Logger?.LogError(invalidOpEx, "Invalid operation error: {Message}", invalidOpEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving all person skill records: {Message}", ex.Message);
                throw new Exception("An unexpected error occurred while retrieving person skill records. Please contact support.", ex);
            }
        }

        public async Task<PersonSkillDTO> getById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to retrieve person skill record with ID: {ID}", ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM getPersonSkillById(@p_id)", connection);
                command.Parameters.AddWithValue("@p_id", ID);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync())
                {
                    if (reader.HasRows && reader.FieldCount >= 3)
                    {
                        var personSkillDTO = new PersonSkillDTO
                        {
                            ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                            PersonID = !reader.IsDBNull(1) ? reader.GetInt64(1) : 0,
                            SkillID = !reader.IsDBNull(2) ? reader.GetInt64(2) : 0
                        };

                        __Logger?.LogInformation("Successfully retrieved person skill record with ID: {ID}", ID);
                        return personSkillDTO;
                    }
                }

                __Logger?.LogWarning("Person skill record with ID: {ID} not found.", ID);
                return new PersonSkillDTO();
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving person skill record with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving person skill record with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<long> addNew(PersonSkillDTO personSkill)
        {
            try
            {
                __Logger?.LogInformation("Attempting to add new person skill record.");

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

                await using var command = new NpgsqlCommand("SELECT * FROM addNewPersonSkill(@p_personid, @p_skillid)", connection);
                command.Parameters.AddWithValue("@p_personid", personSkill.PersonID);
                command.Parameters.AddWithValue("@p_skillid", personSkill.SkillID);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync() && !reader.IsDBNull(0))
                {
                    var newId = reader.GetInt64(0);
                    __Logger?.LogInformation("Successfully added new person skill record with ID: {ID}", newId);
                    return newId;
                }

                __Logger?.LogWarning("Failed to add new person skill record.");
                return 0;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while adding new person skill record.");
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while adding new person skill record.");
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> updateById(PersonSkillDTO personSkill)
        {
            try
            {
                __Logger?.LogInformation("Attempting to update person skill record with ID: {ID}", personSkill.ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM updatePersonSkillById(@p_id, @p_personid, @p_skillid)", connection);
                command.Parameters.AddWithValue("@p_id", personSkill.ID);
                command.Parameters.AddWithValue("@p_personid", personSkill.PersonID);
                command.Parameters.AddWithValue("@p_skillid", personSkill.SkillID);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully updated person skill record with ID: {ID}", personSkill.ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to update person skill record with ID: {ID}", personSkill.ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while updating person skill record with ID: {ID}", personSkill.ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while updating person skill record with ID: {ID}", personSkill.ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> deleteById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to delete person skill record with ID: {ID}", ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM deletePersonSkillById(@p_id)", connection);
                command.Parameters.AddWithValue("@p_id", ID);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully deleted person skill record with ID: {ID}", ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to delete person skill record with ID: {ID}", ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while deleting person skill record with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while deleting person skill record with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<List<PersonSkillDTO>> getByPerson(long PersonID)
        {
            var list = new List<PersonSkillDTO>();

            try
            {
                __Logger?.LogInformation("Attempting to retrieve person skill records for person with ID: {PersonID}", PersonID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM getPersonSkillsByPerson(@p_personid)", connection);
                command.Parameters.AddWithValue("@p_personid", PersonID);

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
                        if (reader.HasRows && reader.FieldCount >= 3)
                        {
                            var personSkillDTO = new PersonSkillDTO
                            {
                                ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                                PersonID = !reader.IsDBNull(1) ? reader.GetInt64(1) : 0,
                                SkillID = !reader.IsDBNull(2) ? reader.GetInt64(2) : 0
                            };

                            if (personSkillDTO.ID <= 0)
                            {
                                __Logger?.LogWarning("Skipping record with invalid ID: {ID}", personSkillDTO.ID);
                                continue;
                            }

                            list.Add(personSkillDTO);
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

                __Logger?.LogInformation("Successfully retrieved {Count} person skill records for person with ID: {PersonID}", list.Count, PersonID);
                return list;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving person skill records for person with ID: {PersonID}", PersonID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving person skill records for person with ID: {PersonID}", PersonID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
    }
}
