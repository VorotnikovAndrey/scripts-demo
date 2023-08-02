using System.Collections.Generic;
using Newtonsoft.Json;

namespace PlayVibe
{
    public class UserModel
    {
        [JsonProperty] public string CurrentLevel = "Location_1";
        [JsonProperty] public Dictionary<string, int> Resources = new Dictionary<string, int>();
    }
}