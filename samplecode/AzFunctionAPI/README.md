# Create Azure Function to expose API from ADT and map data to Asset object

This tool mainly used to connect to Azure Digital Twin via [Azure Identity](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme). The project is using DefaultAzureCrential with system managed identity from Azure Function. 


# Pre-requisite

1. Azure Function - This is required to expose the ADT via Azure Function Http Trigger
1. Azure Digital Twins

# Getting Started

1. Configure [permissions](https://docs.microsoft.com/en-us/azure/digital-twins/tutorial-end-to-end#configure-permissions-for-the-function-app) for the function app.
    1. Using Azure CLI
    1. Assign access role <br />
    ```az functionapp identity show -g <your-resource-group> -n <your-App-Service-function-app-name>```
    1. If the result is empty, create a new system-managed identity
    ```az functionapp identity assign --resource-group <your-resource-group> --name <your-App-Service-function-app-name>```
    1. Assign Azure Function system identity to the Azure Digital Twins Data Owner
    ```az dt role-assignment create --dt-name <your-Azure-Digital-Twins-instance> --assignee "<principal-ID>" --role "Azure Digital Twins Data Owner"```
    1. Build and publish the application to Azure Function (you can automate this using Github Action)

