using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Sense {
	public class Program {
		public static void Main(string[] args) {
			//Parser jack = new Parser();

			/*ThreadStart gimmy4allDelegate = new ThreadStart(jack.gimmy); //rappresentazione metodo con delegato
			Thread gimmy4allThread = new Thread(gimmy4allDelegate); //passo il delegato al costruttore thread
			gimmy4allThread.Start(); //esecuzione thread avviata*/
		}
	}

	class Parser {

		//private List<List<double>>[] sampwin;
		public List<double[,]> sampwin;

		public Parser() {
			Server();
		}

		public void Server() {
			////////////////////////
			////////SOCKET//////////
			////////////////////////

			TcpListener server = null;
			try {
				// Set the TcpListener on port 45555.
				int port = 45555;
				IPAddress localAddr = IPAddress.Parse("127.0.0.1");

				server = new TcpListener(localAddr, port);

				// Start listening for client requests.
				server.Start();

				// Buffer for reading data
				Byte[] bytes = new Byte[256]; //(!) Dubbia utilità di questa variabile

				// Enter the listening loop.
				while (true) {
					Console.Write("Waiting for a connection... ");

					// Perform a blocking call to accept requests.
					TcpClient client = server.AcceptTcpClient();
					Console.WriteLine("Connected!");

					// Get a stream object for reading and writing
					NetworkStream stream = client.GetStream();
					Console.WriteLine("Stream obtained.");

					BinaryReader reader = new BinaryReader(stream);
					Console.WriteLine("Reading stream.");
					int _c = 0;
					// Loop to receive all the data sent by the client.
					try {
						/////////////////////
						//RECOGNIZE PACKAGE//
						/////////////////////

						//FIRST PACKAGE
						//10 byte per il client ID
						byte[] client_id = reader.ReadBytes(10);
						Console.WriteLine("CLIENT ID : {0}", System.Text.Encoding.UTF8.GetString(client_id));

						//4 byte per la frequenza 
						byte[] frequency = reader.ReadBytes(4);
						Console.WriteLine("Sending at {0}MHz", BitConverter.ToInt32(frequency, 0));


						//SENSORS PACKAGEs

						byte[] temp = reader.ReadBytes(2);

						//(!)Xbus_Simulator elimina il preambolo 0xFA (folle!!)
						while (temp[0] != 0xFF || temp[1] != 0x32) {
							temp[0] = temp[1];
							temp[1] = reader.ReadBytes(1)[0];
						}
						Console.WriteLine("Pacchetto {0} Identificato:", ++_c);
						byte bid = temp[0];
						byte mid = temp[1]; //0x32
						byte len = reader.ReadBytes(1)[0];
						int num_sensori = 0;
						byte ext_len_mul = 0;
						byte ext_len_add = 0;
						int byteToRead = 0;
						byte[] package;
						int beg = 0;            //usata per indicare a che indice di package comincia la sezione data

						if (len == 0xFF) {
							//ext length
							ext_len_mul = reader.ReadBytes(1)[0];
							ext_len_add = reader.ReadBytes(1)[0];
							byteToRead = (ext_len_mul * 256) + ext_len_add;
						} else {
							//normal length
							byteToRead = len;
						}

						//byteToRead = lunghezza campo data contatori inclusi

						byte[] data = new byte[byteToRead + 1];     //aggiungo checksum
						data = reader.ReadBytes(byteToRead + 1);    //lettura campo data

						if (len == 0xFF) {
							beg = 7;
							package = new byte[byteToRead + 6];
						} else {
							beg = 5;
							package = new byte[byteToRead + 4];
						}

						num_sensori = (byteToRead - 2) / 52;

						//Build package
						package[0] = 0xFF;
						package[1] = 0x32;
						package[2] = len;

						if (len == 0xFF) {
							package[3] = ext_len_mul;
							package[4] = ext_len_add;
							data.CopyTo(package, 5);
							Console.WriteLine("BID : {0}\nMID : {1}\nLEN : {2}\nEXT_LEN_MUL : {3}\nEXT_LEN_ADD : {4}", package[0], package[1], package[2], package[3], package[4]);
						} else {
							data.CopyTo(package, 3);
							Console.WriteLine("BID : {0}\nMID : {1}\nLEN : {2}", package[0], package[1], package[2]);
						}

						

						/*
						Stato Array Package[]
						package[0] : bid
						package[1] : mid
						package[2] : len
						package[3] : eventuale ext_len_mul
						package[4] : eventuale ext_len_add
						package[fino a n-1] : data {
							package[5-6] : contatore
							package[7..n-1] : sensori
						}
						package[n] : checksum

						LOGICA PACCHETTO:
						Per ogni sensore (num_sensori) ho un pacchetto di 52byte distribuiti nel modo seguente:
						a11,a12,a13,a14 - a21.a22.a23.a24 - a31.a32.a33.a34 - g11[...]
						ovvero
						acc1 - acc2 - acc3 - gyr1 [...]
						Per ogni acc1 etc.. dobbiamo cavarci il double
						*/

						//una volta chiaro il numero di sensori possiamo istanziare sampwin

						sampwin = new List<double[,]>();

						while (true) {
							////////////////////
							//salvataggio dati//
							////////////////////
							double[,] arr = new double[num_sensori, 13];
							for (int i = 0; i < num_sensori; ++i) {
								double field;
								for (int j = 0; j < 13; ++j) {
									field = BitConverter.ToSingle(new byte[] {
										package[i * 52 + j * 4 + beg + 3],
										package[i * 52 + j * 4 + beg + 2],
										package[i * 52 + j * 4 + beg + 1],
										package[i * 52 + j * 4 + beg]
									}, 0);
									Console.Write("{0}; ", field);
									arr[i, j] = field;
								}
								Console.WriteLine();
							}
							sampwin.Add(arr);

							if (num_sensori < 5) {
								package = reader.ReadBytes(byteToRead + 4);
							} else {
								package = reader.ReadBytes(byteToRead + 6);
							}

							Console.WriteLine("-----------------------------------------");
							Console.WriteLine("Lettura Nuovo Pacchetto...ENTER per continuare");
							//Console.Read();

						}
					} catch (IndexOutOfRangeException e) {
						//Console.WriteLine("client.Connected = {0}", client.Connected);
						//Console.WriteLine(e);
						//Quando le stream è esaurito dovrebbe automaticamente generare questa eccezione
					} finally {
						client.Close();
						Console.WriteLine("Client Disconnected.\n");
						Console.WriteLine("-------------------------------------------");
					}
				}
			} catch (SocketException e) {
				Console.WriteLine("SocketException: {0}", e);
			} finally {
				// Stop listening for new clients.
				server.Stop();
			}
		}
	}
}
