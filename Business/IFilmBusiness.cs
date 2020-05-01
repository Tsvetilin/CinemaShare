using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IFilmBusiness
    {
        public Task Add(Film film);

        public Task<Film> Get(string id);

        public Task<IEnumerable<Film>> GetAll();

        public Task Update(Film film);

        public Task Delete(string id);
    }
}
