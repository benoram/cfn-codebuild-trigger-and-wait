using Newtonsoft.Json.Linq;

namespace OramCo.CloudFormation.CustomResource.CodeBuild
{
    public class CustomResourceProperties
    {
        //Note that ServiceToken is the only mandatory field for this, but you can pass whatever you like from the CFN template
        public string ServiceToken {get;set;}
        public string CodeBuildProjectName { get;set;}
    }
}