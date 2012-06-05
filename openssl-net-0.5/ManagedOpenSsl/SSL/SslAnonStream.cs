using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Security;
using System.IO;
using OpenSSL.Crypto;

namespace OpenSSL.SSL
{
    // anonymous SSL stream (the certificate is not used)
    /// <summary>
    /// Create ssl anonymous stream
    /// </summary>
    public class SslAnonStream : AuthenticatedStream
	{
		#region Initialization

		/// <summary>
		/// Create an SslStream based on an existing stream.
		/// </summary>
		/// <param name="stream"></param>
		public SslAnonStream(Stream stream)
			: this(stream, false)
		{
		}

		/// <summary>
		/// Create an SslStream based on an existing stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="leaveInnerStreamOpen"></param>
		public SslAnonStream(Stream stream, bool leaveInnerStreamOpen)
			: base(stream, leaveInnerStreamOpen)
		{
		}

		#endregion

		#region AuthenticatedStream Members
		/// <summary>
		/// Returns whether authentication was successful.
		/// </summary>
		public override bool IsAuthenticated
		{
			get { return sslStream != null; }
		}

		/// <summary>
		/// Indicates whether data sent using this SslStream is encrypted.
		/// </summary>
		public override bool IsEncrypted
		{
			get { return IsAuthenticated; }
		}

