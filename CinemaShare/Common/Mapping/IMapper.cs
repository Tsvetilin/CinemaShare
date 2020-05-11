using CinemaShare.Models.InputModels;
using CinemaShare.Models.JsonModels;
using CinemaShare.Models.ViewModels;
using Data.Models;
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
    }
}
