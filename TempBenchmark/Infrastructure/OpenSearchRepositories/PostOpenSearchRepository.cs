using Bogus;
using OpenSearch.Client;
using PerformanceTestOfMartenDBAndOpenSearch.Infrastructure.OpenSearchConfigurations;
using PerformanceTestOfMartenDBAndOpenSearch.Models.PostEntity;

namespace PerformanceTestOfMartenDBAndOpenSearch.Infrastructure.OpenSearchRepositories
{
    public class PostOpenSearchRepository<TDocument>  where TDocument : class
    {
        private readonly IOpenSearchClientConnection _openSearchClient;

        public PostOpenSearchRepository(IOpenSearchClientConnection openSearchClient)
        {
            _openSearchClient = openSearchClient;
        }

        public async Task AddPostAsync(Guid postId)
        {
            var faker = new Faker<PostEntity>()
                .RuleFor(o => o.PostId, f => f.PickRandom(new List<Guid>() {postId}))
                .RuleFor(o => o.EventName, f => f.PickRandom(new List<string>
                {
                    "OfferAssigned",
                    "OfferAccepted"
                }))
                .RuleFor(o => o.Post, f => f.Lorem.Paragraph())
                .RuleFor(o => o.Channel, f => f.Company.CompanyName())
                .RuleFor(o => o.ChannelObject, f => new ChannelObject
                {
                    Id = f.Random.Int(1, 100),
                    Type = f.PickRandom(new List<string>
                    {
                        "assignment",
                        "transport"
                    })
                })
                .RuleFor(o => o.ChannelOwner, f => new ChannelOwner
                {
                    Id = f.Random.Int(1, 100),
                    Type = f.PickRandom(new List<string>
                    {
                        "carrier",
                        "buyer",
                    })
                });

            var commentFake = new Faker<Comments>()
                .RuleFor(o => o.Comment, f => f.Lorem.Paragraph(5))
                .RuleFor(o => o.UserName, f => f.Random.Word())
                .RuleFor(o => o.CreatedDate, DateTime.Now);

            var data = commentFake.Generate(5);

            IEnumerable<PostEntity> postEntities = faker.Generate(1);
            foreach (var postEntity in postEntities)
            {
                postEntity.Comments = data;
            }

            await _openSearchClient.IndexDocumentAsync(postEntities.FirstOrDefault());
        }

        public async Task AddCommentsInPostAsync(Guid postId)
        {
            var searchResponse = await _openSearchClient.SearchAsync<PostEntity>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            m => m.Match(mt => mt.Field(f => f.PostId).Query(postId.ToString()).Operator(Operator.And))
                        )
                    )
                )
            );

            var post = searchResponse.Documents.FirstOrDefault();
            if (post != null)
            {
                post.Comments ??= new List<Comments>();

                var commentFake = new Faker<Comments>()
                    .RuleFor(o => o.Comment, f => f.Lorem.Paragraph(5))
                    .RuleFor(o => o.UserName, f => f.Random.Word())
                    .RuleFor(o => o.CreatedDate, DateTime.Now);
                var data = commentFake.Generate(1);

                post.Comments.Add(data.FirstOrDefault());

                var response = _openSearchClient.IndexDocument(post);
            }
        }


        public async Task GetRecordByChannelOwnerObjectTypeWithEventNameAsync(string channelObjectType, string channelOwnerType, string eventName)
        {
            var searchReponse = await _openSearchClient.SearchAsync<PostEntity>(s => s
                .Size(1000)
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            m => m.Match(mt => mt.Field(f => f.ChannelObject.Type).Query(channelObjectType).Operator(Operator.And)),
                            m => m.Match(mt => mt.Field(f => f.ChannelOwner.Type).Query(channelOwnerType).Operator(Operator.And)),
                            m => m.Match(mt => mt.Field(f => f.EventName).Query(eventName).Operator(Operator.And))
                        )
                    ))
            );
        }


        public async Task GetCommentsByPostIdAsync(Guid postId)
        {
            var searchReponse = await _openSearchClient.SearchAsync<PostEntity>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            m => m.Match(mt => mt.Field(f => f.PostId).Query(postId.ToString()).Operator(Operator.And))
                        )
                    )
                )
            );
        }

        public async Task AddPostsAsync(IEnumerable<PostEntity> posts)
        {
            await _openSearchClient.IndexManyAsync(posts);
            Console.WriteLine("data added");

        }
    }
}