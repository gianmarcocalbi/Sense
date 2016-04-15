using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using ZedGraph;
using System.Drawing;

namespace Sense {
	/// <summary>
	/// Main Program.
	/// </summary>
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

	/// <summary>
	/// Classe Parser.
	/// </summary>
	class Parser {
		public List<double[,]> sampwin;
		int port;
		TcpListener server;
		IPAddress localAddr;
		Form1.printToServerConsoleDelegate printToServerConsole;
		Form1.setButtonServerStartDelegate setButtonServerStart;
		Form1.eatSampwinDelegate eatSampwinProtected;
		public bool serverIsActive;
		string path;
		int frequence;
		int window;
		private bool sampwinIsFullIdle;
		int clients_amount;

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

		public bool SampwinIsFullIdle {
			get {
				return sampwinIsFullIdle;
			}

			set {
				sampwinIsFullIdle = value;
			}
		}
		
		///<summary>
		///Costruttore Parser.
		///</summary>
		///<param name="p">Porta sul quale aprire la Socket.</param>
		///<param name="ip">Indirizzo ip server.</param>
		///<param name="csvPath">Path dove salvare il file csv.</param>
		///<param name="printToConsoleFunc">Funzione delegata per scrivere sulla console.</param>
		///<param name="setButtonServerStartFunc">Funzione delegata per cambiare il testo di un tasto.</param>
		///<param name="eatSampFunc">Funzione delegata per triggerare il parsing della sampwin.</param>
		public Parser(int p, string ip, string csvPath, int freq, int wind, int clients_amou, Form1.printToServerConsoleDelegate printToConsoleFunc, Form1.setButtonServerStartDelegate setButtonServerStartFunc, Form1.eatSampwinDelegate eatSampFunc) {
			path = csvPath;
			port = p;
			frequence = freq;
			window = wind;
			clients_amount = clients_amou;
			try {
				localAddr = IPAddress.Parse(ip);
				server = new TcpListener(localAddr, port);
			} catch (Exception ex) {
				MessageBox.Show("IP Addressing Error!\n" + ex.Message);
			}
			//Salviamo in locale le funzioni passate al Parser
			sampwin = null;
			printToServerConsole = printToConsoleFunc;
			setButtonServerStart = setButtonServerStartFunc;
			eatSampwinProtected = eatSampFunc;
			serverIsActive = false;
			sampwinIsFullIdle = false;
			setButtonServerStart(serverIsActive); //< Setta il testo sul tasto "serverStart" a START
		}

		///<summary>Attiva il Server aggiornando i parametri in base all'input del'utente.</summary>
		///<param name="p">Porta sul quale aprire la Socket.</param>
		///<param name="ip">Indirizzo ip server.</param>
		///<param name="csvPath">Path dove salvare il file csv.</param>
		public void ActivateServer(int p, string ip, string csvPath, int freq, int wind, int clients_amou) {
			port = p;
			path = csvPath;
			frequence = freq;
			window = wind;
			clients_amount = clients_amou;
			//Proviamo a parsare le informazioni per istanziare il server
			try {
				localAddr = IPAddress.Parse(ip);
				server = new TcpListener(localAddr, port);
				serverIsActive = true;
			} catch (Exception ex) {
				MessageBox.Show("IP Addressing Error!\n" + ex.Message);
				serverIsActive = false;
			}
			setButtonServerStart(serverIsActive);
		}

		///<summary>
		///Disattiva il Server (server.Stop()).
		///</summary>
		public void DeactivateServer() {
			serverIsActive = false;
			setButtonServerStart(serverIsActive);
			server.Stop();
			//printToServerConsole(server.ToString());
			//printToServerConsole("Server Stopped.");
		}
		
