param sku string = 'S2'
param location string = resourceGroup().location 
param appServicePlanName string
param webAppName string
param linuxFxVersion string = 'DOTNETCORE|6.0'
param storageType string = 'Standard_LRS'
param storageName string
param logAnalyticsName string
param appInsightName string
param functionName string

param sqlserverName string
param sqlAdminLogin string
@secure()
param sqlAdminPassword string
param sqlDatabaseName string = 'github_dashboard'

module servicePlan 'modules/ServicePlan.bicep' = {
  name: 'servicePlanDeploy'
  params: {
    sku: sku
    location: location
    appServicePlanName: appServicePlanName
  }
}

module appInsights 'modules/ApplicationInsights.bicep' = {
  name: 'appInsightsDeploy'
  params: {
    logAnalyticsName: logAnalyticsName
    location: location
    appInsightName: appInsightName
  }
}

module storageAccount 'modules/Storage.bicep' = {
  name: 'storageDeploy'
  params: {
    storageAccountType: storageType
    storageAccountName: storageName
    location: location
  }
}

module webApp 'modules/WebApp.bicep' = {
  name: 'webAppDeploy'
  params: {
    webAppName: webAppName
    linuxFxVersion: linuxFxVersion
    location: location
    appServicePlanId: servicePlan.outputs.planId
    AppInsightsInstrumentationKey: appInsights.outputs.appInsightsKey
    AppInsightsConnectionString: appInsights.outputs.appInsightsConnString
    storageAccountName: storageAccount.outputs.storageAccountName
    storageAccountId: storageAccount.outputs.storageAccountId
  }
}

module functionApp 'modules/FunctionApp.bicep' = {
  name: 'functionDeploy'
  params: {
    functionAppName: functionName
    location: location
    planId: servicePlan.outputs.planId
    linuxFxVersion: linuxFxVersion
    storageAccountName: storageAccount.outputs.storageAccountName
    storageAccountId: storageAccount.outputs.storageAccountId
    AppInsightsInstrumentationKey: appInsights.outputs.appInsightsKey
    AppInsightsConnectionString: appInsights.outputs.appInsightsConnString
    sqlserverName: sqlserverName
    databaseName: sqlDatabaseName
    sqlAdministratorLogin: sqlAdminLogin
    sqlAdministratorLoginPassword: sqlAdminPassword
  }
}

module sqlModule 'modules/SqlServer.bicep' = {
  name: 'sqlDeploy'
  params: {
    sqlserverName: sqlserverName
    location: location
    sqlAdministratorLogin: sqlAdminLogin
    sqlAdministratorLoginPassword: sqlAdminPassword
    databaseName: sqlDatabaseName
  }
}
