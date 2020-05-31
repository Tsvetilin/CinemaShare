# CinemaShare
ASP.NET Core MVC Web application for sharing, rating and reviewing films, adding projections in registered cinemas and reserving tickets

## Description
Made as an exam project for IT Career Module 7 "Software Development" by Tsvetilin Tsvetilov, Stoyan Zlatev and Pavlin Marinov

## Stucture of the project
Three-tier architecture

### Data Layer
- Code First
- MySql Database
- Entity Framework
- Data Seeding

### Service Layer
- Web APIs
    - SendGrid
      Email sending
    - OMDb
      Film data fetching from IMDb
    - Cloudinary
      Uploading user's images in the cloud
     
- Template model converting
    - Default service method
      ```
      public async Task<FilmData> GetAsync(string id)
      {
          return await context.FilmDatas.FindAsync(id);
      }
      ```
    - Model templated service method
      ```
      public async Task<TModel> GetAsync<TModel>(string id, Func<FilmData, TModel> mapToModelFunc)
      {
          var film = await GetAsync(id);
          return mapToModelFunc(film);
      }
      ```
      
### Presentation Layer
- ASP.NET Core 3.0
- Custom Mapper
    - Default mapping method
      ```
      private TToModel MapSimilarProperties<TFromModel, TToModel>(TFromModel fromObject)
             where TFromModel : class
             where TToModel : class, new()
        {
            TToModel model = new TToModel();
            if (fromObject != null)
            {
                PropertyInfo[] props = fromObject.GetType().GetProperties();
                var modelType = model?.GetType();
                foreach (var prop in props)
                {
                    var modelProperty = modelType?.GetProperty(prop.Name);
                    if (modelProperty?.PropertyType?.Equals(prop.PropertyType) ?? false)
                    {
                        modelProperty.SetValue(model, prop.GetValue(fromObject));
                    }
                }
            }
            return model;
        }
      ```
    - Mapping Entity model to View model example
      ```
      public ExtendedFilmCardViewModel MapToExtendedFilmCardViewModel(FilmData filmData)
        {
            var viewModel = MapSimilarProperties<FilmData, ExtendedFilmCardViewModel>(filmData);
            if (filmData != null)
            {
                viewModel.Id = filmData.FilmId;
                viewModel.Genres = string.Join(", ", filmData.Genre.Select(a => a.Genre.ToString()));
                viewModel.Rating = Math.Round(filmData.Film.Rating, 1).ToString();
            }
            return viewModel;
        }
      ```

### Unit Testing
- NUnit tests
    - 70% code coverage
- Moq Database mockup
