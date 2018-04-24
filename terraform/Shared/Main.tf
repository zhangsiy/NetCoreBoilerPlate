provider "aws" {
  alias = "<TO_FILL | AWS_REGION>"

  region = "<TO_FILL | AWS_REGION>"
  profile = "<TO_FILL | DOMAIN_NAME>-terraform"
}

terraform {
  backend "s3" {
    profile = "<TO_FILL | DOMAIN_NAME>-terraform"
  }
}

resource "aws_ecr_repository" "netcoresample_service_ecr" {
  provider = "aws.<TO_FILL | AWS_REGION>"

  name = "netcoresample_ecr"
  
  lifecycle {
    prevent_destroy = true
  }
}