using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Team
{
    public string Name { get; }
    public int Points { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference => GoalsFor - GoalsAgainst;
    public int MatchesPlayed { get; set; }

    public Team(string name)
    {
        Name = name;
    }

    public void RecordWin(int goalsFor, int goalsAgainst)
    {
        Points += 3;
        GoalsFor += goalsFor;
        GoalsAgainst += goalsAgainst;
        MatchesPlayed++;
    }

    public void RecordDraw(int goalsFor, int goalsAgainst)
    {
        Points += 1;
        GoalsFor += goalsFor;
        GoalsAgainst += goalsAgainst;
        MatchesPlayed++;
    }

    public void RecordLoss(int goalsFor, int goalsAgainst)
    {
        GoalsFor += goalsFor;
        GoalsAgainst += goalsAgainst;
        MatchesPlayed++;
    }
}

public class MatchResult
{
    public string HomeTeam { get; }
    public string AwayTeam { get; }
    public int HomeGoals { get; }
    public int AwayGoals { get; }

    public MatchResult(string homeTeam, string awayTeam, int homeGoals, int awayGoals)
    {
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;
        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
    }
}

public class LeagueStandings
{
    private readonly Dictionary<string, Team> _teams = new Dictionary<string, Team>();

    public LeagueStandings(IEnumerable<string> teamNames)
    {
        foreach (var name in teamNames)
        {
            _teams[name] = new Team(name);
        }
    }

    public void ProcessMatchResult(MatchResult result)
    {
        if (!_teams.ContainsKey(result.HomeTeam) || !_teams.ContainsKey(result.AwayTeam))
        {
            Console.WriteLine($"Warning: Skipping match with unknown teams ({result.HomeTeam} vs {result.AwayTeam})");
            return;
        }

        var homeTeam = _teams[result.HomeTeam];
        var awayTeam = _teams[result.AwayTeam];

        if (result.HomeGoals > result.AwayGoals)
        {
            homeTeam.RecordWin(result.HomeGoals, result.AwayGoals);
            awayTeam.RecordLoss(result.AwayGoals, result.HomeGoals);
        }
        else if (result.HomeGoals < result.AwayGoals)
        {
            awayTeam.RecordWin(result.AwayGoals, result.HomeGoals);
            homeTeam.RecordLoss(result.HomeGoals, result.AwayGoals);
        }
        else 
        {
            homeTeam.RecordDraw(result.HomeGoals, result.AwayGoals);
            awayTeam.RecordDraw(result.AwayGoals, result.HomeGoals);
        }
    }

    public void PrintStandings()
    {
        var sortedTeams = _teams.Values
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalDifference)
            .ThenBy(t => t.Name) 
            .ToList();

        Console.WriteLine("League Standings:");   
        Console.WriteLine("| Pos | Team          | Pld | Pts | GF | GA | GD |");

        for (int i = 0; i < sortedTeams.Count; i++)
        {
            var team = sortedTeams[i];
            Console.WriteLine($"| {(i + 1),3} | {team.Name,-13} | {team.MatchesPlayed,3} | {team.Points,3} | {team.GoalsFor,2} | {team.GoalsAgainst,2} | {team.GoalDifference,3} |");
        }
       
    }
}
namespace Tournament_Standings
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var teams = new List<string> { "Arsenal", "Chelsea", "Liverpool", "Man City", "Man United", "Tottenham" };

            var standings = new LeagueStandings(teams);

            var matchResults = new List<MatchResult>
        {
            new MatchResult("Arsenal", "Chelsea", 2, 1),
            new MatchResult("Man City", "Tottenham", 3, 3),
            new MatchResult("Liverpool", "Man United", 4, 0),
            new MatchResult("Chelsea", "Man City", 1, 0),
            new MatchResult("Man United", "Arsenal", 1, 1),
            new MatchResult("Tottenham", "Liverpool", 2, 2),
            new MatchResult("Arsenal", "Fake Team", 5, 0)
        };

            foreach (var match in matchResults)
            {
                standings.ProcessMatchResult(match);
            }

            standings.PrintStandings();
        }
    }
}
