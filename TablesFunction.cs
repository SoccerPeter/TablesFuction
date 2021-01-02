using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using SoccerResult.Models;

namespace Company.Function
{
    public static class TablesFunction
    {
        [FunctionName("TablesFunction")]
        public static void Run([TimerTrigger("0 10 9 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Tables started at: {DateTime.Now}");
            _ = runAsync();
            log.LogInformation($"Tables updated at: {DateTime.Now}");
        }

        public static async Task runAsync()
        {
            bool raderat = await DeleteTables();
            string sConn = "Server=tcp:vlqwv4swf2.database.windows.net,1433;Database=dbApp;User ID=sapjappl@vlqwv4swf2;Password=Olle8910;Trusted_Connection=False;Encrypt=True;Connection Timeout=30";

            if (raderat)
            {
                string sqlQuery = "select * from Ligor where UpdateTable = 1";

                using (SqlConnection con = new SqlConnection(sConn))
                {
                    var L = con.Query<Ligor>(sqlQuery);
                    foreach (var item in L)
                    {
                       UpdateTables(item.Id.ToString());
                    }
                }
            }
            
            
        }

        public static void UpdateTables(string Id)
        {
            string sConn = "Server=tcp:vlqwv4swf2.database.windows.net,1433;Database=dbApp;User ID=sapjappl@vlqwv4swf2;Password=Olle8910;Trusted_Connection=False;Encrypt=True;Connection Timeout=30";
            var client = new RestClient("https://api-football-v1.p.rapidapi.com/v2/leagueTable/" + Id);
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "api-football-v1.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "f20164fe94msh8c3df467990dd62p1da9d0jsn6a95b4f407b6");
            IRestResponse response = client.Execute(request);
            Temperatures myTable = JsonConvert.DeserializeObject<Temperatures>(response.Content);

            for (int i = 0; i < myTable.Api.Results; i++)
            {
                var antal = myTable.Api.Standings[i].Count;

                for (int j = 0; j < antal; j++)
                {
                    var T = myTable.Api.Standings[0][j];

                    string sqlQuery = @"INSERT INTO [dbo].[tbLeagueStandings]
                                    ([Liga]
                                    ,[Team]
                                    ,[Played]
                                    ,[PlayedAtHome]
                                    ,[PlayedAway]
                                    ,[Won]
                                    ,[Draw]
                                    ,[Lost]
                                    ,[Goals_For]
                                    ,[Goals_Against]
                                    ,[Goal_Difference]
                                    ,[Points]
                                    ,[LigID])
                                   VALUES
                                    (@Liga
                                    ,@Team
                                    ,@Played
                                    ,@PlayedAtHome
                                    ,@PlayedAway
                                    ,@Won
                                    ,@Draw
                                    ,@Lost
                                    ,@Goals_For
                                    ,@Goals_Against
                                    ,@Goal_Difference
                                    ,@Points
                                    ,@LigID)";

                    using (SqlConnection con = new SqlConnection(sConn))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                        {
                            cmd.Connection.Open();
                            cmd.Parameters.AddWithValue("@Liga", T.Group);
                            cmd.Parameters.AddWithValue("@Team", T.TeamName);
                            cmd.Parameters.AddWithValue("@Played", T.All.MatchsPlayed);
                            cmd.Parameters.AddWithValue("@PlayedAtHome", T.Home.MatchsPlayed);
                            cmd.Parameters.AddWithValue("@PlayedAway", T.Away.MatchsPlayed);
                            cmd.Parameters.AddWithValue("@Won", T.All.Win);
                            cmd.Parameters.AddWithValue("@Draw", T.All.Draw);
                            cmd.Parameters.AddWithValue("@Lost", T.All.Lose);
                            cmd.Parameters.AddWithValue("@Goals_For", T.All.GoalsFor);
                            cmd.Parameters.AddWithValue("@Goals_Against", T.All.GoalsAgainst);
                            cmd.Parameters.AddWithValue("@Goal_Difference", T.All.GoalsFor - T.All.GoalsAgainst);
                            cmd.Parameters.AddWithValue("@Points", T.Points);
                            cmd.Parameters.AddWithValue("@LigID", int.Parse(Id));
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                            cmd.Connection.Close();
                        }
                    }

                }
            }
        }

        public static async Task<bool> DeleteTables()
        {
            string sConn = "Server=tcp:vlqwv4swf2.database.windows.net,1433;Database=dbApp;User ID=sapjappl@vlqwv4swf2;Password=Olle8910;Trusted_Connection=False;Encrypt=True;Connection Timeout=30";
            bool ok = false;
            int antalRader = 0;
            using (SqlConnection con = new SqlConnection(sConn))
            {
                string sqlQuery = @"DELETE FROM tbLeagueStandings";
               using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection.Open();
                    antalRader = cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }

            if(antalRader > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }
}
