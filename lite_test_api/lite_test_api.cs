using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using lite_test.Core;
using lite_test.Infrastructure.CosmosDbData.Repository;
using lite_test.Core.Interfaces;

namespace lite_test_api
{
    public class lite_test_api
    {
        private readonly ILogger<lite_test_api> _log;
        private readonly IBusinessRepository _businessRepo;

        public lite_test_api(ILogger<lite_test_api> log, IBusinessRepository repo)
        {
            this._log = log ?? throw new ArgumentNullException(nameof(log));
            this._businessRepo = repo;

            if (_businessRepo == null || _log == null)
            {
                log.LogError("Null dependencies");
                throw new ArgumentNullException();
            }
        }

        [FunctionName("lite_test_api")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}

