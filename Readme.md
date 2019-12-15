# Creating a new project based on this Boilerplate

## Environment Setup

* Install NetCore 2
* Install VisualStudio 2017
* Install AWS CLI
* Install NodeJs (8.\*.\* as of Jan 2018)
* `npm install -g gulp`

## Domain-level AWS Setup
**Note: The steps in this section will generally only need to be performed once per business domain**

* If a Terraform IAM user and S3 bucket for Terraform states does not yet exist for the domain:
  * Create Terraform IAM user and attach admin right
    * Naming convention: `<domain>-terraform-user`
  * Set up an S3 bucket to hold remote states for Terraform
    * Naming convention: `<domain>-terraform-states`

* Register a local AWS profile for terraform, via AWS CLI
  * `aws configure --profile <TO_FILL | DOMAIN_TERRAFORM_USER>`

* If your application requires pretty DNS, and an AWS hosted zone does not yet exist for the domain (eg. your_domain.vpsvc.com), file a ticket with JetStream to set one up.

## Project Setup

* Rename the following files to have your project name
  * Solution file "NetCoreSample.sln" to `<PROJECT_NAME>`.sln
  * Folder "src\\NetCoreSample" to src\\`<PROJECT_NAME>`
  * Project file "src\\NetCoreSample.csproj" to src\\`<PROJECT_NAME>`.csproj
  * Folder "test\\NetCoreSample.UnitTests" to test\\`<PROJECT_NAME>`.UnitTests
  * Project file "test\\NetCoreSample.UnitTests.csproj" to test\\`<PROJECT_NAME>`.UnitTests.csproj
  
* Perform content text replacement. **Note: the following replacements should be performed on the entire repository, for instance using a text editor like VS Code, not in Visual Studio, in order to capture occurrences outside the C# projects.**
  * Replace, with case sensitivity, "NetCoreSample" to `<PROJECT_NAME>`
  * Replace, with case sensitivity, "netcoresample" to `<PROJECT_NAME_LOWER_CASE>`
  * Replace all "`<TO_FILL | DOMAIN_NAME>`" occurrences with your business/technical domain name, with lower case
  * Replace all "`<TO_FILL | AWS_REGION>`" occurrences with your AWS region
  * Replace all "`<TO_FILL | DOMAIN_TERRAFORM_USER>`" with the name of the AWS IAM Role that terraforms AWS resources for your domain (see AWS Setup above)
  * Replace all "`<TO_FILL | DOMAIN_TERRAFORM_STATES_BUCKET>`" with the name of the S3 bucket that holds Terraform states for your domain (see AWS Setup above)

## Initial Deployment

### Terraform Shared Infrastructure

* `npm install`
* `gulp terraform:shared:apply`
  * Deploys shared (aka: non-environment specific) infrastructure via Terraform, including the Elastic Container Repository that will hold docker containers for the application
* Go to AWS console, copy the resultant ECR URI, and paste it into the gulpfile
  * Location is marked with "`<TO_FILL | ECR_URI>`"

### Compile Docker Image and Push to ECR

* `gulp docker:push --environment=[dev|prod]`
  * This will preflight the project, build, compile the docker image, and push the image to ECR with a tag for the specified environment

### Terraform Fargate Deployment

* `gulp terraform:env:apply --environment=[dev|prod]`
  * Deploys environment-specific resources, including the resources needed for a Fargate deployment (networking, security groups, load balancers, and ECS cluster, task, and service), plus any additional environment-specific resources you define
  * **WARNING: The terraform configuration in this boilerplate makes your service available on the public internet on port 80 via HTTP. Adjust as necessary to provide the necessary security for your service!**

## Optional: Add Route53 for Pretty DNS
* Uncomment the contents of `terraform/Environment/route53.tf`
* Replace `<TO_FILL | HOSTED_ZONE_NAME>` with the name of the AWS hosted zone for your domain
* Confirm that the `name` value of the `aws_route53_record` resource definition will interpolate to your desired URL
  * Run `gulp terraform:env:plan --environment=[dev|prod]` to see the interpolation result 
* `gulp terraform:env:apply --environment=[dev|prod]`

## Optional: Additional AWS Resources / Dependencies
If your service relies on other AWS resources (for instance, for persistence), you will need to 1) create an AWS IAM Role for your service that has the necessary permissions to access those resources, and 2) add definitions for those resources to the Terraform configuration. This boilerplate uses S3 as an example.

