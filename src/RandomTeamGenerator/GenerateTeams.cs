using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RandomTeamGenerator
{
    public static class GenerateTeams
    {
        [FunctionName("GenerateTeams")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", Route = null)]
            HttpRequest req, ExecutionContext context, ILogger log)
        {
            log.LogInformation("GenerateTeams HTTP Trigger received a request.");
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var contacts = config["Emails"].Split(',').ToList();

            if (!int.TryParse(config["TeamCount"], out var teamCount))
                return new BadRequestObjectResult("No team count defined in configuration.");
            
            var teams = CreateTeams(contacts.OrderBy(s => Guid.NewGuid()), teamCount);
            return teams.Count != 0
                ? (ActionResult) new OkObjectResult(teams)
                : new BadRequestObjectResult("No contacts found or error creating teams.");
        }
        
        private static List<List<string>> CreateTeams(IEnumerable<string> contacts, int teamCount)
        {
            var output = new List<List<string>>();
            var contactList = contacts.ToList();
            var teams = (decimal) contactList.Count / teamCount;

            for (var i = 0; i < teams; i++)
            {
                var currentTeam = contactList.Skip(i * teamCount).Take(teamCount);
                output.Add(new List<string>(currentTeam));
            }
            
            var splitLastTwoTeamsDueToTeamSize = teams % 1 < 0.5m && teams % 1 != 0;

            return splitLastTwoTeamsDueToTeamSize ? SplitLastTeams(output, teamCount) : output;
        }

        private static List<List<string>> SplitLastTeams(List<List<string>> teams, int teamCount)
        {
            var teamOneToSplit = teams[^2];
            var teamTwoToSplit = teams[^1];
                
            teams.RemoveAt(teams.Count - 2);
            teams.RemoveAt(teams.Count - 1);
                
            var totalCount = teamCount + teamTwoToSplit.Count;
            var evenSplit = totalCount / 2;
            var remainder = totalCount % 2;
            
            var contactsToMove = teamOneToSplit.Skip(evenSplit + remainder).Take(teamCount - evenSplit).ToList();

            teamTwoToSplit.AddRange(contactsToMove);
            teamOneToSplit.RemoveRange(evenSplit + remainder, contactsToMove.Count);

            teams.Add(teamOneToSplit);
            teams.Add(teamTwoToSplit);

            return teams;
        }
    }
}