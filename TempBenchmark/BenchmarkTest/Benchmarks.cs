using BenchmarkDotNet.Attributes;
using Marten;
using OpenSearch.Client;
using PerformanceTestOfMartenDBAndOpenSearch.Infrastructure.MartenDbService;
using PerformanceTestOfMartenDBAndOpenSearch.Infrastructure.OpenSearchRepositories;
using PerformanceTestOfMartenDBAndOpenSearch.Models.PostEntity;
using UtilityToInsertDataFromCSVtoOpenSearchDB.Infrastructure.OpenSearchConfigurations;
using Weasel.Core;

namespace PerformanceTestOfMartenDBAndOpenSearch.BenchmarkTest
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        private MartenDbPostService _martenDbPostService;
        private IDocumentStore _documentStore;
        private PostOpenSearchRepository<PostEntity> _postOpenSearchRepository;
        private Guid _postId;
        private List<string> _channelObjectTypes;
        private List<string> _channelOwnerTypes;
        private List<string> _eventNames;
        private Random RandomIndex;

        [GlobalSetup]
        public async Task Setup()
        {
            _documentStore = DocumentStore.For(options =>
            {
                options.Connection("Server=marten-postgres;User ID=marten-postgres;Password=121212;Database=testmarten;Port=5432;IntegratedSecurity=true;Pooling=true;"); // docker
                options.AutoCreateSchemaObjects = AutoCreate.All;
                options.Schema.For<PostEntity>().Identity(x => x.PostId).FullTextIndex();
            });
            ;
            _martenDbPostService = new MartenDbPostService(_documentStore);

            _postOpenSearchRepository = new PostOpenSearchRepository<PostEntity>(new OpenSearchClientConnection(new ConnectionSettings(new Uri("https://openserarch:9200"))
                .DefaultIndex("feed_post_data1").BasicAuthentication("admin", "admin").ServerCertificateValidationCallback((_, _, _, _) => true)));


            _postId = new Guid("00001927-2bab-4a65-0d8f-7a400fcbcedd");
            _channelObjectTypes = new List<string> {"assignment", "transport"};
            _channelOwnerTypes = new List<string> {"carrier", "buyer"};
            _eventNames = new List<string> {"OfferAssigned", "OfferAccepted"};
            RandomIndex = new Random();
        }



        #region Benchmarks

        [Benchmark]
        [BenchmarkCategory("GetPosts")]
        public async Task GetPostFromOpenSearch()
        {
            var eventNameRandomizer = RandomIndex.Next(0, _eventNames.Count);
            var channelObjectRandomizer = RandomIndex.Next(0, _channelObjectTypes.Count);
            var channelOwnerRandomizer = RandomIndex.Next(0, _channelOwnerTypes.Count);
            await _postOpenSearchRepository.GetRecordByChannelOwnerObjectTypeWithEventNameAsync(_channelObjectTypes[channelObjectRandomizer], _channelOwnerTypes[channelOwnerRandomizer], _eventNames[eventNameRandomizer]);
        }

        [Benchmark]
        [BenchmarkCategory("GetPosts")]
        public async Task GetPostFromMartenDb()
        {
            var eventNameRandomizer = RandomIndex.Next(0, _eventNames.Count);
            var channelObjectRandomizer = RandomIndex.Next(0, _channelObjectTypes.Count);
            var channelOwnerRandomizer = RandomIndex.Next(0, _channelOwnerTypes.Count);
            await _martenDbPostService.GetRecordByChannelOwnerObjectTypeWithEventNameAsync(_channelObjectTypes[channelObjectRandomizer], _channelOwnerTypes[channelOwnerRandomizer], _eventNames[eventNameRandomizer]);
        }
        [Benchmark]
        [BenchmarkCategory("InsertPost")]
        public async Task InsertPostInOpenSearch()
        {
            await _postOpenSearchRepository.AddPostAsync(_postId);
        }


        [Benchmark]
        [BenchmarkCategory("InsertPost")]
        public async Task InsertPostInMartenDb()
        {
            await _martenDbPostService.AddPostAsync(_postId);
        }

        [Benchmark]
        [BenchmarkCategory("CommentPost")]
        public async Task InsertCommentInOpenSearch()
        {
            await _postOpenSearchRepository.AddCommentsInPostAsync(_postId);
        }

        [Benchmark]
        [BenchmarkCategory("CommentPost")]
        public async Task InsertCommentInMartenDb()
        {
            await _martenDbPostService.AddCommentsInPostAsync(_postId);
        }


        [Benchmark]
        [BenchmarkCategory("GetCommentByPostId")]
        public async Task GetDocumentByPostIdOpenSearch()
        {
            await _postOpenSearchRepository.GetCommentsByPostIdAsync(_postId);
        }

        [Benchmark]
        [BenchmarkCategory("GetCommentByPostId")]
        public async Task GetDocumentByPostIdMartenDb()
        {
            await _martenDbPostService.GetCommentsByPostIdAsync(_postId);
        }
        #endregion
    }
}