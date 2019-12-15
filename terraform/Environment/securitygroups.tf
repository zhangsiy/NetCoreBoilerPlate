resource "aws_security_group" "alb" {
  provider = "aws.<TO_FILL | AWS_REGION>"
  
  name = "ECS ${var.environment}-${var.app_name}-cluster - ALB SecurityGroup"
  description = "ELB Allowed Ports"
  vpc_id = "${aws_vpc.vpc.id}"

  ingress {
    protocol = "tcp"
    from_port = "${var.app_port}"
    to_port = "${var.app_port}"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    protocol = "-1"
    from_port = 0
    to_port = 0
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags {
    Name = "ECS ${var.environment}-${var.app_name}-cluster - ALB SecurityGroup"
    Environment = "${var.environment}"
    Domain = "${var.domain_name}"
    App = "${var.app_name}"
  }
}

resource "aws_security_group" "ecs" {
  provider = "aws.<TO_FILL | AWS_REGION>"
  
  name = "ECS ${var.environment}-${var.app_name}-cluster - ECS SecurityGroup"
  description = "ECS Allowed Ports"
  vpc_id = "${aws_vpc.vpc.id}"

  ingress {
    protocol = "tcp"
    from_port = "${var.app_port}"
    to_port = "${var.app_port}"
    security_groups = ["${aws_security_group.alb.id}"]
  }

  egress {
    protocol    = "-1"
    from_port   = 0
    to_port     = 0
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags {
    Name = "ECS ${var.environment}-${var.app_name}-cluster - ECS SecurityGroup"
    Environment = "${var.environment}"
    Domain = "${var.domain_name}"
    App = "${var.app_name}"
  }
}