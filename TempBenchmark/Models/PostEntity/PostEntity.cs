using OpenSearch.Client;

namespace PerformanceTestOfMartenDBAndOpenSearch.Models.PostEntity
{
    public class PostEntity
    {
        [PropertyName("post_id")]
        public Guid PostId { get; set; }
        [PropertyName("event_name")]
        public string EventName { get; set; }
        [PropertyName("post")]
        public string Post { get; set; }
        [PropertyName("channel")]
        public string Channel { get; set; }
        [PropertyName("channel_object")]
        public ChannelObject ChannelObject { get; set; }
        [PropertyName("channel_owner")]
        public ChannelOwner ChannelOwner { get; set; }
        [PropertyName("coments")]
        public IList<Comments> Comments { get; set; }
    }
    public class Comments
    {
        [PropertyName("comment")]
        public string Comment { get; set; }
        [PropertyName("user_name")]
        public string UserName { get; set; }
        [PropertyName("created_date")]
        public DateTime CreatedDate { get; set; }
    }
    public class ChannelOwner
    {
        [PropertyName("id")]
        public int Id { get; set; }
        [PropertyName("type")]
        public string Type { get; set; }
    }
    public class ChannelObject
    {
        [PropertyName("id")]
        public int Id { get; set; }
        [PropertyName("type")]
        public string Type { get; set; }
    }
}
