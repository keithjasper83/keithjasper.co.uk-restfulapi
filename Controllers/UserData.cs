using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace keithjasper.co.uk_restfulapi.Controllers
{
    /// <summary>
    /// Controller for managing user data through RESTful API endpoints.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private static List<User> users;                            // List to store user data
        private static int nextId = 1;                              // Auto-incrementing ID for new users
        private static readonly string dataFilePath = "users.json"; // Path to the JSON data file

        // List of valid fields for the User object
        private static readonly List<string> validFields =
            typeof(User).GetProperties().Select(property => property.Name).ToList();

        /// <summary>
        /// Constructor for the UsersController class.
        /// </summary>
        public UsersController()
        {
            LoadDataFromFile(); // Load user data from file when the controller is initialized
        }

        /// <summary>
        /// Load user data from a JSON file.
        /// </summary>
        private void LoadDataFromFile()
        {
            if (System.IO.File.Exists(dataFilePath))
            {
                string json = System.IO.File.ReadAllText(dataFilePath);
                users = JsonConvert.DeserializeObject<List<User>>(json);
                nextId = users.Max(u => u.Id) + 1; // Get the next available ID
            }
            else
            {
                users = new List<User>(); // Create a new list if the file doesn't exist
            }
        }

        /// <summary>
        /// Save user data to the JSON file.
        /// </summary>
        private void SaveDataToFile()
        {
            string json = JsonConvert.SerializeObject(users);
            System.IO.File.WriteAllText(dataFilePath, json);
        }

        /// <summary>
        /// Retrieve a list of all users.
        /// </summary>
        /// <returns>List of users stored in the API.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(users);
        }

        /// <summary>
        /// Delete a user from the API based on its ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>HTTP status code indicating the result of the operation.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = users.Find(u => u.Id == id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            users.Remove(user);
            SaveDataToFile();
            return Ok();
        }

        /// <summary>
        /// Add a new user to the API.
        /// </summary>
        /// <param name="user">The user object to add.</param>
        /// <returns>HTTP status code indicating the result of the operation.</returns>
        [HttpPost]
        public IActionResult AddUser(User user)
        {
            // Check if the maximum number of users has been reached
            if (users.Count >= 100)
            {
                return BadRequest("Maximum number of users reached.");
            }

            // Perform model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate incoming user object against valid fields
            var userProperties = typeof(User).GetProperties().Select(p => p.Name);
            if (userProperties.Except(validFields).Any())
            {
                return BadRequest("Invalid fields provided.");
            }

            // Assign a unique ID to the new user and add it to the list
            user.Id = nextId++;
            users.Add(user);
            SaveDataToFile();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        /// <summary>
        /// Retrieve a user from the API based on its ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user object if found, else NotFound.</returns>
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = users.Find(u => u.Id == id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Update an existing user in the API.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="updatedUser">The updated user object.</param>
        /// <returns>HTTP status code indicating the result of the operation.</returns>
        [HttpPut("{id}")]
        public IActionResult EditUser(int id, User updatedUser)
        {
            var userToUpdate = users.FirstOrDefault(u => u.Id == id);
            if (userToUpdate == null)
            {
                return NotFound("User not found");
            }

            // Perform model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate incoming user object against valid fields
            var userProperties = typeof(User).GetProperties().Select(p => p.Name);
            if (updatedUser.GetType().GetProperties().Select(p => p.Name).Except(userProperties).Any())
            {
                return BadRequest("Invalid fields provided.");
            }

            // Update user properties
            userToUpdate.Name = updatedUser.Name;
            userToUpdate.Email = updatedUser.Email;
            userToUpdate.Address = updatedUser.Address;

            SaveDataToFile();
            return Ok(userToUpdate);
        }
    }

    /// <summary>
    /// Defines a user object for the RESTful API.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the user.
        /// </summary>
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        /// <summary>
        /// Email address of the user.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        /// <summary>
        /// Address of the user.
        /// </summary>
        public string Address { get; set; }
    }
}
