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
using System.Runtime.CompilerServices;

namespace Common
{
    public static class Log
    {
        private const string _tag = "ffPlay";
        private const string _empty = "";

        public static void Verbose(
            string message,
            [CallerFilePath] string file = _empty,
            [CallerMemberName] string method = _empty,
            [CallerLineNumber] int line = 0) => Tizen.Log.Verbose(_tag, message, file, method, line);

        public static void Debug(
            string message,
            [CallerFilePath] string file = _empty,
            [CallerMemberName] string method = _empty,
            [CallerLineNumber] int line = 0) => Tizen.Log.Debug(_tag, message, file, method, line);

        public static void Info(
            string message,
            [CallerFilePath] string file = _empty,
            [CallerMemberName] string method = _empty,
            [CallerLineNumber] int line = 0) => Tizen.Log.Info(_tag, message, file, method, line);

        public static void Warn(
            string message,
            [CallerFilePath] string file = _empty,
            [CallerMemberName] string method = _empty,
            [CallerLineNumber] int line = 0) => Tizen.Log.Warn(_tag, message, file, method, line);

        public static void Warn(
            Exception ex,
            string message = _empty,
            [CallerFilePath] string file = _empty,
            [CallerMemberName] string method = _empty,
            [CallerLineNumber] int line = 0) => Tizen.Log.Warn(_tag, $"{message} {ex}", file, method, line);

        public static void Error(
            string message,
            [CallerFilePath] string file = _empty,
            [CallerMemberName] string method = _empty,
            [CallerLineNumber] int line = 0) => Tizen.Log.Error(_tag, message, file, method, line);

        public static void Error(
            Exception ex,
            string message = _empty,
            [CallerFilePath] string file = _empty,
            [CallerMemberName] string method = _empty,
            [CallerLineNumber] int line = 0) => Tizen.Log.Warn(_tag, $"{message} {ex}", file, method, line);

        public static void Fatal(
            string message,
            [CallerFilePath] string file = _empty,
            [CallerMemberName] string method = _empty,
            [CallerLineNumber] int line = 0) => Tizen.Log.Fatal(_tag, message, file, method, line);

        public static void Fatal(
            Exception ex,
            string message = _empty,
            [CallerFilePath] string file = _empty,
            [CallerMemberName] string method = _empty,
            [CallerLineNumber] int line = 0) => Tizen.Log.Warn(_tag, $"{message} {ex}", file, method, line);

        public static void Enter(
            string message = _empty,
            [CallerFilePath] string file = _empty,
            [CallerMemberName] string method = _empty,
            [CallerLineNumber] int line = 0)
            => Tizen.Log.Debug(
                _tag, ReferenceEquals(message, _empty) ? "Enter() ->" : $"Enter( {message} ) ->", file, method, line);

        public static void Exit(
            string message = _empty,
            [CallerFilePath] string file = _empty,
            [CallerMemberName] string method = _empty,
            [CallerLineNumber] int line = 0)
            => Tizen.Log.Debug(
                _tag, ReferenceEquals(message, _empty) ? "Exit() <-" : $"Exit( {message} ) <-", file, method, line);
    }
}