* Create an AWS IAM Role for the service
  * In AWS go to "IAM" > "Roles" and click "Create role"
  * Under "Choose the service that will use this role", select "Elastic Container Service"
  * Under "Select your use case", select "Elastic Container Service Task"
  * Select all policies needed for the role (eg. for S3, select "AmazonS3FullAccess")
  * Tag the role with the appropriate Domain (`<TO_FILL | DOMAIN_NAME>`) and App (`netcoresample`)
  * Name the role following the convention: `netcoresample-ecs-role`
  * Copy the ARN of the new role
* In the `aws_ecs_task_definition` portion of terraform/Environment/ecs.tf, uncomment `task_role_arn` and replace its value with the ARN for the newly created role
* Uncomment the contents of S3.tf (if using S3), or replace it with the appropriate resource definitions for your service's dependencies
* `gulp terraform:env:apply --environment=[dev|prod]`

## Clean Up

* Delete this and all preceding lines, then replace the `<PLACEHOLDERS>` below with the details that are appropriate to your project.

# `<PROJECT_NAME>`

## Development

### Development Environment Setup

* Install NetCore 2
* Install VisualStudio 2017


## Docker

We're using a multi-stage dockerfile to build, test, and run our code in a .net core container. To build and run Docker locally, run

``` Docker
docker build -t "<PROJECT_NAME>:latest" .
docker run -d -e ASPNETCORE_ENVIRONMENT=Local -p 5000:80 "<PROJECT_NAME>:latest"
```

Locally, go to <http://localhost:5000/swagger>


## Deploying Changes

**Note: The following section assumes an existing AWS Fargate deployment.**


### Deployment Environment Setup

* Install AWS CLI
* Register a local AWS profile for terraform, via AWS CLI
  * `aws configure --profile <TO_FILL | DOMAIN_TERRAFORM_USER>`
  * access key and access key id are stored in Secret Server ("`<TO_FILL | DOMAIN_TERRAFORM_USER_SECRET_NAME>`")
* Set AWS CLI default region to "`<TO_FILL | AWS_REGION>`"
* Install NodeJs (8.\*.\* as of Jan 2018)
* `npm install -g gulp`
* Install Docker for Windows

### Deployment Steps

* `npm install`
* `gulp docker:push --environment=[dev|prod]`
  * This will preflight the project, build, compile the docker image, and push the image to ECR with a tag for the specified environment
* Now trigger a service update via AWS console for it to pick up the newly pushed image
  * Open AWS dashboard for vbu-content on `<TO_FILL | AWS_REGION>`
  * Switch to admin role (see https://support.cimpress.cloud/hc/en-us/articles/115009816628-Switching-between-Read-Only-and-Administrator-IAM-roles, "Switching roles via the AWS Console")
  * Go to "Elastic Container Service"
  * Click into "[dev|prod]-`<TO_FILL | CLUSTER_NAME>`", depending on whether you want to deploy to dev or prod respectively
  * Check "[dev|prod]-`<TO_FILL | SERVICE_NAME>`", and click "Update"
  * On "Step 1: Configure service" view, check "Force new deployment"
  * Click "Next" all the way through and click "Update Service"
* Monitor the deployment status by going to the Services tab, clicking on "[dev|prod]-`<TO_FILL | SERVICE_NAME>`", and then going to the Events tab
  * You should see some messages triggered by your deployment about ECS managing tasks, targets, and connections
  * Wait for a message that the service has reached a steady state
  * This should take approximately five to ten minutes
* Once completed, smoke test the following:
  * /LiveCheck
  * /Swagger


## Infrastructure

* Terraform IAM user: "`<TO_FILL | DOMAIN_TERRAFORM_USER>`"
* S3 bucket holding remote states for Terraform: "`<TO_FILL | DOMAIN_TERRAFORM_STATES_BUCKET>`"

### Terraform tasks

| Gulp Task | Description| Notes | Example
| ------------- |-------------|-------------|-------------|
| terraform:download | Downloads Terraform environment | This normally does not require separate execution | gulp terraform:download |
| terraform:env:clean | Cleanup Terraform generated assets from workspace | This normally does not require separate execution | gulp terraform:env:clean |
| terraform:env:init | Initializes Terraform environment in workspace | This normally does not require separate execution | gulp terraform:env:init |
| terraform:env:plan | Generate and show Terraform plan | Specify "environment" flag to point to specific environment | gulp terraform:env:plan --environment=dev |
| terraform:env:apply | Apply infrastructure changes via Terraform | Specify "environment" flag to point to specific environment | gulp terraform:env:apply --environment=dev |
