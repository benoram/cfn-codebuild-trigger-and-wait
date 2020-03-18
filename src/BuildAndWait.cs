using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Amazon.Lambda.Core;
using Amazon.CodeBuild.Model;
using Amazon.CodeBuild;
using System.Diagnostics;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace OramCo.CloudFormation.CustomResource.CodeBuild
{
    public class BuildAndWait
    {
        
        public async Task Execute(JObject input, ILambdaContext context)
        {
            LambdaLogger.Log($"Received input as {input.ToString()}");

            var request = input.ToObject<CustomResourceRequest>();
            var response = new CustomResourceResponse();

            // init the response
            response.StackId = request.stackId;
            response.RequestId = request.requestId;
            response.LogicalResourceId = request.logicalResourceId;
            response.NoEcho = false;
            if (string.IsNullOrEmpty(request.physicalResourceId))
                response.PhysicalResourceId = "";
            else
                response.PhysicalResourceId = request.physicalResourceId;
            response.PhysicalResourceId = "CodeBuildAndWait";
            response.Reason = "See CloudWatch logs for detail";
            response.Status = "FAILED";
            var resourceProperties = request.resourceProperties.ToObject<CustomResourceProperties>();

            switch (request.requestType.ToLower())
            {
                case "create":
                case "update":
                    var buildRequest = new StartBuildRequest();
                    buildRequest.ProjectName = resourceProperties.CodeBuildProjectName;

                    var buildClient = new AmazonCodeBuildClient();

                    LambdaLogger.Log($"Starting build: {buildRequest.ProjectName}");
                    var buildResponse = await buildClient.StartBuildAsync(buildRequest);
                    
                    if (buildResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var statusRequest = new BatchGetBuildsRequest();
                        statusRequest.Ids.Add(buildResponse.Build.Id);

                        while (true)
                        {
                            System.Threading.Thread.Sleep(5000); // Wait 5 seconds between build status checks
                            
                            var statusResponse = await buildClient.BatchGetBuildsAsync(statusRequest);                            

                            Debug.Assert(statusResponse.Builds.Count == 1);

                            LambdaLogger.Log($"Build Id: {statusResponse.Builds[0].Id}; Build Status: {statusResponse.Builds[0].BuildStatus}");
                            var buildStatus = statusResponse.Builds[0].BuildStatus;
                            if (buildStatus == StatusType.SUCCEEDED)
                            {
                                response.Status = "SUCCESS";
                                break;
                            }
                            else if (buildStatus == StatusType.IN_PROGRESS)
                            {
                                continue;
                            }
                            else
                            {
                                response.Status = "FAILED";
                                break;
                            }
                            
                        }
                    }
                    else
                    {
                        LambdaLogger.Log($"StartBuild Failed: {buildResponse.HttpStatusCode}: {buildResponse}");

                    }                    
                    break;
                case "delete":
                    LambdaLogger.Log("Received DELETE request");
                    response.Status="SUCCESS";
                    break;
            }

            context.Logger.LogLine($"Calling CloudFormation callback; status: {response.Status}");
            var http = new HttpClient();
            var httpResponse = await http.PutAsJsonAsync(request.responseUrl, response.ToJson());

            context.Logger.LogLine($"Callback Response: {httpResponse.StatusCode}");

            await Task.CompletedTask;
        }        
    }

    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PutAsJsonAsync(
            this HttpClient httpClient, string url, string json)
        {
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PutAsync(url, content);
        }
    }
}
