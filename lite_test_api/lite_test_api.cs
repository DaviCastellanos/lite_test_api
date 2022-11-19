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

        [FunctionName("SaveBusiness")]
        public async Task<IActionResult> SaveBusiness(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "SaveBusiness")] HttpRequest req)
        {
            BusinessItem item = new BusinessItem();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            item.Name = data ?? data?.Name;
            item.NIT = data ?? data.NIT;
            item.Phone = data ?? data.Phone;
            item.Address = data ?? data.Address;

            try
            {
                await _businessRepo.AddItemAsync(item);
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
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
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "DeleteBusinessByNIT")] HttpRequest req)
        {

            string NIT = req.Query["NIT"];

            try
            {
                var response = await _businessRepo.GetBusinessAsyncByNIT(NIT);

                await _businessRepo.DeleteItemAsync(response.First().Id);
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
            }

            return new OkObjectResult("ok");
        }
    }
}

