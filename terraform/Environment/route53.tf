# data "aws_route53_zone" "hosted-zone" {
#   provider = "aws.<TO_FILL | AWS_REGION>"
  
#   name         = "<TO_FILL | HOSTED_ZONE_NAME>"
#   private_zone = false
# }

# resource "aws_route53_record" "dns" {
#   provider = "aws.<TO_FILL | AWS_REGION>"
  
#   zone_id = "${data.aws_route53_zone.hosted-zone.zone_id}"
#   name    = "${var.environment}.${var.app_name}.${data.aws_route53_zone.hosted-zone.name}"
#   type    = "A"

#   alias {
#     name = "${aws_alb.alb.dns_name}"
#     zone_id = "${aws_alb.alb.zone_id}"
#     evaluate_target_health = false
#   }
# }