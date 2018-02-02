provider "aws" {
  profile = "<TO_FILL | DOMAIN_NAME>-terraform"
  region = "<TO_FILL | AWS_REGION>"
}

terraform {
  backend "s3" {
	  profile = "<TO_FILL | DOMAIN_NAME>-terraform"
  }
}

resource "aws_ecr_repository" "netcoresample_service_ecr" {
  name = "netcoresample_ecr"
  
  lifecycle {
    prevent_destroy = true
  }
}

###############
# Variables
###############

#the environment we are deploying to
variable "environment" {
  type = "string"
}

###############