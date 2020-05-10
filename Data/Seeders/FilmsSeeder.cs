using Data.Enums;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Seeders
{
    public class FilmsSeeder
    {
        public async Task SeedAsync(CinemaDbContext context)
        {
            if (context.Films.Any())
            {
                return;
            }

            var filmsData = new List<FilmData>()
            {
                new FilmData{
                Title = "Joker",
                Description = "In Gotham City, mentally troubled comedian Arthur Fleck is disregarded and mistreated" +
                " by society. He then embarks on a downward spiral of revolution and bloody crime. This path brings " +
                "him face-to-face with his alter-ego: the Joker. ",
                Genre = new[] { new GenreType { Genre = Genre.Drama },
                                new GenreType {Genre=Genre.Crime },
                                new GenreType{Genre=Genre.Thriller}},
                Poster = "https://m.media-amazon.com/images/M/MV5BNGVjNWI4ZGUtNzE0MS00YTJmLWE0ZDctN2ZiYTk2YmI3NTYyXkEyXkFqcGdeQXVyMTkxNjUyNQ@@._V1_SY1000_CR0,675_AL_.jpg",
                Runtime=122,
                Director="Todd Phillips",
                Cast="Joaquin Phoenix, Robert De Niro, Zazie Beetz",
                ReleaseDate=new DateTime(2019,10,4),
                TargetAudience= TargetAudience.Over12,
                },

                new FilmData{
                Title = "The Gentlemen",
                Description = "An American expat tries to sell off his highly profitable marijuana empire in London," +
                              " triggering plots, schemes, bribery and blackmail in an attempt to steal his domain" +
                              " out from under " +
                "him. ",
                Genre = new[] { new GenreType { Genre = Genre.Action },
                                new GenreType {Genre=Genre.Crime },
                                new GenreType{Genre=Genre.Comedy}},
                Poster = "https://m.media-amazon.com/images/M/MV5BMTlkMmVmYjktYTc2NC00ZGZjLWEyOWUtMjc2MDMwMjQwOTA5XkEyXkFqcGdeQXVyNTI4MzE4MDU@._V1_SY1000_SX675_AL_.jpg",
                Runtime=113,
                Director="Guy Ritchie",
                Cast="Matthew McConaughey, Charlie Hunnam, Michelle Dockery",
                ReleaseDate=new DateTime(2020,1,31),
                TargetAudience= TargetAudience.Over12,
                },

                new FilmData{
                Title = "Narcos",
                Description = "A chronicled look at the criminal exploits of Colombian drug lord Pablo Escobar," +
                              " as well as the many other drug kingpins who plagued the country through the years. ",
                Genre = new[] { new GenreType { Genre = Genre.Drama },
                                new GenreType {Genre=Genre.Crime },
                                new GenreType{Genre=Genre.Thriller}},
                Poster = "https://m.media-amazon.com/images/M/MV5BOWVkMzY0YzctMGQzZS00YTEwLThmMzktMGU3MGJkZmJhNGNjXkEyXkFqcGdeQXVyNTc4MjczMTM@._V1_SY1000_CR0,0,675,1000_AL_.jpg",
                Runtime=49,
                Director="Carlo Bernard",
                Cast="Pedro Pascal, Wagner Moura, Boyd Holbrook",
                ReleaseDate=new DateTime(2017,4,20),
                TargetAudience= TargetAudience.Over16,
                },

                 new FilmData{
                Title = "Breaking Bad",
                Description = "A high school chemistry teacher diagnosed with inoperable lung cancer turns to manufacturing" +
                              " and selling methamphetamine in order to secure his family's future. ",
                Genre = new[] { new GenreType { Genre = Genre.Drama },
                                new GenreType {Genre=Genre.Crime },
                                new GenreType{Genre=Genre.Thriller}},
                Poster = "https://m.media-amazon.com/images/M/MV5BMjhiMzgxZTctNDc1Ni00OTIxLTlhMTYtZTA3ZWFkODRkNmE2XkEyXkFqcGdeQXVyNzkwMjQ5NzM@._V1_SY1000_CR0,0,718,1000_AL_.jpg",
                Runtime=49,
                Director="Vince Gilligan",
                Cast="Bryan Cranston, Anna Gunn, Aaron Paul",
                ReleaseDate=new DateTime(2010,12,19),
                TargetAudience= TargetAudience.All,
                }
            };

            var randomNumber = new Random();

            var userId = context.Users.FirstOrDefault(x => x.UserName == "Admin")?.Id;
            foreach (var filmData in filmsData)
            {
                var rating = randomNumber.Next(4, 6);
                var film = new Film
                {
                    Rating = rating,
                    AddedByUserId = userId,
                    Ratings = new List<FilmRating> { new FilmRating { UserId = userId, Rating = rating } }
                };
                filmData.FilmId = film.Id;
                await context.Films.AddAsync(film);
                await context.FilmDatas.AddAsync(filmData);
            }

            await context.SaveChangesAsync();
        }
    }
}
