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

namespace Demuxer.Common
{
    public enum DrmInitDataType
    {
        Cenc,
        KeyIds,
        WebM,
    }

    public class DrmInitData
    {
        public DrmInitDataType DataType;
        public byte[] Data = null;

        public override int GetHashCode()
        {
            var hash = 2058005167 ^ Data.Length;

            var len = Data.Length;
            hash ^= len >= 4
                ? (Data[0] << 12) | (Data[1] << 8) | (Data[2] << 4) | Data[3]
                : hash;

            hash ^= len >= 8
                ? (Data[len - 4] << 12) | (Data[len - 3] << 8) | (Data[len - 2] << 4) | Data[len - 1]
                : hash;
            return hash.GetHashCode();
        }
    }
}