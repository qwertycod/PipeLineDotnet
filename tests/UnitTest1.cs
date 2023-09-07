using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using System.Net;
using System.Text;
using System.Text.Json;

namespace StudentCI.Tests
{
    public class StudentApiTests : IAsyncLifetime, IDisposable
    {

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
                .WithImage("qwertycod/clockbox:latest")      // change your image name here
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

        [Fact]
        public async Task Test1()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new UriBuilder("http", _appContainer.Hostname, _appContainer.GetMappedPublicPort(HttpPort)).Uri;

            var httpResponseMessage = await httpClient.GetAsync("Student/test/3")
                .ConfigureAwait(false);

            var body = await httpResponseMessage.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            Assert.Contains("9", body);
        }

        [Fact]
        public async Task TestGet()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new UriBuilder("http", _appContainer.Hostname, _appContainer.GetMappedPublicPort(HttpPort)).Uri;

            var httpResponseMessage = await httpClient.GetAsync("student")
                .ConfigureAwait(false);

            var body = await httpResponseMessage.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task TestGet1()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new UriBuilder("http", _appContainer.Hostname, _appContainer.GetMappedPublicPort(HttpPort)).Uri;

            var httpResponseMessage = await httpClient.GetAsync("student/2")
                .ConfigureAwait(false);

            var body = await httpResponseMessage.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            Assert.Contains("Wayne", body);
        }

        [Fact]
        public async Task TestPost()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new UriBuilder("http", _appContainer.Hostname, _appContainer.GetMappedPublicPort(HttpPort)).Uri;

            var student = new Student { ID = 17, FirstMidName = "Peter", LastName = "Parker" };
            string studentJson = JsonSerializer.Serialize(student);

            // Create an HttpContent object with the serialized JSON data
            HttpContent content = new StringContent(studentJson, Encoding.UTF8, "application/json");

            var httpResponseMessage = await httpClient.PostAsync("student/Add", content)
                .ConfigureAwait(false);

            var body = await httpResponseMessage.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Assert.NotEmpty(body);
        }
    }

public class Student
{
    public int ID { get; set; }
    public string? LastName { get; set; }
    public string? FirstMidName { get; set; }
    public DateTime EnrollmentDate { get; set; }
}
}
