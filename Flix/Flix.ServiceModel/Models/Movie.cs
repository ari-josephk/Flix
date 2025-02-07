namespace Flix.ServiceModel.Models
{
	public class Movie
	{
		public required int Id { get; set; }
		public string Title { get; set; }
		public string Director { get; set; }
		public DateTime ReleaseDate { get; set; }
		public string? Genre { get; set; }
		public double AverageRating { get; set; }
	}
}