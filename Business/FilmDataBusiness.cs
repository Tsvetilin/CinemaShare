using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
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
        /// Adds new film
        /// </summary>
        /// <param name="filmData">Data of the new film </param>        
        /// <param name="film">New film object</param>
        /// <returns></returns>
        public async Task AddAsync<TModel>(TModel filmData, Film film, Func<TModel, Film, FilmData> mapToModelFunc)
        {
            await context.FilmDatas.AddAsync(mapToModelFunc(filmData, film));
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if film is added in the database
        /// </summary>
        /// <param name="filmTitle">Data of the new film </param>        
        /// <returns>True if added or false if not</returns>
        public bool IsAlreadyAdded(string filmTitle)
        {
            return context.FilmDatas.Any(x => x.Title.ToLower().Equals(filmTitle.ToLower()));
        }
        
        /// <summary>
        /// Gets film by ID
        /// </summary>
        /// <param name="id">The film ID</param>        
        /// <returns>Selected film</returns>
        public async Task<FilmData> GetAsync(string id)
        {
            return await context.FilmDatas.FindAsync(id);
        }

        /// <summary>
        /// Gets a film by ID
        /// </summary>
        /// <param name="id">The film ID</param>        
        /// <returns>The film as TModel</returns>
        public async Task<TModel> GetAsync<TModel>(string id, Func<FilmData, TModel> mapToModelFunc)
        {
            var film = await GetAsync(id);
            return mapToModelFunc(film);
        }

        /// <summary>
        /// Gets all films in the database
        /// </summary>    
        /// <returns>List of films</returns>
        public IEnumerable<FilmData> GetAll()
        {
            return context.FilmDatas.ToList();
        }

        /// <summary>
        /// Gets all films by a selected name
        /// </summary>    
        /// <param name="searchString>Input string</param>
        /// <returns>List of films</returns>
        public IEnumerable<TModel> GetAllByName<TModel>(string searchString, Func<FilmData, TModel> mapToModelFunc)
        {
           return GetAll().Where(x => x.Title.ToLower().Contains(searchString.ToLower()))
                                                .Select(x => mapToModelFunc(x)).ToList();
        }
        
        /// <summary>
        /// Gets a film by a selected name
        /// </summary>    
        /// <param name="title>The film title</param>
        /// <returns>Film</returns>
        public FilmData GetByName(string title)
        {
            var film = context.FilmDatas.FirstOrDefault(x => x.Title.ToLower() == title.ToLower());
            return film;
        }

        /// <summary>
        /// Gets all films on the selected page
        /// and sorts them
        /// </summary>    
        /// <param name="page>Number of the page</param>
        /// <param name="filmsonPage>Number of the films on the page</param>
        /// <param name="sortOption>Sorting criteria</param>
        /// <returns>List of films</returns>
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
        /// <returns>List of films</returns>
        public IEnumerable<TModel> GetTopFilms<TModel>(Func<FilmData,TModel> mapToModelFunc)
        {
            return this.GetAll().OrderByDescending(x => x.Film.Rating).Take(10).Select(x => mapToModelFunc(x)).ToList();
        }

        /// <summary>
        /// Gets the most recently added 4 films
        /// </summary>    
        /// <returns>List of films</returns>
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
        /// <param name="filmData>The updated data of the film</param>
        /// <returns></returns>
        public async Task Update(FilmData filmData)
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
        /// <param name="id>The film ID</param>
        /// <returns></returns>
        public async Task Delete(string id)
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
