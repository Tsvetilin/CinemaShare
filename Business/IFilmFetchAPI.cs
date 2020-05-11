using System;
using System.Threading.Tasks;

namespace Business
{
    public interface IFilmFetchAPI
    {
        public Task<string> FetchFilmAsync<TJsonModel, TInputModel>(string apiKey, string title, Func<TJsonModel, TInputModel> mapFunction);
    }
}
