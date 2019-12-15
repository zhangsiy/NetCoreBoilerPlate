resource "aws_alb" "alb" {
  provider = "aws.<TO_FILL | AWS_REGION>"
  
  subnets = ["${aws_subnet.public_subnets.*.id}"]
  security_groups = ["${aws_security_group.alb.id}"]

  tags {
    Name = "ECS ${var.environment}-${var.app_name}-cluster - ALB"
    Environment = "${var.environment}"
    Domain = "${var.domain_name}"
    App = "${var.app_name}"
  }
}

resource "aws_alb_target_group" "target_group" {
  provider = "aws.<TO_FILL | AWS_REGION>"
  
  port = "${var.app_port}"
  protocol = "HTTP"
  vpc_id = "${aws_vpc.vpc.id}"
  target_type = "ip"

  health_check {
    protocol            = "HTTP"
    path                = "${var.health_check_path}"
    healthy_threshold   = "5"
    unhealthy_threshold = "2"
    timeout             = "5"
    interval            = "30"
    matcher             = "200"
  }

  tags {
    Name = "ECS ${var.environment}-${var.app_name}-cluster - TargetGroup"
    Environment = "${var.environment}"
    Domain = "${var.domain_name}"
    App = "${var.app_name}"
  }
}

resource "aws_alb_listener" "listener" {
  provider = "aws.<TO_FILL | AWS_REGION>"
  
  load_balancer_arn = "${aws_alb.alb.id}"
  port = "${var.app_port}"
  protocol = "HTTP"

  default_action {
    target_group_arn = "${aws_alb_target_group.target_group.id}"
    type = "forward"
  }
}