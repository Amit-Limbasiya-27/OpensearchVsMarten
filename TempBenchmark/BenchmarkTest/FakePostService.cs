using Bogus;
using PerformanceTestOfMartenDBAndOpenSearch.Models.PostEntity;

namespace PerformanceTestOfMartenDBAndOpenSearch.BenchmarkTest;

public class FakePostService
{
    public IEnumerable<PostEntity> GenerateFakePosts()
    {
         var faker = new Faker<PostEntity>()
                .RuleFor(o => o.PostId, faker => faker.Random.Guid())
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
                .RuleFor(o => o.Comment, f => f.Random.Word())
                .RuleFor(o => o.UserName, f => f.Random.Word())
                .RuleFor(o => o.CreatedDate, DateTime.Now);

            var data = commentFake.Generate(5);

            IEnumerable<PostEntity> postEntities = faker.Generate(50000);
            foreach (var postEntity in postEntities)
            {
                postEntity.Comments = data;
            }

        return postEntities;
    }
}