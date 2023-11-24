using Bogus;
using Marten;
using PerformanceTestOfMartenDBAndOpenSearch.Models.PostEntity;

namespace PerformanceTestOfMartenDBAndOpenSearch.Infrastructure.MartenDbService
{
    class MartenDbPostService 
    {
        private readonly IDocumentStore _documentStore;

        public MartenDbPostService(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task<List<PostEntity>> GetAllDetailsAsync()
        {
            //await using var session = _documentStore.QuerySession();
            await using var session = await _documentStore.QuerySerializableSessionAsync();
            var result = await session.Query<PostEntity>().ToListAsync();
            return result.ToList();
        }

        public async Task GetRecordByChannelOwnerObjectTypeWithEventNameAsync(string channelObjectType, string channelOwnerType, string eventName)
        {
            //await using var session = _documentStore.QuerySession();
            await using var session = await _documentStore.QuerySerializableSessionAsync();
            await session.Query<PostEntity>()
                .Where(data => data.ChannelObject.Type == channelObjectType)
                .Where(data => data.ChannelOwner.Type == channelOwnerType)
                .Where(data => data.EventName == eventName)
                .Take(1000)
                .ToListAsync();
        }

        public async Task AddPostsAsync(IEnumerable<PostEntity> postEntities)
        {
            //await using var session = _documentStore.LightweightSession();
            await using var session = await _documentStore.LightweightSerializableSessionAsync();
            session.Store(postEntities);
            await session.SaveChangesAsync();
            Console.WriteLine("data added");
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

            PostEntity postEntity = faker.Generate(1).First();
            postEntity.Comments = data;

            //await using var session = _documentStore.LightweightSession();
            await using var session = await _documentStore.LightweightSerializableSessionAsync();
            session.Store(postEntity);
            await session.SaveChangesAsync();
        }

        public async Task AddCommentsInPostAsync(Guid postId)
        {
            //await using var session = _documentStore.LightweightSession();
            //var post = session.Query<PostEntity>().FirstOrDefault(p => p.PostId == postId);
            await using var session = await _documentStore.LightweightSerializableSessionAsync();
            var post = await session.Query<PostEntity>().FirstOrDefaultAsync(p => p.PostId == postId);

            if (post != null)
            {
                var commentFake = new Faker<Comments>()
                    .RuleFor(o => o.Comment, f => f.Lorem.Paragraph(5))
                    .RuleFor(o => o.UserName, f => f.Random.Word())
                    .RuleFor(o => o.CreatedDate, DateTime.Now);
                var data = commentFake.Generate(1);
                post.Comments.Add(data.First());

                session.Store(post);
                await session.SaveChangesAsync();
            }
        }

        public async Task GetCommentsByPostIdAsync(Guid postId)
        {
            //await using var session = _documentStore.QuerySession();
            //await session.Query<PostEntity>().FirstOrDefaultAsync(p => p.PostId == postId);
            await using var session = await _documentStore.QuerySerializableSessionAsync();
            await session.Query<PostEntity>().FirstOrDefaultAsync(p => p.PostId == postId);
        }
    }
}