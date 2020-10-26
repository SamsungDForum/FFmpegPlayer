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
using Tizen.Applications;
using ElmSharp;
using Common;

namespace FFmpeg.Player
{
    public class PlayerApp : CoreUIApplication
    {
        private Window _window;
        private Player _player;

        private static void LogUnhandledException(object o, UnhandledExceptionEventArgs e) => Log.Fatal(e.ToString());

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
            new PlayerApp().Run(args);
        }

        protected override void OnAppControlReceived(AppControlReceivedEventArgs e)
        {
            Log.Enter();
            base.OnAppControlReceived(e);

            _player = new Player(_window);
            _player.Start("media://url.goes.here/content");

            ReceivedAppControl control = e.ReceivedAppControl;
            if (control.IsReplyRequest)
                control.ReplyToLaunchRequest(new AppControl(), AppControlReplyResult.Succeeded);

            Log.Exit();
        }

        protected override void OnCreate()
        {
            Log.Enter();

            _window = new Window("FFmpeg.Player")
            {
                Geometry = new Rect(0, 0, 1920, 1080)
            };

            base.OnCreate();
            Log.Exit();
        }

        protected override void OnTerminate()
        {
            Log.Enter();

            _player.Dispose();
            base.OnTerminate();

            Log.Exit(@"(⌐■_■) == ε /̵͇̿​̿/’̿’̿ ̿ ̿̿(╥﹏╥)");
        }
    }
}
