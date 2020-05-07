using CinemaShare.Models;
using CinemaShare.Models.JsonModels;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Common.Mapping
{
    public interface IMapper
    {
        public ExtendedFilmCardViewModel MapToExtendedFilmCardViewModel(FilmData filmData);

        public FilmDataViewModel MapToFilmDataViewModel(FilmData filmData);

        public IEnumerable<FilmCardViewModel> MapToFilmCardViewModel(IEnumerable<FilmData> rawFilms);

        public FilmInputModel MapToFilmInputModel(FilmJsonModel filmData);

        public FilmData MapToFilmData(FilmInputModel input, Film film);
    }
}
