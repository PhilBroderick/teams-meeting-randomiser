variable "subscription_id" {
  type = string
}

variable "client_id" {
  type = string 
}

variable "client_secret" {
  type = string
}

variable "tenant_id" {
  type = string
}

variable "rgName" {
  type = string
}

variable "location" {
  type = string
  default = "uksouth"
}

variable "storageName" {
  type = string
}

variable "app_service_plan_name" {
  type = string
}

variable "function_app_name" {
  type = string
}

variable "logic_app_name" {
  type = string
}

variable "logic_app_recurrence_freq" {
  type = string
}

variable "logic_app_recurrence_interval" {
  type = number
}