using DevPract.AllData.Base;
using DevPract.AllData.ViewModels;
using DevPract.Models.Domain;

namespace DevPract.AllData.Services
{
    public interface IMoviesService : IEntityBaseRepository<Movie>
    {
        Task<Movie> GetMovieByIdAsync(int id);
        Task<NewMovieDropdownsVM> GetNewMovieDropdownsValues();
        Task AddNewMovieAsync(NewMovieVM data);
        Task UpdateMovieAsync(NewMovieVM data);
        
    }
}
