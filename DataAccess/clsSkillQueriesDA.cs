using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using DataAccess;
using Npgsql;

namespace Portfolio.DataAccess
{
    public class SkillQueryResultDTO
    {
        public long SkillID { get; set; }
        public string SkillName { get; set; } = string.Empty;
    }

    public interface ISkillQueries
    {
        Task<List<SkillQueryResultDTO>> getSkillsByPerson(long PersonID);
        Task<List<SkillQueryResultDTO>> getSkillsByProject(long ProjectID);
    }

    public class clsSkillQueriesDA : ISkillQueries
    {
        private readonly NpgsqlDataSource __DataSource;
        private readonly ILogger<clsSkillQueriesDA>? __Logger;

        public clsSkillQueriesDA(NpgsqlDataSource DataSource, ILogger<clsSkillQueriesDA>? Logger = null)
        {
            __DataSource = DataSource ?? throw new ArgumentNullException(nameof(DataSource), "DataSource cannot be null.");
            __Logger = Logger;
        }

        public async Task<List<SkillQueryResultDTO>> getSkillsByPerson(long PersonID)
        {
            var list = new List<SkillQueryResultDTO>();

            try
            {
                __Logger?.LogInformation("Attempting to retrieve skills for person with ID: {PersonID}", PersonID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM getSkillsByPerson(@p_personid)", connection);
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
                        if (reader.HasRows && reader.FieldCount >= 2)
                        {
                            var skillResult = new SkillQueryResultDTO
                            {
                                SkillID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                                SkillName = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty
                            };

                            if (skillResult.SkillID <= 0)
                            {
                                __Logger?.LogWarning("Skipping record with invalid SkillID: {SkillID}", skillResult.SkillID);
                                continue;
                            }

                            list.Add(skillResult);
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

                __Logger?.LogInformation("Successfully retrieved {Count} skills for person with ID: {PersonID}", list.Count, PersonID);
                return list;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving skills for person with ID: {PersonID}", PersonID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving skills for person with ID: {PersonID}", PersonID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<List<SkillQueryResultDTO>> getSkillsByProject(long ProjectID)
        {
            var list = new List<SkillQueryResultDTO>();

            try
            {
                __Logger?.LogInformation("Attempting to retrieve skills for project with ID: {ProjectID}", ProjectID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM getSkillsByProject(@p_projectid)", connection);
                command.Parameters.AddWithValue("@p_projectid", ProjectID);

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
                        if (reader.HasRows && reader.FieldCount >= 2)
                        {
                            var skillResult = new SkillQueryResultDTO
                            {
                                SkillID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                                SkillName = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty
                            };

                            if (skillResult.SkillID <= 0)
                            {
                                __Logger?.LogWarning("Skipping record with invalid SkillID: {SkillID}", skillResult.SkillID);
                                continue;
                            }

                            list.Add(skillResult);
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

                __Logger?.LogInformation("Successfully retrieved {Count} skills for project with ID: {ProjectID}", list.Count, ProjectID);
                return list;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving skills for project with ID: {ProjectID}", ProjectID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving skills for project with ID: {ProjectID}", ProjectID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
    }
}
