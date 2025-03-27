using Flix.Stores.ProviderMappings;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Flix.Stores.Models
{
	public class Movie
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public int Id { get; set; }

		[BsonElement("title")]
		public required string Title { get; set; }

		[BsonElement("director")]
		public string Director { get; set; }

		[BsonElement("releaseYear")]
		public int ReleaseYear { get; set; }

		[BsonElement("genre")]
		public string Genre { get; set; }

		[BsonElement("runTime")]
		public int? RunTime { get; set; }

		[BsonElement("coverImage")]
		public string CoverImage { get; set; }

		[BsonElement("trailer")]
		public string Trailer { get; set; }

		[BsonElement("media")]
		public List<string> Media { get; set; }

		[BsonElement("actors")]
		public List<string> Actors { get; set; }

		[BsonElement("providerIds")]
		public Dictionary<Provider, string> ProviderIds { get; set; }
	}
}