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

        public async Task<FilmData> GetByNameAsync(string title)
        {
            return await context.FilmDatas.FirstOrDefaultAsync(x => x.Title.ToLower() == title.ToLower());
        }

        public IEnumerable<TModel> GetFilmsOnPageByName<TModel>(int page, int filmsOnPage,
                                                                Func<FilmData, TModel> mapToModelFunc)
        {
            var sortedFilms = context.FilmDatas.OrderBy(x => x.Title).ToList();
            return GetPageItems(page, filmsOnPage, mapToModelFunc, sortedFilms).ToList();
        }

        public IEnumerable<TModel> GetFilmsOnPageByYear<TModel>(int page, int filmsOnPage,
                                                                Func<FilmData, TModel> mapToModelFunc)
        {
            var sortedFilms = context.FilmDatas.OrderByDescending(x => x.ReleaseDate).ToList();
            return GetPageItems(page, filmsOnPage, mapToModelFunc, sortedFilms).ToList();
        }

        public IEnumerable<TModel> GetFilmsOnPageByRating<TModel>(int page, int filmsOnPage,
                                                                  Func<FilmData, TModel> mapToModelFunc)
        {
            var sortedFilms = context.FilmDatas.OrderByDescending(x => x.Film.Rating).ToList();
            return GetPageItems(page, filmsOnPage, mapToModelFunc, sortedFilms).ToList();
        }

        public IEnumerable<TModel> GetPageItems<TModel>(int page, int filmsOnPage,
                                                        Func<FilmData, TModel> mapToModelFunc,
                                                        IEnumerable<FilmData> orderedFilms = null)
        {
            if (orderedFilms == null)
            {
                orderedFilms = GetAll();
            }

            var selectedFilms = orderedFilms.Skip(filmsOnPage * (page - 1)).Take(filmsOnPage);
            return selectedFilms.Select(x => mapToModelFunc(x));
        }

        public IEnumerable<TModel> GetTopFilms<TModel>(Func<IEnumerable<FilmData>,
                                                            IEnumerable<TModel>> mapToModelFunc)
        {
            return mapToModelFunc(this.GetAll().OrderByDescending(x => x.Film.Rating)?.Take(10)).ToList();
        }

        public IEnumerable<TModel> GetRecentFilms<TModel>(Func<IEnumerable<FilmData>,
                                                               IEnumerable<TModel>> mapToModelFunc)
        {
            return mapToModelFunc(this.GetAll().OrderByDescending(x => x.ReleaseDate)?.Take(4)).ToList();
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
