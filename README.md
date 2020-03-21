# cfn-codebuild-tigger-and-wait

This CloudFormation custom resource can be used to start the build of a codebuild project and wait for a successful build result.

This is useful when you need a build to execute within CloudFormation before using the results from that build in something else. I use it for

- Pushing CFN script to a bucket so they can be used as nested stacks later in the script.
- Pushing code to an S3 bucket so when the CloudFormation script completes my static websites are working.

## How to use

See ```./example.yaml``` for an example script that leverages the function. 

## Other notes

This project is C# and can be built with the SDK, or with an IDE like Visual Studio Community
