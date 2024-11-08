using Newtonsoft.Json;

namespace CosmosDB_Simple_API.Models
{
    public class TaskItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public required string Title { get; set; }

        public bool IsCompleted { get; set; }
    }
}
