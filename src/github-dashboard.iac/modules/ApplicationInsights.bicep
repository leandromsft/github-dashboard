param logAnalyticsName string
param location string
param appInsightName string

var workspaceName = toLower('log-${logAnalyticsName}')
var insightsName = toLower('appi-${appInsightName}')

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-03-01-preview' = {
  name: workspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 120
  }
}

resource appInsights 'microsoft.insights/components@2020-02-02-preview' = {
  name: insightsName
  location: location
  kind: 'string'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

output appInsightsKey string = appInsights.properties.InstrumentationKey
output appInsightsConnString string = appInsights.properties.ConnectionString
