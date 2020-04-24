# Configure the Microsoft Azure Provider.
provider "azurerm" {
    version = "= 2.0.0"
    features{}
}

# Create a resource group
resource "azurerm_resource_group" "RegionAWebApp" {
    name     = "${var.rootname}WebAppRG-${var.environment}"
    location = var.location
    tags     = var.tags  
}

resource "azurerm_app_service_plan" "RegionAWebApp" {
    name     = "${var.rootname}WebAppPlan${var.environment}"
    location = azurerm_resource_group.RegionAWebApp.location
    resource_group_name = azurerm_resource_group.RegionAWebApp.name
    tags     = var.tags

  sku {
    tier = "Free"
    size = "F1"
  }
}

resource "azurerm_app_service" "RegionAWebApp" {
  name                = "${var.rootname}WebAppService${var.environment}"
  location            = azurerm_resource_group.RegionAWebApp.location
  resource_group_name = azurerm_resource_group.RegionAWebApp.name
  app_service_plan_id = azurerm_app_service_plan.RegionAWebApp.id 
}
