using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UI;

namespace TGMTcs
{
    public class POSTeventArgs : EventArgs
    {
        public MemoryStream POSTdata;
    }

    public class GETeventArgs : EventArgs
    {
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class HttpProcessor : IDisposable
    {
        TcpClient m_socket;
        NetworkStream m_networkStream;

        public string http_method;
        public string http_url;
        public string http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();

        private const int BUF_SIZE = 4096;
        private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB



        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public HttpProcessor(TcpClient client)
        {
            m_socket = client;
        }

        ~HttpProcessor()
        {
            
        }

        public virtual void Dispose()
        {
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Process()
        {
            

            // we probably shouldn't be using a streamwriter for all output from handlers either
            //m_outputStream = new StreamWriter(new BufferedStream(m_socket.GetStream()));
            try
            {
               ReadHeaders();
                if (http_method.Equals("GET"))
                {
                    WriteResult(Form1.GetInstance().GetHtml());

                }
                else if (http_method.Equals("POST"))
                {
                    HandlePOSTRequest();
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void ReadHeaders()
        {
            m_networkStream = m_socket.GetStream();
            string strHeaders ="";
            Byte[] bytes;
            if (m_socket.ReceiveBufferSize > 0 && m_networkStream.DataAvailable)
            {
                bytes = new byte[m_socket.ReceiveBufferSize];
                m_networkStream.Read(bytes, 0, m_socket.ReceiveBufferSize);
                strHeaders = Encoding.ASCII.GetString(bytes); //the message incoming
                
            }

            int newLineIdx = strHeaders.IndexOf("\r\n");
            string line = strHeaders.Substring(0, newLineIdx);
            strHeaders = strHeaders.Substring(newLineIdx + 2);

            string[] tokens = line.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];

            while (strHeaders != "")
            {
                newLineIdx = strHeaders.IndexOf("\r\n");
                line = strHeaders.Substring(0, newLineIdx);
                strHeaders = strHeaders.Substring(newLineIdx + 2);
                if (line.Equals(""))
                {

                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                string name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                //Console.WriteLine("header: {0}:{1}", name, value);
                httpHeaders[name] = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void WriteResult(string result)
        {
            if (m_networkStream.CanWrite)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(result);
                m_networkStream.Write(buffer, 0, buffer.Length);

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void HandlePOSTRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            int content_len = 0;
            MemoryStream receivedData = new MemoryStream();
            if (httpHeaders.ContainsKey("Content-Length") || httpHeaders.ContainsKey("content-length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len == 0)
                    content_len = Convert.ToInt32(this.httpHeaders["content-length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(String.Format("POST Content-Length({0}) too big for this simple server", content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {
                    //int numread = this.m_inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    //if (numread == 0)
                    //{
                    //    if (to_read == 0)
                    //    {
                    //        break;
                    //    }
                    //    else
                    //    {
                    //        throw new Exception("client disconnected during post");
                    //    }
                    //}
                    //to_read -= numread;
                    //receivedData.Write(buf, 0, numread);
                }
                receivedData.Seek(0, SeekOrigin.Begin);
            }

            POSTeventArgs e = new POSTeventArgs();
            e.POSTdata = receivedData;
           // m_server.onPOSTrequest?.Invoke(this, e);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void WriteSuccess(string content_type = "text/html")
        {
            if(m_networkStream.CanWrite)
            {
                byte[] buffer = Encoding.ASCII.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: " + content_type + "\r\nConnection: close\r\n");
                m_networkStream.Write(buffer, 0, buffer.Length);

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void WriteFailure()
        {
            //m_outputStream.WriteLine("HTTP/1.0 404 File not found");
            //m_outputStream.WriteLine("Connection: close");
            //m_outputStream.WriteLine("");
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class TGMThttpServer
    {
        protected int m_port;
        TcpListener m_listener;


        public delegate void OnGETrequestHandler(HttpProcessor sender, GETeventArgs e);
        public OnGETrequestHandler onGETrequest;

        public delegate void OnPOSTrequestHandler(HttpProcessor sender, POSTeventArgs e);
        public OnPOSTrequestHandler onPOSTrequest;

        public TGMThttpServer(int port = 80)
        {
            Console.WriteLine("Server ready");
            m_port = port;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        bool isFirst = true;
        public void Listen()
        {
            m_listener = new TcpListener(m_port);
            m_listener.Start();

            try
            {
                while (true)
                {
                    TcpClient client = m_listener.AcceptTcpClient();


                    var childSocketThread = new Thread(() =>
                    {
                        byte[] data = new byte[100];

                        HttpProcessor processor = new HttpProcessor(client);
                        processor.Process();
                        client.Client.Shutdown(SocketShutdown.Both);
                        client.Close();
                        isFirst = !isFirst;
                    });
                    childSocketThread.Start();

                    
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Stop()
        {
            if (m_listener.Server.IsBound)
            {
                m_listener.Server.Close();

            }

            m_listener.Stop();
        }
    }
}
