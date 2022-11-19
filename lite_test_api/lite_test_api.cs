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
using lite_test.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace lite_test_api
{
    public class lite_test_api
    {
        private readonly ILogger<lite_test_api> _log;
        private readonly BusinessRepository _businessRepo;

        public lite_test_api(ILogger<lite_test_api> log, IBusinessRepository repo)
        {
            this._log = log ?? throw new ArgumentNullException(nameof(log));
            this._businessRepo = (BusinessRepository)repo;

            if (_businessRepo == null || _log == null)
            {
                log.LogError("Null dependencies");
                throw new ArgumentNullException();
            }
        }

        [FunctionName("lite_test_api")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetBusiness")] HttpRequest req)
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

        [FunctionName("AddBusiness")]
        public async Task<IActionResult> AddBusiness(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "AddBusiness")] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                BusinessItem item = JsonConvert.DeserializeObject<BusinessItem>(requestBody);

                var response = await _businessRepo.GetBusinessAsyncByNIT(item.NIT);

                if (response.Any())
                    return new BadRequestObjectResult("NIT must be unique");
             

                await _businessRepo.AddItemAsync(item);
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                return new BadRequestObjectResult(req);
            }

            return new OkObjectResult("ok");
        }

        [FunctionName("GetBusinessByNIT")]
        public async Task<IActionResult> GetBusinessByNIT(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetBusinessByNIT")] HttpRequest req)
        {

            string NIT = req.Query["NIT"];

            try
            {
                IEnumerable<BusinessItem> response = await _businessRepo.GetBusinessAsyncByNIT(NIT);

                if (!response.Any())
                    return new NotFoundObjectResult(NIT);
                else
                    return new OkObjectResult(response.First());
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                return new BadRequestObjectResult(e.Message);
            }
        }

        [FunctionName("DeleteBusinessByNIT")]
        public async Task<IActionResult> DeleteBusinessByNIT(
           [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteBusinessByNIT")] HttpRequest req)
        {

            string NIT = req.Query["NIT"];

            try
            {
                var response = await _businessRepo.GetBusinessAsyncByNIT(NIT);

                if (!response.Any())
                    return new NotFoundObjectResult(NIT);

                await _businessRepo.DeleteItemAsync(response.First().Id);
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
                return new BadRequestObjectResult(e);
            }

            return new OkObjectResult("ok");
        }

        [FunctionName("UpdateBusinessByNIT")]
        public async Task<IActionResult> UpdateBusinessByNIT(
           [HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateBusinessByNIT")] HttpRequest req)
        {

            string NIT = req.Query["NIT"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string[] args = requestBody.Split('=');

            try
            {
                var response = await _businessRepo.GetBusinessAsyncByNIT(NIT);

                if (!response.Any())
                    return new NotFoundObjectResult(NIT);

                BusinessItem item = response.First();

                item.GetType().GetProperty(args[0]).SetValue(item, args[1], null);

                await _businessRepo.UpdateItemAsync(item);
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
            }

            return new OkObjectResult("ok");
        }
    }
}

