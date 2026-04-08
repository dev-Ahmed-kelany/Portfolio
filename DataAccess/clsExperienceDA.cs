using Npgsql;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using DataAccess;

namespace Portfolio.DataAccess
{
    public class ExperienceDTO
    {
        public long ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long PersonID { get; set; }
    }

    public interface IExperience : IRepository<ExperienceDTO>
    {
        Task<List<ExperienceDTO>> getByPerson(long PersonID);
    }

    public class clsExperienceDA : IExperience
    {
        private readonly NpgsqlDataSource __DataSource;
        private readonly ILogger<clsExperienceDA>? __Logger;

        public clsExperienceDA(NpgsqlDataSource DataSource, ILogger<clsExperienceDA>? Logger = null)
        {
            __DataSource = DataSource ?? throw new ArgumentNullException(nameof(DataSource), "DataSource cannot be null.");
            __Logger = Logger;
        }

        public async Task<List<ExperienceDTO>> getAll()
        {
            var list = new List<ExperienceDTO>();

            try
            {
                __Logger?.LogInformation("Attempting to retrieve all experience records from database.");

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

                await using var command = new NpgsqlCommand("SELECT * FROM getAllExperiences()", connection);

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
                        if (reader.HasRows && reader.FieldCount >= 7)
                        {
                            var experienceDTO = new ExperienceDTO
                            {
                                ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                                Name = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                                Description = !reader.IsDBNull(2) ? (reader.GetString(2) ?? string.Empty) : string.Empty,
                                CompanyName = !reader.IsDBNull(3) ? (reader.GetString(3) ?? string.Empty) : string.Empty,
                                StartDate = !reader.IsDBNull(4) ? reader.GetDateTime(4) : DateTime.MinValue,
                                EndDate = !reader.IsDBNull(5) ? reader.GetDateTime(5) : null,
                                PersonID = !reader.IsDBNull(6) ? reader.GetInt64(6) : 0
                            };

                            if (experienceDTO.ID <= 0)
                            {
                                __Logger?.LogWarning("Skipping record with invalid ID: {ID}", experienceDTO.ID);
                                continue;
                            }

                            list.Add(experienceDTO);
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

                __Logger?.LogInformation("Successfully retrieved {Count} experience records from database.", list.Count);
                return list;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving all experience records. Error Code: {ErrorCode}", npgsqlEx.ErrorCode);
                throw new InvalidOperationException("A database error occurred while retrieving experience records. Please try again later.", npgsqlEx);
            }
            catch (TimeoutException timeoutEx)
            {
                __Logger?.LogError(timeoutEx, "Database operation timed out while retrieving all experience records.");
                throw new InvalidOperationException("The database operation timed out. Please try again later.", timeoutEx);
            }
            catch (InvalidOperationException invalidOpEx)
            {
                __Logger?.LogError(invalidOpEx, "Invalid operation error: {Message}", invalidOpEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving all experience records: {Message}", ex.Message);
                throw new Exception("An unexpected error occurred while retrieving experience records. Please contact support.", ex);
            }
        }

        public async Task<ExperienceDTO> getById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to retrieve experience record with ID: {ID}", ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM getExperienceById(@p_id)", connection);
                command.Parameters.AddWithValue("@p_id", ID);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync())
                {
                    if (reader.HasRows && reader.FieldCount >= 7)
                    {
                        var experienceDTO = new ExperienceDTO
                        {
                            ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                            Name = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                            Description = !reader.IsDBNull(2) ? (reader.GetString(2) ?? string.Empty) : string.Empty,
                            CompanyName = !reader.IsDBNull(3) ? (reader.GetString(3) ?? string.Empty) : string.Empty,
                            StartDate = !reader.IsDBNull(4) ? reader.GetDateTime(4) : DateTime.MinValue,
                            EndDate = !reader.IsDBNull(5) ? reader.GetDateTime(5) : null,
                            PersonID = !reader.IsDBNull(6) ? reader.GetInt64(6) : 0
                        };

                        __Logger?.LogInformation("Successfully retrieved experience record with ID: {ID}", ID);
                        return experienceDTO;
                    }
                }

