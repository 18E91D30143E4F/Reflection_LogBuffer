using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogBuffer
{
    public class LogBuffer : IDisposable
    {
        public string PathToFile { get; set; }

        private readonly StreamWriter _streamWriter;
        private ObservableCollection<string> _buffer;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        private readonly int iterateTime = 5000; // milliseconds
        private static object locker = new object();

        public LogBuffer(string pathToFile)
        {
            PathToFile = pathToFile;

            _streamWriter = new StreamWriter(pathToFile, append: true, Encoding.Default);
            _buffer = new ObservableCollection<string>();
            _buffer.CollectionChanged += Buffer_Changed;

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            new Task(WriteByTime).Start();
        }

        public void Add(string item) => AddToBuffer(item);

        private void AddToBuffer(string item) => _buffer.Add(item);

        private void Buffer_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_buffer.Count != 10) return;
            foreach (var line in _buffer)
            {
                _streamWriter.WriteLine(line);
            }
            _buffer.Clear();
        }

        private void WriteByTime()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(iterateTime);
                lock (locker)
                {
                    foreach (var line in _buffer)
                    {
                        _streamWriter.WriteLine(line);
                    }
                    _buffer.Clear();
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _streamWriter.Close();
        }
    }
}
