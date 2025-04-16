namespace Flix.ServiceModel.Models
{
	public class Movie
	{
		public required string Id { get; set; }
		public string Title { get; set; }
		public string Director { get; set; }
		public int ReleaseYear { get; set; }
		public string? Genre { get; set; }
		public double AverageRating { get; set; }
	}
}