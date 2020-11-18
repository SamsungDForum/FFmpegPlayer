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
using System.Linq;
using Tizen.Applications;
using ElmSharp;
using Common;

namespace FFmpegPlayer
{
    public class PlayerApp : CoreUIApplication
    {
        private enum WindowKey
        {
            Return,
            Right,
            Left
        }

        private Window _window;
        private EventLoop _eventLoop;
        private ILookup<string, Action> _handledKeys;
        private static void LogUnhandledException(object o, UnhandledExceptionEventArgs e) => Log.Fatal(e.ToString());

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
            new PlayerApp().Run(args);
        }

        private ILookup<string, Action> CreateKeyHandlers(Window win, EventLoop evLoop)
        {
            Log.Enter();

            var handlers = new (string keyName, Action keyHandler)[]
            {
                (WindowKey.Return.ToString(), evLoop.PlayPause),
                (WindowKey.Right.ToString(), evLoop.Forward),
                (WindowKey.Left.ToString(), evLoop.Rewind),
            }.ToLookup(entry => entry.keyName, entry => entry.keyHandler);

            win.KeyUp += OnKeyUp;
            win.BackButtonPressed += (s, e) => Exit();

            Log.Exit();
            return handlers;
        }

        private void OnKeyUp(object sender, EvasKeyEventArgs e)
        {
            Log.Enter(e.KeyName);

            var keyHandler = _handledKeys[e.KeyName].FirstOrDefault();
            keyHandler?.Invoke();

            Log.Exit(keyHandler == null ? "Not handled" : "Handled");
        }

        protected override void OnAppControlReceived(AppControlReceivedEventArgs e)
        {
            Log.Enter();
            base.OnAppControlReceived(e);

            // Create event loop
            _eventLoop = new EventLoop();

            // Add key handler. Requires event loop.
            _handledKeys = CreateKeyHandlers(_window, _eventLoop);

            // Start playback
            _ = _eventLoop.SendMessage(PlayerEvent.Open, _window);
            _window.Show();

            ReceivedAppControl control = e.ReceivedAppControl;
            if (control.IsReplyRequest)
                control.ReplyToLaunchRequest(new AppControl(), AppControlReplyResult.Succeeded);

            Log.Exit();
        }

        protected override void OnCreate()
        {
            Log.Enter();

            _window = new Window("FFmpegPlayer.Window")
            {
                Geometry = new Rect(0, 0, 1920, 1080)
            };

            base.OnCreate();
            Log.Exit();
        }

        protected override void OnTerminate()
        {
            Log.Enter();

            _eventLoop.Dispose();

            base.OnTerminate();
            Log.Exit(@"(⌐■_■) == ε /̵͇̿​̿/’̿’̿ ̿ ̿̿(╥﹏╥)");
        }
    }
}
