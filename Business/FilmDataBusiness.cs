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

        public async Task AddAsync<TModel>(TModel filmData, Film film, Func<TModel, Film, FilmData> mapToModelFunc)
        {
            await context.FilmDatas.AddAsync(mapToModelFunc(filmData, film));
            await context.SaveChangesAsync();
        }

        public bool IsAlreadyAdded(string filmTitle)
        {
            return context.FilmDatas.Any(x => x.Title.ToLower().Equals(filmTitle.ToLower()));
        }
        
        public async Task<FilmData> GetAsync(string id)
        {
            return await context.FilmDatas.FindAsync(id);
        }

        
        public async Task<TModel> GetAsync<TModel>(string id, Func<FilmData, TModel> mapToModelFunc)
        {
            var film = await GetAsync(id);
            return mapToModelFunc(film);
        }

        public IEnumerable<FilmData> GetAll()
        {
            return context.FilmDatas.ToList();
        }

        public IEnumerable<TModel> GetAllByName<TModel>(string searchString, Func<FilmData, TModel> mapToModelFunc)
        {
           return GetAll().Where(x => x.Title.ToLower().Contains(searchString.ToLower()))
                                                .Select(x => mapToModelFunc(x)).ToList();
        }

        public FilmData GetByName(string title)
        {
            var film = context.FilmDatas.FirstOrDefault(x => x.Title.ToLower() == title.ToLower());
            return film;
        }

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

        public IEnumerable<TModel> GetTopFilms<TModel>(Func<FilmData,TModel> mapToModelFunc)
        {
            return this.GetAll().OrderByDescending(x => x.Film.Rating)?.Take(10).Select(x => mapToModelFunc(x)).ToList();
        }

        public IEnumerable<TModel> GetRecentFilms<TModel>(Func<FilmData,TModel> mapToModelFunc)
        {
            return this.GetAll().OrderByDescending(x => x.ReleaseDate)?.Take(4).Select(x=>mapToModelFunc(x)).ToList();
        }

        public int CountAllFilms()
        {
            return context.Films.Count();
        }

        public async Task Update(FilmData filmData)
        {
            var filmDataInContext = await context.FilmDatas.FindAsync(filmData.FilmId);
            if (filmDataInContext != null)
            {
                context.Entry(filmDataInContext).CurrentValues.SetValues(filmData);
                await context.SaveChangesAsync();
            }
        }

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
