Перед развертыванием:
	* LayersCreator:
		* GSDataDir
	* SmartEco:appsettings.json:
		* "Server"

Пересчет измеренных данных:
	* SmartEcoAPI:MeasuredDatasController:GetMeasuredData
	* SmartEcoAPI:MeasuredDatasController:GetMeasuredData [HttpGet("{id}")]
	* SmartEcoAPI:MonitoringPostsController:GetEcoserviceMonitoringPostsExceed
	* LayersCreator:Main