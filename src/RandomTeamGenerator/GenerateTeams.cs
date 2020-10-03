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
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req, ExecutionContext context, ILogger log)
        {
            log.LogInformation("GenerateTeams HTTP Trigger received a request.");
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var contacts = config["Emails"].Split(',').ToList();
            var teamCount = int.Parse(config["TeamCount"]);
            
            var teams = CreateTeams(contacts.OrderBy(s => Guid.NewGuid()), teamCount);
            return teams.Count != 0
                ? (ActionResult) new OkObjectResult(teams)
                : new BadRequestObjectResult("No contacts found.");
        }
        
        private static List<List<string>> CreateTeams(IEnumerable<string> contacts, int teamCount)
        {
            var output = new List<List<string>>();
            var contactList = contacts.ToList();
            var teams = Math.Round((decimal) contactList.Count / teamCount);

            for (var i = 0; i < teams; i++)
            {
                var c = contactList.Skip(i * teamCount).Take(teamCount);
                var team = new List<string>();
                team.AddRange(c);
                output.Add(team);
            }

            return output;
        }
    }
}