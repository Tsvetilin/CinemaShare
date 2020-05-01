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

        public Task<IEnumerable<FilmData>> GetAll();

        public Task Update(FilmData filmData);

        public Task Delete(string id);
    }
}
