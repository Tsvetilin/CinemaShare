using CinemaShare.Models.InputModels;
using CinemaShare.Models.JsonModels;
using CinemaShare.Models.ViewModels;
using Data.Models;
using System;
using System.Collections.Generic;

namespace CinemaShare.Common.Mapping
{
    public interface IMapper
    {
        public ExtendedFilmCardViewModel MapToExtendedFilmCardViewModel(FilmData filmData);

        public FilmDataViewModel MapToFilmDataViewModel(FilmData filmData);

        public CinemaDataViewModel MapToCinemaDataViewModel(Cinema cinema);

        public IEnumerable<FilmCardViewModel> MapToFilmCardViewModel(IEnumerable<FilmData> rawFilms);

        public CinemaCardViewModel MapToCinemaCardViewModel(Cinema rawCinemas);

        public FilmInputModel MapToFilmInputModel(FilmJsonModel filmData);

        public FilmUpdateInputModel MapToFilmUpdateInputModel(FilmData filmData);

        public CinemaInputModel MapToCinemaUpdateInputModel(Cinema cinema);

        public FilmData MapToFilmData(FilmInputModel input, Film film);

        public FilmData MapToFilmData(FilmUpdateInputModel input);

        public Cinema MapToCinemaData(CinemaInputModel input);

        public FilmProjection MapToFilmProjection(ProjectionInputModel input, FilmData film, Cinema cinema);

        public ProjectionCardViewModel MapToProjectionCardViewModel(FilmProjection filmProjection);

        public ProjectionInputModel MapToProjectionInputModel(FilmProjection filmProjection);

        public ProjectionDataViewModel MapToProjectionDataViewModel(FilmProjection filmProjection);

        public TicketCardViewModel MapToTicketCardViewModel(ProjectionTicket ticket);

        public ProjectionTicket MapToProjectionTicket(string userId, UpdateTicketInputModel input, FilmProjection projection, DateTime timeStamp);
    
        public ProjectionTicket MapToProjectionTicket(string userId, TicketInputModel input, FilmProjection projection, DateTime timeStamp);
    }
}
