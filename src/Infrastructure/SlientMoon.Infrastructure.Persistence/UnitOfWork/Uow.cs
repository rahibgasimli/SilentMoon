using Microsoft.EntityFrameworkCore.Storage;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using SlientMoon.Infrastructure.Persistence.Contexts;
using SlientMoon.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public class Uow : IUow
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public IPomodoroRepository PomodoroRepository { get; }
    public ITopicRepository TopicRepository { get; }
    public IUserRepository UserRepository { get; }
    public IAdminRepository AdminRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public ICategoryTypeRepository CategoryTypeRepository { get; }
    public IReminderRepository ReminderRepository { get; }
    public ICourseRepository CourseRepository { get; }
    public ITrackRepository TrackRepository { get; }
    public INarratorRepository NarratorRepository { get; }
    public IGenericRepository<Favorite> FavoriteRepository { get; }
    public IDailyThoughtRepository DailyThoughtRepository { get; }

    public Uow(
        AppDbContext context,
        IPomodoroRepository pomodoroRepository,
        IUserRepository userRepository,
        IAdminRepository adminRepository,
        ITopicRepository topicRepository,
        ICourseRepository courseRepository,
        ITrackRepository trackRepository,
        INarratorRepository narratorRepository,
        IDailyThoughtRepository dailyThoughtRepository,
        ICategoryRepository categoryRepository,
        ICategoryTypeRepository categoryTypeRepository,
        IReminderRepository reminderRepository,
        IGenericRepository<Favorite> favoriteRepository)
    {
        _context = context;
        TopicRepository = topicRepository;
        CourseRepository = courseRepository;
        TrackRepository = trackRepository;
        NarratorRepository = narratorRepository;
        DailyThoughtRepository = dailyThoughtRepository;
        PomodoroRepository = pomodoroRepository;
        UserRepository = userRepository;
        AdminRepository = adminRepository;
        CategoryRepository = categoryRepository;
        CategoryTypeRepository = categoryTypeRepository;
        ReminderRepository = reminderRepository;
        FavoriteRepository = favoriteRepository;
    }

    public IGenericRepository<T> GenericRepository<T>() where T : class
    {
        return (IGenericRepository<T>)_repositories.GetOrAdd(
            typeof(T),
            _ => new GenericRepository<T>(_context)
        );
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction != null) return;
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
        _transaction?.Dispose();
    }

}