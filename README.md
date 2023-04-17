# SmartEco
<p align="center" width="100%">
    <img width="80%" src="src/Infrastructure/Client/SmartEco.Web/wwwroot/images/SmartEcoLogo2.png">
</p>

## docs
Contains project documentation: product information, text files, requirements, project schemas, database schemas, etc.

## src
Contains all projects required for functioning, debugging, execution and operation.

### Common
Contains library projects that provide common elements and functionality for use in the infrastructure

* **SmartEco.Common** - contains general enums, models (requests, responses, filters), services and opitions.

  * :e-mail: To use the email sending service:
    ````json
    {
      "Email": {
        "SmtpServer": "smtp.gmail.com",
        "SmtpPort": 465,
        "DisplayName": "SmartEco",
        "SenderAddress": "sender@gmail.com",
        "SenderName": "sender@gmail.com",
        "Password": "password1234",
        "UseSsl": true
      }
    }
    ````
    Connecting the extension:
    ````csharp
    services.AddEmailSender(Configuration);
    ````

* **SmartEco.Common.Data** - contains all the necessary elements for interacting with the database: context, entities, migrations, repositories (for working with the context - 'Repository' pattern).

  * :bookmark_tabs: To use database repository:
      ````json
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=SmartEco;Username=postgres;Password=postgres;Port=5433"
      }
      ````
      Connecting the extension:
      ````csharp
      services.RegisterRepository(Configuration);
      ````
### Infrastructure
Contains core infrastructure projects

#### :iphone: Client :iphone:
Projects that are intended for interaction on the client side: web applications, WFP, WinForms, etc.

* **SmartEco.Akimato** - separate project for Akimato.

* **SmartEco.Web** - new version of web application for SmartEco written in .NET 7.

* **SmartEco** - the main version of the SmartEco web application written in .Net Core 2.2.

#### :desktop_computer: Server :desktop_computer:
Projects that are designed to work on the server side: Gateway API, managers, etc.

* **SmartEco.Gateway** - new version of Gateway API for SmartEco written in .NET 7.

* **SmartEcoApi.Akimato** - separate API project for Akimato.

* **SmartEcoApi** - the main API version for SmartEco written in .Net Core 2.2.

#### :gear: Services :gear:
Projects that run as standalone applications and as background services: Console apps, Windows services, daemons, etc.

* **Clarity** - data collection apps for Clarity posts.

* **GetPollutersData** - data collection applications for pollution sources.

* **GetPostsData** - performs several basic functions:
  * parsing received data for Ecoservice posts;
  * data averaging;
  * insert data to inactive posts;
  * insert "Lead" to Ecoservice Shymkent posts;
  * backup data;
  * data checking for sending deviations from the norm by e-mail;
  * writing AQI data to a file for download by Almaty;
  * sending Excel report for Zhanats posts

* **Kazhydromet** - data collection applications for Kazhydromet posts.

* **LayersCreator** - application for creating layers of visual pollution based on data.

* **TCPListener** - application to get data for Ecoservice posts in raw form.

## tests
Contains projects for application testing: unit tests (xUnit), integration tests, load tests.