		public void RunClient(TcpClient client, int client_index) {
			///Se supera la chiamata vuol dire che è avvenuta la connessione col Client.
			printToServerConsole("Connected!\n");

			///Ottiene lo stream dal Client per leggere i dati inviati dallo stesso.
			NetworkStream stream = null;
			try {
				stream = client.GetStream();
			} catch (ObjectDisposedException ex) {
				///The TcpClient has been closed.
				throw;
			} catch (InvalidOperationException ex) {
				///The TcpClient is not connected to a remote host.
				throw new InvalidOperationException("Error while getting client stream.\n" + ex.Message);
			}
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
					printToServerConsole(String.Format("- BID : {0}\n- MID : {1}\n- LEN : {2}\n- EXT_LEN_MUL : {3}\n- EXT_LEN_ADD : {4}\n", package[0], package[1], package[2], package[3], package[4]));
					//Console.WriteLine("BID : {0}\nMID : {1}\nLEN : {2}\nEXT_LEN_MUL : {3}\nEXT_LEN_ADD : {4}", package[0], package[1], package[2], package[3], package[4]);
				} else {
					data.CopyTo(package, 3);
					printToServerConsole(String.Format("- BID : {0}\n- MID : {1}\n- LEN : {2}\n", package[0], package[1], package[2]));
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
				List<double[,]> _sampwin = new List<double[,]>();

				while (serverIsActive && package.Length > 0) {
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
					_sampwin.Add(arr);

					///Roba varia
					//(!)Sistemare window

					if (_sampwin.Count <= window * frequence) {
						if (_sampwin.Count == window * frequence) {
							//eatSampwinProtected(sampwin.GetRange(sampwin.Count - window * frequence, window * frequence));
							eatSampwinProtected(_sampwin, client_index);
						}
					} else if (_sampwin.Count % ((window * frequence) / 2) == 0 && _sampwin.Count != 0) {
						///Funzione che triggera la lettura della sampwin per la creazione dei grafici.
						//eatSampwinProtected(sampwin.GetRange(sampwin.Count - 500, 500)); //(!)Valutare gli errori generati da questo metodo
						eatSampwinProtected(_sampwin, client_index);
					}
					///Quando lo stream da leggere è terminato l'operazione ReadBytes ritornerà un array vuoto e quindi la lettura terminerà.
					if (num_sensori < 5) {
						package = reader.ReadBytes(byteToRead + 4);
					} else {
						package = reader.ReadBytes(byteToRead + 6);
					}
				}

				///Se effettivamente è stato letto tutto lo stream allora procedo.
				///Altrimenti vuol dire che il processo è stato interrotto con un richiesta bloccante (e.g. stop manuale del server da parte dell'utente).
				if (package.Length == 0) {
					///Lettura dati terminata, tutti i dati in arrivo dal Client sono stati ricevuti, parsati e inseriti nella lista sampwin.
					printToServerConsole("Stream finished.\n");
					sampwinIsFullIdle = true;
					///Funzione che triggera la lettura della sampwin per la creazione dei grafici.
					eatSampwinProtected(_sampwin, client_index); //(!)Valutare gli errori generati da questo metodo
					printToServerConsole("Creating file CSV in " + path + "...\n");

					///Creazione CSV.
					//(!)
					/*if (!writeMatrixToCSV(sampwin, path + @"\sampwin.csv")) {
						printToServerConsole("Csv File creation Error.\n");
					} else {
						printToServerConsole("File CSV created in " + path + ".\n");
					}*/
				}
			} catch (IndexOutOfRangeException ex) {
				printToServerConsole("Error Handler reveals Server-Client communication to be interrupted.\n");
				///Eccezione che si verifica in seguito ad un'interruzione precoce della comunicazione Server-Client che causa un utilizzo scorretto di array all'interno del codice.
				///La gestiamo consideranso la comunicazione interrotta e pertanto ignoriamo e stoppiamo la comunicazione col Client. 
			} catch (ArgumentOutOfRangeException ex) {
				printToServerConsole("Error Handler reveals Server-Client communication to be interrupted.\n");
			} catch (Exception ex) {
				throw new Exception("Exception in Client #" + client_index + " thread\n"  + ex.ToString() + "\n"); //(!)codice di cui verificare il corretto funzionamento
			} finally {
				client.Close();
				printToServerConsole("Client Disconnected.\n");
			}
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
						try {
							server.Start();
						} catch (SocketException ex) {
							throw;
						}

