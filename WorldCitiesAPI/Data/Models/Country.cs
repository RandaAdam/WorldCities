using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace WorldCitiesAPI.Data.Models
{
    public class Country
    {
        #region properties
       
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        // Country code (in ISO 3166-1 ALPHA-2 format)
        [JsonPropertyName("iso2")]
        public string ISO2 { get; set; } = null!;

        // Country code (in ISO 3166-1 ALPHA-3 format)
        [JsonPropertyName("iso3")]
        public string ISO3 { get; set; } = null!;
        #endregion

        public ICollection<City>? Cities { get; set; } = null!;
    }
}
