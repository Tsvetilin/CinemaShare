using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public class FilmProjectionBusiness : IFilmProjectionBusiness
    {
        private readonly CinemaDbContext context;

        public FilmProjectionBusiness(CinemaDbContext context)
        {
            this.context = context;
        }

        public async Task Add(FilmProjection filmProjection)
        {
            await context.FilmProjections.AddAsync(filmProjection);
            await context.SaveChangesAsync();
        }

        public async Task<FilmProjection> Get(string id)
        {
            return await context.FilmProjections.FindAsync(id);
        }

        public int CountAllProjections()
        {
            return context.FilmProjections.Count();

        }
        public IEnumerable<TModel> GetPageItems<TModel>(int page, int projectionsOnPage, Func<FilmProjection, TModel> mapToModelFunc)
        {
            var projections = GetAll();
            var selectedCinemas = projections.Skip(projectionsOnPage * (page - 1)).Take(projectionsOnPage);
            return selectedCinemas.Select(x => mapToModelFunc(x)).ToList();
        }

        public async Task<TModel> GetAsync<TModel>(string id, Func<FilmProjection, TModel> mapToModelFunc)
        {
            var projection = await Get(id);
            return mapToModelFunc(projection);
        }

        public IEnumerable<FilmProjection> GetAll()
        {
            return context.FilmProjections.ToList();
        }

        public IEnumerable<TModel> GetAllByCinemaId<TModel>(string cinemaId, Func<FilmProjection, TModel> mapToModelFunc)
        {
            return context.FilmProjections.Where(x=>x.CinemaId==cinemaId).ToList().Select(x => mapToModelFunc(x)).ToList();
        }

        public IEnumerable<TModel> GetAll<TModel>(Func<FilmProjection, TModel> mapToModelFunc)
        {
            return GetAll().Select(x => mapToModelFunc(x)).ToList();
        }

        public async Task Update(FilmProjection film)
        {
            var filmInContext = await context.FilmProjections.FindAsync(film.Id);
            if (filmInContext != null)
            {
                context.Entry(filmInContext).CurrentValues.SetValues(film);
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(string id)
        {
            var filmInContext = await context.FilmProjections.FindAsync(id);
            if (filmInContext != null)
            {
                context.FilmProjections.Remove(filmInContext);
                await context.SaveChangesAsync();
            }
        }
    }
}
