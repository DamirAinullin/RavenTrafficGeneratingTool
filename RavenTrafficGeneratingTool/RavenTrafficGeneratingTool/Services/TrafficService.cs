using System;
using System.Threading;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Raven.Client;
using Raven.Client.Document;
using RavenTrafficGeneratingTool.Messages;
using RavenTrafficGeneratingTool.Model;

namespace RavenTrafficGeneratingTool.Services
{
    public class TrafficService : ITrafficService, IDisposable
    {
        private IDocumentStore _documentStore;
        private Timer _timer;

        public void Start(string databaseUrl, int timesPerMinute)
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
            if (_documentStore != null)
            {
                if (!_documentStore.WasDisposed)
                {
                    _documentStore.Dispose();
                }
            }
            _documentStore = new DocumentStore
            {
                Url = databaseUrl,
                DefaultDatabase = "TestDatabase"
            };
            _documentStore.Initialize();

            int dueTime = Convert.ToInt32(60000.0 / timesPerMinute);
            _timer = new Timer(SendRequestCallback, null, 0, dueTime);
        }

        private void SendRequestCallback(object obj)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                var randomEntity = new RandomEntity
                {
                    Value1 = Guid.NewGuid().ToString(),
                    Value2 = Guid.NewGuid().ToString()
                };
                session.Store(randomEntity);

                try
                {
                    session.SaveChanges();
                }
                catch (ObjectDisposedException)
                {
                }
                
                Messenger.Default.Send(new RandomMessage { Text = randomEntity.ToString() });
                DispatcherHelper.CheckBeginInvokeOnUI(CommandManager.InvalidateRequerySuggested);
            }
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
            _documentStore?.Dispose();
            _documentStore = null;
        }
    }
}
