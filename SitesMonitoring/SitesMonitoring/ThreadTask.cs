using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SitesMonitoring.Models
{
    public class ThreadTask : IDisposable
    {       
        public ThreadTask()
        {
            _disposed = false;
            _locker = new object();
            _syncThread = new BackgroundWorker()
            {
                WorkerSupportsCancellation = true
            };
            _stopingThreadEvent = new ManualResetEvent(false);
            _stopedThreadEvent = new ManualResetEvent(false);
        }

        #region IDisposable

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    _stopedThreadEvent.Dispose();
                    _stopingThreadEvent.Dispose();
                    _syncThread.Dispose();
                }

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~ThreadTask()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion IDisposable            

        private object _locker;

        IConfiguration configuration;

        /// <summary>
        /// Поток.
        /// </summary>
        private BackgroundWorker _syncThread;

        /// <summary>
        /// Событие требования остановки фонового потока.
        /// </summary>
        private ManualResetEvent _stopingThreadEvent;

        /// <summary>
        /// Фоновый поток остановлен.
        /// </summary>
        private ManualResetEvent _stopedThreadEvent;

        public void Start()
        {
            RunWorker();          
        }

        public void Stop()
        {
            StopWorker();           
        }
        /// <summary>
        /// Запуск потока.
        /// </summary>    
        private void RunWorker()
        {           
            lock (_locker)
            {
                if (!_syncThread.IsBusy)
                {                        
                    _syncThread.WorkerSupportsCancellation = true;
                    _syncThread.RunWorkerCompleted += SyncThread_RunWorkerCompleted;
                    _syncThread.DoWork += Work;
                    _stopingThreadEvent.Reset();
                    _stopedThreadEvent.Reset();
                    _syncThread.RunWorkerAsync(this);
                    var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");
                    configuration = builder.Build();
                }
            }            
        }
       
        private void Work(object sender, DoWorkEventArgs e)
        {            
            while (!IsCancellationPending)
            {
                Standby();
                WorkingCycle();
            }           
           
        }
        private static Uri GetUri(string s)
        {
            return new UriBuilder(s).Uri;
        }

        public async void WorkingCycle()
        {
            if (configuration == null)
                return;
            var optionsBuilder = new DbContextOptionsBuilder<SitesMonitoringContext>();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("SitesMonitoringContext"));

            using (var context = new SitesMonitoringContext(optionsBuilder.Options))
            {
                foreach (Site item in context.Site.ToList<Site>())
                {
                    try
                    {
                        Uri uri = GetUri(item.URL);

                        HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        item.Status = 1;
                        item.LastCheckedTime = DateTime.Now;
                    }
                    catch
                    {
                        item.Status = 0;
                        item.LastCheckedTime = DateTime.Now;
                    }
                    context.Update(item);
                    await context.SaveChangesAsync();
                }
            }
        }

                     

        /// <summary>
        /// Поток синхронизации завершен.
        /// </summary>
        /// <param name="sender">Отправитель события.</param>
        /// <param name="e">Параметр события.</param>
        private void SyncThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (_locker)
            {
                _syncThread.RunWorkerCompleted -= SyncThread_RunWorkerCompleted;
                _stopedThreadEvent.Set();
            }
        }

        /// <summary>
        /// Остановка потока синхронизации.
        /// </summary>
        private WaitHandle StopWorker()
        {
            _syncThread.CancelAsync();
            _stopingThreadEvent.Set();
            return _stopedThreadEvent;
        }

        private bool IsCancellationPending
        {
            get
            {
                return _syncThread.CancellationPending;
            }
        }

        //сколько ждать
        private void Standby()
        {
            if (configuration == null)
                return;
            var optionsBuilder = new DbContextOptionsBuilder<SitesMonitoringContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("SitesMonitoringContext"));

            using (var context = new SitesMonitoringContext(optionsBuilder.Options))
            {
                SitesController sc = new SitesController(context);
                int sec =  sc.GetWaitTime();
                _stopingThreadEvent.WaitOne(new TimeSpan(0, 0, sec));
            }
        }

       
        
    }
}
