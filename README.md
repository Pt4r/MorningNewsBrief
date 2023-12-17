# Morning News Briefing APP
<sub> by Stratos Palaiologos </sub>

Check the project's [documentation](https://morning-news-brief.azurewebsites.net/api-docs) or dive into [swagger docs](https://morning-news-brief.azurewebsites.net/docs) 

## Add a new API
To add a new API follow the steps bellow:
1. Create a new proxy service at `MorningNewsBrief.Common.Services`
2. Add proxy to DI
3. Add new proxy folder under Models/Proxies with the name of the proxy
4. Add new `Filters` folder under the new proxy folder and create the api Filter model
5. Create the Api DTO under the new proxy folder
6. Create the Api Response model
7. Edit the proxy service
8. At the `MorningNewsBrief.Common.Services.NewsBriefService` edit the `GetNewsBriefing()` method and add the following:
   1. A new task to get the data
   2. Add the date to the main DTO
   3. Add a new SetCached method with the new api model and filter
