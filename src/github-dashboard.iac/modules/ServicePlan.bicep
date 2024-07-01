param sku string
param location string
param appServicePlanName string

var planName = toLower('plan-${appServicePlanName}')

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: planName
  location: location
  properties: {
    reserved: true
  }
  sku: {
    name: sku
  }
  kind: 'linux'
}

output planId string = appServicePlan.id
