using System;
using System.Collections.Generic;
using System.Text;

namespace SoccerResult.Models
{
    // ApiTables myDeserializedClass = JsonConvert.DeserializeObject<ApiTables>(myJsonResponse); 

    public partial class Temperatures
    {
        public ApiT Api { get; set; }
    }

    public partial class ApiT
    {
        public long? Results { get; set; }
        public List<List<StandingT>> Standings { get; set; }
    }

    public partial class StandingT
    {
        public long? Rank { get; set; }
        public long? TeamId { get; set; }
        public string TeamName { get; set; }
        public Uri Logo { get; set; }
        public string Group { get; set; }
        public string Forme { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public All All { get; set; }
        public All Home { get; set; }
        public All Away { get; set; }
        public long? GoalsDiff { get; set; }
        public long? Points { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
    }

    public partial class All
    {
        public long? MatchsPlayed { get; set; }
        public long? Win { get; set; }
        public long? Draw { get; set; }
        public long? Lose { get; set; }
        public long? GoalsFor { get; set; }
        public long? GoalsAgainst { get; set; }
    }

    public enum Group { PremierLeague };

    public enum Status { Same };
}
