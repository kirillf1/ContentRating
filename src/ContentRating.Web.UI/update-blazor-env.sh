cp /var/www/web/appsettings.json /var/www/web/appsettings.json.temp && \
[ ! -z "${API_BASE_URL}" ] && jq --arg aVar "$API_BASE_URL" '.ApiSettings.BaseUrl = $aVar' /var/www/web/appsettings.json.temp | sponge /var/www/web/appsettings.json.temp; \
[ ! -z "${SIGNALR_HUB_URL}" ] && jq --arg aVar "$SIGNALR_HUB_URL" '.ApiSettings.SignalRHubUrl = $aVar' /var/www/web/appsettings.json.temp | sponge /var/www/web/appsettings.json.temp; \
rm /var/www/web/appsettings.json && \
mv /var/www/web/appsettings.json.temp /var/www/web/appsettings.json