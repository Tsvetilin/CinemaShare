﻿using CinemaShare.Models.InputModels;
using CinemaShare.Models.JsonModels;
using CinemaShare.Models.ViewModels;
using Data.Enums;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CinemaShare.Common.Mapping
{
    public class Mapper : IMapper
    {
        ///<summary>
        /// Makes an instance of <typeparamref name="TToModel"/>
        /// and maps all the data from <paramref name="fromObject"/>
        /// to the new instance where the propery name and type are the same
        ///</summary>
        ///<param name="fromObject"> Object to map from</param>
        ///<typeparam name="TFromModel">Model to map from</typeparam>
        ///<typeparam name="TToModel">Model to map to</typeparam>
        ///<returns>Mapped object of type <typeparamref name="TToModel"/></returns>
        private TToModel MapSimilarProperties<TFromModel, TToModel>(TFromModel fromObject)
             where TFromModel : class
             where TToModel : class, new()
        {
            TToModel viewModel = new TToModel();
            if (fromObject != null)
            {
                PropertyInfo[] props = fromObject.GetType().GetProperties();
                var modelType = viewModel?.GetType();
                foreach (var prop in props)
                {
                    var modelProperty = modelType?.GetProperty(prop.Name);
                    if (modelProperty?.PropertyType?.Equals(prop.PropertyType) ?? false)
                    {
                        modelProperty.SetValue(viewModel, prop.GetValue(fromObject));
                    }
                }
            }
            return viewModel;
        }

        ///<summary>
        /// Converts FilmData object to FilmCardViewModel
        ///</summary>
        ///<param name="filmData">FilmData object to map from </param>
        /// <returns>Mapped instance of type <see cref="FilmCardViewModel"/></returns>
        public FilmCardViewModel MapToFilmCardViewModel(FilmData filmData)
        {
            return new FilmCardViewModel
            {
                Title = filmData.Title,
                Genres = string.Join(", ", filmData.Genre.Select(a => a.Genre.ToString())),
                Poster = filmData.Poster,
                Rating = Math.Round(filmData.Film.Rating, 1).ToString(),
                Id = filmData.FilmId
            };
        }

        ///<summary>
        /// Converts FilmData object to ExtendedFilmCardViewModel
        ///</summary>
        ///<param name="filmData"> FilmData object to map from</param>
        ///<returns>Mapped instance of type <see cref="ExtendedFilmCardViewModel"/></returns>
        public ExtendedFilmCardViewModel MapToExtendedFilmCardViewModel(FilmData filmData)
        {
            var viewModel = MapSimilarProperties<FilmData, ExtendedFilmCardViewModel>(filmData);
            if (filmData != null)
            {
                viewModel.Id = filmData.FilmId;
                viewModel.Genres = string.Join(", ", filmData.Genre.Select(a => a.Genre.ToString()));
                viewModel.Rating = Math.Round(filmData.Film.Rating, 1).ToString();
            }
            return viewModel;
        }

        ///<summary>
        /// Converts FilmData object to FilmDataViewModel
        ///</summary>
        ///<param name="filmData"> FilmData object to map from </param>
        ///<returns>Mapped instance of type <see cref="FilmDataViewModel"/></returns>
        public FilmDataViewModel MapToFilmDataViewModel(FilmData filmData)
        {
            var viewModel = MapSimilarProperties<FilmData, FilmDataViewModel>(filmData);
            if (filmData != null)
            {
                viewModel.Id = filmData.FilmId;
                viewModel.Genres = string.Join(", ", filmData.Genre.Select(a => a.Genre.ToString()));
                viewModel.Rating = Math.Round(filmData.Film.Rating, 1).ToString();
                viewModel.FilmProjections = filmData.Film.FilmProjection.Select(x => MapToProjectionCardViewModel(x))
                                                                        .OrderByDescending(x => x.Date)
                                                                        .ToList();
                viewModel.FilmReviews = filmData.Film.FilmReviews.Select(x => MapToFilmReviewViewMode(x))
                                                                 .OrderByDescending(x => x.CreatedOn)
                                                                 .ToList();
                viewModel.CreatedByUserId = filmData.Film.AddedByUserId;
            }
            return viewModel;
        }

        ///<summary>
        /// Converts FilmReview object to FilmReviewViewModel
        ///</summary>
        ///<param name="review"> FilmReview object to map from</param> 
        ///<returns>Mapped instance of type <see cref="FilmReviewViewModel"/></returns>
        public FilmReviewViewModel MapToFilmReviewViewMode(FilmReview review)
        {
            return new FilmReviewViewModel
            {
                Content = review.Content,
                CreatedOn = review.CreatedOn,
                UserGender = review.User.Gender,
                Username = review.User.UserName,
            };
        }

        ///<summary>
        /// Converts FilmData object to FilmUpdateInputModel
        ///</summary>
        ///<param name="filmData"> FilmData object to map from</param>
        ///<returns>Mapped instance of type <see cref="FilmUpdateInputModel"/></returns>
        public FilmUpdateInputModel MapToFilmUpdateInputModel(FilmData filmData)
        {
            var viewModel = MapSimilarProperties<FilmData, FilmUpdateInputModel>(filmData);
            if (filmData != null)
            {
                viewModel.Genre = filmData.Genre.Select(x => x.Genre).ToList();
            }
            return viewModel;
        }

        ///<summary>
        /// Converts FilmJsonModel data to FilmUpdateInputModel
        ///</summary>
        ///<param name="filmData"> FilmData object to map from </param>
        ///<returns>Mapped instance of type <see cref="FilmInputModel"/></returns>
        public FilmInputModel MapToFilmInputModel(FilmJsonModel filmData)
        {
            if (filmData.Response == "False")
            {
                return new FilmInputModel { Error = filmData.Error };
            }

            var filmInputModel = new FilmInputModel
            {
                Title = filmData.Title,
                Director = filmData.Director,
                Cast = filmData.Actors,
                Description = filmData.Plot,
                TargetAudience = TargetAudience.All,
            };

            var posterUrl = filmData.Poster;
            if (!string.IsNullOrWhiteSpace(posterUrl))
            {
                var indexToCut = posterUrl.LastIndexOf('@');
                if (indexToCut > 1)
                {
                    posterUrl = posterUrl?.Substring(0, posterUrl.LastIndexOf('@') + 1) + "._V1_SY1000_SX675_AL_.jpg";
                }
            }
            filmInputModel.Poster = posterUrl;

            var genres = new List<Genre>();
            foreach (var genre in filmData.Genre.Split(", "))
            {
                foreach (Genre type in Enum.GetValues(typeof(Genre)))
                {
                    if (genre == type.ToString())
                    {
                        genres.Add(type);
                        break;
                    }
                }
            }
            if (genres.Count == 0)
            {
                genres.Add(Genre.Uncategorized);
            }
            filmInputModel.Genre = genres;

            if (!DateTime.TryParse(filmData.Released, out DateTime releaseDate))
            {
                releaseDate = DateTime.Now;
            }
            filmInputModel.ReleaseDate = releaseDate;

            if (!int.TryParse(filmData.Runtime.Split(" ")?[0], out int runtime))
            {
                runtime = 0;
            }
            filmInputModel.Runtime = runtime;

            return filmInputModel;
        }

        ///<summary>
        /// Converts FilmInputModel data to FilmData
        ///</summary>
        ///<param name="film"> Film object to map from</param>
        ///<param name="input">FilmInputModel object to map from</param>
        ///<returns>Mapped instance of type <see cref="FilmData"/></returns>
        public FilmData MapToFilmData(FilmInputModel input, Film film)
        {
            var model = MapSimilarProperties<FilmInputModel, FilmData>(input);
            model.FilmId = film.Id;
            model.Genre = input.Genre.Select(x => new GenreType() { Genre = x }).ToList();
            return model;
        }

        ///<summary>
        /// Converts FilmUpdateInputModel data to FilmData
        ///</summary>
        ///<param name="input"> FilmUpdateInputModel object to map from<param>
        ///<returns>Mapped instance of type <see cref="FilmData"/></returns>
        public FilmData MapToFilmData(FilmUpdateInputModel input)
        {
            var model = MapSimilarProperties<FilmUpdateInputModel, FilmData>(input);
            model.Genre = input.Genre.Select(genre => new GenreType() { Genre = genre }).ToList();
            return model;
        }

        ///<summary>
        /// Converts Cinema object to CinemaDataViewModel
        ///</summary>
        ///<param name="cinema"> Cinema object to map from</param>
        ///<returns>Mapped instance of type <see cref="CinemaDataViewModel"/></returns>
        public CinemaDataViewModel MapToCinemaDataViewModel(Cinema cinema)
        {
            var viewModel = MapSimilarProperties<Cinema, CinemaDataViewModel>(cinema);
            if (cinema != null)
            {
                viewModel.Mananger = cinema.Manager.UserName;
            }
            return viewModel;
        }

        ///<summary>
        /// Converts Cinema data to CinemaCardViewModel
        ///</summary>
        ///<param name="rawCinema"> Cinema object to map from</param>
        ///<returns>Mapped instance of type <see cref="CinemaCardViewModel"/></returns>
        public CinemaCardViewModel MapToCinemaCardViewModel(Cinema rawCinema)
        {
            var viewModel = MapSimilarProperties<Cinema, CinemaCardViewModel>(rawCinema);
            viewModel.Mananger = rawCinema.Manager.UserName;
            return viewModel;
        }

        ///<summary>
        /// Converts Cinema object to CinemaInputModel
        ///</summary>
        ///<param name="cinema"> Cinema object to map from</param>
        ///<returns>Mapped instance of type <see cref="CinemaInputModel"/></returns>
        public CinemaInputModel MapToCinemaUpdateInputModel(Cinema cinema)
        {
            var viewModel = MapSimilarProperties<Cinema, CinemaInputModel>(cinema);
            return viewModel;
        }

        ///<summary>
        /// Converts CinemaInputModel object to Cinema
        ///</summary>
        ///<param name="input"> CinemaInputModel object to map from</param>
        ///<returns>Mapped instance of type <see cref="Cinema"/></returns>
        public Cinema MapToCinemaData(CinemaInputModel input)
        {
            var model = MapSimilarProperties<CinemaInputModel, Cinema>(input);
            return model;
        }

        ///<summary>
        /// Creates FilmProjection object
        ///</summary>
        ///<param name="input">Projection data to map from</param>
        ///<param name="film"> Film object to map from</param>
        ///<param name="cinema"> Cinema object to map from</param>
        ///<returns>Mapped instance of type <see cref="FilmProjection"/></returns>
        public FilmProjection MapToFilmProjection(ProjectionInputModel input, FilmData film, Cinema cinema)
        {
            return new FilmProjection
            {
                CinemaId = cinema.Id,
                Date = input.Date,
                FilmId = film.FilmId,
                ProjectionType = input.ProjectionType,
                TotalTickets = input.TotalTickets,
                TicketPrices = new TicketPrices
                {
                    AdultPrice = input.AdultsTicketPrice,
                    StudentPrice = input.StudentsTicketPrice,
                    ChildrenPrice = input.ChildrenTicketPrice
                }
            };
        }

        ///<summary>
        /// Converts filmProjection object to ProjectionCardViewModel
        ///</summary>
        ///<param name="filmProjection"> FilmProjection object to map from</param>
        ///<returns>Mapped instance of type <see cref="ProjectionCardViewModel"/></returns>
        public ProjectionCardViewModel MapToProjectionCardViewModel(FilmProjection filmProjection)
        {
            return new ProjectionCardViewModel
            {
                Id = filmProjection.Id,
                CinemaName = filmProjection.Cinema.Name,
                FilmTitle = filmProjection.Film.FilmData.Title,
                ProjectionType = filmProjection.ProjectionType,
                Date = filmProjection.Date,
                CinemaCity = filmProjection.Cinema.City,
                CinemaCountry = filmProjection.Cinema.Country
            };
        }

        ///<summary>
        /// Converts filmProjection object to ProjectionInputModel
        ///</summary>
        ///<param name="filmProjection"> FilmProjection object to map from</param>
        ///<returns>Mapped instance of type <see cref="ProjectionInputModel"/></returns>
        public ProjectionInputModel MapToProjectionInputModel(FilmProjection filmProjection)
        {
            return new ProjectionInputModel
            {
                Date = filmProjection.Date,
                FilmTitle = filmProjection.Film.FilmData.Title,
                ProjectionType = filmProjection.ProjectionType,
                TotalTickets = filmProjection.TotalTickets,
                ChildrenTicketPrice = filmProjection.TicketPrices.ChildrenPrice,
                AdultsTicketPrice = filmProjection.TicketPrices.AdultPrice,
                StudentsTicketPrice = filmProjection.TicketPrices.StudentPrice
            };
        }

        ///<summary>
        /// Converts FilmProjection object to ProjectionDataViewModel
        ///</summary>
        ///<param name="dilmProjcetion"> FilmProjection object to map from </param>
        ///<returns>Mapped instance of type <see cref="ProjectionDataViewModel"/></returns>
        public ProjectionDataViewModel MapToProjectionDataViewModel(FilmProjection filmProjection)
        {
            return new ProjectionDataViewModel
            {
                Id = filmProjection.Id,
                CinemaName = filmProjection.Cinema.Name,
                FilmTitle = filmProjection.Film.FilmData.Title,
                ProjectionType = filmProjection.ProjectionType,
                Date = filmProjection.Date,
                TotalTickets = filmProjection.TotalTickets,
                FilmRuntime = filmProjection.Film.FilmData.Runtime,
                FilmTargetAudience = filmProjection.Film.FilmData.TargetAudience,
                CinemaCity = filmProjection.Cinema.City,
                CinemaCountry = filmProjection.Cinema.Country,
                TicketsSold = filmProjection.ProjectionTickets.Count(),
                ChildrenTicketPrice = filmProjection.TicketPrices.ChildrenPrice,
                AdultsTicketPrice = filmProjection.TicketPrices.AdultPrice,
                StudentsTicketPrice = filmProjection.TicketPrices.StudentPrice,
                FilmId = filmProjection.FilmId,
                CinemaId = filmProjection.CinemaId,
            };
        }

        ///<summary>
        /// Converts ProjectionTicket object to TicketCardViewModel
        ///</summary>
        ///<param name="ticket"> ProjectionTicket object to map from</param>
        ///<returns>Mapped instance of type <see cref="TicketCardViewModel"/></returns>
        public TicketCardViewModel MapToTicketCardViewModel(ProjectionTicket ticket)
        {
            var viewModel = MapSimilarProperties<ProjectionTicket, TicketCardViewModel>(ticket);
            viewModel.CinemaName = ticket.Projection.Cinema.Name;
            viewModel.FilmTitle = ticket.Projection.Film.FilmData.Title;
            viewModel.ProjectionType = ticket.Projection.ProjectionType;
            viewModel.ProjectionDate = ticket.Projection.Date;
            return viewModel;
        }

        ///<summary>
        /// Updates information about users' tickets with selected parameters
        ///</summary>
        ///<param name="userId">Id of the user</param>
        ///<param name="input">Ticket information</param>
        ///<param name="projection">Projection information</param>
        ///<param name="DateTime">Ticket time paramater</param>
        ///<returns>Mapped instance of type <see cref="ProjectionTicket"/></returns>
        public ProjectionTicket MapToProjectionTicket(string userId,
                                                      TicketInputModel input,
                                                      FilmProjection projection,
                                                      DateTime timeStamp)
        {
            return new ProjectionTicket
            {
                ProjectionId = projection.Id,
                Seat = input.Seat,
                Type = input.TicketType,
                Price = (double)projection.TicketPrices.GetType().
                                                        GetProperty($"{input.TicketType.ToString()}Price").
                                                        GetValue(projection.TicketPrices),
                HolderId = userId,
                ReservedOn = timeStamp
            };
        }
    }
}
