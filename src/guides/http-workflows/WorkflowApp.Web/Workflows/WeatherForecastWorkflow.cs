using System.Net;
using System.Net.Mime;
using System.Text;
using Elsa.Http;
using Elsa.Http.Options;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;
using Microsoft.Extensions.Options;

namespace WorkflowApp.Web.Workflows;

public class WeatherForecastWorkflow : WorkflowBase
{
    private readonly Uri _serverAddress;

    public WeatherForecastWorkflow(IOptions<HttpActivityOptions> options)
    {
        _serverAddress = options.Value.BaseUrl;
    }
    
    protected override void Build(IWorkflowBuilder builder)
    {
        var weatherForecastApiUrl = new Uri(_serverAddress, "weatherforecast");
        var weatherForecastResponseVariable = builder.WithVariable<ICollection<WeatherForecast>>();

        builder.Root = new Sequence
        {
            Activities =
            {
                // Expose this workflow as an HTTP endpoint.
                new HttpEndpoint
                {
                    Path = new("/weatherforecast"),
                    SupportedMethods = new(new[] { HttpMethods.Get }),
                    CanStartWorkflow = true
                },

                // Invoke another API endpoint. Could be a remote server, but here we are invoking an API hosted in the same app.
                new SendHttpRequest
                {
                    Url = new(weatherForecastApiUrl),
                    Method = new(HttpMethods.Get),
                    ParsedContent = new(weatherForecastResponseVariable)
                },

                // Write back the weather forecast.
                new WriteHttpResponse
                {
                    ContentType = new(MediaTypeNames.Text.Html),
                    StatusCode = new(HttpStatusCode.OK),
                    Content = new(context =>
                    {
                        var weatherForecasts = weatherForecastResponseVariable.Get(context)!;
                        var sb = new StringBuilder();

                        sb.AppendLine(
                            """
<!doctype html>
    <html>
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <script src="https://cdn.tailwindcss.com"></script>
    </head>
    <body>
        <div class="px-4 sm:px-6 lg:px-8">
        <div class="mt-8 flex flex-col">
        <div class="-my-2 -mx-4 overflow-x-auto sm:-mx-6 lg:-mx-8">
          <div class="inline-block min-w-full py-2 align-middle md:px-6 lg:px-8">
            <div class="overflow-hidden shadow ring-1 ring-black ring-opacity-5 md:rounded-lg">
              <table class="min-w-full divide-y divide-gray-300">
                <thead class="bg-gray-50">
                  <tr>
                    <th scope="col" class="py-3.5 pl-4 pr-3 text-left text-sm font-semibold text-gray-900 sm:pl-6">Date</th>
                    <th scope="col" class="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Temperature (C/F)</th>
                    <th scope="col" class="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Summary</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-gray-200 bg-white">
""");
                        foreach (var weatherForecast in weatherForecasts)
                        {
                            sb.AppendLine("<tr>");
                            sb.AppendLine($"""<td class="whitespace-nowrap py-4 pl-4 pr-3 text-sm font-medium text-gray-900 sm:pl-6">{weatherForecast.Date}</td>""");
                            sb.AppendLine($"""<td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500">{weatherForecast.TemperatureC}/{weatherForecast.TemperatureF}</td>""");
                            sb.AppendLine($"""<td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500">{weatherForecast.Summary}</td>""");
                            sb.AppendLine("</tr>");
                        }

                        sb.AppendLine(
"""
                </tbody>
            </table>
        </div>
        </div>
        </div>
        </div>
    </body>
    </html>
""");
                        return sb.ToString();
                    })
                }
            }
        };
    }
}