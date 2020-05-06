using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IFilmDataBusiness
    {
        public Task Add(FilmData filmData);

        public Task<FilmData> Get(string id);

        public IEnumerable<FilmData> GetAll();

        public IEnumerable<FilmData> GetFilmsOnPageByName(int page, int filmsOnPage);

        public IEnumerable<FilmData> GetFilmsOnPageByYear(int page, int filmsOnPage);

        public IEnumerable<FilmData> GetFilmsOnPageByRating(int page, int filmsOnPage);

        public IEnumerable<FilmData> GetPageItems(int page, int filmsOnPage, IEnumerable<FilmData> orderedFilms = null);

        public int CountAllFilms();

        public Task Update(FilmData filmData);

        public Task Delete(string id);
    }
}
