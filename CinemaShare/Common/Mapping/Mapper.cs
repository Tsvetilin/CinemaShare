using CinemaShare.Models;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CinemaShare.Common.Mapping
{
    public class Mapper : IMapper
    {
        private TTo MapSimilarProperties<TFrom, TTo>(TFrom fromObject)
             where TFrom : class
             where TTo : class , new()
        {
            TTo viewModel = new TTo();//Activator.CreateInstance<TTo>();
            PropertyInfo[] props = fromObject.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (viewModel?.GetType()?.GetProperty(prop.Name)?.PropertyType?.Equals(prop.PropertyType) ?? false)
                {
                    viewModel.GetType().GetProperty(prop.Name).SetValue(viewModel, prop.GetValue(fromObject));
                }
            }

            return viewModel;
        }

        public ExtendedFilmCardViewModel MapToExtendedFilmCardViewModel(FilmData filmData)
        {
            var viewModel = MapSimilarProperties<FilmData, ExtendedFilmCardViewModel>(filmData);
            viewModel.Id = filmData.FilmId;
            viewModel.Genres = string.Join(", ", filmData.Genre.Select(a => a.Genre.ToString()));
            viewModel.Rating = filmData.Film.Rating.ToString();
            return viewModel;
        }

        public FilmDataViewModel MapToFilmDataViewModel(FilmData filmData)
        {
            var viewModel = MapSimilarProperties<FilmData, FilmDataViewModel>(filmData);
            viewModel.Id = filmData.FilmId;
            viewModel.Genres = string.Join(", ", filmData.Genre.Select(a => a.Genre.ToString()));
            viewModel.Rating = filmData.Film.Rating.ToString();
            viewModel.FilmProjections = filmData.Film.FilmProjection.ToList();
            viewModel.FilmReviews = filmData.Film.FilmReviews.ToList();
            return viewModel;
        }


        public IEnumerable<FilmCardViewModel> MapToFilmCardViewModel(IEnumerable<FilmData> rawFilms)
        {
            return rawFilms.Select(x => new FilmCardViewModel
            {
                Title = x.Title,
                Genres = string.Join(", ", x.Genre.Select(a => a.Genre.ToString())),
                Poster = x.Poster,
                Rating = x.Film.Rating.ToString(),
                Id = x.FilmId
            });
        }
    }
}
