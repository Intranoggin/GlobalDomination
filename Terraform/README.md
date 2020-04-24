# Terraform Infrastructure

## Setup

Current Work in Progress. This Terraform Script will ultimately create the full geo-distributed PaaS infrastructure.

The starting point for this example was my [Basic Web App Example](https://github.com/Intranoggin/TerraformDev/tree/master/BasicWebApp) and the [Terraform App Service documentation](https://registry.terraform.io/providers/hashicorp/azurerm/2.1.0/docs/resources/app_service)

My [Terraform Dev](https://github.com/Intranoggin/TerraformDev/tree/master/BasicRG) project has some basic info on getting Terraform set up, or you can go straight to the the source [Learn Terraform](https://learn.terraform.com).

Create the infrastructure by exectuting the terraform command
PS C:\Code\GlobalDomination\Terraform> terraform apply -var-file="dev.tfvars"

This current version will create

- a resource group called "GlobalDomWebAppRG-Dev"
- a WebAppService Plan using the F1 free tier called "GlobalDomWebAppPlanDev"
- an App Service called "GlobalDomWebAppServiceDev"
