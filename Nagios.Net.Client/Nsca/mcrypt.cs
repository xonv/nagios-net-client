// Copyright (c) 2012, XBRL Cloud Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer. Redistributions in
// binary form must reproduce the above copyright notice, this list of
// conditions and the following disclaimer in the documentation and/or
// other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
// IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nagios.Net.Client.Nsca
{

    public partial class NativeConstants
    {

        /// MCRYPT_API_VERSION -> 20021217
        public const int MCRYPT_API_VERSION = 20021217;

        /// LIBMCRYPT_VERSION -> "2.5.5rc1"
        public const string LIBMCRYPT_VERSION = "2.5.5rc1";

        /// MCRYPT_FAILED -> 0x0
        public const int MCRYPT_FAILED = 0;

        /// MCRYPT_BLOWFISH -> "blowfish"
        public const string MCRYPT_BLOWFISH = "blowfish";

        /// MCRYPT_DES -> "des"
        public const string MCRYPT_DES = "des";

        /// MCRYPT_3DES -> "tripledes"
        public const string MCRYPT_3DES = "tripledes";

        /// MCRYPT_3WAY -> "threeway"
        public const string MCRYPT_3WAY = "threeway";

        /// MCRYPT_GOST -> "gost"
        public const string MCRYPT_GOST = "gost";

        /// MCRYPT_SAFER_SK64 -> "safer-sk64"
        public const string MCRYPT_SAFER_SK64 = "safer-sk64";

        /// MCRYPT_SAFER_SK128 -> "safer-sk128"
        public const string MCRYPT_SAFER_SK128 = "safer-sk128";

        /// MCRYPT_CAST_128 -> "cast-128"
        public const string MCRYPT_CAST_128 = "cast-128";

        /// MCRYPT_XTEA -> "xtea"
        public const string MCRYPT_XTEA = "xtea";

        /// MCRYPT_RC2 -> "rc2"
        public const string MCRYPT_RC2 = "rc2";

        /// MCRYPT_TWOFISH -> "twofish"
        public const string MCRYPT_TWOFISH = "twofish";

        /// MCRYPT_CAST_256 -> "cast-256"
        public const string MCRYPT_CAST_256 = "cast-256";

        /// MCRYPT_SAFERPLUS -> "saferplus"
        public const string MCRYPT_SAFERPLUS = "saferplus";

        /// MCRYPT_LOKI97 -> "loki97"
        public const string MCRYPT_LOKI97 = "loki97";

        /// MCRYPT_SERPENT -> "serpent"
        public const string MCRYPT_SERPENT = "serpent";

        /// MCRYPT_RIJNDAEL_128 -> "rijndael-128"
        public const string MCRYPT_RIJNDAEL_128 = "rijndael-128";

        /// MCRYPT_RIJNDAEL_192 -> "rijndael-192"
        public const string MCRYPT_RIJNDAEL_192 = "rijndael-192";

        /// MCRYPT_RIJNDAEL_256 -> "rijndael-256"
        public const string MCRYPT_RIJNDAEL_256 = "rijndael-256";

        /// MCRYPT_ENIGMA -> "enigma"
        public const string MCRYPT_ENIGMA = "enigma";

        /// MCRYPT_ARCFOUR -> "arcfour"
        public const string MCRYPT_ARCFOUR = "arcfour";

        /// MCRYPT_WAKE -> "wake"
        public const string MCRYPT_WAKE = "wake";

        /// MCRYPT_CBC -> "cbc"
        public const string MCRYPT_CBC = "cbc";

        /// MCRYPT_ECB -> "ecb"
        public const string MCRYPT_ECB = "ecb";

        /// MCRYPT_CFB -> "cfb"
        public const string MCRYPT_CFB = "cfb";

        /// MCRYPT_OFB -> "ofb"
        public const string MCRYPT_OFB = "ofb";

        /// MCRYPT_nOFB -> "nofb"
        public const string MCRYPT_nOFB = "nofb";

        /// MCRYPT_STREAM -> "stream"
        public const string MCRYPT_STREAM = "stream";
    }

    public partial class NativeMethods
    {

        /// Return Type: MCRYPT->CRYPT_STREAM*
        ///algorithm: char*
        ///a_directory: char*
        ///mode: char*
        ///m_directory: char*
        [System.Runtime.InteropServices.DllImportAttribute("libmcrypt.dll", EntryPoint = "mcrypt_module_open", CharSet=System.Runtime.InteropServices.CharSet.Ansi, CallingConvention=System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern System.IntPtr mcrypt_module_open(System.IntPtr algorithm, System.IntPtr a_directory, System.IntPtr mode, System.IntPtr m_directory);


        /// Return Type: int
        ///td: MCRYPT->CRYPT_STREAM*
        [System.Runtime.InteropServices.DllImportAttribute("libmcrypt.dll", EntryPoint = "mcrypt_module_close", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int mcrypt_module_close(System.IntPtr td);


        /// Return Type: int
        ///td: MCRYPT->CRYPT_STREAM*
        ///key: void*
        ///lenofkey: int
        ///IV: void*
        [System.Runtime.InteropServices.DllImportAttribute("libmcrypt.dll", EntryPoint = "mcrypt_generic_init", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int mcrypt_generic_init(System.IntPtr td, System.IntPtr key, int lenofkey, System.IntPtr IV);


        /// Return Type: int
        ///td: MCRYPT->CRYPT_STREAM*
        [System.Runtime.InteropServices.DllImportAttribute("libmcrypt.dll", EntryPoint = "mcrypt_generic_deinit", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int mcrypt_generic_deinit(System.IntPtr td);


        /// Return Type: int
        ///td: MCRYPT->CRYPT_STREAM*
        [System.Runtime.InteropServices.DllImportAttribute("libmcrypt.dll", EntryPoint = "mcrypt_generic_end", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int mcrypt_generic_end(System.IntPtr td);


        /// Return Type: int
        ///td: MCRYPT->CRYPT_STREAM*
        ///plaintext: void*
        ///len: int
        [System.Runtime.InteropServices.DllImportAttribute("libmcrypt.dll", EntryPoint = "mdecrypt_generic", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int mdecrypt_generic(System.IntPtr td, System.IntPtr plaintext, int len);


        /// Return Type: int
        ///td: MCRYPT->CRYPT_STREAM*
        ///plaintext: void*
        ///len: int
        [System.Runtime.InteropServices.DllImportAttribute("libmcrypt.dll", EntryPoint = "mcrypt_generic", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int mcrypt_generic(System.IntPtr td, System.IntPtr plaintext, int len);


    }


}
