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

using Common;
using Demuxer.Common;
using ElmSharp;
using FFmpegPlayer.Common;
using FFmpegPlayer.DataProviders;
using FFmpegPlayer.DataReaders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tizen.TV.Multimedia;

namespace FFmpegPlayer.DataPresenters.EsPlayer
{
    public sealed class EsPlayerPresenter : DataPresenter
    {
        private static readonly TimeSpan SeekDistance = TimeSpan.FromSeconds(10);

        private ESPlayer _esPlayer;
        private DataProvider _dataProvider;
        private FactoryDelegate<DataReader> _createDataReader;
        private DataReader _dataReaderSession;
        private event EosDelegate EosHandler;
        private event ErrorDelegate ErrorHandler;

        public EsPlayerPresenter()
        {
            EsPlayerExtensions.Init();
        }

        public override async Task Open(Window presenterWindow)
        {
            Log.Enter();

            var openTask = _dataProvider.Open();
            Log.Info("Opening data provider");

            _esPlayer = CreateESplayer(presenterWindow);
            Log.Info("ESPlayer created");

            var clipConfig = await openTask;

            // Configure platform player and initiate playback.
            var (readyToTransferTask, prepareAsyncTask) = PrepareESplayer(_esPlayer, clipConfig.StreamConfigs);
            await readyToTransferTask;

            // Start data transfer.
            _dataReaderSession = _createDataReader()
                .AddHandler(ErrorHandler)
                .With(_dataProvider, PresentPacket);

            // Wait for PreapareAsync completion.
            await prepareAsyncTask;
            Log.Info("PrepareAsync() completed");

            // Start playback.
            _esPlayer.Start();
            Log.Info("Plyback started");

            Log.Exit();
        }

        public override async Task Seek(SeekDirection direction)
        {
            Log.Enter();

            // Check if stream is seekable. Duration > 0 = seekable.
            var streamDuration = _dataProvider.CurrentConfiguration.Duration;
            if (streamDuration == TimeSpan.Zero)
            {
                Log.Info("Stream is not seekable");
                return;
            }

            // Terminate reading session if running.
            var resumeAfterSeek = _dataReaderSession != null;
            if (resumeAfterSeek)
            {
                _esPlayer.Pause();
                await _dataReaderSession.DisposeAsync();
            }

            // Compute next position
            var position = SeekNextPosition(direction, streamDuration);
            Log.Info($"Seek {direction} to {position}. Playing: {resumeAfterSeek}");

            // Start DataProvider seek.
            var seekDataTask = _dataProvider.Seek(position);
            Log.Info("Data provider seeking.");

            // Start EsPlayer seek.
            var (readyToTransferTask, seekAsyncTask) = SeekESplayer(
                _esPlayer,
                position,
                _dataProvider.CurrentConfiguration.StreamConfigs.Count);
            Log.Info("ESPlayer seeking.");

            // Wait for ready to transfer
            await Task.WhenAll(readyToTransferTask, seekDataTask);
            var newSession = _createDataReader()
                .AddHandler(ErrorHandler)
                .With(_dataProvider, PresentPacket);
            Log.Info($"Data start position {seekDataTask.Result}");

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
                _dataReaderSession = newSession;
                Log.Info("Playback resumed");
            }

            Log.Exit();
        }

        public override Task Suspend()
        {
            Log.Enter();

            Task pauseTask;
            if (_dataReaderSession == null)
            {
                pauseTask = Task.CompletedTask;
            }
            else
            {
                _esPlayer.Pause();
                pauseTask = Task.WhenAll(_dataProvider.Suspend(), _dataReaderSession.DisposeAsync());
                _dataReaderSession = null;

                Log.Info("Pausing");
            }

            Log.Exit();
            return pauseTask;
        }

        public override Task Resume()
        {
            Log.Enter();

            Task resumeTask;
            if (_dataReaderSession != null)
            {
                resumeTask = Task.CompletedTask;
            }
            else
            {
                resumeTask = _dataProvider.Resume();
                _esPlayer.Resume();
                _dataReaderSession = _createDataReader()
                    .AddHandler(ErrorHandler)
                    .With(_dataProvider, PresentPacket);

                Log.Info("Resuming");
            }

            Log.Exit();
            return resumeTask;
        }

        public override DataPresenter With(DataProvider dataProvider)
        {
            Log.Enter(dataProvider.GetType().ToString());

            _dataProvider = dataProvider;

            Log.Exit();
            return this;
        }

