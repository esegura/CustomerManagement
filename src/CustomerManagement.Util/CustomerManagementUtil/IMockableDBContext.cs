using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomerManagement.Util
{
    public interface IMockableDBContext : IDisposable
    {
        int SaveChanges();

        T Save<T>(T entity) where T : class;

        T Update<T>(T entity) where T : class;

        T Delete<T>(T entity) where T : class;

        T Delete<T>(long id) where T : class;

        T Attach<T>(T entity) where T : class;
    }
}
