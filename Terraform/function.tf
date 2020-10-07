resource "azurerm_storage_account" "storage" {
  account_replication_type  = "LRS"
  account_tier              = "Standard"
  location                  = azurerm_resource_group.rg.location
  name                      = var.storageName
  resource_group_name       = azurerm_resource_group.rg.name
}

resource "azurerm_app_service_plan" "appservice" {
  location            = azurerm_resource_group.rg.location
  name                = var.app_service_plan_name
  resource_group_name = azurerm_resource_group.rg.name
  kind                = "FunctionApp"
  
  sku {
    size = "Y1"
    tier = "Dynamic"
  }
}

resource "azurerm_function_app" "functionapp" {
  app_service_plan_id        = azurerm_app_service_plan.appservice.id
  location                   = azurerm_app_service_plan.appservice.location
  name                       = var.function_app_name
  resource_group_name        = azurerm_resource_group.rg.name
  storage_account_name       = azurerm_storage_account.storage.name
  storage_account_access_key = azurerm_storage_account.storage.primary_access_key
}

# Get the master key for use in the Logic app
resource "azurerm_template_deployment" "functionkey" {
  deployment_mode = "Incremental"
  name = "functionkeys-01"
  resource_group_name = azurerm_resource_group.rg.name
  
  template_body = <<DEPLOY
  {
    "$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {
      "functionApp": {
        "type": "string"
      }
    },
    "variables": {
      "functionAppId": "[resourceId('Microsoft.Web/sites', parameters('functionApp'))]"
    },
    "resources": [],
    "outputs": {
      "functionKey": {
        "type": "string",
        "value": "[listkeys(concat(variables('functionAppId'), '/host/default'), '2018-11-01').functionKeys.default]"
      }
    }
  }
  DEPLOY
  
  parameters = {
    "functionApp" = azurerm_function_app.functionapp.name
  }
  
  depends_on = [ azurerm_function_app.functionapp ]
}
