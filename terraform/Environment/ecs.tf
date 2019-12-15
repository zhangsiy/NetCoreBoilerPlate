resource "aws_ecs_cluster" "cluster" {
  provider = "aws.<TO_FILL | AWS_REGION>"
  
  name = "${var.environment}-${var.app_name}-cluster"

  tags {
    Environment = "${var.environment}"
    Domain = "${var.domain_name}"
    App = "${var.app_name}"
  }
}

resource "aws_cloudwatch_log_group" "log_group" {
  provider = "aws.<TO_FILL | AWS_REGION>"
  
  name = "ecs/${var.environment}-${var.app_name}-task-definition"

  tags {
    Environment = "${var.environment}"
    Domain = "${var.domain_name}"
    App = "${var.app_name}"
  }
}

data "aws_iam_role" "ecsTaskExecutionRole" {
  provider = "aws.<TO_FILL | AWS_REGION>"
  
  name = "ecsTaskExecutionRole"
}

resource "aws_ecs_task_definition" "task" {
  provider = "aws.<TO_FILL | AWS_REGION>"
  
  family = "${var.environment}-${var.app_name}-task-definition"
  network_mode = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = "${var.fargate_cpu}"
  memory                   = "${var.fargate_memory}"
  execution_role_arn       = "${data.aws_iam_role.ecsTaskExecutionRole.arn}"
#   task_role_arn          = "<TO_FILL | TASK_ROLE_ARN>"
  
  container_definitions = <<DEFINITION
  [
    {
      "image": "${var.image_uri}",
      "memoryReservation": ${var.fargate_memory},
      "name": "${var.environment}-${var.app_name}-container",
      "networkMode": "awsvpc",
      "portMappings": [
        {
          "containerPort": ${var.app_port},
          "hostPort": ${var.app_port}
        }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "${aws_cloudwatch_log_group.log_group.name}",
          "awslogs-region": "<TO_FILL | AWS_REGION>",
          "awslogs-stream-prefix": "ecs"
        }
      },
      "environment": [{
        "name": "ASPNETCORE_ENVIRONMENT",
        "value": "${var.environment == "prod" ? "Production" : "Development"}"
      }]
    }
  ]
  DEFINITION

  tags {
    Environment = "${var.environment}"
    Domain = "${var.domain_name}"
    App = "${var.app_name}"
  }
}

resource "aws_ecs_service" "service" {
  provider = "aws.<TO_FILL | AWS_REGION>"
  
  name = "${var.environment}-${var.app_name}-container-service"
  cluster = "${aws_ecs_cluster.cluster.id}"
  task_definition = "${aws_ecs_task_definition.task.family}"
  desired_count = "${var.app_count}"
  launch_type = "FARGATE"
  health_check_grace_period_seconds = 240

  network_configuration {
    security_groups = ["${aws_security_group.ecs.id}"]
    subnets = ["${aws_subnet.public_subnets.*.id}"]
    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = "${aws_alb_target_group.target_group.id}"
    container_name = "${var.environment}-${var.app_name}-container"
    container_port = "${var.app_port}"
  }

  depends_on = [
    "aws_alb_listener.listener",
  ]
}