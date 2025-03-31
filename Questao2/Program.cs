using Newtonsoft.Json;
using Questao2;

public class Program
{
    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("https://jsonmock.hackerrank.com/api/football_matches");
        int currentPage = 1;
        int lastPage = int.MaxValue;
        int teamGoals = 0;
        int teamNumber = 1;

        string queryParams = $"?year={year}";
        while (currentPage <= lastPage)
        {
            var response = client.Send(new HttpRequestMessage(HttpMethod.Get, queryParams + $"&team{teamNumber}={team}&page={currentPage}"));
            var jsonResult = response.Content.ReadAsStringAsync().Result;

            var footballMatches = JsonConvert.DeserializeObject<FootballMatchesResult>(jsonResult);
            lastPage = footballMatches.Total_pages;

            if (teamNumber == 1)
                teamGoals += footballMatches.Data.Sum(x => x.Team1goals);
            else
                teamGoals += footballMatches.Data.Sum(x => x.Team2goals);

            currentPage++;
            if (currentPage > lastPage && teamNumber == 1)
            {
                teamNumber = 2;
                currentPage = 1;
                lastPage = int.MaxValue;
            }
        }

        return teamGoals;
    }
}