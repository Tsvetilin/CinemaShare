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

        public async Task Add(FilmData filmData)
        {
            await context.FilmDatas.AddAsync(filmData);
            await context.SaveChangesAsync();
        }

        public bool IsAlreadyAdded(string filmTitle)
        {
            return  context.FilmDatas.Any(x => x.Title.ToLower().Equals(filmTitle.ToLower()));
        }

        public async Task<FilmData> Get(string id)
        {
            return await context.FilmDatas.FindAsync(id);
        }

        public IEnumerable<FilmData> GetAll()
        {
            return context.FilmDatas.Include(x=>x.Film).Include(x=>x.Genre).ToList();
        }

        public IEnumerable<FilmData> GetFilmsOnPageByName (int page, int filmsOnPage)
        {
            return GetPageItems( page, filmsOnPage, context.FilmDatas.OrderBy(x => x.Title)).ToList();
        }

        public IEnumerable<FilmData> GetFilmsOnPageByYear(int page, int filmsOnPage)
        {
            return GetPageItems(page, filmsOnPage, context.FilmDatas.OrderByDescending(x => x.ReleaseDate)).ToList();
        }

        public IEnumerable<FilmData> GetFilmsOnPageByRating(int page, int filmsOnPage)
        {
            return GetPageItems(page, filmsOnPage, context.FilmDatas.OrderByDescending(x => x.Film.Rating)).ToList();
        }

        public IEnumerable<FilmData> GetPageItems( int page, int filmsOnPage, IEnumerable<FilmData> orderedFilms = null)
        {
            if(orderedFilms==null)
            {
                orderedFilms = GetAll();
            }

            return orderedFilms.Skip(filmsOnPage * (page - 1)).Take(filmsOnPage);
        }

        public int CountAllFilms()
        {
            return context.Films.Count();
        }

        public async Task Update(FilmData filmData)
        {
            var filmDataInContext = await context.Films.FindAsync(filmData.Film);
            if (filmDataInContext != null)
            {
                context.Entry(filmDataInContext).CurrentValues.SetValues(filmData);
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(string id)
        {
            var filmInContext = await context.Films.FindAsync(id);
            if (filmInContext != null)
            {
                context.Films.Remove(filmInContext);
                await context.SaveChangesAsync();
            }
        }
    }
}
