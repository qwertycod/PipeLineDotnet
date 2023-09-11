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
                .WithImage("qwertycod/clockbox:latest")      // dockerimagestudentci or change your image name here
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


        // will run on local machine only, uncomment to run
        [Fact]
        public async Task TestGet0()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new UriBuilder("http", _appContainer.Hostname, _appContainer.GetMappedPublicPort(HttpPort)).Uri;

            var httpResponseMessage = await httpClient.GetAsync("student")
                .ConfigureAwait(false);

            var body = await httpResponseMessage.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        }

        //// will run on local machine only, uncomment to run
        //[Fact]
        //public async Task TestPostGetStudents()
        //{
        //    using var httpClient = new HttpClient();
        //    httpClient.BaseAddress = new UriBuilder("http", _appContainer.Hostname, _appContainer.GetMappedPublicPort(HttpPort)).Uri;

        //    var student = new Student { id = _id, firstmidname = "Ryan", lastname = "Parker" };
        //    string studentJson = JsonSerializer.Serialize(student);

        //    // Create an HttpContent object with the serialized JSON data
        //    HttpContent content = new StringContent(studentJson, Encoding.UTF8, "application/json");

        //    var httpResponseMessage = await httpClient.PostAsync("student/Add", content)
        //        .ConfigureAwait(false);

        //    var body = await httpResponseMessage.Content.ReadAsStringAsync()
        //        .ConfigureAwait(false);

        //    Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
        //    Assert.NotEmpty(body);

        //    var httpResponseMessage1 = await httpClient.GetAsync("student/" + _id)
        //      .ConfigureAwait(false);

        //    var body1 = await httpResponseMessage1.Content.ReadAsStringAsync()
        //        .ConfigureAwait(false);

        //    Assert.Equal(HttpStatusCode.OK, httpResponseMessage1.StatusCode);
        //    Assert.Contains("Ryan", body1);
        //}

        // will run on local machine only, uncomment to run
        [Fact]
        public async Task TestPostGetBird()
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new UriBuilder("http", _appContainer.Hostname, _appContainer.GetMappedPublicPort(HttpPort)).Uri;

            var bird = new Bird { id = _id, name = "Sparrow" };
            string birdJson = JsonSerializer.Serialize(bird);

            // Create an HttpContent object with the serialized JSON data
            HttpContent content = new StringContent(birdJson, Encoding.UTF8, "application/json");

            var httpResponseMessage = await httpClient.PostAsync("bird/Add", content)
                .ConfigureAwait(false);

            var body = await httpResponseMessage.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Assert.NotEmpty(body);

            var httpResponseMessage1 = await httpClient.GetAsync("bird/" + _id)
              .ConfigureAwait(false);

            var body1 = await httpResponseMessage1.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage1.StatusCode);
            Assert.Contains("Sparrow", body1);
        }

    }


    public class Student
    {
        public int id { get; set; }
        public string? lastname { get; set; }
        public string? firstmidname { get; set; }
        public DateTime enrollmentdate { get; set; }
    }

    public class Bird
    {
        public int id { get; set; }
        public string? name { get; set; }
    }
}
