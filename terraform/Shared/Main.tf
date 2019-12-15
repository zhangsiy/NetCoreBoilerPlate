provider "aws" {
  alias = "<TO_FILL | AWS_REGION>"

  region = "<TO_FILL | AWS_REGION>"
  profile = "<TO_FILL | DOMAIN_TERRAFORM_USER>"
}

# store remote terraform state in S3
terraform {
  backend "s3" {
    profile = "<TO_FILL | DOMAIN_TERRAFORM_USER>"
  }
}

resource "aws_ecr_repository" "netcoresample_service_ecr" {
  provider = "aws.<TO_FILL | AWS_REGION>"

  name = "netcoresample_ecr"
  
  lifecycle {
    prevent_destroy = true
  }
}