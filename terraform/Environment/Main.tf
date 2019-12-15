# store remote terraform state in S3
terraform {
  backend "s3" {
    profile = "<TO_FILL | DOMAIN_TERRAFORM_USER>"
  }
}