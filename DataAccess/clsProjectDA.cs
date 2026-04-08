using Npgsql;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using DataAccess;

namespace Portfolio.DataAccess
{
    public class ProjectDTO
    {
        public long ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string GitHubUrl { get; set; } = string.Empty;
        public string LiveUrl { get; set; } = string.Empty;
        public long PersonID { get; set; }
    }

    public interface IProject : IRepository<ProjectDTO>
    {
        Task<List<ProjectDTO>> getByPerson(long PersonID);
    }

    public class clsProjectDA : IProject
    {
        private readonly NpgsqlDataSource __DataSource;
        private readonly ILogger<clsProjectDA>? __Logger;

        public clsProjectDA(NpgsqlDataSource DataSource, ILogger<clsProjectDA>? Logger = null)
        {
            __DataSource = DataSource ?? throw new ArgumentNullException(nameof(DataSource), "DataSource cannot be null.");
            __Logger = Logger;
        }

        public async Task<List<ProjectDTO>> getAll()
        {
            var list = new List<ProjectDTO>();

            try
            {
                __Logger?.LogInformation("Attempting to retrieve all project records from database.");

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

                await using var command = new NpgsqlCommand("SELECT * FROM getAllProjects()", connection);

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
                        if (reader.HasRows && reader.FieldCount >= 8)
                        {
                            var projectDTO = new ProjectDTO
                            {
                                ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                                Title = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                                Description = !reader.IsDBNull(2) ? (reader.GetString(2) ?? string.Empty) : string.Empty,
                                Details = !reader.IsDBNull(3) ? (reader.GetString(3) ?? string.Empty) : string.Empty,
                                ImageUrl = !reader.IsDBNull(4) ? (reader.GetString(4) ?? string.Empty) : string.Empty,
                                GitHubUrl = !reader.IsDBNull(5) ? (reader.GetString(5) ?? string.Empty) : string.Empty,
                                LiveUrl = !reader.IsDBNull(6) ? (reader.GetString(6) ?? string.Empty) : string.Empty,
                                PersonID = !reader.IsDBNull(7) ? reader.GetInt64(7) : 0
                            };

                            if (projectDTO.ID <= 0)
                            {
                                __Logger?.LogWarning("Skipping record with invalid ID: {ID}", projectDTO.ID);
                                continue;
                            }

                            list.Add(projectDTO);
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

                __Logger?.LogInformation("Successfully retrieved {Count} project records from database.", list.Count);
                return list;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving all project records. Error Code: {ErrorCode}", npgsqlEx.ErrorCode);
                throw new InvalidOperationException("A database error occurred while retrieving project records. Please try again later.", npgsqlEx);
            }
            catch (TimeoutException timeoutEx)
            {
                __Logger?.LogError(timeoutEx, "Database operation timed out while retrieving all project records.");
                throw new InvalidOperationException("The database operation timed out. Please try again later.", timeoutEx);
            }
            catch (InvalidOperationException invalidOpEx)
            {
                __Logger?.LogError(invalidOpEx, "Invalid operation error: {Message}", invalidOpEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving all project records: {Message}", ex.Message);
                throw new Exception("An unexpected error occurred while retrieving project records. Please contact support.", ex);
            }
        }

        public async Task<ProjectDTO> getById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to retrieve project record with ID: {ID}", ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM getProjectById(@p_id)", connection);
                command.Parameters.AddWithValue("@p_id", ID);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync())
                {
                    if (reader.HasRows && reader.FieldCount >= 8)
                    {
                        var projectDTO = new ProjectDTO
                        {
                            ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                            Title = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                            Description = !reader.IsDBNull(2) ? (reader.GetString(2) ?? string.Empty) : string.Empty,
                            Details = !reader.IsDBNull(3) ? (reader.GetString(3) ?? string.Empty) : string.Empty,
                            ImageUrl = !reader.IsDBNull(4) ? (reader.GetString(4) ?? string.Empty) : string.Empty,
                            GitHubUrl = !reader.IsDBNull(5) ? (reader.GetString(5) ?? string.Empty) : string.Empty,
                            LiveUrl = !reader.IsDBNull(6) ? (reader.GetString(6) ?? string.Empty) : string.Empty,
                            PersonID = !reader.IsDBNull(7) ? reader.GetInt64(7) : 0
                        };

                        __Logger?.LogInformation("Successfully retrieved project record with ID: {ID}", ID);
                        return projectDTO;
                    }
                }