		/// <summary>
		/// Indicates whether both server and client have been authenticated.
		/// </summary>
		public override bool IsMutuallyAuthenticated
		{
			get
			{
				if (IsAuthenticated && (IsServer ? sslStream.RemoteCertificate != null : sslStream.LocalCertificate != null))
				{
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Indicates whether the local side of the connection was authenticated as the server.
		/// </summary>
		public override bool IsServer
		{
			get { return sslStream is SslStreamServer; }
		}

		/// <summary>
		/// Indicates whether the data sent using this stream is signed.
		/// </summary>
		public override bool IsSigned
		{
			get { return IsAuthenticated; }
		}

		#endregion

		#region Stream Members
		/// <summary>
		/// Gets a value indicating whether the current stream supports reading.
		/// </summary>
		public override bool CanRead
		{
			get { return InnerStream.CanRead; }
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		public override bool CanSeek
		{
			get { return InnerStream.CanSeek; }
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports writing.
		/// </summary>
		public override bool CanWrite
		{
			get { return InnerStream.CanWrite; }
		}

		/// <summary>
		/// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
		/// </summary>
		public override void Flush()
		{
			InnerStream.Flush();
		}

		/// <summary>
		/// Gets the length in bytes of the stream.
		/// </summary>
		public override long Length
		{
			get { return InnerStream.Length; }
		}

		/// <summary>
		/// Gets or sets the position within the current stream.
		/// </summary>
		public override long Position
		{
			get { return InnerStream.Position; }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Gets or sets a value, in miliseconds, that determines how long the stream will attempt to read before timing out.
		/// </summary>
		public override int ReadTimeout
		{
			get { return InnerStream.ReadTimeout; }
			set { InnerStream.ReadTimeout = value; }
		}

		/// <summary>
		/// Gets or sets a value, in miliseconds, that determines how long the stream will attempt to write before timing out.
		/// </summary>
		public override int WriteTimeout
		{
			get { return InnerStream.WriteTimeout; }
			set { InnerStream.WriteTimeout = value; }
		}

		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			return EndRead(BeginRead(buffer, offset, count, null, null));
		}

		/// <summary>
		/// Begins an asynchronous read operation.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <param name="asyncCallback"></param>
		/// <param name="asyncState"></param>
		/// <returns></returns>
		public override IAsyncResult BeginRead(
			byte[] buffer,
			int offset,
			int count,
			AsyncCallback asyncCallback,
			Object asyncState)
		{
			TestConnectionIsValid();

			return sslStream.BeginRead(buffer, offset, count, asyncCallback, asyncState);
		}

		/// <summary>
		/// Waits for the pending asynchronous read to complete.
				/// </summary>
		/// <param name="asyncResult"></param>
		/// <returns></returns>
		public override int EndRead(IAsyncResult asyncResult)
		{
			TestConnectionIsValid();

			return sslStream.EndRead(asyncResult);
		}

		/// <summary>
		/// Not supported
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="origin"></param>
		/// <returns></returns>
		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Sets the length of the current stream.
		/// </summary>
		/// <param name="value"></param>
		public override void SetLength(long value)
		{
			InnerStream.SetLength(value);
		}

		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			TestConnectionIsValid();

			EndWrite(BeginWrite(buffer, offset, count, null, null));
		}

		/// <summary>
		/// Begins an asynchronous write operation.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <param name="asyncCallback"></param>
		/// <param name="asyncState"></param>
		/// <returns></returns>
		public override IAsyncResult BeginWrite(
			byte[] buffer,
			int offset,
			int count,
			AsyncCallback asyncCallback,
			Object asyncState)
		{
			TestConnectionIsValid();

			return sslStream.BeginWrite(buffer, offset, count, asyncCallback, asyncState);
		}

		/// <summary>
		/// Ends an asynchronous write operation.
		/// </summary>
		/// <param name="asyncResult"></param>
		public override void EndWrite(IAsyncResult asyncResult)
		{
			TestConnectionIsValid();

			sslStream.EndWrite(asyncResult);
		}

		/// <summary>
		/// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream.		
		/// </summary>
		public override void Close()
		{
			TestConnectionIsValid();

			base.Close();
			sslStream.Close();
		}
		#endregion

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public CipherAlgorithmType CipherAlgorithm
		{
			get
			{
				if (!IsAuthenticated)
					return CipherAlgorithmType.None;
				return sslStream.CipherAlgorithm;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int CipherStrength
		{
			get
			{
				if (!IsAuthenticated)
					return 0;
				return sslStream.CipherStrength;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public HashAlgorithmType HashAlgorithm
		{
			get
			{
				if (!IsAuthenticated)
					return HashAlgorithmType.None;
				return sslStream.HashAlgorithm;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int HashStrength
		{
			get
			{
				if (!IsAuthenticated)
					return 0;
				return sslStream.HashStrength;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ExchangeAlgorithmType KeyExchangeAlgorithm
		{
			get
			{
				if (!IsAuthenticated)
					return ExchangeAlgorithmType.None;
				return sslStream.KeyExchangeAlgorithm;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int KeyExchangeStrength
		{
			get
			{
				if (!IsAuthenticated)
					return 0;
				return sslStream.KeyExchangeStrength;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public SslProtocols SslProtocol
		{
			get
			{
				if (!IsAuthenticated)
					return SslProtocols.None;
				return sslStream.SslProtocol;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public List<string> CipherList
		{
			get { return sslStream.CipherList; }
		}

		#endregion //Properties

		#region Methods
		/// <summary>
		///
		/// </summary>
        /// <param name="dh"></param>
		/// <param name="enabledSslProtocols"></param>
		/// <param name="sslStrength"></param>
        public virtual void AuthenticateAsServer(
            DH dh,
			SslProtocols enabledSslProtocols,
			SslStrength sslStrength)
		{
			EndAuthenticateAsServer(BeginAuthenticateAsServer(dh, enabledSslProtocols, sslStrength, null, null));
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="dh"></param>
		/// <param name="asyncCallback"></param>
		/// <param name="asyncState"></param>
		/// <returns></returns>
		public virtual IAsyncResult BeginAuthenticateAsServer(
            DH dh,
			AsyncCallback asyncCallback,
			Object asyncState)
		{
			return BeginAuthenticateAsServer(dh, SslProtocols.Default, SslStrength.Medium, asyncCallback, asyncState);
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="dh"></param>
		/// <param name="enabledSslProtocols"></param>
		/// <param name="sslStrength"></param>
		/// <param name="asyncCallback"></param>
		/// <param name="asyncState"></param>
		/// <returns></returns>
		public virtual IAsyncResult BeginAuthenticateAsServer(
            DH dh,
			SslProtocols enabledSslProtocols,
			SslStrength sslStrength,
			AsyncCallback asyncCallback,
			Object asyncState)
		{
			if (IsAuthenticated)
			{
				throw new InvalidOperationException("SslStream is already authenticated");
			}
			// Initialize the server stream
            SslAnonStreamServer server_stream = new SslAnonStreamServer(InnerStream, false, dh, enabledSslProtocols, sslStrength);
			// Set the internal sslStream
			sslStream = server_stream;
			// Start the read operation
			return BeginRead(new byte[0], 0, 0, asyncCallback, asyncState);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ar"></param>
		public virtual void EndAuthenticateAsServer(IAsyncResult ar)
		{
			TestConnectionIsValid();

			// Finish the async AuthenticateAsServer call - EndRead/Write call will throw exception on error
			EndRead(ar);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Renegotiate()
		{
			TestConnectionIsValid();

			EndRenegotiate(BeginRenegotiate(null, null));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public IAsyncResult BeginRenegotiate(AsyncCallback callback, object state)
		{
			TestConnectionIsValid();

			sslStream.Renegotiate();

			if (sslStream is SslStreamClient)
			{
				return BeginWrite(new byte[0], 0, 0, callback, state);
			}
			else
			{
				return BeginRead(new byte[0], 0, 0, callback, state);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="asyncResult"></param>
		public void EndRenegotiate(IAsyncResult asyncResult)
		{
			TestConnectionIsValid();

			if (sslStream is SslStreamClient)
			{
				EndWrite(asyncResult);
			}
			else
			{
				EndRead(asyncResult);
			}
		}

		#endregion

		#region Helpers
		private void TestConnectionIsValid()
		{
			if (sslStream == null)
			{
				throw new InvalidOperationException("SslStream has not been authenticated");
			}
		}
		#endregion

		#region Fields
		SslStreamBase sslStream;
		#endregion
	}
}
