$Env:PORT=5003

dapr run --app-port $Env:Port --app-id scrapper-store-actor --components-path ../../../components --dapr-http-port 3503 -- dotnet watch run

## Deployment

Create `eth` table in azure table storage !!!