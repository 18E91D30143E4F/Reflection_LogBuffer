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
        public int MaxBufferSize { get; set; } = 5;

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
            _buffer.CollectionChanged += Buffer_Changed;

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            new Task(WriteByTime).Start();
        }

        public void Add(string item) => AddToBuffer(item);

        private void AddToBuffer(string item) => _buffer.Add(item);

        private void Buffer_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_buffer.Count != MaxBufferSize) return;
            foreach (var line in _buffer)
            {
                _streamWriter.WriteLine(line);
            }
            _buffer.Clear();
            _outputFunction("Data write to file max buffer size");
        }

        private void WriteByTime()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(IterateTime);
                lock (Locker)
                {
                    if (_buffer.Count != 0)
                    {
                        foreach (var line in _buffer)
                        {
                            _streamWriter.WriteLine(line);
                        }
                        _buffer.Clear();
                        _outputFunction("\nData write to file by time");
                    }
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
