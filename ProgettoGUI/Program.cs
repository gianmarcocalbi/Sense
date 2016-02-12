using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ProgettoGUI {
	static class Program {
		/// <summary>
		/// Punto di ingresso principale dell'applicazione.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var form = new Form1();
			Application.Run(form);
		}
	}

	class Parser {

		//private List<List<double>>[] sampwin;
		public List<double[,]> sampwin;
		TcpListener server;
		IPAddress localAddr;
		int port;
		printToConsole printToServerConsole;
		setButtonServerStartDel setButtonServerStart;
		public bool serverIsActive;
		
		public TcpListener Server {
			get {
				return server;
			}

			set {
				server = value;
			}
		}

		public IPAddress LocalAddr {
			get {
				return localAddr;
			}

			set {
				localAddr = value;
			}
		}

		public int Port {
			get {
				return port;
			}

			set {
				port = value;
			}
		}
		

		//public Parser() {
		//	StartServer();
		//}

		//~Parser() {

		//}

		public delegate void printToConsole(string s);
		public delegate void setButtonServerStartDel(bool b);

		public Parser(int p, string ip, printToConsole del, setButtonServerStartDel fun) {
			port = p;
			try {
				localAddr = IPAddress.Parse(ip);
				server = new TcpListener(localAddr, port);
			} catch (Exception ex) {
				MessageBox.Show("Errore IP Addressing 00!\n" + ex.Message);
			}
			printToServerConsole = del;
			setButtonServerStart = fun;
			serverIsActive = false;
			setButtonServerStart(serverIsActive);
			//StartServer();
		}

		/*public delegate void ActivateServerDelegate();
		

		public void ActivateServerProtected() {
			if (this.serverIsActive.InvokeRequired)
				Invoke(new ActivateServerDelegate(ActivateServerProtected), new object[] { });
			else
				serverIsActive = true;
		}*/


		public void ActivateServer(int p, string ip) {
			port = p;
			try {
				localAddr = IPAddress.Parse(ip);
				server = new TcpListener(localAddr, port);
				serverIsActive = true;
			} catch (Exception ex) {
				MessageBox.Show("Errore IP Addressing 00!\n" + ex.Message);
				serverIsActive = false;
			}
			setButtonServerStart(serverIsActive);
		}

		public void DeactivateServer() {
			serverIsActive = false;
			setButtonServerStart(serverIsActive);
			server.Stop();
			//printToServerConsole(server.ToString());
			//printToServerConsole("Server Stopped.");
		}

		public void StartServer() {
			////////////////////////
			////////SOCKET//////////
			////////////////////////
			while (true) {
				if (serverIsActive) {
					try {
						// Start listening for client requests.
						server.Start();
						printToServerConsole(String.Format("Server Started on port {0} at IP {1}\n", port, localAddr));

						// Buffer for reading data
						Byte[] bytes = new Byte[256]; //(!) Dubbia utilità di questa variabile

						// Enter the listening loop.
						while (serverIsActive) {
							printToServerConsole("Waiting for a connection...\n");
							//Console.Write("Waiting for a connection... ");

							// Perform a blocking call to accept requests.
							TcpClient client = server.AcceptTcpClient();
							printToServerConsole("Connected!\n");
							//Console.WriteLine("Connected!");

							// Get a stream object for reading and writing
							NetworkStream stream = client.GetStream();
							//Console.WriteLine("Stream obtained.");

							BinaryReader reader = new BinaryReader(stream);
							//Console.WriteLine("Reading stream.");
							int _c = 0;
							// Loop to receive all the data sent by the client.
							try {
								/////////////////////
								//RECOGNIZE PACKAGE//
								/////////////////////

								//FIRST PACKAGE
								//10 byte per il client ID
								byte[] client_id = reader.ReadBytes(10);
								printToServerConsole(String.Format("CLIENT ID : {0}\n", System.Text.Encoding.UTF8.GetString(client_id)));
								//Console.WriteLine("CLIENT ID : {0}", System.Text.Encoding.UTF8.GetString(client_id));

								//4 byte per la frequenza 
								byte[] frequency = reader.ReadBytes(4);
								printToServerConsole(String.Format("Sending at {0}MHz\n", BitConverter.ToInt32(frequency, 0)));
								//Console.WriteLine("Sending at {0}MHz", BitConverter.ToInt32(frequency, 0));


								//SENSORS PACKAGEs

								byte[] temp = reader.ReadBytes(2);

								//(!)Xbus_Simulator elimina il preambolo 0xFA (folle!!)
								while (temp[0] != 0xFF || temp[1] != 0x32) {
									temp[0] = temp[1];
									temp[1] = reader.ReadBytes(1)[0];
								}
								//Console.WriteLine("Pacchetto {0} Identificato:", ++_c);
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
									printToServerConsole(String.Format("BID : {0}\nMID : {1}\nLEN : {2}\nEXT_LEN_MUL : {3}\nEXT_LEN_ADD : {4}\n", package[0], package[1], package[2], package[3], package[4]));
									//Console.WriteLine("BID : {0}\nMID : {1}\nLEN : {2}\nEXT_LEN_MUL : {3}\nEXT_LEN_ADD : {4}", package[0], package[1], package[2], package[3], package[4]);
								} else {
									data.CopyTo(package, 3);
									printToServerConsole(String.Format("BID : {0}\nMID : {1}\nLEN : {2}\n", package[0], package[1], package[2]));
									//Console.WriteLine("BID : {0}\nMID : {1}\nLEN : {2}", package[0], package[1], package[2]);
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

								while (serverIsActive) {
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
											//Console.Write("{0}; ", field);
											arr[i, j] = field;
											//printToServerConsole(String.Format("{0}; ", field));
										}
										//Console.WriteLine();
									}
									sampwin.Add(arr);

									if (num_sensori < 5) {
										package = reader.ReadBytes(byteToRead + 4);
									} else {
										package = reader.ReadBytes(byteToRead + 6);
									}

									//Console.WriteLine("-----------------------------------------");
									//Console.WriteLine("Lettura Nuovo Pacchetto...ENTER per continuare");
									//Console.Read();
								}
							} catch (IndexOutOfRangeException ex) {
								//Ignore this Exception
								//Quando le stream è esaurito dovrebbe automaticamente generare questa eccezione
								printToServerConsole("Stream finished.\n");
								printToServerConsole("Creating file CSV...\n");
								if (!writeMatrixToCSV(sampwin, @"C:\Users\Gianmarco\Desktop\sampwin.csv")) {
									MessageBox.Show("Errore creazione CSV");
								} else {
									printToServerConsole("File CSV created in {path}.\n");
								}
							} catch (Exception ex) {
								MessageBox.Show("Errore Connessione 00!\n" + ex.Message);
							} finally {
								client.Close();
								printToServerConsole("Client Disconnected.\n");
								//Console.WriteLine("Client Disconnected.\n");
								//Console.WriteLine("-------------------------------------------");
							}
						}
					} catch (SocketException ex) {
						//Ignore this Exception
					} catch (InvalidOperationException ex) {
						//Ignore this Exception
					} catch (Exception ex) {
						MessageBox.Show("Errore Connessione 01!\n" + ex.StackTrace);
					} finally {
						// Stop listening for new clients.
						server.Stop();
						serverIsActive = false;
						setButtonServerStart(serverIsActive);
						printToServerConsole("Server Stopped.\n");
					}
				}
			}
		}

		public static bool writeMatrixToCSV(List<double[,]> matrix, string path) {
			//List<double[num_sensori, 13]
			//campione 1 -> accx;accy;..........qua3;qua4;;accx;accy;..........qua3;qua4;;
			//campione 2 -> accx;accy;..........qua3;qua4;;accx;accy;..........qua3;qua4;;
			//campione 3 -> accx;accy;..........qua3;qua4;;accx;accy;..........qua3;qua4;;
			try {
				string csv = "";
				foreach (double[,] d in matrix) {
					for (int i = 0; i < d.GetLength(0); ++i) {
						for (int j = 0; j < d.GetLength(1); ++j) {
							csv += d[i, j] + ";";
						}
						csv += ";";
					}
					csv += "\n";
				}
				File.WriteAllText(path, csv.ToString());
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
				MessageBox.Show(ex.StackTrace);
				return false;
			}
			return true;
		}
	}
}
