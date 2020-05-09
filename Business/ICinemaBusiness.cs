using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface ICinemaBusiness
    {
        public Task Add(Cinema cinema);

        public Task<Cinema> Get(string id);

        public IEnumerable<Cinema> GetAll();

        public Task Update(Cinema cinema);

        public Task Delete(string id);
        public int CountAllCinemas();
        public IEnumerable<TModel> GetPageItems<TModel>(int page, int cinemasOnPage,
                                                       Func<Cinema, TModel> mapToModelFunc);
    }
}
