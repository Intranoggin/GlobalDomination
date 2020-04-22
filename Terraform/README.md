# Terraform Infrastructure

## Setup

Current Work in Progress. This Terraform Script will ultimately create the full geo-distributed PaaS infrastructure.

The starting point for this example was my [Basic Web App Example](https://github.com/Intranoggin/TerraformDev/tree/master/BasicWebApp) and the [Terraform App Service documentation](https://registry.terraform.io/providers/hashicorp/azurerm/2.1.0/docs/resources/app_service)

This current version will create

- a resource group called "tfBasicWebAppRGDev"
- a WebAppService Plan using the F1 free tier called "tfBasicWebAppPlanDev"
- an App Service called "tfBasicWebAppServiceDev"
