using Microsoft.AspNetCore.Mvc;
using Portfolio.Business;
using Portfolio.DataAccess;

namespace PortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/Users")]
    public class UserController : Controller
    {
        private readonly clsUser __User;
        private readonly ILogger<UserController> __Logger;

        public UserController(clsUser User, ILogger<UserController> Logger)
        {
            __User = User;
            __Logger = Logger;
        }

        /// <summary>
        /// Get all users without passwords (safe for listing)
        /// </summary>
        /// <returns>List of users without password information</returns>
        /// <response code="200">Successfully retrieved users</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserWithoutPasswordDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<UserWithoutPasswordDTO>>> GetAll()
        {
            try
            {
                __Logger.LogInformation("Retrieving all users");
                var users = await __User.getAllUsers();
                __Logger.LogInformation("Successfully retrieved {Count} users", users.Count);
                return Ok(users);
            }
            catch (InvalidOperationException ex)
            {
                __Logger.LogError(ex, "Invalid operation while retrieving users");
                return StatusCode(500, new { message = "Database operation failed", error = ex.Message });
            }
            catch (Exception ex)
            {
                __Logger.LogError(ex, "Unexpected error while retrieving users");
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user by id without password (safe single user retrieval)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User information without password</returns>
        /// <response code="200">User found</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(UserWithoutPasswordDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UserWithoutPasswordDTO>> GetById(long id)
        {
            try
            {
                __Logger.LogInformation("Retrieving user with ID: {UserId}", id);

                if (id <= 0)
                {
                    __Logger.LogWarning("Invalid user ID provided: {UserId}", id);
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var user = await __User.getUserById(id);
                
                if (user == null || user.ID == 0)
                {
                    __Logger.LogWarning("User not found with ID: {UserId}", id);
                    return NotFound(new { message = "User not found" });
                }

                __Logger.LogInformation("Successfully retrieved user with ID: {UserId}", id);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                __Logger.LogError(ex, "Invalid operation while retrieving user {UserId}", id);
                return StatusCode(500, new { message = "Database operation failed", error = ex.Message });
            }
            catch (Exception ex)
            {
                __Logger.LogError(ex, "Unexpected error while retrieving user {UserId}", id);
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        /// <summary>
        /// Authenticate user with username and password
        /// </summary>
        /// <remarks>
        /// This endpoint returns the full user DTO including password hash for server-side validation.
        /// NOTE: Password should be validated server-side and should never be sent to client in production.
        /// Consider implementing JWT token response instead.
        /// </remarks>
        /// <param name="credentials">User credentials (username and password)</param>
        /// <returns>User information including password for validation</returns>
        /// <response code="200">Authentication successful</response>
        /// <response code="400">Bad request - missing credentials</response>
        /// <response code="401">Unauthorized - invalid credentials</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(UserWithoutPasswordDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UserWithoutPasswordDTO>> Authenticate([FromBody] UserCredentialsRequest credentials)
        {
            try
            {
                __Logger.LogInformation("Authentication attempt for username: {Username}", credentials?.Username);

                if (credentials == null)
                {
                    __Logger.LogWarning("Null credentials provided for authentication");
                    return BadRequest(new { message = "Credentials are required" });
                }

                if (string.IsNullOrWhiteSpace(credentials.Username) || string.IsNullOrWhiteSpace(credentials.Password))
                {
                    __Logger.LogWarning("Empty username or password provided");
                    return BadRequest(new { message = "Username and password are required" });
                }

                var user = await __User.getUserByCredentials(credentials.Username, credentials.Password);

                if (user == null || user.ID == 0)
                {
                    __Logger.LogWarning("Authentication failed for username: {Username}", credentials.Username);
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                if (!user.IsActive)
                {
                    __Logger.LogWarning("Authentication attempt for inactive user: {Username}", credentials.Username);
                    return Unauthorized(new { message = "User account is inactive" });
                }

                __Logger.LogInformation("Successful authentication for user: {UserId}", user.ID);
                
                // In production, you should return a JWT token here instead of the user object
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                __Logger.LogError(ex, "Invalid operation during authentication");
                return StatusCode(500, new { message = "Database operation failed", error = ex.Message });
            }
            catch (Exception ex)
            {
                __Logger.LogError(ex, "Unexpected error during authentication");
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new user (Admin only)
        /// </summary>
        /// <param name="dto">User data</param>
        /// <returns>Created user location</returns>
        /// <response code="201">User created successfully</response>
        /// <response code="400">Bad request - invalid data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Create([FromBody] UserDTO dto)
        {
            try
            {
                __Logger.LogInformation("Attempting to create new user: {Username}", dto?.Username);

                if (dto == null)
                {
                    __Logger.LogWarning("Null user data provided");
                    return BadRequest(new { message = "User data is required" });
                }

                if (string.IsNullOrWhiteSpace(dto.Username))
                {
                    __Logger.LogWarning("Empty username provided");
                    return BadRequest(new { message = "Username is required" });
                }

                if (string.IsNullOrWhiteSpace(dto.Password))
                {
                    __Logger.LogWarning("Empty password provided for user: {Username}", dto.Username);
                    return BadRequest(new { message = "Password is required" });
                }

                if (dto.PersonID <= 0)
                {
                    __Logger.LogWarning("Invalid PersonID provided: {PersonID}", dto.PersonID);
                    return BadRequest(new { message = "Valid PersonID is required" });
                }

                var id = await __User.addNewUser(dto);

                if (id <= 0)
                {
                    __Logger.LogError("Failed to create user: {Username}", dto.Username);
                    return StatusCode(500, new { message = "Failed to create user" });
                }

                __Logger.LogInformation("Successfully created user with ID: {UserId}", id);
                return CreatedAtAction(nameof(GetById), new { id }, null);
            }
            catch (InvalidOperationException ex)
            {
                __Logger.LogError(ex, "Invalid operation while creating user");
                return StatusCode(500, new { message = "Database operation failed", error = ex.Message });
            }
            catch (Exception ex)
            {
                __Logger.LogError(ex, "Unexpected error while creating user");
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update user (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="dto">Updated user data</param>
        /// <returns>No content</returns>
        /// <response code="204">User updated successfully</response>
        /// <response code="400">Bad request - invalid data or ID mismatch</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Update(long id, [FromBody] UserDTO dto)
        {
            try
            {
                __Logger.LogInformation("Attempting to update user with ID: {UserId}", id);

                if (id <= 0)
                {
                    __Logger.LogWarning("Invalid user ID provided: {UserId}", id);
                    return BadRequest(new { message = "Invalid user ID" });
                }

                if (dto == null)
                {
                    __Logger.LogWarning("Null user data provided for update");
                    return BadRequest(new { message = "User data is required" });
                }

                if (id != dto.ID)
                {
                    __Logger.LogWarning("ID mismatch in update request: URL ID {UrlId} vs DTO ID {DtoId}", id, dto.ID);
                    return BadRequest(new { message = "URL ID does not match user data ID" });
                }

                if (string.IsNullOrWhiteSpace(dto.Username))
                {
                    __Logger.LogWarning("Empty username provided in update");
                    return BadRequest(new { message = "Username is required" });
                }

                var ok = await __User.updateUserById(dto);

                if (!ok)
                {
                    __Logger.LogWarning("User not found for update: {UserId}", id);
                    return NotFound(new { message = "User not found" });
                }

                __Logger.LogInformation("Successfully updated user with ID: {UserId}", id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                __Logger.LogError(ex, "Invalid operation while updating user {UserId}", id);
                return StatusCode(500, new { message = "Database operation failed", error = ex.Message });
            }
            catch (Exception ex)
            {
                __Logger.LogError(ex, "Unexpected error while updating user {UserId}", id);
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete user (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>No content</returns>
        /// <response code="204">User deleted successfully</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                __Logger.LogInformation("Attempting to delete user with ID: {UserId}", id);

                if (id <= 0)
                {
                    __Logger.LogWarning("Invalid user ID provided for deletion: {UserId}", id);
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var ok = await __User.deleteUserById(id);

                if (!ok)
                {
                    __Logger.LogWarning("User not found for deletion: {UserId}", id);
                    return NotFound(new { message = "User not found" });
                }

                __Logger.LogInformation("Successfully deleted user with ID: {UserId}", id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                __Logger.LogError(ex, "Invalid operation while deleting user {UserId}", id);
                return StatusCode(500, new { message = "Database operation failed", error = ex.Message });
            }
            catch (Exception ex)
            {
                __Logger.LogError(ex, "Unexpected error while deleting user {UserId}", id);
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        /// <summary>
        /// Toggle user active status (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="isActive">New active status</param>
        /// <returns>No content</returns>
        /// <response code="204">User status updated successfully</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("{id:long}/active")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> ToggleActive(long id, [FromBody] bool isActive)
        {
            try
            {
                __Logger.LogInformation("Attempting to toggle active status for user ID: {UserId} to {IsActive}", id, isActive);

                if (id <= 0)
                {
                    __Logger.LogWarning("Invalid user ID provided: {UserId}", id);
                    return BadRequest(new { message = "Invalid user ID" });
                }

                var ok = await __User.toogleActive(id, isActive);

                if (!ok)
                {
                    __Logger.LogWarning("User not found for status toggle: {UserId}", id);
                    return NotFound(new { message = "User not found" });
                }

                __Logger.LogInformation("Successfully updated user status for ID: {UserId}", id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                __Logger.LogError(ex, "Invalid operation while toggling active status for user {UserId}", id);
                return StatusCode(500, new { message = "Database operation failed", error = ex.Message });
            }
            catch (Exception ex)
            {
                __Logger.LogError(ex, "Unexpected error while toggling active status for user {UserId}", id);
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        /// <summary>
        /// Change user password (requires authentication)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="passwordChange">Current and new password</param>
        /// <returns>No content</returns>
        /// <response code="204">Password changed successfully</response>
        /// <response code="400">Bad request - invalid data</response>
        /// <response code="401">Unauthorized - incorrect current password</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("{id:long}/change-password")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> ChangePassword(long id, [FromBody] PasswordChangeRequest passwordChange)
        {
            try
            {
                __Logger.LogInformation("Password change requested for user ID: {UserId}", id);

                if (id <= 0)
                {
                    __Logger.LogWarning("Invalid user ID provided: {UserId}", id);
                    return BadRequest(new { message = "Invalid user ID" });
                }

                if (passwordChange == null)
                {
                    __Logger.LogWarning("Null password change request");
                    return BadRequest(new { message = "Password change data is required" });
                }

                if (string.IsNullOrWhiteSpace(passwordChange.CurrentPassword) || string.IsNullOrWhiteSpace(passwordChange.NewPassword))
                {
                    __Logger.LogWarning("Empty current or new password");
                    return BadRequest(new { message = "Current password and new password are required" });
                }

                if (passwordChange.CurrentPassword == passwordChange.NewPassword)
                {
                    __Logger.LogWarning("New password same as current for user {UserId}", id);
                    return BadRequest(new { message = "New password must be different from current password" });
                }

                var ok = await __User.changePassword(id, passwordChange.CurrentPassword, passwordChange.NewPassword);

                if (!ok)
                {
                    __Logger.LogWarning("User not found for password change: {UserId}", id);
                    return NotFound(new { message = "User not found" });
                }

                __Logger.LogInformation("Successfully changed password for user ID: {UserId}", id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                __Logger.LogError(ex, "Invalid operation during password change for user {UserId}", id);
                return StatusCode(500, new { message = "Database operation failed", error = ex.Message });
            }
            catch (Exception ex)
            {
                __Logger.LogError(ex, "Unexpected error during password change for user {UserId}", id);
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }
    }

    /// <summary>
    /// Request model for user authentication
    /// </summary>
    public class UserCredentialsRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for password change
    /// </summary>
    public class PasswordChangeRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
