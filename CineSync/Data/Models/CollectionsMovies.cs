﻿namespace CineSync.Data.Models
{
    public class CollectionsMovies : Item
    {
        public uint MovieId { get; set; }

        public uint MovieCollectionId { get; set; }

        public Movie Movie { get; set; } = null!;

        public MovieCollection MovieCollection { get; set; } = null!;

        public override bool Equals(object? obj)
        {

            if (obj == null || GetType() != obj.GetType())
                return false;

            CollectionsMovies castObj = (CollectionsMovies)obj;

            return Id == castObj.Id && MovieId == castObj.MovieId;

        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
