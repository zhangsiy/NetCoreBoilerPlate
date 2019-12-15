# resource "aws_s3_bucket" "data-bucket" {
#   provider = "aws.<TO_FILL | AWS_REGION>"
  
#   bucket = "${var.domain_name}-${var.app_name}-data-bucket-${var.environment}"
#   acl    = "private"

#   versioning {
#     enabled = true
#   }

#   cors_rule {
#     allowed_headers = ["Authorization"]
#     allowed_origins = ["*"]
#     allowed_methods = ["GET"]
#     max_age_seconds = 3000
#   }
  
#   tags {
#     Environment = "${var.environment}"
#     Domain = "${var.domain_name}"
#     App = "${var.app_name}"
#   }
  
#   lifecycle {
#     prevent_destroy = true
#   }
# }