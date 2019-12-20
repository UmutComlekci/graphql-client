using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.Client.Http {

	public class GraphQLHttpClient : IDisposable, IGraphQLClient {
		public Uri EndPoint { get; set; }

		public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions {
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		private readonly GraphQLHttpClientOptions _options;
		private readonly HttpClient _httpClient;

		/// <summary>Whether the instance has been disposed.</summary>
		private bool isDisposed;

		public GraphQLHttpClient(string endPoint) : this(new Uri(endPoint)) { }

		public GraphQLHttpClient(Uri endPoint) : this(endPoint, new HttpClient()) { }

		public GraphQLHttpClient(string endPoint, HttpClient httpClient) : this(new Uri(endPoint), httpClient) { }

		public GraphQLHttpClient(Uri endPoint, HttpClient httpClient) {
			EndPoint = endPoint;
			_httpClient = httpClient;
		}

		public GraphQLHttpClient(string endPoint, GraphQLHttpClientOptions options) : this(new Uri(endPoint), options) { }

		public GraphQLHttpClient(Uri endPoint, GraphQLHttpClientOptions options) : this(endPoint, options, new HttpClient()) { }

		public GraphQLHttpClient(string endPoint, GraphQLHttpClientOptions options, HttpClient httpClient)
			: this(new Uri(endPoint), options, httpClient) { }

		public GraphQLHttpClient(Uri endPoint, GraphQLHttpClientOptions options, HttpClient httpClient) {
			EndPoint = endPoint;
			_httpClient = httpClient;
			_options = options;
		}

		public async Task<GraphQLHttpResponse<TResponse>> SendHttpQueryAsync<TVariable, TResponse>(GraphQLHttpRequest<TVariable> request, CancellationToken cancellationToken = default) {
			using var httpRequestMessage = GenerateHttpRequestMessage(request);
			using var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);
			if (!httpResponseMessage.IsSuccessStatusCode) {
				throw new GraphQLHttpException(httpResponseMessage);
			}

			var bodyStream = await httpResponseMessage.Content.ReadAsStreamAsync();
			return await JsonSerializer.DeserializeAsync<GraphQLHttpResponse<TResponse>>(bodyStream, JsonSerializerOptions, cancellationToken);
		}

		public async Task<GraphQLHttpResponse<TResponse>> SendHttpQueryAsync<TResponse>(GraphQLHttpRequest request, CancellationToken cancellationToken = default) =>
			await SendHttpQueryAsync<dynamic, TResponse>(request, cancellationToken);

		public async Task<GraphQLHttpResponse<TResponse>> SendHttpMutationAsync<TVariable, TResponse>(GraphQLHttpRequest<TVariable> request, CancellationToken cancellationToken = default) {
			await Task.CompletedTask;
			throw new NotImplementedException();
		}

		public async Task<GraphQLResponse<TResponse>> SendQueryAsync<TVariable, TResponse>(GraphQLRequest<TVariable> request, CancellationToken cancellationToken = default) {
			await Task.CompletedTask;
			throw new NotImplementedException();
		}

		public async Task<GraphQLResponse<TResponse>> SendMutationAsync<TVariable, TResponse>(GraphQLRequest<TVariable> request, CancellationToken cancellationToken = default) {
			await Task.CompletedTask;
			throw new NotImplementedException();
		}

		private HttpRequestMessage GenerateHttpRequestMessage<T>(GraphQLRequest<T> request) {
			return new HttpRequestMessage(HttpMethod.Post, EndPoint) {
				Content = new StringContent(JsonSerializer.Serialize(request, JsonSerializerOptions), Encoding.UTF8, "application/json")
			};
		}

		public async Task<GraphQLResponse<R>> SendQueryAsync<R>(GraphQLRequest request, CancellationToken cancellationToken = default) {
			await Task.CompletedTask;
			throw new NotImplementedException();
		}

		public async Task<GraphQLResponse<R>> SendMutationAsync<R>(GraphQLRequest request, CancellationToken cancellationToken = default) {
			await Task.CompletedTask;
			throw new NotImplementedException();
		}

		public void Dispose() => Dispose(true);

		/// <summary>Free resources used by the client.</summary>
		/// <param name="isDisposing">Whether the dispose method was explicitly called.</param>
		protected virtual void Dispose(bool isDisposing) {
			if (isDisposed)
				return;

			if (isDisposing)
				_httpClient.Dispose();

			isDisposed = true;
		}
	}
}
