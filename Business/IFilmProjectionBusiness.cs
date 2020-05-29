using Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business
{
    public interface IFilmProjectionBusiness
    {
        public Task AddAsync(FilmProjection filmProjection);

        public Task<FilmProjection> GetAsync(string id);

        public int CountAllProjections();

        public IEnumerable<TModel> GetPageItems<TModel>(int page, 
                                                        int projectionsOnPage,
                                                        Func<FilmProjection, TModel> mapToModelFunc);

        public Task<TModel> GetAsync<TModel>(string id, Func<FilmProjection, TModel> mapToModelFunc);

        public IEnumerable<FilmProjection> GetAll();

        public IEnumerable<TModel> GetAll<TModel>(Func<FilmProjection, TModel> mapToModelFunc);

        public IEnumerable<TModel> GetAllByCinemaId<TModel>(string cinemaId, Func<FilmProjection, TModel> mapToModelFunc);

        public Task UpdateAsync(FilmProjection projection, string projectionsUrlPattern, string ticketsUrlPattern);

        public Task DeleteAsync(string id, string projectionsUrlPattern);
    }
}
