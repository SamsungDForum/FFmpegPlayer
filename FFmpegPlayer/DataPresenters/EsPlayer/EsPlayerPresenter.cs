/*!
 * https://github.com/SamsungDForum/JuvoPlayer
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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Demuxer.Common;
using ElmSharp;
using FFmpegPlayer.DataProviders;
using FFmpegPlayer.DataReaders;
using FFmpegPlayer.PlatformPlayer;
using Tizen.TV.Multimedia;

namespace FFmpegPlayer.DataPresenters.EsPlayer
{
    public class EsPlayerPresenter : DataPresenter
    {
        private static readonly TimeSpan SeekDistance = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan SubmitFullDelay = TimeSpan.FromMilliseconds(150);

        private readonly Window _window;
        private CancellationToken _token = CancellationToken.None;
        private DataProvider _dataProvider;
        private DataReader _dataReader;

        private ESPlayer _esPlayer;
        private IDisposable _readingSession;

        public EsPlayerPresenter(Window window)
        {
            _window = window;
            EsPlayerExtensions.Init();
        }

        // TODO: Review adding cancellation support
        public override async Task Open()
        {
            Log.Enter();

            var openTask = _dataProvider.Open();

            var player = CreateESplayer(_window);
            Log.Info("Platform player created");

            var clipConfig = await openTask;
            Log.Info("Data provider opened");

            // Configure platform player and initiate playback.
            var (readyToTransferTask, prepareAsyncTask) = PrepareESplayer(player, clipConfig.StreamConfigs);
            await readyToTransferTask;

            // Start data transfer.
            _esPlayer = player;
            _readingSession = _dataReader.Create(_dataProvider, PresentPacket, _token);
            Log.Info("Transfer started");

            await prepareAsyncTask;
            Log.Info("PrepareAsync() completed");

            // Start playback.
            player.Start();
            Log.Info("Plyback started");

            Log.Exit();
        }

        // TODO: Review adding cancellation support
        public override async Task Seek(SeekDirection direction)
        {
            Log.Enter();

            bool resumeAfterSeek = _readingSession != null;
            Log.Info($"Seek {direction}. Playing: {resumeAfterSeek}");

            if (resumeAfterSeek)
            {
                // Playback's running. Stop it.
                _readingSession.Dispose();
                _readingSession = null;
                _esPlayer.Pause();
            }

            // Compute new position based on direction and ESPlayer playback position.
            _esPlayer.GetPlayingTime(out var position);
            if (direction == SeekDirection.Forward)
            {
                position += SeekDistance;
                //TODO: Clip to max duration? Needed?
            }
            else
            {
                position -= SeekDistance;
                if (position < TimeSpan.Zero)
                    position = TimeSpan.Zero;
            }

            // Seek data provider.
            var seekDataTask = _dataProvider.Seek(position);
            Log.Info($"Data provider seeking to {position}");

            // Seek ESPlayer.
            var (readyToTransferTask, seekAsyncTask) = SeekESplayer(
                _esPlayer,
                position,
                _dataProvider.GetCurrentConfiguration().StreamConfigs.Count);
            Log.Info($"ESPlayer seeking to {position}");
            await Task.WhenAll(readyToTransferTask, seekDataTask);

            // data provider and ESPlayer ready to transfer.
            var newSession = _dataReader.Create(_dataProvider, PresentPacket, _token);
            Log.Info($"Transfer started. Data seek position {seekDataTask.Result}");

            // Wait for seek async completion.
            await seekAsyncTask;
            Log.Info("SeekAsync() completed");

            if (!resumeAfterSeek)
            {
                // Stop data transfer
                _esPlayer.Pause();
                newSession.Dispose();
                Log.Info("Playback suspended");
            }
            else
            {
                // Resume playback.
                _esPlayer.Resume();
                _readingSession = newSession;
                Log.Info("Playback resumed");
            }

            Log.Exit();
        }

        public override void Pause()
        {
            Log.Enter();

            if (_readingSession != null)
            {
                _readingSession.Dispose();
                _readingSession = null;

                _esPlayer.Pause();

                Log.Info("Paused");
            }

            Log.Exit();
        }

        public override void Resume()
        {
            Log.Enter();

            if (_readingSession == null)
            {
                _esPlayer.Resume();
                _readingSession = _dataReader.Create(_dataProvider, PresentPacket, _token);

                Log.Info("Resumed");
            }

            Log.Exit();
        }

        public override DataPresenter With(DataProvider dataProvider)
        {
            Log.Enter("DataProvider");

            _dataProvider = dataProvider;

            Log.Exit();
            return this;
        }

        public override DataPresenter With(CancellationToken token)
        {
            Log.Enter("CancellationToken");

            _token = token;

            Log.Exit();
            return this;
        }

        public override DataPresenter With(DataReader dataReader)
        {
            Log.Enter("DataReader");

            _dataReader = dataReader;

            Log.Exit();
            return this;
        }

        private async ValueTask PresentPacket(Packet packet, TimeSpan presentationDelay, CancellationToken token)
        {
            if (presentationDelay != default)
            {
                try
                {
                    await Task.Delay(presentationDelay, token);
                }
                catch (OperationCanceledException)
                {
                    // Ignore to push last packet out
                }
            }

            SubmitStatus status;
            do
            {
                status = _esPlayer.Submit(packet);
                switch (status)
                {
                    case SubmitStatus.Full:
                        Log.Warn($"{packet.StreamType}:{packet.Pts} {SubmitStatus.Full}");
                        await Task.Delay(SubmitFullDelay, token);
                        break;

                    case SubmitStatus.InvalidPacket:
                        Log.Error($"{packet.StreamType}:{packet.Pts} {SubmitStatus.InvalidPacket}");
                        // Ignore and try next packet.
                        status = SubmitStatus.Success;
                        break;

                    case SubmitStatus.NotPrepared:
                    case SubmitStatus.OutOfMemory:
                        throw new Exception($"{packet.StreamType}:{packet.Pts} {status}");
                }

                // Cancellation when resubmission is required will result in packet loss.
            } while (status != SubmitStatus.Success && !token.IsCancellationRequested);
        }

        private static ESPlayer CreateESplayer(Window playerWindow)
        {
            Log.Enter();

            var player = new ESPlayer();
            player.Open();
            player.SetDisplay(playerWindow);

            player.EOSEmitted += OnEos;
            player.ErrorOccurred += OnError;
            player.BufferStatusChanged += OnBufferStatusChanged;

            playerWindow.Show();

            Log.Exit();
            return player;
        }

        private static void OnEos(object sender, EOSEventArgs eosArgs)
        {
            Log.Info("End Of Stream.");
        }

        private static void OnError(object sender, ErrorEventArgs errorArgs)
        {
            Log.Fatal($"Playbak error {errorArgs.ErrorType}");
            throw new Exception($"Playbak error {errorArgs.ErrorType}");
        }

        private static void OnBufferStatusChanged(object sender, BufferStatusEventArgs bufferArgs)
        {
            Log.Debug($"{bufferArgs.StreamType} {bufferArgs.BufferStatus}");
        }

        private static (Task readyToTransferTask, Task prepareAsyncTask) PrepareESplayer(ESPlayer player, IList<StreamConfig> streamConfigs)
        {
            Log.Enter();

            foreach (var config in streamConfigs)
            {
                switch (config)
                {
                    case AudioStreamConfig audioConfig:
                        player.SetStream(audioConfig.ESAudioStreamInfo());
                        break;

                    case VideoStreamConfig videoConfig:
                        player.SetStream(videoConfig.ESVideoStreamInfo());
                        break;
                }
            }

            // Completes when all streams report onReadyToPreapre.
            var readyToTransferTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            int streamCount = streamConfigs.Count;
            Log.Info($"PreperaingAsync( {streamCount} streams )");
            var prepareAsyncTask = player.PrepareAsync(OnReadyToPreapare);

            Log.Exit();
            return (readyToTransferTcs.Task, prepareAsyncTask);

            void OnReadyToPreapare(Tizen.TV.Multimedia.StreamType stream)
            {
                if (--streamCount == 0)
                    readyToTransferTcs.SetResult(null);

                Log.Info($"onReadyToPreapre( {stream} )");
            }
        }

        private static (Task readyToTransferTask, Task seekAsyncTask) SeekESplayer(ESPlayer player, TimeSpan seekTo, int streamCount)
        {
            Log.Enter();

            // Completes when all streams report onReadyToSeek.
            var readyToTransferTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            Log.Info($"SeekingAsync( {streamCount} streams )");
            var seekAsyncTask = player.SeekAsync(seekTo, OnReadyToSeek);

            Log.Exit();
            return (readyToTransferTcs.Task, seekAsyncTask);

            void OnReadyToSeek(Tizen.TV.Multimedia.StreamType stream, TimeSpan position)
            {
                if (--streamCount == 0)
                    readyToTransferTcs.SetResult(null);

                Log.Info($"onReadyToSeek( {stream} {position} )");
            }
        }

        public override void Dispose()
        {
            Log.Enter();

            Log.Info($"Disposing data reader: {_dataReader != null}");
            _dataReader?.Dispose();
            _dataReader?.WaitCompletion();
            _dataReader = null;
            _readingSession = null;

            Log.Info($"Disposing ESPlayer: {_esPlayer != null}");
            _esPlayer?.Dispose();
            _esPlayer = null;

            Log.Info($"Disposing data provider: {_dataProvider != null}");
            _dataProvider?.Dispose();
            _dataProvider = null;

            Log.Exit();
        }
    }
}
