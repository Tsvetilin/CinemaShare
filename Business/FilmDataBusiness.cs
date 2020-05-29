using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public class FilmDataBusiness : IFilmDataBusiness
    {
        private readonly CinemaDbContext context;

        public FilmDataBusiness(CinemaDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds new film in the database
        /// </summary>
        /// <param name="filmData">Data of the new film </param>        
        /// <param name="film">Existing film object, data to be added to</param>
        /// <param name="mapToModelFunc">
        /// Method that maps the <typeparamref name="TModel"/> and the Film model to FilmData model
        /// </param>
        /// <typeparam name="TModel">Model, the Film data model to be mapped from</typeparam>
        public async Task AddAsync<TModel>(TModel filmData, Film film, Func<TModel, Film, FilmData> mapToModelFunc)
        {
            await context.FilmDatas.AddAsync(mapToModelFunc(filmData, film));
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if film is added in the database
        /// </summary>
        /// <param name="filmTitle">Title of the film to check for</param>        
        /// <returns>True if already added or false if not</returns>
        public bool IsAlreadyAdded(string filmTitle)
        {
            return context.FilmDatas.Any(x => x.Title.ToLower().Equals(filmTitle.ToLower()));
        }
        
        /// <summary>
        /// Gets film by ID
        /// </summary>
        /// <param name="id">The film ID</param>        
        /// <returns>Selected film, returns null if not found</returns>
        public async Task<FilmData> GetAsync(string id)
        {
            return await context.FilmDatas.FindAsync(id);
        }

        /// <summary>
        /// Gets a film by ID
        /// </summary>
        /// <param name="id">The film ID to search for</param>
        /// <param name="mapToModelFunc">Method that maps the Film data to <typeparamref name="TModel"/></param>
        /// <typeparam name="TModel">Model, the Film data model to be mapped to</typeparam>
        /// <returns>The film data as <typeparamref name="TModel"/>, null if not found</returns>
        public async Task<TModel> GetAsync<TModel>(string id, Func<FilmData, TModel> mapToModelFunc)
        {
            var film = await GetAsync(id);
            return mapToModelFunc(film);
        }

        /// <summary>
        /// Gets all film's data in the database
        /// </summary>    
        /// <returns>List of all film's data </returns>
        public IEnumerable<FilmData> GetAll()
        {
            return context.FilmDatas.ToList();
        }

        /// <summary>
        /// Get all films' titles for selection listing
        /// </summary>
        /// <returns>Dictionary containing all films' titles</returns>
        public IDictionary<string,string> GetAllFilmsTitles()
        {
            return GetAll().ToList().Select(x => x.Title).OrderBy(x => x).ToDictionary(x => x);
        }

        /// <summary>
        /// Gets all films by a selected name
        /// </summary>    
        /// <param name="searchString">Film title to seatch for</param>
        /// <param name="mapToModelFunc">Method that maps the Film data to <typeparamref name="TModel"/></param>
        /// <typeparam name="TModel">Model, the Film data model to be mapped to</typeparam>
        /// <returns>List of films found mapped to <typeparamref name="TModel"/></returns>
        public IEnumerable<TModel> GetAllByName<TModel>(string searchString, Func<FilmData, TModel> mapToModelFunc)
        {
           return GetAll().Where(x => x.Title.ToLower().Contains(searchString.ToLower()))
                                                .Select(x => mapToModelFunc(x)).ToList();
        }
        
        /// <summary>
        /// Gets a film by a selected name
        /// </summary>    
        /// <param name="title">The film title</param>
        /// <returns>The film result, null if not found</returns>
        public FilmData GetByName(string title)
        {
            var film = context.FilmDatas.FirstOrDefault(x => x.Title.ToLower() == title.ToLower());
            return film;
        }

        /// <summary>
        /// Gets all films on the selected page
        /// and sorts them
        /// </summary>    
        /// <param name="page">Number of the page</param>
        /// <param name="filmsOnPage">Number of the films on the page</param>
        /// <param name="sortOption">Sorting criteria</param>
        /// <param name="mapToModelFunc">Method that maps the Film data to <typeparamref name="TModel"/></param>
        /// <typeparam name="TModel">Model, the Film data model to be mapped to</typeparam>
        /// <returns>List of films mapped to <typeparamref name="TModel"/></returns>
        public IEnumerable<TModel> GetPageItems<TModel>(int page, int filmsOnPage, string sortOption,
                                                        Func<FilmData, TModel> mapToModelFunc)
        {
            var sortedFilms = GetAll();
            if (sortOption == "Name")
            {
                sortedFilms = sortedFilms.OrderBy(x => x.Title);
            }
            else if (sortOption == "Year")
            {
                sortedFilms = sortedFilms.OrderByDescending(x => x.ReleaseDate);
            }
            else if (sortOption == "Rating")
            {
                sortedFilms = sortedFilms.OrderByDescending(x => x.Film.Rating);
            }

            var selectedFilms = sortedFilms.Skip(filmsOnPage * (page - 1)).Take(filmsOnPage);
            return selectedFilms.Select(x => mapToModelFunc(x));
        }

        /// <summary>
        /// Gets top 10 films with the best rating
        /// </summary>    
        /// <param name="mapToModelFunc">Method that maps the Film data to <typeparamref name="TModel"/></param>
        /// <typeparam name="TModel">Model, the Film data model to be mapped to</typeparam>
        /// <returns>List of films mapped to <typeparamref name="TModel"/></returns>
        public IEnumerable<TModel> GetTopFilms<TModel>(Func<FilmData,TModel> mapToModelFunc)
        {
            return this.GetAll().OrderByDescending(x => x.Film.Rating).Take(10).Select(x => mapToModelFunc(x)).ToList();
        }

        /// <summary>
        /// Gets the most recently released 4 films
        /// </summary>    
        /// <param name="mapToModelFunc">Method that maps the Film data to <typeparamref name="TModel"/></param>
        /// <typeparam name="TModel">Model, the Film data model to be mapped to</typeparam>
        /// <returns>List of films mapped to <typeparamref name="TModel"/></returns>
        public IEnumerable<TModel> GetRecentFilms<TModel>(Func<FilmData,TModel> mapToModelFunc)
        {
            return this.GetAll().OrderByDescending(x => x.ReleaseDate).Take(4).Select(x=>mapToModelFunc(x)).ToList();
        }

        /// <summary>
        /// Gets the count of all films in the database
        /// </summary>    
        /// <returns>Count</returns>
        public int CountAllFilms()
        {
            return context.Films.Count();
        }

        /// <summary>
        /// Updates the data about a selected film
        /// </summary>    
        /// <param name="filmData">The updated data of the film</param>
        public async Task UpdateAsync(FilmData filmData)
        {
            var filmDataInContext = await context.FilmDatas.FindAsync(filmData.FilmId);
            if (filmDataInContext != null)
            {
                context.Entry(filmDataInContext).CurrentValues.SetValues(filmData);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes film by ID
        /// </summary>    
        /// <param name="id">The film ID</param>
        public async Task DeleteAsync(string id)
        {
            var filmInContext = await context.FilmDatas.FindAsync(id);
            if (filmInContext != null)
            {
                context.FilmDatas.Remove(filmInContext);
                context.Films.Remove(filmInContext.Film);
                await context.SaveChangesAsync();
            }
        }
    }
}
