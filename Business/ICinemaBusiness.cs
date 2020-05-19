using Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business
{
    public interface ICinemaBusiness
    {
        public Task AddAsync(Cinema cinema);

        public Task<Cinema> GetAsync(string id);

        public bool IsAlreadyAdded(string cinemaName);

        public Task<TModel> GetAsync<TModel>(string id, Func<Cinema, TModel> mapToModelFunc);

        public IEnumerable<Cinema> GetAll();

        public Task UpdateAsync(Cinema cinema, string ticketUrlPattern);
               
        public Task DeleteAsync(string id, string projectionsUrlPattern);

        public int CountAllCinemas();

        public IEnumerable<TModel> GetPageItems<TModel>(int page, int cinemasOnPage,
                                                       Func<Cinema, TModel> mapToModelFunc);

        public IEnumerable<TModel> GetAllByName<TModel>(string searchString, Func<Cinema, TModel> mapToModelFunc);

        public IEnumerable<TModel> GetAllByCity<TModel>(string searchString, Func<Cinema, TModel> mapToModelFunc);

        public IEnumerable<TModel> GetSearchResults<TModel>(string searchString, Func<Cinema, TModel> mapToModelFunc);

    }
}
