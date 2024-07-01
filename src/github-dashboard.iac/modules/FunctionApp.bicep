param functionAppName string
param planId string
param location string
param storageAccountName string
param storageAccountId string
param AppInsightsInstrumentationKey string
param AppInsightsConnectionString string
param linuxFxVersion string

param sqlserverName string
param databaseName string
param sqlAdministratorLogin string
@secure()
param sqlAdministratorLoginPassword string

var functionName = toLower('func-${functionAppName}')
var MyConnectionString = 'Server=tcp:${sqlserverName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${databaseName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

resource functionApp 'Microsoft.Web/sites@2022-09-01' = {
  name: functionName
  location: location
  kind: 'functionapp'
  identity:{
    type:'SystemAssigned'
  }
  properties: {
    serverFarmId: planId
    clientAffinityEnabled: false
    siteConfig: {
      alwaysOn: true
      linuxFxVersion: linuxFxVersion
    }
    httpsOnly: true
  }
}

resource appsettings 'Microsoft.Web/sites/config@2022-09-01' = {
  parent: functionApp
  name: 'appsettings'
  properties: {
    AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccountId, '2021-09-01').keys[0].value}'
    strgithubapp_STORAGE: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccountId, '2021-09-01').keys[0].value}'
    MyDbContext: MyConnectionString
    APPINSIGHTS_INSTRUMENTATIONKEY: AppInsightsInstrumentationKey
    APPLICATIONINSIGHTS_CONNECTION_STRING: AppInsightsConnectionString
    FUNCTIONS_EXTENSION_VERSION: '~4'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    ftpsState: 'Disabled'
    minTlsVersion: '1.2'
  }
}
