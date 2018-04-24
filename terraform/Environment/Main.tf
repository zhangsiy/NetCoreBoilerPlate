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

resource "aws_s3_bucket" "netcoresample-data-bucket" {
  provider = "aws.<TO_FILL | AWS_REGION>"

  bucket = "<TO_FILL | DOMAIN_NAME>-netcoresample-data-bucket-${var.environment}"
  acl    = "private"

  versioning {
    enabled = true
  }

  cors_rule {
    allowed_headers = ["Authorization"]
    allowed_origins = ["*"]
    allowed_methods = ["GET"]
    max_age_seconds = 3000
  }
  
  tags {
    Environment = "${var.environment}"
    Domain = "<TO_FILL | DOMAIN_NAME>"
    App = "NetCoreSampleService"
  }
  
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