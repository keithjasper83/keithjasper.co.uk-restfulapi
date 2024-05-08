using Microsoft.AspNetCore.Mvc;

namespace keithjasper.co.uk_restfulapi.Controllers
{
    /// <summary>
    /// Controller for managing a counter.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CounterController : ControllerBase
    {
        private static int counter;

        private static readonly string counterFilePath = "counter.txt";

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterController"/> class.
        /// </summary>
        public CounterController()
        {
            LoadCounterFromFile();
        }

        /// <summary>
        /// Loads the counter value from a file.
        /// </summary>
        private void LoadCounterFromFile()
        {
            if (System.IO.File.Exists(counterFilePath))
            {
                string value = System.IO.File.ReadAllText(counterFilePath);
                if (int.TryParse(value, out int loadedCounter))
                {
                    counter = loadedCounter;
                }
            }
        }

        /// <summary>
        /// Saves the counter value to a file.
        /// </summary>
        private void SaveCounterToFile()
        {
            System.IO.File.WriteAllText(counterFilePath, counter.ToString());
        }

        /// <summary>
        /// Retrieves the current value of the counter.
        /// </summary>
        /// <returns>The current value of the counter.</returns>
        [HttpGet]
        public IActionResult GetCounter()
        {
            // Increment the counter and save the updated value
            counter++;
            SaveCounterToFile();
            return Ok(counter);
        }
    }
}
