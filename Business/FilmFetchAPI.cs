using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Business
{
    public class FilmFetchAPI : IFilmFetchAPI
    {
        private readonly string apiKey;

        public FilmFetchAPI(string apiKey)
        {
            this.apiKey = apiKey;
        }

        /// <summary>
        /// Gets a information about selected film from the Web API
        /// </summary>
        /// <param name="title">Film title</param>
        /// <param name="mapFunction">
        /// Method that maps the <typeparamref name="TJsonModel"/> to <typeparamref name="TInputModel"/>
        /// </param>
        /// <typeparam name="TInputModel"></typeparam>
        /// <typeparam name="TJsonModel">Model that represents the Json result of the fetching</typeparam>
        /// <return>Information as Json parsed from <typeparamref name="TInputModel"/></return>
        public async Task<string> FetchFilmAsync<TJsonModel, TInputModel>(string title, 
                                                                          Func<TJsonModel, TInputModel> mapFunction)
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
