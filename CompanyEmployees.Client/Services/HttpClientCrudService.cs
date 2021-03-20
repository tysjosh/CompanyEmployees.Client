using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CompanyEmployees.Client.Services
{
    public class HttpClientCrudService : IHttpClientServiceImplementation
	{
        private static readonly HttpClient _httpClient = new HttpClient();
		private readonly JsonSerializerOptions _options;


		public HttpClientCrudService()
        {
			_httpClient.BaseAddress = new Uri("https://localhost:5001/api/");
			_httpClient.Timeout = new TimeSpan(0, 0, 30);
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			_httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("text/xml"));
			_options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
        }
		public async Task Execute()
		{
			await GetCompanies();
		}

		public async Task GetCompanies()
        {
			var response = await _httpClient.GetAsync("companies");
			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync();

			var companies = new List<CompanyDto>();

			if(response.Content.Headers.ContentType.MediaType == "application/json")
            {
				companies = JsonSerializer.Deserialize<List<CompanyDto>>(content, _options);
			}
			else if(response.Content.Headers.ContentType.MediaType == "text/xml")
            {
				var doc = XDocument.Parse(content);
				foreach(var element in doc.Descendants())
                {
					element.Attributes().Where(a => a.IsNamespaceDeclaration).Remove();
					element.Name = element.Name.LocalName;
                }

				var serializer = new XmlSerializer(typeof(List<CompanyDto>));
				companies = (List<CompanyDto>)serializer.Deserialize(new StringReader(doc.ToString()));

            }

        }
	}
}
