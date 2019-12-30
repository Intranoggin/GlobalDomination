using Database.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Database.Seed
{
    public class DataSeeder
    {
        private const string FacilitiesSeedFileName = "FacilitiesSeed.json";
        private const string FacilityTypesSeedFileName = "FacilityTypesSeed.json";
        private static string SEEDDATAPATH = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Seed{Path.DirectorySeparatorChar}Contents{Path.DirectorySeparatorChar}InitializeDatabase{Path.DirectorySeparatorChar}";

        public static List<Facilities> InitializeDatabaseFacilities()
        {
            List<Facilities> facilities = JsonSerializer.Deserialize<List<Facilities>>(ReadSeedData(FacilitiesSeedFileName));
            return facilities;
        }
        public static List<FacilityTypes> InitializeDatabaseFacilityTypes()
        {
            List<FacilityTypes> facilityTypes = JsonSerializer.Deserialize<List<FacilityTypes>>(ReadSeedData(FacilityTypesSeedFileName));
            return facilityTypes;
        }

        private static string ReadSeedData(string filename)
        {
            return File.ReadAllText($"{SEEDDATAPATH}{filename}");
        }
    }
}
