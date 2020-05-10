using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IFilmBusiness
    {
        public Task AddAsync(Film film);

        public Task<Film> GetAsync(string id);

        public IEnumerable<Film> GetAll();

        public Task RateAsync(string filmId, string userId, int rating);

        public Task UpdateAsync(Film film);

        public Task DeleteAsync(string id);
    }
}
