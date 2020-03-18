using Newtonsoft.Json;

namespace OramCo.CloudFormation.CustomResource.CodeBuild
{
    public class CustomResourceResponse
    {
        public string Status {get;set;}
        public string Reason {get;set;}
        public string PhysicalResourceId {get;set;}
        public string StackId {get;set;}
        public string RequestId {get;set;}
        public string LogicalResourceId {get;set;}
        public bool NoEcho {get;set;}

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}