        public override DataPresenter With(FactoryDelegate<DataReader> createDataReader)
        {
            Log.Enter(nameof(FactoryDelegate<DataReader>));

            _createDataReader = createDataReader;

            Log.Exit();
            return this;
        }

        public override DataPresenter AddHandlers(EosDelegate eosHandler, ErrorDelegate errorHandler)
        {
            Log.Enter();

            EosHandler += eosHandler;
            ErrorHandler += errorHandler;

            Log.Exit();
            return this;
        }

        private TimeSpan SeekNextPosition(SeekDirection direction, TimeSpan streamDuration)
        {
            // Compute new position based on direction and ESPlayer playback position.
            _esPlayer.GetPlayingTime(out var position);

            Log.Enter(position.ToString());

            if (direction == SeekDirection.Forward)
            {
                position += SeekDistance;
                if (position >= streamDuration)
                    position = streamDuration;
            }
            else
            {
                position -= SeekDistance;
                if (position < TimeSpan.Zero)
                    position = TimeSpan.Zero;
            }

            Log.Exit(position.ToString());
            return position;
        }

        private PresentPacketResult PresentPacket(Packet packet)
        {
            // Handle End of all streams
            if (packet == null)
            {
                var configuredStreams = _dataProvider.CurrentConfiguration.StreamConfigs;
                foreach (var stream in configuredStreams)
                    _esPlayer.SubmitEosPacket(stream.StreamType().ESStreamType());

                Log.Info("EOS issued");
                return PresentPacketResult.Success;
            }

            var status = _esPlayer.Submit(packet);
            Log.Verbose($"{packet.Pts} {packet.StreamType} {status}");
            switch (status)
            {
                case SubmitStatus.Full:
                    Log.Warn($"{packet.StreamType}:{packet.Pts} {SubmitStatus.Full}");
                    return PresentPacketResult.Retry;

                case SubmitStatus.InvalidPacket:
                    Log.Error($"{packet.StreamType}:{packet.Pts} {SubmitStatus.InvalidPacket}");
                    // Ignore and try next packet.
                    return PresentPacketResult.Success;

                case SubmitStatus.NotPrepared:
                case SubmitStatus.OutOfMemory:
                    Log.Fatal($"{packet.StreamType}:{packet.Pts} {status}");
                    return PresentPacketResult.Fail;

                default:
                    return PresentPacketResult.Success;
            }
        }

        private ESPlayer CreateESplayer(Window playerWindow)
        {
            Log.Enter();

            var player = new ESPlayer();
            player.Open();
            player.SetDisplay(playerWindow);

            player.EOSEmitted += OnEos;
            player.ErrorOccurred += OnError;
            player.BufferStatusChanged += OnBufferStatusChanged;

            Log.Exit();
            return player;
        }

        private void OnEos(object sender, EOSEventArgs eosArgs)
        {
            Log.Enter();

            Log.Info("End of stream");
            EosHandler?.Invoke();

            Log.Exit($"Handler invoked: {EosHandler != null}");
        }

        private void OnError(object sender, ErrorEventArgs errorArgs)
        {
            Log.Enter();

            var errorMsg = errorArgs.ErrorType.ToString();
            Log.Fatal($"Playbak error {errorMsg}");
            ErrorHandler?.Invoke(errorMsg);

            Log.Exit($"Handler invoked: {ErrorHandler != null}");
        }

        private static void OnBufferStatusChanged(object sender, BufferStatusEventArgs bufferArgs)
        {
            Log.Debug($"{bufferArgs.StreamType} {bufferArgs.BufferStatus}");
        }

        private static (Task readyToTransferTask, Task prepareAsyncTask) PrepareESplayer(ESPlayer player, ICollection<StreamConfig> streamConfigs)
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
            Log.Info($"PreparingAsync( {streamCount} streams )");
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

            Log.Info($"Terminating DataReader session: {_dataReaderSession != null}");
            _dataReaderSession?.Dispose();
            _dataReaderSession = null;
            _createDataReader = null;

            Log.Info($"Disposing data provider: {_dataProvider != null}");
            _dataProvider?.Dispose();
            _dataProvider = null;

            Log.Info($"Disposing ESPlayer: {_esPlayer != null}");
            _esPlayer?.Dispose();
            _esPlayer = null;

            ErrorHandler = null;
            EosHandler = null;

            Log.Exit();
        }
    }
}
