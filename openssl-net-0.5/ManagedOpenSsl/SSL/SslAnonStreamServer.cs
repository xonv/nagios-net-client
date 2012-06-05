using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenSSL.Core;
using OpenSSL.Crypto;

namespace OpenSSL.SSL
{
    class SslAnonStreamServer : SslStreamBase
    {
        public SslAnonStreamServer(
            Stream stream, 
            bool ownStream,
            DH dh,
            SslProtocols enabledSslProtocols,
            SslStrength sslStrength)
            : base(stream, ownStream)
        {
            // Initialize the SslContext object
            InitializeServerContext(dh, enabledSslProtocols, sslStrength);
            
            // Initalize the Ssl object
            ssl = new Ssl(sslContext);
            // Initialze the read/write bio
            read_bio = BIO.MemoryBuffer(false);
            write_bio = BIO.MemoryBuffer(false);
            // Set the read/write bio's into the the Ssl object
            ssl.SetBIO(read_bio, write_bio);
            read_bio.SetClose(BIO.CloseOption.Close);
            write_bio.SetClose(BIO.CloseOption.Close);
            // Set the Ssl object into server mode
            ssl.SetAcceptState();
        }

        internal protected override bool ProcessHandshake()
        {
            bool bRet = false;
            int nRet = 0;
            
            if (handShakeState == HandshakeState.InProcess)
            {
                nRet = ssl.Accept();
            }
            else if (handShakeState == HandshakeState.RenegotiateInProcess)
            {
                nRet = ssl.DoHandshake();
            }
            else if (handShakeState == HandshakeState.Renegotiate)
            {
                nRet = ssl.DoHandshake();
                ssl.State = Ssl.SSL_ST_ACCEPT;
                handShakeState = HandshakeState.RenegotiateInProcess;
            }
            SslError lastError = ssl.GetError(nRet);
            if (lastError == SslError.SSL_ERROR_WANT_READ || lastError == SslError.SSL_ERROR_WANT_WRITE || lastError == SslError.SSL_ERROR_NONE)
            {
                if (nRet == 1) // success
                {
                    bRet = true;
                }
            }
            else
            {
                // Check to see if we have alert data in the write_bio that needs to be sent
                if (write_bio.BytesPending > 0)
                {
                    // We encountered an error, but need to send the alert
                    // set the handshakeException so that it will be processed
                    // and thrown after the alert is sent
                    handshakeException = new OpenSslException();
                }
                else
                {
                    // No alert to send, throw the exception
                    throw new OpenSslException();
                }
            }
            return bRet;
        }

        private void InitializeServerContext(
            DH dh,
            SslProtocols enabledSslProtocols,
            SslStrength sslStrength)
        {
            // Initialize the context
            sslContext = new SslContext(SslMethod.SSLv23_server_method);

            // Remove support for protocols not specified in the enabledSslProtocols
            if ((enabledSslProtocols & SslProtocols.Ssl2) != SslProtocols.Ssl2)
            {
                sslContext.Options |= SslOptions.SSL_OP_NO_SSLv2;
            }
            if ((enabledSslProtocols & SslProtocols.Ssl3) != SslProtocols.Ssl3 &&
                ((enabledSslProtocols & SslProtocols.Default) != SslProtocols.Default))
            {
                // no SSLv3 support
                sslContext.Options |= SslOptions.SSL_OP_NO_SSLv3;
            }
            if ((enabledSslProtocols & SslProtocols.Tls) != SslProtocols.Tls &&
                (enabledSslProtocols & SslProtocols.Default) != SslProtocols.Default)
            {
                sslContext.Options |= SslOptions.SSL_OP_NO_TLSv1;
            }

            // Set the context mode
            sslContext.Mode = SslMode.SSL_MODE_AUTO_RETRY;

            sslContext.SetVerify(VerifyMode.SSL_VERIFY_NONE, null);
                       
            // Set the cipher string
            sslContext.SetCipherList("ADH");

            sslContext.SetTmpDhCallback(dh);
            
        }
    }
}
