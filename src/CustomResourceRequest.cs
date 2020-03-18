using Newtonsoft.Json.Linq;

namespace OramCo.CloudFormation.CustomResource.CodeBuild
{
    public class CustomResourceRequest
    {
        public string requestType {get;set;}
        public string responseUrl {get;set;}
        public string stackId {get;set;}
        public string requestId {get;set;}
        public string resourceType {get;set;}
        public string logicalResourceId {get;set;}
        public string physicalResourceId {get;set;}
        public JObject resourceProperties {get;set;}
        public string oldResourceProperties {get;set;}

    }
}