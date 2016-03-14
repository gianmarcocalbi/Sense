using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Sense {
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
		public List<double[,]> sampwin;
		int port;
		TcpListener server;
		IPAddress localAddr;
		printToConsoleDel printToServerConsole;
		setButtonServerStartDel setButtonServerStart;
		eatSampwinProtectedDel eatSampwinProtected;
		public bool serverIsActive;
		string path;
		
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

		///Delegate printToConsoleDel Declarations
		public delegate void printToConsoleDel(string s);

		///Delegate setButtonServerStartDel Declarations
		public delegate void setButtonServerStartDel(bool b);

		///Delegate eatSampwinProtectedDel Declarations
		public delegate void eatSampwinProtectedDel(List<double[,]> matrix);

		///<summary>
		///Costruttore Parser.
		///</summary>
		///<param name="p">Porta sul quale aprire la Socket.</param>
		///<param name="ip">Indirizzo ip server.</param>
		///<param name="csvPath">Path dove salvare il file csv.</param>
		///<param name="printToConsoleFunc">Funzione delegata per scrivere sulla console.</param>
		///<param name="setButtonServerStartFunc">Funzione delegata per cambiare il testo di un tasto.</param>
		///<param name="eatSampFunc">Funzione delegata per triggerare il parsing della sampwin.</param>
		public Parser(int p, string ip, string csvPath, printToConsoleDel printToConsoleFunc, setButtonServerStartDel setButtonServerStartFunc, eatSampwinProtectedDel eatSampFunc) {
			path = csvPath;
			port = p;
			try {
				localAddr = IPAddress.Parse(ip);
				server = new TcpListener(localAddr, port);
			} catch (Exception ex) {
				MessageBox.Show("Errore IP Addressing 00!\n" + ex.Message);
			}
			//Salviamo in locale le funzioni passate al Parser
			printToServerConsole = printToConsoleFunc;
			setButtonServerStart = setButtonServerStartFunc;
			eatSampwinProtected = eatSampFunc;
			serverIsActive = false;
			setButtonServerStart(serverIsActive); //< Setta il testo sul tasto "serverStart" a START
		}

		///<summary>Attiva il Server aggiornando i parametri in base all'input del'utente.</summary>
		///<param name="p">Porta sul quale aprire la Socket.</param>
		///<param name="ip">Indirizzo ip server.</param>
		///<param name="csvPath">Path dove salvare il file csv.</param>
		public void ActivateServer(int p, string ip, string csvPath) {
			port = p;
			path = csvPath;
			//Proviamo a parsare le informazioni per istanziare il server
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

		///<summary>Disattiva il Server (server.Stop()).</summary>
		public void DeactivateServer() {
			serverIsActive = false;
			setButtonServerStart(serverIsActive);
			server.Stop();
			//printToServerConsole(server.ToString());
			//printToServerConsole("Server Stopped.");
		}

		/// <summary>
		/// Metodo che gestisce il funzionamento del Server. Da usare in un thread.
		/// </summary>
		public void StartServer() {
			///Implementazione SOCKET
			while (true) {
				if (serverIsActive) {
					try {
						///Server Start: comincia ad ascoltare sulla socket.
						server.Start();
						printToServerConsole(String.Format("Server Started on port {0} at IP {1}\n", port, localAddr));

						///Finchè il server è attivo inizia il loop:
						///1) aspetta una connessione
						///2) accetta connessione
						///3) risolve la richiesta
						///4) disconnessione e ritorno al punto 1)
						while (serverIsActive) {
							printToServerConsole("Waiting for a connection...\n");

							///Lancia una chiamata bloccante (blocca il thread) aspettando la connessione di un Client.
							TcpClient client = server.AcceptTcpClient();
							///Se supera la chiamata vuol dire che è avvenuta la connessione col Client.
							printToServerConsole("Connected!\n");

							///Ottiene lo stream dal Client per leggere i dati inviati dallo stesso.
							NetworkStream stream = client.GetStream();
							//Console.WriteLine("Stream obtained.");

							BinaryReader reader = new BinaryReader(stream);
							//Console.WriteLine("Reading stream.");

							///Comincia la lettura del pacchetto.
							try {

								///Lettura primo pacchetto "da scartare" perché sono solo dati di connessione.
								//10 byte per il client ID
								byte[] client_id = reader.ReadBytes(10);
								printToServerConsole(String.Format("CLIENT ID : {0}\n", System.Text.Encoding.UTF8.GetString(client_id)));
								//Console.WriteLine("CLIENT ID : {0}", System.Text.Encoding.UTF8.GetString(client_id));

								//4 byte per la frequenza 
								byte[] frequency = reader.ReadBytes(4);
								printToServerConsole(String.Format("Sending at {0}MHz\n", BitConverter.ToInt32(frequency, 0)));
								//Console.WriteLine("Sending at {0}MHz", BitConverter.ToInt32(frequency, 0));


								///Lettura del campo DATA e dei suoi parametri ovvero i dati che arrivano dai Sensori (dati clue dello streaming).
								byte[] temp = reader.ReadBytes(2);

								//(!)Xbus_Simulator elimina il preambolo 0xFA (mi fido di questo commento fatto un mese fa, oggi è il 14/03/16)
								///Cerco l'inizio del pacchetto ovvero FF32.
								while (temp[0] != 0xFF || temp[1] != 0x32) {
									temp[0] = temp[1];
									temp[1] = reader.ReadBytes(1)[0];
								}

								byte bid = temp[0]; //0xFF
								byte mid = temp[1]; //0x32
								byte len = reader.ReadBytes(1)[0];
								int num_sensori = 0;
								byte ext_len_mul = 0;
								byte ext_len_add = 0;
								int byteToRead = 0;
								byte[] package;
								int beg = 0; //usata per indicare a che indice di package comincia la sezione data

								///Cerco di capire la lunghezza del campo DATA.
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

								///Costruisco l'array di byte[] package che consiste effettivamente nel pacchetto inviato dal Client.
								package[0] = 0xFF;
								package[1] = 0x32;
								package[2] = len;

								printToServerConsole(String.Format("Package Structure:\n"));

								if (len == 0xFF) {
									package[3] = ext_len_mul;
									package[4] = ext_len_add;
									data.CopyTo(package, 5);
									printToServerConsole(String.Format("- BID : {0}\n- MID : {1}\n- LEN : {2} bytes\n- EXT_LEN_MUL : {3}\n- EXT_LEN_ADD : {4} bytes\n", package[0], package[1], package[2], package[3], package[4]));
									//Console.WriteLine("BID : {0}\nMID : {1}\nLEN : {2}\nEXT_LEN_MUL : {3}\nEXT_LEN_ADD : {4}", package[0], package[1], package[2], package[3], package[4]);
								} else {
									data.CopyTo(package, 3);
									printToServerConsole(String.Format("- BID : {0}\n- MID : {1}\n- LEN : {2} bytes\n", package[0], package[1], package[2]));
									//Console.WriteLine("BID : {0}\nMID : {1}\nLEN : {2}", package[0], package[1], package[2]);
								}
								
								///Stato Array Package[]
								///package[0] : bid
								///package[1] : mid
								///package[2] : len
								///package[3] : eventuale ext_len_mul
								///package[4] : eventuale ext_len_add
								///package[fino a n-1] : data {
								///		package[5-6] : contatore
								///		package[7..n-1] : sensori
								///}
								///package[n] : checksum

								///LOGICA PACCHETTO:
								///Per ogni sensore (num_sensori) ho un pacchetto di 52byte distribuiti nel modo seguente:
								///a11,a12,a13,a14 - a21.a22.a23.a24 - a31.a32.a33.a34 - g11[...]
								///ovvero
								///acc1 - acc2 - acc3 - gyr1 [...]
								///Per ogni acc1 etc.. dobbiamo cavarci il double

								///Una volta chiaro il numero di sensori (solitamente sono sempre 5) possiamo istanziare e trattare la sampwin.
								sampwin = new List<double[,]>();
								
								while (serverIsActive) {
									///Creazione sampwin.
									///Ogni ciclo di questo loop identifica un campione, ovvero un istante catturato dai vari sensori e inviato simultaneamente.
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
											arr[i, j] = field;
											//printToServerConsole(String.Format("{0}; ", field));
										}
										//Console.WriteLine();
									}
									sampwin.Add(arr);

									///Quando lo stream da leggere è terminato questa operazione genera un errore IndexOutOfRangeException.
									///Pertanto l'errore IndexOutOfRangeException è voluto e cercato per capire quando terminare la lettura dello stream dal Client e chiudere la connessione.
									if (num_sensori < 5) {
										package = reader.ReadBytes(byteToRead + 4);
									} else {
										package = reader.ReadBytes(byteToRead + 6);
									}
								}
							} catch (IndexOutOfRangeException ex) {
								//Ignore this Exception
								///Quando le stream è esaurito dovrebbe automaticamente generare questa eccezione.
								printToServerConsole("Stream finished.\n");

								///Funzione che triggera la lettura della sampwin per la creazione dei grafici.
								eatSampwinProtected(sampwin);

								printToServerConsole("Creating file CSV in " + path + "...\n");

								///Creazione CSV.
								if (!writeMatrixToCSV(sampwin, path + "sampwin.csv")) {
									MessageBox.Show("Errore creazione CSV");
								} else {
									printToServerConsole("File CSV created in " + path + ".\n");
								}
								//(!) aggiungere throw!
							} catch (Exception ex) {
								throw new Exception(ex.Message); //(!)codice di cui verificare il corretto funzionamento
								//MessageBox.Show("Errore Connessione 0x00!\n" + ex.Message);
							} finally {
								client.Close();
								printToServerConsole("Client Disconnected.\n");
							}
						}
					} catch (SocketException ex) {
						//(!)Ignore this Exception
					} catch (InvalidOperationException ex) {
						//(!)Ignore this Exception
					} catch (Exception ex) {
						MessageBox.Show("Errore Connessione 0x01!\n" + ex.Message);
					} finally {
						///Stop listening for new clients.
						///Server Stop.
						server.Stop();
						serverIsActive = false;
						setButtonServerStart(serverIsActive);
						printToServerConsole("Server Stopped.\n");
					}
				}
			}
		}

		/// <summary>
		/// Scrive la sampwin su un file CSV.
		/// </summary>
		/// <param name="matrix">Sampwin da stampare su file.</param>
		/// <param name="csvPath">Path dove salvare la Sampwin.</param>
		/// <returns>Esito booleano dell'operazione di stampa su file.</returns>
		public static bool writeMatrixToCSV(List<double[,]> matrix, string csvPath) {
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
				File.WriteAllText(csvPath, csv.ToString());
			} catch (Exception ex) {
				MessageBox.Show("Print stream to CSV Error\n" + ex.Message);
				return false;
			}
			return true;
		}
	}
}