                __Logger?.LogWarning("Experience record with ID: {ID} not found.", ID);
                return new ExperienceDTO();
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving experience record with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving experience record with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<long> addNew(ExperienceDTO experience)
        {
            try
            {
                __Logger?.LogInformation("Attempting to add new experience record.");

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

                await using var command = new NpgsqlCommand("SELECT * FROM addNewExperience(@p_name, @p_description, @p_companyname, @p_startdate, @p_enddate, @p_personid)", connection);
                command.Parameters.AddWithValue("@p_name", experience.Name ?? string.Empty);
                command.Parameters.AddWithValue("@p_description", experience.Description ?? string.Empty);
                command.Parameters.AddWithValue("@p_companyname", experience.CompanyName ?? string.Empty);
                command.Parameters.AddWithValue("@p_startdate", experience.StartDate);
                command.Parameters.AddWithValue("@p_enddate", experience.EndDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@p_personid", experience.PersonID);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync() && !reader.IsDBNull(0))
                {
                    var newId = reader.GetInt64(0);
                    __Logger?.LogInformation("Successfully added new experience record with ID: {ID}", newId);
                    return newId;
                }

                __Logger?.LogWarning("Failed to add new experience record.");
                return 0;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while adding new experience record.");
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while adding new experience record.");
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> updateById(ExperienceDTO experience)
        {
            try
            {
                __Logger?.LogInformation("Attempting to update experience record with ID: {ID}", experience.ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM updateExperienceById(@p_id, @p_name, @p_description, @p_companyname, @p_startdate, @p_enddate, @p_personid)", connection);
                command.Parameters.AddWithValue("@p_id", experience.ID);
                command.Parameters.AddWithValue("@p_name", experience.Name ?? string.Empty);
                command.Parameters.AddWithValue("@p_description", experience.Description ?? string.Empty);
                command.Parameters.AddWithValue("@p_companyname", experience.CompanyName ?? string.Empty);
                command.Parameters.AddWithValue("@p_startdate", experience.StartDate);
                command.Parameters.AddWithValue("@p_enddate", experience.EndDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@p_personid", experience.PersonID);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully updated experience record with ID: {ID}", experience.ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to update experience record with ID: {ID}", experience.ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while updating experience record with ID: {ID}", experience.ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while updating experience record with ID: {ID}", experience.ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> deleteById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to delete experience record with ID: {ID}", ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM deleteExperienceById(@p_id)", connection);
                command.Parameters.AddWithValue("@p_id", ID);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully deleted experience record with ID: {ID}", ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to delete experience record with ID: {ID}", ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while deleting experience record with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while deleting experience record with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<List<ExperienceDTO>> getByPerson(long PersonID)
        {
            var list = new List<ExperienceDTO>();

            try
            {
                __Logger?.LogInformation("Attempting to retrieve experience records for person with ID: {PersonID}", PersonID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM getExperiencesByPerson(@p_personid)", connection);
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
                        if (reader.HasRows && reader.FieldCount >= 7)
                        {
                            var experienceDTO = new ExperienceDTO
                            {
                                ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                                Name = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                                Description = !reader.IsDBNull(2) ? (reader.GetString(2) ?? string.Empty) : string.Empty,
                                CompanyName = !reader.IsDBNull(3) ? (reader.GetString(3) ?? string.Empty) : string.Empty,
                                StartDate = !reader.IsDBNull(4) ? reader.GetDateTime(4) : DateTime.MinValue,
                                EndDate = !reader.IsDBNull(5) ? reader.GetDateTime(5) : null,
                                PersonID = !reader.IsDBNull(6) ? reader.GetInt64(6) : 0
                            };

                            if (experienceDTO.ID <= 0)
                            {
                                __Logger?.LogWarning("Skipping record with invalid ID: {ID}", experienceDTO.ID);
                                continue;
                            }

                            list.Add(experienceDTO);
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

                __Logger?.LogInformation("Successfully retrieved {Count} experience records for person with ID: {PersonID}", list.Count, PersonID);
                return list;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving experience records for person with ID: {PersonID}", PersonID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving experience records for person with ID: {PersonID}", PersonID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
    }
}
