using System;
using System.Threading.Tasks;

namespace Business
{
    public interface IFilmFetchAPI
    {
        public Task<string> FetchFilmAsync<TJsonModel, TInputModel>(string title, Func<TJsonModel, TInputModel> mapFunction);
    }
}
