param webAppName string
param linuxFxVersion string
param location string
param appServicePlanId string
param AppInsightsInstrumentationKey string
param AppInsightsConnectionString string
param storageAccountName string
param storageAccountId string

var webSiteName = toLower('app-${webAppName}')

resource appService 'Microsoft.Web/sites@2022-09-01' = {
  name: webSiteName
  location: location
  properties: {
    serverFarmId: appServicePlanId
    siteConfig: {
      alwaysOn: true
      linuxFxVersion: linuxFxVersion
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: AppInsightsInstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: AppInsightsConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~2'
        }
        {
          name: 'XDT_MicrosoftApplicationInsights_Mode'
          value: 'default'
        }
        {
          name: 'ConnectionStrings__BlobStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccountId, '2021-09-01').keys[0].value}'
        }
      ]
    }
  }
}
