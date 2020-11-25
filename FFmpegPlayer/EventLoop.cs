/*!
 * https://github.com/SamsungDForum/FFmpegPlayer
 * Copyright 2020, Samsung Electronics Co., Ltd
 * Licensed under the MIT license
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using ElmSharp;
using FFmpegPlayer.DataPresenters;
using FFmpegPlayer.DataPresenters.EsPlayer;
using FFmpegPlayer.DataProviders.SingleSource;
using FFmpegPlayer.DataReaders;
using FFmpegPlayer.DataReaders.Generic;
using FFmpegPlayer.DataSources;
using FFmpegPlayer.DataSources.FFmpeg;
using FFmpegPlayer.DataSources.FFmpeg.Options;

namespace FFmpegPlayer
{
    public enum PlayerEvent
    {
        Close,
        Open,
        Seek,
        Suspend,
        Resume,
        EndOfStream,
        Error
    }

    public class EventLoop : IDisposable
    {
        private EventChannel<PlayerEvent> _eventChannel;
        private CancellationTokenSource _sessionCts;
        private Task<Task> _eventLoopTask;
        private bool? _playPause;

        public EventLoop()
        {
            _sessionCts = new CancellationTokenSource();
            _eventChannel = new EventChannel<PlayerEvent>(_sessionCts.Token);

            _eventLoopTask = Task.Factory.StartNew(EventLoopTask,
                TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning);
        }

        public void Forward()
        {
            Log.Enter();

            if (!_playPause.HasValue)
            {
                Log.Warn("Not playing");
            }
            else
            {
                var eventTask = _eventChannel.Send(PlayerEvent.Seek, SeekDirection.Forward);
                if (!eventTask.IsCompleted)
                    eventTask.AsTask().GetAwaiter().GetResult();
            }

            Log.Exit();
        }

        public void Rewind()
        {
            Log.Enter();

            if (!_playPause.HasValue)
            {
                Log.Warn("Not playing");
            }
            else
            {
                var eventTask = _eventChannel.Send(PlayerEvent.Seek, SeekDirection.Backward);
                if (!eventTask.IsCompleted)
                    eventTask.AsTask().GetAwaiter().GetResult();

                Log.Exit();
            }
        }

        public void PlayPause()
        {
            Log.Enter();

            if (!_playPause.HasValue)
            {
                Log.Warn("Not playing");
            }
            else
            {
                var eventTask = _eventChannel.Send(_playPause.Value ? PlayerEvent.Suspend : PlayerEvent.Resume);
                if (!eventTask.IsCompleted)
                    eventTask.AsTask().GetAwaiter().GetResult();
            }

            Log.Exit();
        }

        public ValueTask SendMessage(PlayerEvent ev)
        {
            return _eventChannel.Send(ev);
        }

        public ValueTask SendMessage(PlayerEvent ev, object evArgs)
        {
            return _eventChannel.Send(ev, evArgs);
        }

        private void OnError(string errorMessage)
        {
            _ = _eventChannel.Send(PlayerEvent.Error, errorMessage);
        }

        private void OnEos()
        {
            _ = _eventChannel.Send(PlayerEvent.EndOfStream);
        }

        private async Task EventLoopTask()
        {
            Log.Enter();

            DataPresenter presenter = new EsPlayerPresenter()
                .AddHandlers(OnEos, OnError)
                .With(DataReader.CreateFactoryFor<GenericPacketReader>())
                .With(new SingleSourceDataProvider()
                    .Add(new BufferedGenericSource()
                        .AddHandler(OnError)
                        //.Add("http://multiplatform-f.akamaihd.net/i/multi/april11/sintel/sintel-hd_,512x288_450_b,640x360_700_b,768x432_1000_b,1024x576_1400_m,.mp4.csmil/master.m3u8")
                        .Add("rtsp://106.120.45.49/test.ts")
                        .With(new DataSourceOptions()
                            .Set(RtspOption.BufferSize, 1024 * 1024)
                            .Set(RtspOption.Timeout, 5 * 1000000))));

            try
            {
                Log.Info("Started");

                while (!_sessionCts.IsCancellationRequested)
                {
                    (PlayerEvent message, object messageArg) = await _eventChannel.Read();
                    switch (message)
                    {
                        case PlayerEvent.Close:
                        case PlayerEvent.EndOfStream:
                            _sessionCts.Cancel();
                            _playPause = null;
                            break;

                        case PlayerEvent.Error when messageArg is string errorMessage:
                            Log.Fatal(errorMessage);
                            _sessionCts.Cancel();
                            _playPause = null;
                            break;

                        case PlayerEvent.Open when messageArg is Window presenterWindow:
                            await presenter.Open(presenterWindow);
                            _playPause = true;
                            break;

                        case PlayerEvent.Suspend:
                            await presenter.Suspend();
                            _playPause = false;
                            break;

                        case PlayerEvent.Resume:
                            await presenter.Resume();
                            _playPause = true;
                            break;

                        case PlayerEvent.Seek when messageArg is SeekDirection seekDirection:
                            await presenter.Seek(seekDirection);
                            break;

                        default:
                            throw new ArgumentException($"Invalid message {message} argument '{messageArg?.GetType()}'",
                                nameof(messageArg));
                    }

                    Log.Info($"Processed {message}");
                }
            }
            catch (OperationCanceledException)
            {
                // silent ignore
            }
            catch (Exception e)
            {
                Log.Fatal(e);
            }
            finally
            {
                Log.Info("Disposing presenter");
                presenter.Dispose();

                Log.Exit();
            }
        }

        public void Dispose()
        {
            Log.Enter();

            Log.Info("Terminating EventLoopTask");
            _sessionCts.Cancel();
            _eventLoopTask.GetAwaiter().GetResult().GetAwaiter().GetResult();
            _eventLoopTask = null;

            Log.Info("Disposing event channel");
            _eventChannel.Dispose();
            _eventChannel = null;

            _sessionCts.Dispose();
            _sessionCts = null;

            Log.Exit();
        }
    }
}
