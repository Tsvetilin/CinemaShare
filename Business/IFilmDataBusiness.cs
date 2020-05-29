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

        public IDictionary<string, string> GetAllFilmsTitles();

        public IEnumerable<TModel> GetPageItems<TModel>(int page, int filmsOnPage, string sortOption,
                                                       Func<FilmData, TModel> mapToModelFunc);

        public IEnumerable<TModel> GetTopFilms<TModel>(Func<FilmData, TModel> mapToModelFunc);

        public IEnumerable<TModel> GetRecentFilms<TModel>(Func<FilmData, TModel> mapToModelFunc);

        public IEnumerable<TModel> GetAllByName<TModel>(string searchString, Func<FilmData, TModel> mapToModelFunc);

        public int CountAllFilms();

        public Task UpdateAsync(FilmData filmData);

        public Task DeleteAsync(string id);
    }
}
