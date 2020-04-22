variable "location"{}

variable "prefix"{
    type = string
    default = "my"
 }

 variable "environment"{
     type = string
     default = "Dev"
 }

variable "tags" {
    type = map(string)
    default = {
        Environment = "Terraform Dev"
        Dept = "Basic"
  }
}

# variable "admin_username" {
#     type = "string"
#     description = "Administrator user name for virtual machine"
# }

# variable "admin_password" {
#     type = "string"
#     description = "Password must meet Azure complexity requirements"
# }



# variable "sku" {
#     default = {
#         westus = "16.04-LTS"
#         eastus = "18.04-LTS"
#     }
# }