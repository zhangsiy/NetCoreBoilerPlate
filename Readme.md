### Environment Setup

* Install NetCore 2
* Upgrade VisualStudio 2017
* Install AWS CLI
* Install NodeJs (8.*.* as of Jan 2018)
* npm install -g gulp


### Project Setup

* Rename below files to have your project name
  * Solution file "NetCoreSample.sln" to <PROJECT_NAME>.sln
  * Folder "NetCoreSample.Service" to <PROJECT_NAME>.Service
  * Project file "NetCoreSample.Service.csproj" to <PROJECT_NAME>.Service.csproj
 
* Perform content text replacement
  * Replace, with case sensitivity, "NetCoreSample" to <PROJECT_NAME> 
  * Replace, with case sensitivity, "netcoresample" to <PROJECT_NAME_LOWER_CASE> 
  * Replace all "<TO_FILL | DOMAIN_NAME>" occurances with your business/technical domain name, with lower case
  * Replace all "<TO_FILL | AWS_REGION>" occurances with your AWS region


### AWS Setup

* Create terraform IAM user, "<TO_FILL | DOMAIN_NAME>-terraform" and attach admin right
* Setup a S3 bucket to hold remote states for Terraform. Name it "<TO_FILL | DOMAIN_NAME>-terraform-bucket"

* Register a local AWS profile for terraform, via AWS CLI 
  * "aws configure --profile <TO_FILL | DOMAIN_NAME>-terraform"


### Infrastructure Setup

* npm install
* Deploy shared (aka: non-environment specific) infrastructure via Terraform
  * gulp terraform:shared:apply
* Go to AWS console, and copy the resultant ECR URI into the gulp file
  * Location is marked with "<TO_FILL | ECR_URI>"


### Your First Docker 

* gulp docker:push to compile, preflight the project and build and push the docker image to ECR


## Nest Steps 

* Implement/adopt a scaffolding/templating solution to assist the initialization of a project based on this boilerplate. 