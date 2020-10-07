# change to resource_group_template_deployment at some point
resource "azurerm_template_deployment" "logicappdeployment" {
  deployment_mode     = "Incremental"
  name                = "logicappdeploy-01"
  resource_group_name = azurerm_resource_group.rg.name
  
  template_body       = file("../arm/logic-app-deploy.json")
  
  parameters = {
    "resourceGroup"      = azurerm_resource_group.rg.name
    "location"           = azurerm_resource_group.rg.location
    "logicAppName"       = var.logic_app_name
    "subscriptionId"     = var.subscription_id
    "functionAppUrl"     = azurerm_function_app.functionapp.default_hostname
    "functionAppKey"     = lookup(azurerm_template_deployment.functionkey.outputs, "functionKey")
  }
}