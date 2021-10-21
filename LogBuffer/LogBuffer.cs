using System;
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
        public int IterateTime { get; set; } = 5000; // milliseconds
        public int MaxBufferSize { get; set; } = 5; // message count

        private readonly StreamWriter _streamWriter;
        private readonly ObservableCollection<string> _buffer;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;
        private static readonly object Locker = new object();

        public delegate void OutputFunction(string message);
        private readonly OutputFunction _outputFunction;

        public LogBuffer(string pathToFile, OutputFunction outputFunction)
        {
            PathToFile = Path.GetFullPath(pathToFile);
            _outputFunction = outputFunction;

            _streamWriter = new StreamWriter(pathToFile, append: true, Encoding.Default);
            _buffer = new ObservableCollection<string>();
            _buffer.CollectionChanged += Buffer_ChangedAsync;

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            new Task(WriteByTimeAsync).Start();
        }

        public void Add(string item) => AddToBuffer(item);
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _streamWriter.Close();
        }

        private void AddToBuffer(string item)
        {
            lock (Locker)
            {
                _buffer.Add(item);
            }
        }

        private async Task WriteLineAsync(string line) => await _streamWriter.WriteLineAsync(line);
        private async void Buffer_ChangedAsync(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (_buffer.Count != MaxBufferSize) return;
            foreach (var line in _buffer)
            {
                await WriteLineAsync(line);
            }

            _buffer.Clear();
            _outputFunction("\nData write to file max buffer size");

        }

        private async void WriteByTimeAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(IterateTime);

                if (_buffer.Count != 0)
                {
                    foreach (var line in _buffer)
                    {
                        await WriteLineAsync(line);
                    }
                    _buffer.Clear();
                    _outputFunction("\nData write to file by time");
                }
            }
        }
    }
}

