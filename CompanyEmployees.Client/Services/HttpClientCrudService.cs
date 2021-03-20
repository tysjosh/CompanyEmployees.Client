using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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
			var companies = JsonSerializer.Deserialize<List<CompanyDto>>(content, _options);

        }
	}
}
