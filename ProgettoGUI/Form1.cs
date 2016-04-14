using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace Sense {
	public partial class Form1 : Form {
		private Parser parser;
		Thread threadParser;
		Random random = new Random();
		int frequence = 50;
		int window = 10;
		GraphPane myPane;
		int selectedChart;
		int selectedSensor;
		int selectedSensorType;
		string csvPath;
		List<double[,]> mySampwin;
		int smoothRange;
		double motoStart = 0;
		double fermoStart = 0;
		double winTime = 0;
		string action = null;
		string state = null;
		string turnAction = null;
		double turnStart = 0;
		string turnPossibleAction = null;
		double turnPossibleStart = 0;
		double stateStart = 0;
		double degree = 10;
		string outToFileStr = "";
		double refAngolo = 0;
		DateTime startTime = new DateTime(1900, 1, 1, 0, 0, 0, 0);

		/// <summary>
		/// Costruttore Primario
		/// </summary>
		public Form1() {
			///Inizializza i componenti grafici
			InitializeComponent();
			///
			myPane = zedGraphControl1.GraphPane;
			///Finestra
			window = (int)numericUpDownFinestra.Value;
			///Frequenza
			this.comboBoxFrequenza.SelectedIndex = comboBoxFrequenza.FindStringExact("50");
			frequence = Int32.Parse(comboBoxFrequenza.Text);
			///CSV Location
			csvPath = Directory.GetCurrentDirectory();
			textBoxCSVPath.Text = csvPath;
			///CSV Location hint EventHandler
			this.textBoxCSVPath.MouseEnter += new System.EventHandler(this.textBoxCSVPath_Enter);
			///numericUpDownSmoothing maximum value
			numericUpDownSmoothing.Maximum = Math.Floor((decimal)(window * frequence / 2));
			smoothRange = (int)numericUpDownSmoothing.Value;
			///Creazione Parser (Server)
			parser = new Parser(
				Int32.Parse(textBoxPort.Text),
				String.Format("{0}.{1}.{2}.{3}", textBoxIP1.Text, textBoxIP2.Text, textBoxIP3.Text, textBoxIP4.Text),
				csvPath,
				frequence,
				window,
				printToServerConsoleProtected,
				setButtonServerStartProtected,
				eatSampwinProtected
			);

			///I controlli su selectedChart, selectedSensorType, selectedSensor devono essere fatti dopo aver istanziato il parser perché chiamano una funzione di parser.
			///Altrimenti ci sarebbe Eccezione del tipo "riferimento a null".
			///selectedSensor
			comboBoxNumSensore.SelectedIndex = comboBoxNumSensore.FindStringExact("1 (Bacino)");
			selectedSensor = comboBoxNumSensore.SelectedIndex;
			///selectedSensorType
			comboBoxTipoSensore.SelectedIndex = comboBoxTipoSensore.FindStringExact("Acc");
			selectedSensorType = comboBoxTipoSensore.SelectedIndex;
			///selectedChart
			comboBoxChart.SelectedIndex = comboBoxChart.FindStringExact("Modulo");
			selectedChart = comboBoxChart.SelectedIndex;
			///Server thread
			threadParser = new Thread(parser.StartServer);
			threadParser.IsBackground = true;
			threadParser.Start();
		}

		/// <summary>
		/// Evento di click sul tasto START del server.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonServerStartClick(object sender, EventArgs e) {
			if (parser.serverIsActive) {
				///Se il server è attivo allora lo STOPpiamo
				zedGraphControl1.GraphPane.CurveList.Clear();
				zedGraphControl1.Invalidate();
				zedGraphControl1.GraphPane.Title.Text = "Chart";
				zedGraphControl1.GraphPane.XAxis.Title.Text = "x";
				zedGraphControl1.GraphPane.YAxis.Title.Text = "y";
				zedGraphControl1.AxisChange();
				parser.DeactivateServer();
				parser.sampwin = null; //(!) Mettere o non mettere questo è un dilemma conan.
			} else {
				///Se il server è fermo allora lo STARTiamo
				frequence = Int32.Parse(comboBoxFrequenza.Text);
				numericUpDownSmoothing.Maximum = Math.Floor((decimal)(window * frequence / 2));
				try {
					parser.ActivateServer(
						Int32.Parse(textBoxPort.Text),
						String.Format("{0}.{1}.{2}.{3}", textBoxIP1.Text, textBoxIP2.Text, textBoxIP3.Text, textBoxIP4.Text),
						csvPath,
						frequence,
						window
					);
				} catch (SocketException exc) {
					richTextConsole.AppendText(String.Format("{0}\n", exc));
				}
			}
		}

		/// <summary>
		/// Funzione che trigghera quando il Form1 viene caricato.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Load(object sender, EventArgs e) {   //(!) Valutare l'utilità di questo metodo
																//SAMPWIN ARRAY TRIDIMENSIONALE, SCRITTO CHIARAMENTE NELLA CONSEGNA, LA POSSIAMO SCRIVERE COME double[, ,] ANZICHE double[][][] SCRITTURA VAGAMENTE PIU BARBARICA
																//SI FA RIFERIMENTO A DUE SAMPWIN UNA CON LE INIZIALI MAIUSCOLE TRIDIMENSIONALE ED UNA TUTTA IN MINUSCOLO CON 

			this.Text = "Sense";
			this.Opacity = 1; //assolutamente inutile, ma in se l'istruzione mi piaceva, magari riesco a fare i grafici meno trasparenti
							  //this.Size = new Size(1280, 960); //non può essere utilizzato come una normale chiamata a metodo this.Size(x,y), verificato con errore a compilazione
			this.CenterToScreen();
		}

		public double[] extractDimension(List<double[,]> sampwin, int selSensor, int selSensorType, char xyz) {
			int dim = sampwin.Count();
			double[] extractedDimension = new double[dim];
			for (int i = 0; i < dim; ++i) {
				double[,] instant = sampwin[i];
				if (xyz == 'x') {
					extractedDimension[i] = instant[selSensor, selSensorType * 3];
				} else if (xyz == 'y') {
					extractedDimension[i] = instant[selSensor, selSensorType * 3 + 1];
				} else if (xyz == 'z') {
					extractedDimension[i] = instant[selSensor, selSensorType * 3 + 2];
				} else {
					extractedDimension[i] = 0;
				}
			}
			return extractedDimension;
		}

		public double[] extractDimension(List<double[,]> sampwin, char xyz) {
			return extractDimension(sampwin, selectedSensor, selectedSensorType, xyz);
		}

		/// <summary>
		/// Modulo.
		/// </summary>
		/// <param name="sampwin"></param>
		/// <returns>Array di valori modulo.</returns>
		public double[] module(List<double[,]> sampwin)    //PRIMA OPERAZIONE: MODULO
		{
			return module(sampwin, selectedSensor, selectedSensorType);
		}

		/// <summary>
		/// Overload Modulo che consente impostazione manuale del sensore e tipo di sensore selezionati.
		/// </summary>
		/// <param name="sampwin">Sampwin.</param>
		/// <param name="selSensor">Sensore da considerare.</param>
		/// <param name="selSensorType">Tipo sensore da considerare.</param>
		/// <returns>Array di valori modulo.</returns>
		public double[] module(List<double[,]> sampwin, int selSensor, int selSensorType)    //PRIMA OPERAZIONE: MODULO
		{
			int dim = sampwin.Count();
			double[] arrayModulo = new double[dim];
			for (int i = 0; i < dim; ++i) {
				double[,] instant = sampwin[i];
				arrayModulo[i] = Math.Sqrt(Math.Pow(instant[selSensor, selSensorType * 3 + 0], 2) + Math.Pow(instant[selSensor, selSensorType * 3 + 1], 2) + Math.Pow(instant[selSensor, selSensorType * 3 + 2], 2));
				//printToServerConsoleProtected(arrayModulo[i] + "\n");
			}
			return arrayModulo;
		}

		/// <summary>
		/// Operazione di Smoothing.
		/// </summary>
		/// <param name="popolazione">Array di valori da Smoothare.</param>
		/// <param name="range">Range di Smoothing.</param>
		/// <returns>Array di valori Smoothati.</returns>
		public double[] smoothing(double[] popolazione, int range)              //SECONDA OPERAZIONE: SMOOTHING
		{
			int size = popolazione.GetLength(0);
			double[] smooth = new double[size];
			int finestra = 0, dx = 0, sx = 0;
			double media = 0;
			for (int i = 0; i < size; ++i) {
				if (i < range) {
					sx = i;
				} else {
					sx = range;
				}
				if (i < size - range) {
					dx = range;
				} else {
					dx = size - i - 1;
				}
				finestra = dx + sx + 1;
				for (int j = i - sx; j <= i + dx; ++j)
					media += popolazione[j];
				media /= finestra;
				smooth[i] = media;
				media = 0;
			}
			return smooth;
		}

		/// <summary>
		/// Operazione di Derivata.
		/// </summary>
		/// <param name="sampwin">Sampwin.</param>
		/// <returns>Array di valori della Derivata.</returns>
		public double[] rapportoIncrementale(List<double[,]> sampwin)           //TERZA OPERAZIONE: DERIVATA
		{
			int dim = sampwin.Count();
			double[] rapportoIncrementale = new double[dim];
			for (int i = 0; i < dim - 1; i++) //ci vorra di sicuro dim - 1
			{
				double[,] instant1 = sampwin[i];
				double[,] instant2 = sampwin[i + 1];
				rapportoIncrementale[i] = (instant1[selectedSensor, selectedSensorType] - instant2[selectedSensor, selectedSensorType]) / ((double)1 / frequence);
			}
			return rapportoIncrementale;
		}

		/// <summary>
		/// Operazione per calcolare la Deviazione Standard.
		/// </summary>
		/// <param name="popolazione">Popolazione sulla quale calcolare la D.S.</param>
		/// <param name="range">Range entro il quale calcolare la media da usare per il calcolo della D.S.</param>
		/// <returns>Array di valori della D.S.</returns>
		public double[] deviazioneStandard(double[] popolazione, int range)     //QUARTA OPERAZIONE: DEVIAZIONE STANDARD
		{
			double[] smooth = smoothing(popolazione, range);
			int size = popolazione.GetLength(0);
			double[] deviazioneStandard = new double[size];
			int finestra = 0, dx = 0, sx = 0;
			for (int i = 0; i < size; ++i) {
				if (i < range) { sx = i; } else
					sx = range;
				if (size - range > i) { dx = range; } else
					dx = size - i - 1;
				finestra = dx + sx + 1;
				deviazioneStandard[i] = Math.Sqrt(Math.Pow((popolazione[i] - smooth[i]), 2) / (finestra));
			}
			return deviazioneStandard;
		}

		public double[,] angoliDiEulero(List<double[,]> sampwin, int selSensor) {
			double q0, q1, q2, q3;
			int dim = sampwin.Count();
			double[,] arrayAngoli = new double[3, dim];
			for (int i = 0; i < dim; ++i) {
				//estrazione della quadrupla del campione iesimo
				double[,] instant = sampwin[i];
				q0 = instant[selSensor, 9];
				q1 = instant[selSensor, 10];
				q2 = instant[selSensor, 11];
				q3 = instant[selSensor, 12];
				//Link per capire il significato degli angoli: https://goo.gl/jn8Dxg
				//roll/phi
				arrayAngoli[0, i] = Math.Atan((2 * q2 * q3 + 2 * q0 * q1) / (2 * Math.Pow(q0, 2) + 2 * Math.Pow(q3, 2) - 1));
				//pitch/theta
				arrayAngoli[1, i] = -Math.Asin(2 * q1 * q3 - 2 * q0 * q2);
				//yaw/psi
				arrayAngoli[2, i] = Math.Atan((2 * q1 * q2 + 2 * q0 * q3) / (2 * Math.Pow(q0, 2) + 2 * Math.Pow(q1, 2) - 1));
			}
			return arrayAngoli;
		}

		/// <summary>
		/// Operazione per il calcolo degli Angoli di Eulero.
		/// </summary>
		/// <param name="sampwin">Sampwin.</param>
		/// <returns>Matrice avente ad ogni colonna i 3 angoli di inclinazione.</returns>
		public double[,] angoliDiEulero(List<double[,]> sampwin)                //QUINTA OPERAZIONE: ANGOLI DI EULERO
		{
			return angoliDiEulero(sampwin, selectedSensor);
		}

		public double[,] angoliDiEuleroContinua(List<double[,]> sampwin, int selSensor) {
			double[,] arctan = angoliDiEulero(sampwin, selSensor); //con selected sensor
			int sfasamento = 0;
			double[,] thetaCorretto = new double[3, sampwin.Count]; //da sistemare con la dimensione giusta una volta stabiliti nomi definitivi 
			thetaCorretto[0, 0] = arctan[0, 0];
			thetaCorretto[1, 0] = arctan[1, 0];
			thetaCorretto[2, 0] = arctan[2, 0];
			double instant = 0;
			for (int j = 0; j < 3; j++) {
				for (int i = 1; i < sampwin.Count; i++) { //importante partenza da 1 //sistemare
					instant = arctan[j, i] - arctan[j, i - 1]; // differenza di segni opposti sempre risultato distante da 0
					if (Math.Abs(instant) > 2.5) {
						//ha fatto il salto, il lato non ancora identificato
						//2,5 per ora viene calibrato con un euristica grazie ad un dei primi casi stronzi di salto interno al range, modificabile ovviamente
						//printToServerConsoleProtected("i : " + i + ", instant : " + instant + "\n");
						if (instant < 0) {
							//da -pi/2 (questo è i) a pi/2 (questo è i - 1) in quanto la loro differenza risulta negativa
							//adesso finché non ne esco sono nella finestra -pi/2 -3/2pi (caso in cui sfasamento = 0)
							sfasamento++;
						} else {
							//da pi/2 (questo è i) a -pi/2 (questo è i - 1) in quanto la loro differenza risulta positiva
							sfasamento--;
						}
					}
					thetaCorretto[j, i] = arctan[j, i] + sfasamento * Math.PI;
				}
			}
			return thetaCorretto;
		}

		/// <summary>
		/// Operazione per il calcolo di arcotangente(magnY/magnZ).
		/// </summary>
		/// <param name="sampwin">Sampwin.</param>
		/// <returns>Array contenente i valori.</returns>
		public double[] arctanMyMz(List<double[,]> sampwin) {
			double[] arctan = new double[sampwin.Count];
			for (int i = 0; i < sampwin.Count; i++) {
				arctan[i] = Math.Atan(sampwin[i][selectedSensor, 7] / sampwin[i][selectedSensor, 8]);
			}
			return arctan;
		}

		/// <summary>
		/// Operazione per il calcolo di arcotangente(magnY/magnZ) senza discontinuità.
		/// </summary>
		/// <param name="sampwin">Sampwin.</param>
		/// <returns>Array contenente i valori.</returns>
		public double[] arctanMyMzContinua(List<double[,]> sampwin) {
			double[] arctan = arctanMyMz(sampwin);
			int sfasamento = 0;
			double[] thetaCorretto = new double[arctan.Length]; //da sistemare con la dimensione giusta una volta stabiliti nomi definitivi 
			thetaCorretto[0] = arctan[0];
			double instant = 0;
			for (int i = 1; i < arctan.Length; i++) { //importante partenza da 1 //sistemare
				instant = arctan[i] - arctan[i - 1]; // differenza di segni opposti sempre risultato distante da 0
				if (Math.Abs(instant) > 2.5) {
					//ha fatto il salto, il lato non ancora identificato
					//2,5 per ora viene calibrato con un euristica grazie ad un dei primi casi stronzi di salto interno al range, modificabile ovviamente
					//printToServerConsoleProtected("i : " + i + ", instant : " + instant + "\n");
					if (instant < 0) {
						//da -pi/2 (questo è i) a pi/2 (questo è i - 1) in quanto la loro differenza risulta negativa
						//adesso finché non ne esco sono nella finestra -pi/2 -3/2pi (caso in cui sfasamento = 0)
						sfasamento++;
					} else {
						//da pi/2 (questo è i) a -pi/2 (questo è i - 1) in quanto la loro differenza risulta positiva
						sfasamento--;
					}
				}
				thetaCorretto[i] = arctan[i] + sfasamento * Math.PI;
			}
			return thetaCorretto;
		}

		public PointPairList computeDeadReckoning(List<double[,]> sampwin) {
			//sistemare theta e a con valori veri presi da sampwin
			//appunto jack: abbiamo a disposizione thetaCorretto
			//double[] theta = arctanMyMzContinua(sampwin);
			int fattoreRealismoGimConJak = 4;
			double[,] eulero = angoliDiEuleroContinua(sampwin, 0);
			double[] theta = new double[sampwin.Count];
			for (int i = 0; i < sampwin.Count; i++) {
				theta[i] = eulero[2, i];
			}
			double[] pitch = new double[sampwin.Count];
			for (int i = 0; i < sampwin.Count; i++) {
				pitch[i] = eulero[1, i];
			}
			double[] acc = extractDimension(sampwin, 0, 0, 'y');
			PointPairList p = new PointPairList();
			double[] x = new double[sampwin.Count];
			double[] y = new double[sampwin.Count];
			double ds = 0;
			double t = 1 / (double)frequence;
			double dx, dy, v0 = 1;
			//printToServerConsoleProtected(String.Format("t = {0} - frequence : {1} \n", t, frequence));
			//double[] theta;
			x[0] = 0;
			y[0] = 0;
			//p.Add(x[0], y[0]);
			//printToServerConsoleProtected(String.Format("Punto {0}-esimo : ({1}, {2})\n", 0, x[0], y[0]));
			//aggiunta cordinata partenza a pplist
			//gia' dentro un ciclo
			double acc1;
			double[] dev = deviazioneStandard(acc, 10);
			double theta1 = theta[0];
			for (int i = 1; i < sampwin.Count; i++) {
				//acc1 = acc[i] * Math.Cos(pitch[i]);
				//v0 = ds / t; //prima iterazione velocita' nulla ovviamente
				v0 = dev[i];
				if (action != "fermo") {
					ds = v0 * t; //prima iterazione dx = 0 + (1/2)*a*t*t
				} else {
					ds = 0;
				}
				if (Math.Abs(theta1 - theta[i - 1]) > (Math.PI / 180 * degree))
					theta1 = theta[i - 1];
				//scomponimento dx lungo le sue componenti grazie all'angolo ecc
				dx = ds * Math.Cos(theta1) * fattoreRealismoGimConJak;
				dy = ds * Math.Sin(theta1) * fattoreRealismoGimConJak;
				//printToServerConsoleProtected(String.Format("v0 : {4} - dx : {0} - dy : {1} - ds : {2} - acc : {3}\n", dx, dy, ds, acc[i], v0));
				x[i] = x[i - 1] + dx; //sistemare x0, y0 con attuali valori dell-array 
				y[i] = y[i - 1] + dy;

				//printToServerConsoleProtected(String.Format("Punto {0}-esimo : ({1}, {2})\n", i, x[i], y[i]));


			}
			x = smoothing(x, 50);
			y = smoothing(y, 50);
			for (int i = 0; i < sampwin.Count; i++)
				p.Add(x[i], y[i]);
			return p;
		}

		/// <summary>
		/// Overload di populate() che considera tutti i valori dell'array in input.
		/// </summary>
		/// <param name="array">Array contenente i valori di f(x).</param>
		/// <returns>Lista di punti (x,y).</returns>
		private PointPairList populate(double[] array) {
			return populate(array, 0, array.Length);
		}

		/// <summary>
		/// Funzione per creare una lista di punti (x,y=f(x)) da un array di valori double.
		/// Ogni cella di double[] contiene un valore di f(x) dove x (tempo) è calcolata sulla base della frequenza di campionamento.
		/// </summary>
		/// <param name="array">Array contenente i valori di f(x).</param>
		/// <param name="begin">Indice del primo elemento del dominio di f(x).</param>
		/// <param name="range">Range di elementi dell'intervallo che comporrà il dominio di f(x).</param>
		/// <returns>Lista di punti (x,y) da plottare.</returns>
		private PointPairList populate(double[] array, int begin, int range) {
			int length = array.Length;
			if (begin < 0) {
				begin = 0;
			}
			if (begin + range < length) {
				length = begin + range;
			}
			PointPairList list = new PointPairList();
			for (int i = begin; i < length; ++i)
				list.Add((double)i / frequence, array[i]);
			return list;
		}
		//Plotting Functions END

		//Old Functions BEGIN
		/*private void createGraph(ZedGraph.ZedGraphControl zedGraphControl, int drawX, int drawY, int sizeX, int sizeY, string titolo, string x, string y) {
            zedGraphControl.Location = new Point(drawX, drawY);
            zedGraphControl.Size = new Size(sizeX, sizeY);
            myPane = zedGraphControl.GraphPane;
            myPane.Title.Text = titolo;
            myPane.XAxis.Title.Text = x;
            myPane.YAxis.Title.Text = y;
        }*/

		/*public double[,] generateSampwin() //generazione simulata di un sampwin semplificato
        {
            int firstDimension = 13;
            double[,] sampwin = new double[firstDimension, frequence * window];
            for (int i = 0; i < firstDimension; ++i)
                sampwin[i, 0] = random.Next(-100, 100);
            for (int i = 0; i < firstDimension; ++i)
                for (int j = 1; j < frequence * window; ++j)
                    sampwin[i, j] = sampwin[i, j - 1] + (random.Next(-100, 100));
            return sampwin;
        }*/

		/*public double[] multiToSingleArray(double[,] multiArray, int firstDimension) {
            //if 0 <= firstDimension <= 2 stiamo estraendo una delle coordinate per la simulazione di un primo generico sensore
            //if 9 <= firstDimension <= 12 stiamo estraendo uno dei quaternioni
            int dim = multiArray.GetLength(1);
            double[] singleArray = new double[dim];
            for (int i = 0; i < dim; ++i)
                singleArray[i] = multiArray[firstDimension, i];
            return singleArray;
        }*/

		protected override bool ProcessDialogKey(Keys keyData) //escape 
		{
			if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape) {
				this.Close();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		//Old Functions END

		//Delegate functions BEGIN
		/// <summary>
		/// Delegato per scrivere sulla console del Server.
		/// </summary>
		/// <param name="s">Stringa da scrivere.</param>
		public delegate void printToServerConsoleDelegate(string s);

		/// <summary>
		/// Stampa sulla console del Server.
		/// </summary>
		/// <param name="s">Stringa da scrivere.</param>
		public void printToServerConsoleProtected(string s) {
			if (this.richTextConsole.InvokeRequired) {
				Invoke(new printToServerConsoleDelegate(printToServerConsoleProtected), new object[] { s });
			} else {
				richTextConsole.AppendText(s);
				if (checkBoxConsoleAutoFlow.Checked) {
					richTextConsole.SelectionStart = richTextConsole.Text.Length;
					richTextConsole.ScrollToCaret();
				}
			}
		}

		/// <summary>
		/// Delegato per impostare il valore del tasto per avviare il Server.
		/// </summary>
		/// <param name="b">True se il server viene startato altrimenti false.</param>
		public delegate void setButtonServerStartDelegate(bool b);

		/// <summary>
		/// Imposta il testo del tasto di avvio del Server.
		/// </summary>
		/// <param name="b">True se il server viene startato altrimenti false.</param>
		public void setButtonServerStartProtected(bool b) {
			if (this.buttonServerStart.InvokeRequired) {
				Invoke(new setButtonServerStartDelegate(setButtonServerStartProtected), new object[] { b });
			} else {
				if (b) {
					///Disabilita input server quando server attivo
					textBoxPort.Enabled = false;
					textBoxIP1.Enabled = false;
					textBoxIP2.Enabled = false;
					textBoxIP3.Enabled = false;
					textBoxIP4.Enabled = false;
					comboBoxFrequenza.Enabled = false;
					numericUpDownFinestra.Enabled = false;
					textBoxCSVPath.Enabled = false;
					buttonSelectFolder.Enabled = false;
					buttonServerStart.Text = "STOP";
				} else {
					///Riabilita input server quando server inattivo
					textBoxPort.Enabled = true;
					textBoxIP1.Enabled = true;
					textBoxIP2.Enabled = true;
					textBoxIP3.Enabled = true;
					textBoxIP4.Enabled = true;
					comboBoxFrequenza.Enabled = true;
					numericUpDownFinestra.Enabled = true;
					textBoxCSVPath.Enabled = true;
					buttonSelectFolder.Enabled = true;
					buttonServerStart.Text = "START";
				}
			}
		}

		/// <summary>
		/// Delegato per plottare la sampwin.
		/// </summary>
		/// <param name="sampwin">Sampwin.</param>
		public delegate void eatSampwinDelegate(List<double[,]> sampwin);

		/// <summary>
		/// Plotta la sampwin.
		/// </summary>
		/// <param name="sampwin">Sampwin.</param>
		public void eatSampwinProtected(List<double[,]> sampwin) {
			if (this.zedGraphControl1.InvokeRequired) {
				Invoke(new eatSampwinDelegate(eatSampwinProtected), new object[] { sampwin });
			} else {
				//Quando il server ha finito di leggere la sampwin ce ne salviamo una copia il locale.
				if (parser.SampwinIsFullIdle) {
					mySampwin = sampwin;
				}
				DrawSampwin(sampwin);
				ParseActions(sampwin);
			}
		}
		//Delegate functions END

		public void DrawSampwin(List<double[,]> sampwin) {

			List<Curve> myCurveList = new List<Curve>();
			List<LineItem> myLineList = new List<LineItem>();
			myPane.CurveList.Clear();
			zedGraphControl1.Invalidate();
			myPane.Title.Text = "Chart";
			myPane.XAxis.Title.Text = "time (seconds)";

			switch (selectedSensorType) {
				case 0:
					///acc
					myPane.YAxis.Title.Text = "m²";
					break;
				case 1:
					///gyr
					myPane.YAxis.Title.Text = "y";
					break;
				case 2:
					///mag
					myPane.YAxis.Title.Text = "Tesla (?)";
					break;
				case 3:
					///qua
					myPane.YAxis.Title.Text = "roba di quaternioni";
					break;
				default:
					///bohh
					myPane.YAxis.Title.Text = "none";
					break;
			}

			switch (selectedChart) {
				case 0:
					myCurveList.Add(new Curve("Module", module(sampwin), Color.Blue));
					myPane.Title.Text = "Module";
					break;
				case 1:
					myCurveList.Add(new Curve("Derivate", rapportoIncrementale(sampwin), Color.Blue));
					myPane.Title.Text = "Derivate";
					break;
				case 2:
					int length = sampwin.Count();
					double[,] instant = angoliDiEuleroContinua(sampwin, selectedSensor); //(!) mettere angoliDiEulero
					double[] Phi = new double[length];
					double[] Theta = new double[length];
					double[] Psi = new double[length];
					for (int i = 0; i < length; i++) {
						Phi[i] = instant[0, i];
						Theta[i] = instant[1, i];
						Psi[i] = instant[2, i];
					}
					myCurveList.Add(new Curve("Phi", Phi, Color.Cyan));
					myCurveList.Add(new Curve("Theta", Theta, Color.Magenta));
					myCurveList.Add(new Curve("Psi", Psi, Color.YellowGreen));
					myPane.YAxis.Title.Text = "rad";
					break;
				case 3:
					myCurveList.Add(new Curve("Standard Deviation", deviazioneStandard(module(sampwin), smoothRange), Color.Blue));
					myPane.Title.Text = "Standard Deviation";
					break;
				case 4:
					myCurveList.Add(new Curve("arcotangente(magnY/magnZ)", arctanMyMzContinua(sampwin), Color.Blue));
					myPane.Title.Text = "arcotangente(magnY/magnZ)";
					myPane.YAxis.Title.Text = "arcotangente(magnY/magnZ)";
					break;
				case 5:
					///Dead Reckoning
					//(!)Usare Curve anche qua
					myPane.Title.Text = "Path";
					myPane.YAxis.Title.Text = "m";
					myPane.XAxis.Title.Text = "m";
					PointPairList tempPPL = computeDeadReckoning(sampwin);
					LineItem tmpLine1 = myPane.AddCurve("Path", tempPPL, Color.BlueViolet, SymbolType.None);
					PointPairList templist1 = new PointPairList();
					PointPairList templist2 = new PointPairList();
					templist1.Add(tempPPL.First().X, tempPPL.First().Y);
					LineItem tmpLine2 = myPane.AddCurve("Start", templist1, Color.Red, SymbolType.Triangle);
					templist2.Add(tempPPL.Last().X, tempPPL.Last().Y);
					LineItem tmpLine3 = myPane.AddCurve("End", templist2, Color.Red, SymbolType.Circle);
					//myLineList.Add(tmpLine);
					break;
				default:
					break;
			}

			if (checkBoxSmoothing.Checked) {
				foreach (Curve c in myCurveList) {
					c.PointsValue = smoothing(c.PointsValue, smoothRange);
					c.Label += " smoothed";
				}
			}
			if (checkBoxSegmentation.Checked) {
				//myCurveList = segmentation(myCurveList);
			}
			if (checkBoxNoiseCanceling.Checked) {
				List<Curve> myNewCurveList = new List<Curve>();
				for (int i = 0; i < myCurveList.Count; i++) {
					double[] instant1 = myCurveList[i].PointsValue;
					double[] instant2 = deviazioneStandard(myCurveList[i].PointsValue, smoothRange);
					for (int j = 0; j < myCurveList[i].PointsValue.Length; j++) {
						instant1[j] += instant2[j];
						instant2[j] = instant1[j] - 2 * instant2[j];
					}
					myNewCurveList.Add(new Curve("instant1", instant1, Color.Cyan));
					myNewCurveList.Add(new Curve("instant2", instant2, Color.Cyan));
					myNewCurveList.Add(new Curve(myCurveList[i].Label, myCurveList[i].PointsValue, Color.Blue));
				}
				myCurveList = myNewCurveList;
			}

			foreach (Curve c in myCurveList) {
				PointPairList ppl = new PointPairList();
				if (checkBoxPlotDomain.Checked) {
					ppl = populate(c.PointsValue, c.PointsValue.Length - window * frequence, c.PointsValue.Length);
				} else {
					ppl = populate(c.PointsValue);
				}
				LineItem myLine = myPane.AddCurve(c.Label, ppl, c.Color, c.SymbolType);
				myLineList.Add(myLine);
				//(!)printToServerConsoleProtected(c.Label + " chart drawn.\n");
			}

			zedGraphControl1.AxisChange();
			zedGraphControl1.Refresh();
		}

		public void ParseActions(List<double[,]> sampwin) {

			/************************************/
			/****** Parse Actions ***************/
			/************************************/
			List<double[,]> parsingMatrix = new List<double[,]>();

			if (sampwin.Count > window * frequence) {
				parsingMatrix = sampwin.GetRange(sampwin.Count - window * frequence, window * frequence);
			} else {
				parsingMatrix = sampwin;
			}

			///MOTO-STAZIONAMENTO
			if (startTime.Year == 1900) {
				startTime = DateTime.Now;
			}

			///Deviazione Standard modulo accelerometro
			double[] stDevArray = smoothing(deviazioneStandard(module(parsingMatrix, 0, 0), 10), 10);
			double[] accXArray = smoothing(extractDimension(sampwin, 0, 0, 'x'), 10);
			double[] thetaMagnArray = smoothing(arctanMyMzContinua(sampwin), 10);
			double[] thetaMagnStDevArray = smoothing(deviazioneStandard(thetaMagnArray, 10), 10);
			double time = 0;
			refAngolo = thetaMagnArray[0];
			DateTime tempTime = startTime;
			for (int i = 0; i < stDevArray.Length; i++) {
				time = (sampwin.Count - window * frequence > 0 ? (sampwin.Count - window * frequence + (double)i) / frequence : (double)i / frequence);
				if (time > winTime) {
					///Moto-stazionamento
					if (stDevArray[i] < 0.01) {
						//(!) 0.01 valore determinato in modo empirico altamente fallace
						//possibile moto stazionario
						//finisce il moto
						//(!) sistemare tutta sta roba ci sono mille variabili inutili (length e end non servono)
						if (action == "non-fermo" /*&& fermoEnd <= time*/) {
							///Fine del moto.
							///Stampa l'azione di moto appena terminata.
							outToFileStr += tempTime.AddSeconds(motoStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time).ToString("HH:mm:ss") + " non-fermo\n";
							//printToServerConsoleProtected(motoStart + " " + motoEnd + " non-fermo\n");
							//save start point moto stazionario
							fermoStart = time; ///L'inizio del moto-stazionario coincide con la fine del moto.
						}
						///Viene impostata l'azione attuale. 
						action = "fermo";
					} else {
						//possibile moto motoso
						//finisce il moto stazionario
						if (action == "fermo" /*&& motoEnd <= time*/) {
							//il non moto è finito, mi salvo i dati che devo salvare
							//save end point non moto
							outToFileStr += tempTime.AddSeconds(fermoStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time).ToString("HH:mm:ss") + " fermo\n";
							//printToServerConsoleProtected(fermoStart + " " + fermoEnd + " fermo\n");
							//save start point moto stazionario
							motoStart = time;
						}
						action = "non-fermo";
					}

					///Lay-Stand-Sit
					if (accXArray[i] <= 2.7) {
						if (state != "Lay" && state != null) {
							outToFileStr += tempTime.AddSeconds(stateStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time).ToString("HH:mm:ss") + " " + state + "\n";
							stateStart = time;
						}
						state = "Lay";
					} else if (2.7 < accXArray[i] && accXArray[i] <= 3.7) {
						if (state != "LaySit" && state != null) {
							outToFileStr += tempTime.AddSeconds(stateStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time).ToString("HH:mm:ss") + " " + state + "\n";
							stateStart = time;
						}
						state = "LaySit";
					} else if (3.7 < accXArray[i] && accXArray[i] <= 7) {
						if (state != "Sit" && state != null) {
							outToFileStr += tempTime.AddSeconds(stateStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time).ToString("HH:mm:ss") + " " + state + "\n";
							stateStart = time;
						}
						state = "Sit";
					} else { //> 7 
						if (state != "Stand" && state != null) {
							outToFileStr += tempTime.AddSeconds(stateStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time).ToString("HH:mm:ss") + " " + state + "\n";
							stateStart = time;
						}
						state = "Stand";
					}

					///Girata
					if (Math.Abs(thetaMagnArray[i] - refAngolo) < 0.15) { //più o meno (5 gradi == 0.087 radianti), 0.5 = 30 gradi, una volta sistemato il tempo potremmo mettere 0.15
																		  //ci vuole un ulteriore if che dice che non può proseguire se è allo stesso tempo fermo e si sta sedendo, approfondire
						if (turnAction != "prosegue") { //vuol dire che non può mai iniziare con una girata? concordare con gimmy
							if (turnPossibleAction != "prosegue") {
								turnPossibleStart = time;
								turnPossibleAction = "prosegue";
							}
							if (time - turnPossibleStart > 0.3) {
								//importante AddSeconds abbia time - timePossibleStart
								outToFileStr += tempTime.AddSeconds(turnStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time - turnPossibleStart).ToString("HH:mm:ss") + " " + turnAction + "\n";
								turnAction = turnPossibleAction;
								turnStart = turnPossibleStart;
							}
						}
					} else {
						//segue if per determinare direzione della svolta
						if (thetaMagnArray[i] - refAngolo < 0) {
							//DX
							//non mi è chiaro il discorso del null visto che viene applicato a tutte, non trovo l'inizializzazione forse è quello, np mi spiegherà gimmy
							if (turnAction != "girata dx") {    //vuol dire che non può mai iniziare con una girata? concordare con gimmy
								if (turnPossibleAction != "girata dx") {
									turnPossibleStart = time;
									turnPossibleAction = "girata dx";
								} else if (time - turnPossibleStart > 0.3) {
									//importante AddSeconds abbia time - timePossibleStart
									outToFileStr += tempTime.AddSeconds(turnStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time - turnPossibleStart).ToString("HH:mm:ss") + " " + turnAction + "\n";
									turnAction = turnPossibleAction;
									turnStart = turnPossibleStart;
								}
							}
						} else {
							//SX
							if (turnAction != "girata sx") {    //vuol dire che non può mai iniziare con una girata? concordare con gimmy
								if (turnPossibleAction != "girata sx") {
									turnPossibleStart = time;
									turnPossibleAction = "girata sx";
								} else if (time - turnPossibleStart > 0.3) {
									//importante AddSeconds abbia time - timePossibleStart
									outToFileStr += tempTime.AddSeconds(turnStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time - turnPossibleStart).ToString("HH:mm:ss") + " " + turnAction + "\n";
									turnAction = turnPossibleAction;
									turnStart = turnPossibleStart;
								}
							}
						}
						//variazione avvenuta
						refAngolo = thetaMagnArray[i];
					}
					/* salvare questa parte metti che non funziona la ricostruiamo a partire da questa tanto erano praticamente simmetriche

                    if (turnAction != "prosegue" && turnAction != null) {
                                                    outToFileStr += tempTime.AddSeconds(turnStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time).ToString("HH:mm:ss") + " " + turnAction + "\n";
                                                    turnStart = time;//sto mantenendo la direzione
                                                }
                                                turnAction = "prosegue";

                                                    //SX
                                                    if (turnAction != "girata sx" && turnAction != null && time - turnStart > 0.5) {
                                                        outToFileStr += tempTime.AddSeconds(turnStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time).ToString("HH:mm:ss") + " " + turnAction + "\n";
                                                        turnStart = time;//sto mantenendo la direzione
                                                    }
                                                    turnAction = "girata sx";
                                                }
                                                //variazione avvenuta
                                                refAngolo = thetaMagnArray[i];
                                            }
                    */

					/*
                     double theta1 = theta[0];
            for (int i = 1; i < sampwin.Count; i++) {
                acc1 = acc[i] * Math.Cos(pitch[i]);
                //v0 = ds / t; //prima iterazione velocita' nulla ovviamente
                v0 = ds * t;
                if (action != "fermo") {
                    ds = v0 * t + 0.5 * acc1 * t * t; //prima iterazione dx = 0 + (1/2)*a*t*t
                } else {
                    ds = 0;
                }
                if (Math.Abs(theta1 - theta[i - 1]) > (Math.PI / 180 * degree))
                    theta1 = theta[i -1];
                    */


				}
			}
			winTime = time;

			if (parser.SampwinIsFullIdle) {
				//Quando il parser ha letto tutta la sampwin allora...
				//Crea un nuovo file di log senza sovrascriverne.
				int t = 0;
				while (File.Exists(csvPath + @"\actions_log_" + t + ".txt")) {
					t++;
				}
				StreamWriter actionFile = new StreamWriter(csvPath + @"\actions_log_" + t + ".txt", true);
				//Se lo stato non è null allora stampo la sua fine.
				if (state != null) {
					actionFile.WriteLine(outToFileStr + tempTime.AddSeconds(stateStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time).ToString("HH:mm:ss") + " " + state);
					outToFileStr = "";
				}
				//Se c'è moto allora ne stampo la fine.
				if (action != null) {
					actionFile.WriteLine(outToFileStr + tempTime.AddSeconds(motoStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(winTime).ToString("HH:mm:ss") + " " + action);
					action = null;
					outToFileStr = "";
				}

				if (turnAction != null) {
					actionFile.WriteLine(outToFileStr + tempTime.AddSeconds(turnStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(winTime).ToString("HH:mm:ss") + " " + turnAction);
					turnAction = null;
					outToFileStr = "";
				}

				motoStart = 0;
				fermoStart = 0;
				action = null;
				state = null;
				stateStart = 0;
				turnStart = 0;
				turnPossibleStart = 0;
				turnAction = null;
				outToFileStr = "";
				winTime = 0;
				startTime = new DateTime(1900, 1, 1);
				actionFile.Close();
				printToServerConsoleProtected("Action log file created " + csvPath + @"\actions_log_" + t + ".txt\n");
			}
		}

		/****************************************************/
		/*** Eventi triggherati da input Utente sulla GUI ***/
		/****************************************************/
		private void comboBoxFrequenza_SelectedIndexChanged(object sender, EventArgs e) {
			//frequence = Int32.Parse(comboBoxFrequenza.Text);
			//numericUpDownSmoothing.Maximum = Math.Floor((decimal)(window * frequence / 2));
		}

		private void buttonSelectFolder_Click(object sender, EventArgs e) {
			DialogResult result = folderBrowserDialog1.ShowDialog();
			if (result == DialogResult.OK) {
				csvPath = folderBrowserDialog1.SelectedPath;
				textBoxCSVPath.Text = csvPath;
			}
		}

		private void comboBoxChart_SelectedIndexChanged(object sender, EventArgs e) {
			selectedChart = comboBoxChart.SelectedIndex;
			if (parser.sampwin != null) {
				DrawSampwin(parser.sampwin);
			}
		}

		private void comboBoxTipoSensore_SelectedIndexChanged(object sender, EventArgs e) {
			selectedSensorType = comboBoxTipoSensore.SelectedIndex;
			if (parser.sampwin != null) {
				DrawSampwin(parser.sampwin);
			}
		}

		private void comboBoxNumSensore_SelectedIndexChanged(object sender, EventArgs e) {
			selectedSensor = comboBoxNumSensore.SelectedIndex;
			if (parser.sampwin != null) {
				DrawSampwin(parser.sampwin);
			}
		}

		private void numericUpDownFinestra_ValueChanged(object sender, EventArgs e) {
			window = (int)numericUpDownFinestra.Value;
			numericUpDownSmoothing.Maximum = Math.Floor((decimal)(window * frequence / 2));
		}

		private void textBoxCSVPath_Enter(object sender, EventArgs e) {
			TextBox TB = (TextBox)sender;
			int VisibleTime = 500;  //in milliseconds

			ToolTip tt = new ToolTip();
			tt.Show("CSV Path", TB, 0, 20, VisibleTime);
		}

		private void buttonClearConsole_Click(object sender, EventArgs e) {
			richTextConsole.Text = "";
			if (parser.serverIsActive) {
				richTextConsole.Text = "Server is Active.\n";
			}
		}

		private void checkBoxSmoothing_CheckedChanged(object sender, EventArgs e) {
			if (parser.sampwin != null) {
				DrawSampwin(parser.sampwin);
			}
			//(!)numericUpDownSmoothing.Enabled = checkBoxSmoothing.Checked;
		}

		private void checkBoxSegmentation_CheckedChanged(object sender, EventArgs e) {
			if (parser.sampwin != null) {
				DrawSampwin(parser.sampwin);
			}
		}

		private void checkBoxNoiseCanceling_CheckedChanged(object sender, EventArgs e) {
			if (parser.sampwin != null) {
				DrawSampwin(parser.sampwin);
			}
		}

		private void checkBoxPlotDomain_CheckedChanged(object sender, EventArgs e) {
			if (parser.sampwin != null) {
				DrawSampwin(parser.sampwin);
			}
		}

		private void numericUpDownSmoothing_ValueChanged(object sender, EventArgs e) {
			smoothRange = (int)numericUpDownSmoothing.Value;
			if (parser.sampwin != null) {
				DrawSampwin(parser.sampwin);
			}
		}
	}
}
