##########################################################################################################
# Environment-specific variables passed in by gulp - do not modify
##########################################################################################################

variable "environment" {
  description = "Environment we're creating resources for (eg. dev or prod)"
  type = "string"
}

variable "image_uri" {
  description = "URI for docker image in ECR"
  type = "string"
}


############################################################################################################
# Variables that identify your application - replace these default values before running terraform commands
############################################################################################################

variable "domain_name" {
  description = "The lowercased name of the business domain, used in resource names and tags"
  type = "string"
  default = "<TO_FILL | DOMAIN_NAME>"
}

variable "app_name" {
  description = "The lowercased name of the application, used in resource names and tags"
  type = "string"
  default = "netcoresample"
}


##########################################################################################################
# Variables specific to your infrastructure needs - modify as needed for your application
##########################################################################################################

variable "az_count" {
  description = "Number of availability zones to cover in a given region"
  type = "string"
  default = 2
}

variable "app_port" {
  type = "string"
  default = 80
}

variable "app_count" {
  description = "Number of docker containers to run"
  type = "string"
  default = 1
}

variable "fargate_cpu" {
  description = "Fargate instance CPU units to provision (1 vCPU = 1024 CPU units)"
  type = "string"
  default = "256"
}

variable "fargate_memory" {
  description = "Fargate instance memory to provision (in MiB)"
  type = "string"
  default = "512"
}

variable "health_check_path" {
  type = "string"
  default = "/LiveCheck"
}