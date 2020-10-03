resource "azurerm_logic_app_workflow" "logicappworkflow" {
  location = azurerm_resource_group.rg.location
  name = var.logic_app_name
  resource_group_name = azurerm_resource_group.rg.name
}

resource "azurerm_logic_app_trigger_recurrence" "recurrencetrigger" {
  frequency = var.logic_app_recurrence_freq
  interval = var.logic_app_recurrence_interval
  logic_app_id = azurerm_logic_app_workflow.logicappworkflow.id
  name = "logic-app-recurrence-trigger"
}