						printToServerConsole(String.Format("Server Started on port {0} at IP {1}\n", port, localAddr));

						///Finchè il server è attivo inizia il loop:
						///1) aspetta una connessione
						///2) accetta connessione
						///3) risolve la richiesta
						///4) disconnessione e ritorno al punto 1)
						while (serverIsActive) {
							printToServerConsole("Waiting for a connection...\n");

							///Lancia una chiamata bloccante (blocca il thread) aspettando la connessione di un Client.
							//clients_amount = 1;
							TcpClient[] client = new TcpClient[clients_amount];
							bool multi_client = clients_amount > 1;
							for (int i = 0; i < clients_amount; i++) {
								try {
									client[i] = server.AcceptTcpClient();
								} catch (InvalidOperationException ex) {
									///Il listener non è stato avviato con una chiamata a Start.
									throw new InvalidOperationException("Client connection error.\n" + ex.Message);
								} catch (SocketException ex) {
									throw;
								} finally {
									sampwinIsFullIdle = false;
								}
							}
							
							///Se supera la chiamata vuol dire che è avvenuta la connessione col Client.
							printToServerConsole("All clients(n = " + clients_amount + ") connected.\n");
							if (clients_amount > 1) { 
								Thread[] clientThreads = new Thread[clients_amount - 1];

								/*
									clients_amount = 2;
									client[2] = {mainClient, client2}; indici : [0,1]
									clientThreads[1] = {client2Thread}; indici : [0]
								*/
								for (int i = 0; i < (clients_amount-1); i++) {
									clientThreads[i] = new Thread(() => RunClient(client[i+1], i+1));
									clientThreads[i].IsBackground = true;
									clientThreads[i].Start();
									Thread.Sleep(100);
								}
							} 
							//printToServerConsole("Connected!\n");

							///Client principale.
							TcpClient mainClient = client[0];

							///Ottiene lo stream dal Client per leggere i dati inviati dallo stesso.
							NetworkStream stream = null;
							try {
								stream = mainClient.GetStream();
							} catch (ObjectDisposedException ex) {
								///The TcpClient has been closed.
								throw;
							} catch (InvalidOperationException ex) {
								///The TcpClient is not connected to a remote host.
								throw new InvalidOperationException("Error while getting client stream.\n" + ex.Message);
							}
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
									printToServerConsole(String.Format("- BID : {0}\n- MID : {1}\n- LEN : {2}\n- EXT_LEN_MUL : {3}\n- EXT_LEN_ADD : {4}\n", package[0], package[1], package[2], package[3], package[4]));
									//Console.WriteLine("BID : {0}\nMID : {1}\nLEN : {2}\nEXT_LEN_MUL : {3}\nEXT_LEN_ADD : {4}", package[0], package[1], package[2], package[3], package[4]);
								} else {
									data.CopyTo(package, 3);
									printToServerConsole(String.Format("- BID : {0}\n- MID : {1}\n- LEN : {2}\n", package[0], package[1], package[2]));
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

								while (serverIsActive && package.Length > 0) {
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

									///Roba varia
									//(!)Sistemare window

									if (sampwin.Count <= window * frequence) {
										if (sampwin.Count == window * frequence) {
											//eatSampwinProtected(sampwin.GetRange(sampwin.Count - window * frequence, window * frequence));
											eatSampwinProtected(sampwin, 0);
										}
									} else if (sampwin.Count % ((window * frequence) / 2) == 0 && sampwin.Count != 0) {
										///Funzione che triggera la lettura della sampwin per la creazione dei grafici.
										//eatSampwinProtected(sampwin.GetRange(sampwin.Count - 500, 500)); //(!)Valutare gli errori generati da questo metodo
										eatSampwinProtected(sampwin, 0);
									}
									///Quando lo stream da leggere è terminato l'operazione ReadBytes ritornerà un array vuoto e quindi la lettura terminerà.
									if (num_sensori < 5) {
										package = reader.ReadBytes(byteToRead + 4);
									} else {
										package = reader.ReadBytes(byteToRead + 6);
									}
								}

								///Se effettivamente è stato letto tutto lo stream allora procedo.
								///Altrimenti vuol dire che il processo è stato interrotto con un richiesta bloccante (e.g. stop manuale del server da parte dell'utente).
								if (package.Length == 0) {
									///Lettura dati terminata, tutti i dati in arrivo dal Client sono stati ricevuti, parsati e inseriti nella lista sampwin.
									printToServerConsole("Stream finished.\n");
									sampwinIsFullIdle = true;
									///Funzione che triggera la lettura della sampwin per la creazione dei grafici.
									eatSampwinProtected(sampwin, 0); //(!)Valutare gli errori generati da questo metodo
									printToServerConsole("Creating file CSV in " + path + "...\n");

									///Creazione CSV.
									//(!)
									/*if (!writeMatrixToCSV(sampwin, path + @"\sampwin.csv")) {
										printToServerConsole("Csv File creation Error.\n");
									} else {
										printToServerConsole("File CSV created in " + path + ".\n");
									}*/
								}
							} catch (IndexOutOfRangeException ex) {
								printToServerConsole("Error Handler reveals Server-Client communication to be interrupted.\n");
								///Eccezione che si verifica in seguito ad un'interruzione precoce della comunicazione Server-Client che causa un utilizzo scorretto di array all'interno del codice.
								///La gestiamo consideranso la comunicazione interrotta e pertanto ignoriamo e stoppiamo la comunicazione col Client. 
							} catch (ArgumentOutOfRangeException ex) {
								printToServerConsole("Error Handler reveals Server-Client communication to be interrupted.\n");
							} catch (Exception ex) {
								throw; 
							} finally {
								mainClient.Close();
								printToServerConsole("Client Disconnected.\n");
							}
						}
					} catch (ObjectDisposedException ex) {
						MessageBox.Show(ex.Message);
					} catch (InvalidOperationException ex) {
						MessageBox.Show(ex.Message);
					} catch (SocketException ex) {
						///Ingora questa eccezione: vuol dire che è stata effettuata una chiamata bloccante da parte dell'utente.
					} catch (Exception ex) {
						MessageBox.Show("Generic error caught! (This error shouldn't occur)\n"
							+ "Exception thrown in " + ex.TargetSite + "\n"
							+ "\n\n" + ex.ToString()
						);
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
				int t = 0;
				while (File.Exists(csvPath + "_" + t)) {
					t++;
				}
				File.WriteAllText(csvPath + "_" + t, csv.ToString());
			} catch (Exception ex) {
				MessageBox.Show("Print stream to CSV Error\n" + ex.Message);
				return false;
			}
			return true;
		}
	}

	/*public class Chart {
		ZedGraphControl zdc;
		GraphPane myPane;
		List<Curve> myCurveList;
		public Chart(ref ZedGraphControl zdc) {
			this.zdc = zdc;
			myPane = zdc.GraphPane;
			myCurveList = new List<Curve>();
		}

		public void SetXAxisTitle(string XAxisTitle) {
			myPane.XAxis.Title.Text = XAxisTitle;
		}

		public void SetYAxisTitle(string YAxisTitle) {
			myPane.YAxis.Title.Text = YAxisTitle;
		}

		public void SetTitle(string title) {
			myPane.Title.Text = title;
		}

		public void addCurve(Curve curve) {
		}

		public void ClearGraphPane() {
			myPane.CurveList.Clear();
			zdc.Invalidate();
		}
	}*/

	/// <summary>
	/// Classe Curve.
	/// Consiste in una curva da plottare sul zedGraphControl.
	/// </summary>
	public class Curve {
		private string label;           /// Etichetta curva.
		private double[] pointsValue;   /// Array di valori double della funzione.
		private PointPairList pointsValueList;   /// Array di valori double della funzione.
		private Color color;            /// Colore curva.
		private SymbolType symbolType;  /// Tipo simbolo.

		//Properties Begin
		public string Label {
			get {
				return label;
			}

			set {
				label = value;
			}
		}

		public Color Color {
			get {
				return color;
			}

			set {
				color = value;
			}
		}

		public SymbolType SymbolType {
			get {
				return symbolType;
			}

			set {
				symbolType = value;
			}
		}

		public double[] PointsValue {
			get {
				return (double[])pointsValue.Clone();
			}

			set {
				pointsValue = value;
			}
		}

		public PointPairList PointsValueList {
			get {
				return pointsValueList;
			}

			set {
				pointsValueList = value;
			}
		}

		//Properties End

		/// <summary>
		/// Costruttore Primario (Completo).
		/// </summary>
		/// <param name="label">Etichetta curva.</param>
		/// <param name="pointsValue">Array di valori double della funzione.</param>
		/// <param name="color">Colore curva.</param>
		/// <param name="symbolType">Tipo simbolo.</param>
		public Curve(string label, double[] pointsValue, Color color, SymbolType symbolType) {
			this.label = label;
			this.pointsValue = pointsValue;
			pointsValueList = null;
			this.color = color;
			this.symbolType = symbolType;
		}

		public Curve(string label, PointPairList pointsValueList, Color color, SymbolType symbolType) {
			this.label = label;
			this.pointsValueList = pointsValueList;
			pointsValue = null;
			this.color = color;
			this.symbolType = symbolType;
		}

		/// <summary>
		/// Overload Costruttore Primario.
		/// </summary>
		/// <param name="label">Etichetta curva.</param>
		/// <param name="pointsValue">Array di valori double della funzione.</param>
		/// <param name="color">Colore curva.</param>
		public Curve(string label, double[] pointsValue, Color color) : this(label, pointsValue, color, SymbolType.None) { }

		/// <summary>
		/// Overload Costruttore Primario.
		/// </summary>
		/// <param name="label">Etichetta curva.</param>
		/// <param name="pointsValue">Array di valori double della funzione.</param>
		public Curve(string label, double[] pointsValue) : this(label, pointsValue, Color.Blue, SymbolType.None) { }

		/// <summary>
		/// Overload Costruttore Primario.
		/// </summary>
		/// <param name="pointsValue">Array di valori double della funzione.</param>
		public Curve(double[] pointsValue) : this("", pointsValue, Color.Blue, SymbolType.None) { }

		/// <summary>
		/// Overload Costruttore Primario.
		/// </summary>
		/// <param name="label">Etichetta curva.</param>
		/// <param name="color">Colore curva.</param>
		/// <param name="symbolType">Tipo simbolo.</param>
		public Curve(string label, Color color, SymbolType symbolType) {
			this.label = label;
			this.pointsValue = null;
			this.color = color;
			this.symbolType = symbolType;
		}

		/// <summary>
		/// Overload Costruttore Primario.
		/// </summary>
		/// <param name="label">Etichetta curva.</param>
		/// <param name="color">Colore curva.</param>
		public Curve(string label, Color color) : this(label, (double[])null, color, SymbolType.None) { }

		/// <summary>
		/// Overload Costruttore Primario.
		/// </summary>
		/// <param name="label">Etichetta curva.</param>
		public Curve(string label) : this(label, (double[])null, Color.Blue, SymbolType.None) { }

		/// <summary>
		/// Overload Costruttore Primario senza argomenti.
		/// </summary>
		public Curve() : this("", (double[])null, Color.Blue, SymbolType.None) { }

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		/// <param name="other">Curva da copiare.</param>
		public Curve(Curve other) : this(other.Label, other.PointsValue, other.Color, other.SymbolType) { }

		/// <summary>
		/// Valida la Curva.
		/// </summary>
		/// <returns>True se la Curva è valida per il plotting, false altrimenti.</returns>
		public bool IsValid() {
			bool b = true;
			if (pointsValue == null) {
				b = false;
			}
			return b;
		}
	}
}
