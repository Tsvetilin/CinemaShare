using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Business
{
    public class FilmFetchAPI : IFilmFetchAPI
    {
        public async Task<string> FetchFilmAsync<TJsonModel, TInputModel>(string apiKey, string title, Func<TJsonModel, TInputModel> mapFunction)
        {
            using HttpClient client = new HttpClient();
            string url = $"https://www.omdbapi.com/?apikey={apiKey}&t={title}";
            var json = await client.GetStringAsync(url);
            var filmData = JsonConvert.DeserializeObject<TJsonModel>(json);
            var filmInputModel = mapFunction(filmData);
            var serializedFilm = JsonConvert.SerializeObject(filmInputModel);
            return serializedFilm;
        }
    }
}
