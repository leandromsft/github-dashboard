// https://github.com/Azure/azure-quickstart-templates/blob/master/quickstarts/microsoft.storage/storage-blob-container/main.bicep

@description('Storage Account type')
@allowed([
  'Premium_LRS'
  'Premium_ZRS'
  'Standard_GRS'
  'Standard_GZRS'
  'Standard_LRS'
  'Standard_RAGRS'
  'Standard_RAGZRS'
  'Standard_ZRS'
])
param storageAccountType string = 'Standard_LRS'

@description('Location for the storage account.')
param location string

@description('The name of the Storage Account')
param storageAccountName string

var storageName = toLower('stvm${storageAccountName}')

resource sa 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: storageName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
}

//resource containerIssue 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-06-01' = {
//  name: '${sa.name}/default/github-issue'
//}

//resource containerWorkflow 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-06-01' = {
//  name: '${sa.name}/default/github-workflow'
//}

output storageAccountId string = sa.id
output storageAccountName string = sa.name
