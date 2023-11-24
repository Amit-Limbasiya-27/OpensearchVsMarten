using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Marten;
using Npgsql;
using OpenSearch.Client;
using PerformanceTestOfMartenDBAndOpenSearch.BenchmarkTest;
using PerformanceTestOfMartenDBAndOpenSearch.Infrastructure.MartenDbService;
using PerformanceTestOfMartenDBAndOpenSearch.Infrastructure.OpenSearchRepositories;
using PerformanceTestOfMartenDBAndOpenSearch.Models.PostEntity;
using UtilityToInsertDataFromCSVtoOpenSearchDB.Infrastructure.OpenSearchConfigurations;
using Weasel.Core;

namespace PerformanceTestOfMartenDBAndOpenSearch
{
    class Program
    {

        private static async Task Main(string[] args)
        {
            ////FakePostService _fakePostService;
            //MartenDbPostService _martenDbPostService;
            //IDocumentStore _documentStore;
            //PostOpenSearchRepository<PostEntity> _postOpenSearchRepository;

            ////make the database if not exist
            //bool databaseExists=false;
            //var databaseName = "testmarten";

            //// Connection string of docker
            //var connectionString = "Host=marten-postgres;Username=marten-postgres;Password=121212;Port=5432;IntegratedSecurity=true;Pooling=true;";

            //// Check if the database exists, and create it if necessary
            //using (var connection = new NpgsqlConnection(connectionString))
            //{
            //    connection.Open();
            //    var databaseExistsQuery = $"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'";
            //    using (var command = new NpgsqlCommand(databaseExistsQuery, connection))
            //    {
            //        databaseExists = command.ExecuteScalar() != null;

            //        if (!databaseExists)
            //        {
            //            Console.WriteLine("database not found.");
            //            // Create the database
            //            var createDatabaseQuery = $"CREATE DATABASE {databaseName}";
            //            using (var createCommand = new NpgsqlCommand(createDatabaseQuery, connection))
            //            {
            //                createCommand.ExecuteNonQuery();
            //                Console.WriteLine("database is created.");
            //            }
            //        }
            //    }
            //}

            //_documentStore = DocumentStore.For(options =>
            //    {
            //        options.Connection("Server=marten-postgres;User ID=marten-postgres;Password=121212;Database=testmarten;Port=5432;IntegratedSecurity=true;Pooling=true;"); // docker
            //        options.AutoCreateSchemaObjects = AutoCreate.All;
            //        options.Schema.For<PostEntity>().Identity(x => x.PostId).FullTextIndex();
            //    });
            //;
            //_martenDbPostService = new MartenDbPostService(_documentStore);

            //_postOpenSearchRepository = new PostOpenSearchRepository<PostEntity>(new OpenSearchClientConnection(new ConnectionSettings(new Uri("https://openserarch:9200"))
            //    .DefaultIndex("feed_post_data1").BasicAuthentication("admin", "admin").ServerCertificateValidationCallback((_, _, _, _) => true)));
            ////_fakePostService = new();

            ////for (int i = 0; i < 100; i++)
            ////{
            ////    var posts = _fakePostService.GenerateFakePosts();
            ////    await _martenDbPostService.AddPostsAsync(posts);
            ////    //await _postOpenSearchRepository.AddPostsAsync(posts);
            ////}
            BenchmarkRunner.Run<Benchmarks>(new DebugInProcessConfig());
        }
    }
}