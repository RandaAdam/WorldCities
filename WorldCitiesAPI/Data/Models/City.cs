using Microsoft.EntityFrameworkCore;

namespace WorldCitiesAPI.Data.Models
{
    public class City
    {
        #region properties
      
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Lat { get; set; }

        public decimal Lon { get; set; }

        //country id foreign key
        public int CountryId { get; set; }
        #endregion

        public Country? Country { get; set; }= null!;

    }
}
