# Azure Infrastructure Setup Script
# Run these commands in Azure CLI to create required resources

# ============================================
# VARIABLES - CUSTOMIZE THESE
# ============================================
RESOURCE_GROUP="rg-solarsystem"
LOCATION="eastus"
APP_SERVICE_PLAN="asp-solarsystem"
WEBAPP_NAME="solarsystem-app"
SQL_SERVER_NAME="sql-solarsystem"
SQL_DB_NAME="SolarSystemDb"
SQL_ADMIN_USER="sqladmin"
SQL_ADMIN_PASSWORD="YourStrongPassword123!"  # Change this!

# ============================================
# 1. CREATE RESOURCE GROUP
# ============================================
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

# ============================================
# 2. CREATE APP SERVICE PLAN (Linux)
# ============================================
az appservice plan create \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku B1 \
  --is-linux

# ============================================
# 3. CREATE WEB APP
# ============================================
az webapp create \
  --name $WEBAPP_NAME \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --runtime "DOTNETCORE:9.0"

# ============================================
# 4. CREATE SQL SERVER
# ============================================
az sql server create \
  --name $SQL_SERVER_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --admin-user $SQL_ADMIN_USER \
  --admin-password $SQL_ADMIN_PASSWORD

# ============================================
# 5. CREATE SQL DATABASE
# ============================================
az sql db create \
  --name $SQL_DB_NAME \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER_NAME \
  --service-objective S0

# ============================================
# 6. CONFIGURE SQL FIREWALL (Allow Azure Services)
# ============================================
az sql server firewall-rule create \
  --name "AllowAzureServices" \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER_NAME \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# ============================================
# 7. GET CONNECTION STRING
# ============================================
CONNECTION_STRING=$(az sql db show-connection-string \
  --name $SQL_DB_NAME \
  --server $SQL_SERVER_NAME \
  --client ado.net \
  --output tsv)

echo "Connection String (update password):"
echo $CONNECTION_STRING

# ============================================
# 8. CONFIGURE WEB APP SETTINGS
# ============================================
az webapp config appsettings set \
  --name $WEBAPP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings \
    "ConnectionStrings__DefaultConnection=$CONNECTION_STRING" \
    "ASPNETCORE_ENVIRONMENT=Production" \
    "NasaApi__ApiKey=$NASA_API_KEY"

# ============================================
# 9. ENABLE MANAGED IDENTITY (for GitHub Actions)
# ============================================
az webapp identity assign \
  --name $WEBAPP_NAME \
  --resource-group $RESOURCE_GROUP

# ============================================
# 10. CREATE SERVICE PRINCIPAL FOR GITHUB
# ============================================
az ad sp create-for-rbac \
  --name "sp-solarsystem-github" \
  --role contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/$RESOURCE_GROUP \
  --sdk-auth

# Output will contain:
# - clientId (AZURE_CLIENT_ID)
# - clientSecret
# - tenantId (AZURE_TENANT_ID)
# - subscriptionId (AZURE_SUBSCRIPTION_ID)

echo ""
echo "============================================"
echo "NEXT STEPS:"
echo "============================================"
echo "1. Copy the service principal output above"
echo "2. Add these secrets to GitHub repository:"
echo "   - AZURE_CLIENT_ID"
echo "   - AZURE_TENANT_ID"
echo "   - AZURE_SUBSCRIPTION_ID"
echo "   - DB_CONNECTION_STRING"
echo "3. Update WEBAPP_NAME in .github/workflows/azure-deploy.yml"
echo "4. Push to main branch to trigger deployment"
echo "============================================"
