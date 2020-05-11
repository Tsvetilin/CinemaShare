using Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business
{
    public interface IFilmDataBusiness
    {
        public Task AddAsync<TModel>(TModel filmData, Film film, Func<TModel, Film, FilmData> mapToModelFunc);

        public Task<TModel> GetAsync<TModel>(string id, Func<FilmData, TModel> mapToModelFunc);

        public Task<FilmData> GetAsync(string id);

        public FilmData GetByName(string title);

        public bool IsAlreadyAdded(string filmTitle);

        public IEnumerable<FilmData> GetAll();

        public IEnumerable<TModel> GetFilmsOnPageByName<TModel>(int page, int filmsOnPage,
                                                                Func<FilmData, TModel> mapToModelFunc);

        public IEnumerable<TModel> GetFilmsOnPageByYear<TModel>(int page, int filmsOnPage,
                                                                Func<FilmData, TModel> mapToModelFunc);

        public IEnumerable<TModel> GetFilmsOnPageByRating<TModel>(int page, int filmsOnPage,
                                                                Func<FilmData, TModel> mapToModelFunc);

        public IEnumerable<TModel> GetPageItems<TModel>(int page, int filmsOnPage,
                                                        Func<FilmData, TModel> mapToModelFunc,
                                                        IEnumerable<FilmData> orderedFilms = null);
        public IEnumerable<TModel> GetTopFilms<TModel>(Func<IEnumerable<FilmData>,
                                                               IEnumerable<TModel>> mapToModelFunc);

        public IEnumerable<TModel> GetRecentFilms<TModel>(Func<IEnumerable<FilmData>,
                                                               IEnumerable<TModel>> mapToModelFunc);

        public IEnumerable<TModel> GetAllByName<TModel>(string searchString, Func<FilmData, TModel> mapToModelFunc);

        public int CountAllFilms();

        public Task Update(FilmData filmData);

        public Task Delete(string id);
    }
}
