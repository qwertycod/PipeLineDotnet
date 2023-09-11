using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using System;
using System.Net;
using System.Text;
using System.Text.Json;

namespace StudentCI.Tests
{
    public class StudentApiTests : IAsyncLifetime, IDisposable
    {
        private int _id = new Random().Next(0, 9999);
        private const ushort HttpPort = 80;

        private readonly CancellationTokenSource _cts = new(TimeSpan.FromMinutes(1));

        private readonly IDockerNetwork _network;

        private readonly IDockerContainer _dbContainer;

        private readonly IDockerContainer _appContainer;

        public StudentApiTests()
        {
            _network = new TestcontainersNetworkBuilder()
                .WithName(Guid.NewGuid().ToString("D"))
                .Build();

            _dbContainer = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage("postgres")
                .WithNetwork(_network)
                .WithNetworkAliases("db")
                .WithVolumeMount("postgres-data", "/var/lib/postgresql/data")
                .Build();

            _appContainer = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage("qwertycod/clockbox:latest")      // clockbox or change your image name here
                .WithNetwork(_network)
                .WithPortBinding(HttpPort, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(HttpPort))
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _network.CreateAsync(_cts.Token)
                .ConfigureAwait(false);

            await _dbContainer.StartAsync(_cts.Token)
                .ConfigureAwait(false);

            await _appContainer.StartAsync(_cts.Token)
                .ConfigureAwait(false);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _cts.Dispose();
        }

        //[Fact]
        //public async Task Test_check_if_api_is_running()
        //{
        //    using var httpClient = new HttpClient();
        //    httpClient.BaseAddress = new UriBuilder("http", _appContainer.Hostname, _appContainer.GetMappedPublicPort(HttpPort)).Uri;

        //    var httpResponseMessage = await httpClient.GetAsync("bird/test/3")
        //        .ConfigureAwait(false);

        //    var body = await httpResponseMessage.Content.ReadAsStringAsync()
        //        .ConfigureAwait(false);

        //    Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        //    Assert.Contains("9", body);
        //}

        [Fact]
        public async Task TestPostGetBird()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new UriBuilder("http", _appContainer.Hostname, _appContainer.GetMappedPublicPort(HttpPort)).Uri;

            var bird = new Bird { id = 101, name = "Sparrow" };
            string birdJson = JsonSerializer.Serialize(bird);

            // Create an HttpContent object with the serialized JSON data
            HttpContent content = new StringContent(birdJson, Encoding.UTF8, "application/json");

            var httpResponseMessage = await httpClient.PostAsync("bird/Add", content)
                .ConfigureAwait(false);

            var body = await httpResponseMessage.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Assert.NotEmpty(body);

            var httpResponseMessage1 = await httpClient.GetAsync("bird/" + 101)
              .ConfigureAwait(false);

            var body1 = await httpResponseMessage1.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage1.StatusCode);
            Assert.Contains("Sparrow", body1);
        }

        [Fact]
        public async Task Test_check_if_product_api_is_running()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new UriBuilder("http", _appContainer.Hostname, _appContainer.GetMappedPublicPort(HttpPort)).Uri;

            var httpResponseMessage = await httpClient.GetAsync("product")
                .ConfigureAwait(false);

            var body = await httpResponseMessage.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Test_check_if_product_api_post_is_running()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new UriBuilder("http", _appContainer.Hostname, _appContainer.GetMappedPublicPort(HttpPort)).Uri;

            var product = new Product { id = 101, name = "Apple" };
            string productJson = JsonSerializer.Serialize(product);

            // Create an HttpContent object with the serialized JSON data
            HttpContent content1 = new StringContent(productJson, Encoding.UTF8, "application/json");

            var httpResponseMessage = await httpClient.PostAsync("product/Add", content1)
                .ConfigureAwait(false);

            var body = await httpResponseMessage.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Assert.NotEmpty(body);
        }
    }

    public class Bird
    {
        public int id { get; set; }
        public string? name { get; set; }
    }

    public class Product
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? category { get; set; }
        public decimal price { get; set; }
    }
}