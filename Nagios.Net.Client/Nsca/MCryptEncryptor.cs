using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nagios.Net.Client.Nsca
{

    public class MCryptEncryptor : IDisposable
    {
        IntPtr td;
        byte[] algName;
        byte[] mode;

        public MCryptEncryptor(string algorithm, byte[] key, byte[] iv)
        {
            unsafe
            {
                algName = System.Text.ASCIIEncoding.ASCII.GetBytes(algorithm);
                mode = System.Text.ASCIIEncoding.ASCII.GetBytes(NativeConstants.MCRYPT_CFB);
                fixed (byte* alg = algName)
                {
                    fixed (byte* m = mode)
                    {
                        td = NativeMethods.mcrypt_module_open(new IntPtr(alg), IntPtr.Zero, new IntPtr(m), IntPtr.Zero);
                    }
                }
                fixed (byte* k = key)
                {
                    fixed (byte* i = iv)
                    {
                        NativeMethods.mcrypt_generic_init(td, new IntPtr(k), key.Length, new IntPtr(i));
                    }
                }
            }
        }

        public byte[] Encrypt(byte[] buf)
        {
            byte[] outBuf = new byte[buf.Length];
            Array.Copy(buf, outBuf, buf.Length);

            for (int x = 0; x < buf.Length; x++)
                unsafe
                {
                    fixed (byte* p = outBuf)
                    {
                        NativeMethods.mcrypt_generic(td, new IntPtr(p + x), 1);
                    }
                }
            return outBuf;
        }

        public void Dispose()
        {
            unsafe
            {
                NativeMethods.mcrypt_generic_end(td);
            }
        }
    }
}
