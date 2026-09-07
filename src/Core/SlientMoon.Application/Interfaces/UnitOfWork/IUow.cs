using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

public interface IUow : IDisposable
{
    IGenericRepository<T> GenericRepository<T>() where T : class;
    IPomodoroRepository PomodoroRepository { get; }
    IUserRepository UserRepository { get; }
    IAdminRepository AdminRepository { get; }
    ITopicRepository TopicRepository { get; }
    IReminderRepository ReminderRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    ICategoryTypeRepository CategoryTypeRepository { get; }
    ICourseRepository CourseRepository { get; }
    ITrackRepository TrackRepository { get; }
    IDailyThoughtRepository DailyThoughtRepository { get; }
    IGenericRepository<Favorite> FavoriteRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}