                __Logger?.LogWarning("Project record with ID: {ID} not found.", ID);
                return new ProjectDTO();
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving project record with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving project record with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<long> addNew(ProjectDTO project)
        {
            try
            {
                __Logger?.LogInformation("Attempting to add new project record.");

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

                await using var command = new NpgsqlCommand("SELECT * FROM addNewProject(@p_title, @p_description, @p_details, @p_imageurl, @p_githuburl, @p_liveurl, @p_personid)", connection);
                command.Parameters.AddWithValue("@p_title", project.Title ?? string.Empty);
                command.Parameters.AddWithValue("@p_description", project.Description ?? string.Empty);
                command.Parameters.AddWithValue("@p_details", project.Details ?? string.Empty);
                command.Parameters.AddWithValue("@p_imageurl", project.ImageUrl ?? string.Empty);
                command.Parameters.AddWithValue("@p_githuburl", project.GitHubUrl ?? string.Empty);
                command.Parameters.AddWithValue("@p_liveurl", project.LiveUrl ?? string.Empty);
                command.Parameters.AddWithValue("@p_personid", project.PersonID);

                await using var reader = await command.ExecuteReaderAsync();

                if (reader == null)
                {
                    __Logger?.LogError("Failed to execute query. Reader is null.");
                    throw new InvalidOperationException("Failed to execute database query.");
                }

                if (await reader.ReadAsync() && !reader.IsDBNull(0))
                {
                    var newId = reader.GetInt64(0);
                    __Logger?.LogInformation("Successfully added new project record with ID: {ID}", newId);
                    return newId;
                }

                __Logger?.LogWarning("Failed to add new project record.");
                return 0;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while adding new project record.");
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while adding new project record.");
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> updateById(ProjectDTO project)
        {
            try
            {
                __Logger?.LogInformation("Attempting to update project record with ID: {ID}", project.ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM updateProjectById(@p_id, @p_title, @p_description, @p_details, @p_imageurl, @p_githuburl, @p_liveurl, @p_personid)", connection);
                command.Parameters.AddWithValue("@p_id", project.ID);
                command.Parameters.AddWithValue("@p_title", project.Title ?? string.Empty);
                command.Parameters.AddWithValue("@p_description", project.Description ?? string.Empty);
                command.Parameters.AddWithValue("@p_details", project.Details ?? string.Empty);
                command.Parameters.AddWithValue("@p_imageurl", project.ImageUrl ?? string.Empty);
                command.Parameters.AddWithValue("@p_githuburl", project.GitHubUrl ?? string.Empty);
                command.Parameters.AddWithValue("@p_liveurl", project.LiveUrl ?? string.Empty);
                command.Parameters.AddWithValue("@p_personid", project.PersonID);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully updated project record with ID: {ID}", project.ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to update project record with ID: {ID}", project.ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while updating project record with ID: {ID}", project.ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while updating project record with ID: {ID}", project.ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<bool> deleteById(long ID)
        {
            try
            {
                __Logger?.LogInformation("Attempting to delete project record with ID: {ID}", ID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM deleteProjectById(@p_id)", connection);
                command.Parameters.AddWithValue("@p_id", ID);

                var result = await command.ExecuteScalarAsync();

                if (result != null && result is bool boolResult)
                {
                    __Logger?.LogInformation("Successfully deleted project record with ID: {ID}", ID);
                    return boolResult;
                }

                __Logger?.LogWarning("Failed to delete project record with ID: {ID}", ID);
                return false;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while deleting project record with ID: {ID}", ID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while deleting project record with ID: {ID}", ID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<List<ProjectDTO>> getByPerson(long PersonID)
        {
            var list = new List<ProjectDTO>();

            try
            {
                __Logger?.LogInformation("Attempting to retrieve project records for person with ID: {PersonID}", PersonID);

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

                await using var command = new NpgsqlCommand("SELECT * FROM getProjectsByPerson(@p_personid)", connection);
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
                        if (reader.HasRows && reader.FieldCount >= 8)
                        {
                            var projectDTO = new ProjectDTO
                            {
                                ID = !reader.IsDBNull(0) ? reader.GetInt64(0) : 0,
                                Title = !reader.IsDBNull(1) ? (reader.GetString(1) ?? string.Empty) : string.Empty,
                                Description = !reader.IsDBNull(2) ? (reader.GetString(2) ?? string.Empty) : string.Empty,
                                Details = !reader.IsDBNull(3) ? (reader.GetString(3) ?? string.Empty) : string.Empty,
                                ImageUrl = !reader.IsDBNull(4) ? (reader.GetString(4) ?? string.Empty) : string.Empty,
                                GitHubUrl = !reader.IsDBNull(5) ? (reader.GetString(5) ?? string.Empty) : string.Empty,
                                LiveUrl = !reader.IsDBNull(6) ? (reader.GetString(6) ?? string.Empty) : string.Empty,
                                PersonID = !reader.IsDBNull(7) ? reader.GetInt64(7) : 0
                            };

                            if (projectDTO.ID <= 0)
                            {
                                __Logger?.LogWarning("Skipping record with invalid ID: {ID}", projectDTO.ID);
                                continue;
                            }

                            list.Add(projectDTO);
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

                __Logger?.LogInformation("Successfully retrieved {Count} project records for person with ID: {PersonID}", list.Count, PersonID);
                return list;
            }
            catch (NpgsqlException npgsqlEx)
            {
                __Logger?.LogError(npgsqlEx, "PostgreSQL error occurred while retrieving project records for person with ID: {PersonID}", PersonID);
                throw new InvalidOperationException("A database error occurred.", npgsqlEx);
            }
            catch (Exception ex)
            {
                __Logger?.LogError(ex, "Unexpected error occurred while retrieving project records for person with ID: {PersonID}", PersonID);
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
    }
}
