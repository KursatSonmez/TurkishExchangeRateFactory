using ExchangeRateFactory.Demo.Data.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ExchangeRateFactory.Demo
{
    public class DataSeeder : IDisposable
    {
        private readonly ExchangeRateFactoryDbContext _context;
        public DataSeeder(
            ExchangeRateFactoryDbContext context
            )
        {
            _context = context;
        }

        public async Task Seed()
        {
            if (_context.Database.IsInMemory() == false)
                await _context.Database.MigrateAsync();
            else
            {
                await _context.Database.EnsureDeletedAsync();
                await _context.Database.EnsureCreatedAsync();
            }
        }

        #region IDisposable

        private void DisposeManagedResources()
        {
        }
        private void DisposeNativeResources()
        {
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
                DisposeManagedResources();

            DisposeNativeResources();
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DataSeeder() => Dispose(false);

        #endregion
    }
}
