using Npgsql;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using DataAccess;

namespace Portfolio.DataAccess
{
    public class AboutDTO
    {
        public long ID { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string FocusArea { get; set; } = string.Empty;
        public string Highlighted { get; set; } = string.Empty;
        public long PersonID { get; set; }
    }

    public interface IAbout : IRepository<AboutDTO>
    {
        Task<AboutDTO> getByPerson(long PersonID);
    }

    public class clsAboutDA : IAbout
    {
        private readonly NpgsqlDataSource __DataSource;
        private readonly ILogger<clsAboutDA>? __Logger;

        public clsAboutDA(NpgsqlDataSource DataSource, ILogger<clsAboutDA>? Logger = null)
        {
            __DataSource = DataSource ?? throw new ArgumentNullException(nameof(DataSource), "DataSource cannot be null.");
            __Logger = Logger;
        }

        public async Task<List<AboutDTO>> getAll()
        {
            var list = new List<AboutDTO>();

            try
            {
                __Logger?.LogInformation("Attempting to retrieve all about records from database.");

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

                await using var command = new NpgsqlCommand("SELECT * FROM getAllAbout()", connection);

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
                            var aboutDTO = new AboutDTO
                            {
                                ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                                Summary = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                                FocusArea = !reader.IsDBNull(2) ? (reader.GetString(2) ?? string.Empty) : string.Empty,
                                Highlighted = !reader.IsDBNull(3) ? (reader.GetString(3) ?? string.Empty) : string.Empty,
                                PersonID = !reader.IsDBNull(4) ? reader.GetInt64(4) : 0
                            };

                            if (aboutDTO.ID <= 0)
                            {
                                __Logger?.LogWarning("Skipping record with invalid ID: {ID}", aboutDTO.ID);
                                continue;
                            }

                            list.Add(aboutDTO);
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

                __Logger?.LogInformation("Successfully retrieved {Count} about records from database.", list.Count);
                return list;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving all about records. Error Code: {ErrorCode}", npgsqlEx.ErrorCode);
                throw new InvalidOperationException("A database error occurred while retrieving about records. Please try again later.", npgsqlEx);
            }
            catch (TimeoutException timeoutEx)
            {
                __Logger?.LogError(timeoutEx, "Database operation timed out while retrieving all about records.");
                throw new InvalidOperationException("The database operation timed out. Please try again later.", timeoutEx);
            }
            catch (InvalidOperationException invalidOpEx)
            {
                __Logger?.LogError(invalidOpEx, "Invalid operation error: {Message}", invalidOpEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving all about records: {Message}", ex.Message);
                throw new Exception("An unexpected error occurred while retrieving about records. Please contact support.", ex);
            }
        }

        public async Task<AboutDTO> getById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to retrieve about record with ID: {ID}", ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM getAboutById(@p_id)", connection);
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
                        var aboutDTO = new AboutDTO
                        {
                            ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                            Summary = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                            FocusArea = !reader.IsDBNull(2) ? (reader.GetString(2) ?? string.Empty) : string.Empty,
                            Highlighted = !reader.IsDBNull(3) ? (reader.GetString(3) ?? string.Empty) : string.Empty,
                            PersonID = !reader.IsDBNull(4) ? reader.GetInt64(4) : 0
                        };

                        __Logger?.LogInformation("Successfully retrieved about record with ID: {ID}", ID);
                        return aboutDTO;
                    }
                }

                __Logger?.LogWarning("About record with ID: {ID} not found.", ID);
                return new AboutDTO();
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving about record with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving about record with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<long> addNew(AboutDTO about)
        {
            try
            {
                __Logger?.LogInformation("Attempting to add new about record.");

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

                await using var command = new NpgsqlCommand("SELECT * FROM addNewAbout(@p_summary, @p_focusarea, @p_highlighted, @p_personid)", connection);
                command.Parameters.AddWithValue("@p_summary", about.Summary ?? string.Empty);
                command.Parameters.AddWithValue("@p_focusarea", about.FocusArea ?? string.Empty);
                command.Parameters.AddWithValue("@p_highlighted", about.Highlighted ?? string.Empty);
                command.Parameters.AddWithValue("@p_personid", about.PersonID);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync() && !reader.IsDBNull(0))
                {
                    var newId = reader.GetInt64(0);
                    __Logger?.LogInformation("Successfully added new about record with ID: {ID}", newId);
                    return newId;
                }

                __Logger?.LogWarning("Failed to add new about record.");
                return 0;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while adding new about record.");
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while adding new about record.");
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> updateById(AboutDTO about)
        {
            try
            {
                __Logger?.LogInformation("Attempting to update about record with ID: {ID}", about.ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM updateAboutById(@p_id, @p_summary, @p_focusarea, @p_highlighted, @p_personid)", connection);
                command.Parameters.AddWithValue("@p_id", about.ID);
                command.Parameters.AddWithValue("@p_summary", about.Summary ?? string.Empty);
                command.Parameters.AddWithValue("@p_focusarea", about.FocusArea ?? string.Empty);
                command.Parameters.AddWithValue("@p_highlighted", about.Highlighted ?? string.Empty);
                command.Parameters.AddWithValue("@p_personid", about.PersonID);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully updated about record with ID: {ID}", about.ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to update about record with ID: {ID}", about.ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while updating about record with ID: {ID}", about.ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while updating about record with ID: {ID}", about.ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> deleteById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to delete about record with ID: {ID}", ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM deleteAboutById(@p_id)", connection);
                command.Parameters.AddWithValue("@p_id", ID);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully deleted about record with ID: {ID}", ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to delete about record with ID: {ID}", ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while deleting about record with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while deleting about record with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<AboutDTO> getByPerson(long PersonID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to retrieve about record for person with ID: {PersonID}", PersonID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM getAboutByPerson(@p_personid)", connection);
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
                        var aboutDTO = new AboutDTO
                        {
                            ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                            Summary = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                            FocusArea = !reader.IsDBNull(2) ? (reader.GetString(2) ?? string.Empty) : string.Empty,
                            Highlighted = !reader.IsDBNull(3) ? (reader.GetString(3) ?? string.Empty) : string.Empty,
                            PersonID = !reader.IsDBNull(4) ? reader.GetInt64(4) : 0
                        };

                        __Logger?.LogInformation("Successfully retrieved about record for person with ID: {PersonID}", PersonID);
                        return aboutDTO;
                    }
                }

                __Logger?.LogWarning("About record for person with ID: {PersonID} not found.", PersonID);
                return new AboutDTO();
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving about record for person with ID: {PersonID}", PersonID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving about record for person with ID: {PersonID}", PersonID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
    }
}
