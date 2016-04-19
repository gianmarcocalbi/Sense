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
	/// <summary>
	/// Partial Class Form1
	/// </summary>
	public partial class Form1 : Form {
		/// Oggetto Parser.
		private Parser parser;
		/// Thread nel quale gira il parser, o meglio la funzione che gestisce il Server.
		Thread threadParser;
		/// Frequenza di campionamento.
		int frequence = 50;
		/// Dimensione della finestra in secondi.
		int window = 10;
		/// Pannello zedgraph sul quale disegnare i grafici.
		GraphPane myPane;
		/// Indice grafico selezionato.
		int selectedChart;
		/// Indice posizione sensore selezionato.
		int selectedSensor;
		/// Indice tipo di sensore selezionato.
		int selectedSensorType;
		/// Path in cui salvare il file csv/actions_log.
		string csvPath;
		/// Sampwin salvata in locale dopo che il server viene stoppato per poter disegnare ancora i grafici relativi a quel campione.
		List<double[,]> mySampwin;
		/// Range per lo smoothing: anche raggio dell'intorno in cui guardare per fare la media per la deviazione standard.
		int smoothRange;
		/// Var riconoscimento azione - moto : inizio del moto.
		double motoStart = 0;
		/// Var riconoscimento azione - moto : inizio dello stato di fermo.
		double fermoStart = 0;
		/// Var riconoscimento azioni : tempo fine finestra precedente.
		double winTime = 0;
		/// Var riconoscimento azione - moto : nome azione in corso.
		string action = null;
		/// Var riconoscimento azione - posizione : nome posizione attuale.
		string state = null;
		/// Var riconoscimento azione - posizione : inizio stato in corso.
		double stateStart = 0;
		/// Var riconoscimento azione - girata : tipo di girata.
		string turnAction = null;
		/// Var riconoscimento azione - girata : inizio girata attuale.
		double turnStart = 0;
		/// Var riconoscimento azione - girata : tipo di girata che potrebbe essere quella attuale.
		string turnPossibleAction = null;
		/// Var riconoscimento azione - girata : possibile inizio per la girata possibile attuale.
		double turnPossibleStart = 0;
		/// Var riconoscimento azione - girata : grado minimo per essere considerato significativo.
		double degree = 10;
		/// Var riconoscimento azione - girata : ultimo angolo di riferimento.
		double refAngolo = 0;

		String segAction = null;					//!< Azione attuale rilevata nella segmentazione.
		private String segPossibleAction = null;	//!< Azione possibile che potrebbe essere in atto nell'operazione di segmentazione.
		int segStart = 0;							//!< Inizio dell'azione corrente nell'operazione di segmentazione.
		int segPossibleStart = 0;                   //!< Inizio dell'azione plausibile nell'operazione di segmentazione.

		/// Var riconoscimento azioni : stringa da stampare su file.
		string outToFileStr = "";
		/// Numero di client che si vuole connettere al server.
		int clientsAmount = 0;
		/// Array di curve di supporto contenente il path di dead reckoning di ogni client (massimo 10).
		Curve[] multiClientCurves = new Curve[10];
		/// Data iniziale di default.
		DateTime startTime = new DateTime(1900, 1, 1, 0, 0, 0, 0);
		bool printCSV;

		/// <summary>
		/// Costruttore Primario form.
		/// </summary>
		public Form1() {
			//Inizializza i componenti grafici
			InitializeComponent();
			myPane = zedGraphControl1.GraphPane;
			//Finestra
			window = (int)numericUpDownFinestra.Value;
			//Frequenza
			this.comboBoxFrequenza.SelectedIndex = comboBoxFrequenza.FindStringExact("50");
			frequence = Int32.Parse(comboBoxFrequenza.Text);
			//CSV Location
			csvPath = Directory.GetCurrentDirectory() + @"\_output";
			printCSV = checkBoxSaveCsv.Checked;
			try {
				System.IO.Directory.CreateDirectory(csvPath);
			} catch (Exception e) {
				printToServerConsoleProtected("Impossibile creare la cartella " + csvPath + "\n");
			}
			textBoxCSVPath.Text = csvPath;
			//CSV Location hint EventHandler
			this.textBoxCSVPath.MouseEnter += new System.EventHandler(this.textBoxCSVPath_Enter);
			//numericUpDownSmoothing maximum value
			numericUpDownSmoothing.Maximum = Math.Floor((decimal)(window * frequence / 2));
			smoothRange = (int)numericUpDownSmoothing.Value;
			clientsAmount = (int)numericUpDownClientsAmount.Value;
			selectedChart = comboBoxChart.SelectedIndex;
			checkBoxSegmentation.Checked = false;
			checkBoxSegmentation.Enabled = (selectedChart == 0 ? true : false);
			//Creazione Parser (Server)
			parser = new Parser(
				Int32.Parse(textBoxPort.Text),
				String.Format("{0}.{1}.{2}.{3}", textBoxIP1.Text, textBoxIP2.Text, textBoxIP3.Text, textBoxIP4.Text),
				csvPath,
				printCSV,
				frequence,
				window,
				clientsAmount,
				printToServerConsoleProtected,
				setButtonServerStartProtected,
				eatSampwinProtected
			);

			//I controlli su selectedChart, selectedSensorType, selectedSensor devono essere fatti dopo aver istanziato il parser perché chiamano una funzione di parser.
			//Altrimenti ci sarebbe Eccezione del tipo "riferimento a null".
			//selectedSensor
			comboBoxNumSensore.SelectedIndex = comboBoxNumSensore.FindStringExact("1 (Bacino)");
			selectedSensor = comboBoxNumSensore.SelectedIndex;
			//selectedSensorType
			comboBoxTipoSensore.SelectedIndex = comboBoxTipoSensore.FindStringExact("Acc");
			selectedSensorType = comboBoxTipoSensore.SelectedIndex;
			//selectedChart
			comboBoxChart.SelectedIndex = comboBoxChart.FindStringExact("Modulo");
			selectedChart = comboBoxChart.SelectedIndex;
			//Server thread
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
				//Se il server è attivo allora lo STOPpiamo
				parser.DeactivateServer();
				parser.sampwin = null;
				if (clientsAmount > 1) {
					mySampwin = null; //Previene la stampa indesiderata di altri grafici una volta stoppato il server.
				}
				multiClientCurves = null;
				zedGraphControl1.GraphPane.CurveList.Clear();
				zedGraphControl1.Invalidate();
				zedGraphControl1.GraphPane.Title.Text = "Chart";
				zedGraphControl1.GraphPane.XAxis.Title.Text = "x";
				zedGraphControl1.GraphPane.YAxis.Title.Text = "y";
				zedGraphControl1.AxisChange();
			} else {
				//Se il server è fermo allora lo STARTiamo
				frequence = Int32.Parse(comboBoxFrequenza.Text);
				numericUpDownSmoothing.Maximum = Math.Floor((decimal)(window * frequence / 2));
				try {
					parser.ActivateServer(
						Int32.Parse(textBoxPort.Text),
						String.Format("{0}.{1}.{2}.{3}", textBoxIP1.Text, textBoxIP2.Text, textBoxIP3.Text, textBoxIP4.Text),
						csvPath,
						printCSV,
						frequence,
						window,
						clientsAmount
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
		private void Form1_Load(object sender, EventArgs e) {
			this.Text = "Sense";
			this.Opacity = 1;
			this.CenterToScreen();
		}

		/// <summary>
		/// Estrae il valori selezionati dalla sampwin.
		/// </summary>
		/// <param name="sampwin">Sampwin campione.</param>
		/// <param name="selSensor">Posizione sensore da estrarre.</param>
		/// <param name="selSensorType">Tipo sensore da estrarre.</param>
		/// <param name="xyz">Dimensione da estrarre tra x,y e z.</param>
		/// <returns>Array di valori estratti dalla Sampwin.</returns>
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

		/// <summary>
		/// Estrae il valori selezionati dalla sampwin tenendo come sensori quelli selezionati dall'utente.
		/// </summary>
		/// <param name="sampwin">Sampwin campione.</param>
		/// <param name="xyz">Dimensione da estrarre tra x,y e z.</param>
		/// <returns>Array di valori estratti dalla Sampwin.</returns>
		public double[] extractDimension(List<double[,]> sampwin, char xyz) {
			return extractDimension(sampwin, selectedSensor, selectedSensorType, xyz);
		}

		/// <summary>
		/// Operazione per il calcolo del modulo tenendo conto dei sensori selezionati dall'utente.
		/// </summary>
		/// <param name="sampwin">Sampwin campione.</param>
		/// <returns>Array di valori del modulo.</returns>
		public double[] module(List<double[,]> sampwin)    //PRIMA OPERAZIONE: MODULO
		{
			return module(sampwin, selectedSensor, selectedSensorType);
		}

		/// <summary>
		/// Overload Modulo che consente impostazione manuale del sensore e tipo di sensore selezionati.
		/// </summary>
		/// <param name="sampwin">Sampwin campione.</param>
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
		/// Operazione di Smoothing dei valori di una certa popolazione.
		/// La media viene calcolata nell'intorno di raggio = range.
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
		/// Operazione per il calcolo della Derivata di un certo campione.
		/// </summary>
		/// <param name="sampwin">Sampwin campione.</param>
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
		/// Operazione per calcolare la Deviazione Standard di una certa popolazione di valori.
		/// Come media viene usato lo smoothing.
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

		/// <summary>
		/// Operazione per calcolare gli angoli di Eulero.
		/// </summary>
		/// <param name="sampwin">Sampwin campione.</param>
		/// <param name="selSensor">Sensore selezionato di cui calcolare gli angoli.</param>
		/// <returns>Matrice avente ad ogni colonna i 3 angoli di inclinazione.</returns>
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
		/// Operazione per il calcolo degli Angoli di Eulero overload che tiene come sensori quelli già selezionati dall'utente.
		/// </summary>
		/// <param name="sampwin">Sampwin campione.</param>
		/// <returns>Matrice avente ad ogni colonna i 3 angoli di inclinazione.</returns>
		public double[,] angoliDiEulero(List<double[,]> sampwin)                //QUINTA OPERAZIONE: ANGOLI DI EULERO
		{
			return angoliDiEulero(sampwin, selectedSensor);
		}

		/// <summary>
		/// Operazione per l'eliminazione delle discontinuità dalla funzione degli angoli di Eulero.
		/// </summary>
		/// <param name="sampwin">Sampwin campione.</param>
		/// <param name="selSensor">Sensore selezionato.</param>
		/// <returns>Matrice avente ad ogni colonna i 3 angoli di inclinazione con discontinuità eliminate.</returns>
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
		/// Operazione per il calcolo di arcotangente(magnY/magnZ) che esprime l'angolo rispetto al polo magnetico.
		/// </summary>
		/// <param name="sampwin">Sampwin campione.</param>
		/// <returns>Array contenente i valori in radianti.</returns>
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
		/// <param name="sampwin">Sampwin campione.</param>
		/// <returns>Array contenente i valori in radianti senza discontinuità.</returns>
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

		/// <summary>
		/// Operazione per calcolare il path di un soggetto nel piano ortogonale alla superficie terrestre.
		/// </summary>
		/// <param name="sampwin">Sampwin campione.</param>
		/// <returns>Lista di punti che compongono il path da disegnare.</returns>
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
			//double acc1;
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

		/// <summary>
		/// Click esc to exit.
		/// </summary>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessDialogKey(Keys keyData) //escape 
		{
			if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape) {
				this.Close();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}

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
					//Disabilita input server quando server attivo
					textBoxPort.Enabled = false;
					textBoxIP1.Enabled = false;
					textBoxIP2.Enabled = false;
					textBoxIP3.Enabled = false;
					textBoxIP4.Enabled = false;
					comboBoxFrequenza.Enabled = false;
					numericUpDownFinestra.Enabled = false;
					numericUpDownClientsAmount.Enabled = false;
					textBoxCSVPath.Enabled = false;
					buttonSelectFolder.Enabled = false;
					checkBoxSaveCsv.Enabled = false;
					buttonServerStart.Text = "STOP";
					//Se il server è multiclient allora.
					if (clientsAmount > 1) {
						//Disabilito tutti i tipi di input del chart.
						comboBoxChart.Enabled = false;
						comboBoxTipoSensore.Enabled = false;
						comboBoxNumSensore.Enabled = false;
						checkBoxPlotDomain.Enabled = false;
						checkBoxSegmentation.Enabled = false;
						checkBoxSegmentation.Checked = false;
						checkBoxSmoothing.Enabled = false;
						checkBoxSmoothing.Checked = false;
						numericUpDownSmoothing.Enabled = false;
						//Imposto come chart di stampa il dead reckoning.
						comboBoxChart.SelectedItem = "Dead Reckoning";
					}
				} else {
					//Riabilita input server quando server inattivo.
					textBoxPort.Enabled = true;
					textBoxIP1.Enabled = true;
					textBoxIP2.Enabled = true;
					textBoxIP3.Enabled = true;
					textBoxIP4.Enabled = true;
					comboBoxFrequenza.Enabled = true;
					numericUpDownFinestra.Enabled = true;
					numericUpDownClientsAmount.Enabled = true;
					textBoxCSVPath.Enabled = true;
					buttonSelectFolder.Enabled = true;
					checkBoxSaveCsv.Enabled = true;
					buttonServerStart.Text = "START";
					//Se il server è multiclient allora..
					if (clientsAmount > 1) {
						//Riabilito tutti i tipi di input del chart disabilitati in precedenza.
						comboBoxChart.Enabled = true;
						comboBoxTipoSensore.Enabled = true;
						comboBoxNumSensore.Enabled = true;
						checkBoxPlotDomain.Enabled = true;
						checkBoxSmoothing.Enabled = true;
						checkBoxSmoothing.Checked = true;
						numericUpDownSmoothing.Enabled = true;
						checkBoxSegmentation.Enabled = (selectedChart == 0 ? true : false);
						comboBoxChart.SelectedIndex = 0;
					}
				}
			}
		}

		/// <summary>
		/// Delegato per plottare la sampwin.
		/// </summary>
		/// <param name="sampwin">Sampwin.</param>
		/// <param name="client_index">Indice client.</param>
		public delegate void eatSampwinDelegate(List<double[,]> sampwin, int client_index);

		/// <summary>
		/// Operazione per il plot e l'analisi della sampwin.
		/// </summary>
		/// <param name="sampwin">Sampwin campione.</param>
		/// <param name="client_index">Indice client che chiama la funzione.</param>
		public void eatSampwinProtected(List<double[,]> sampwin, int client_index) {
			if (this.zedGraphControl1.InvokeRequired) {
				Invoke(new eatSampwinDelegate(eatSampwinProtected), new object[] { sampwin, client_index });
			} else {
				//Quando il server ha finito di leggere la sampwin ce ne salviamo una copia il locale.
				if (clientsAmount == 1) {
					if (parser.SampwinIsFullIdle) {
						mySampwin = sampwin;
					}
					DrawSampwin(sampwin);
					ParseActions(sampwin);
				} else {
					//clients_amount > 1
					DrawSampwinMultiClient(sampwin, client_index);
				}
			}
		}

		//Delegate functions END

		/// <summary>
		/// Operazione che plotta la sampwin su Zedgraph.
		/// </summary>
		/// <param name="sampwin">Sampwin campione.</param>
		public void DrawSampwin(List<double[,]> sampwin) {

			List<Curve> myCurveList = new List<Curve>();
			List<LineItem> myLineList = new List<LineItem>();
			myPane.CurveList.Clear();
			zedGraphControl1.Invalidate();
			myPane.Title.Text = "Chart";
			myPane.XAxis.Title.Text = "time (seconds)";

			switch (selectedSensorType) {
				case 0:
					//acc
					myPane.YAxis.Title.Text = "m/s²";
					break;
				case 1:
					//gyr
					myPane.YAxis.Title.Text = "Rad/s²";
					break;
				case 2:
					//mag
					myPane.YAxis.Title.Text = "Tesla";
					break;
				case 3:
					//qua
					myPane.YAxis.Title.Text = "y";
					break;
				default:
					//bohh
					myPane.YAxis.Title.Text = "none";
					break;
			}

			switch (selectedChart) {
				case 0:
					//Modulo
					myCurveList.Add(new Curve("Module", module(sampwin), Color.Blue));
					myPane.Title.Text = "Module";
					break;
				case 1:
					//Derivata
					myCurveList.Add(new Curve("Derivate", rapportoIncrementale(sampwin), Color.Blue));
					myPane.Title.Text = "Derivate";
					myPane.YAxis.Title.Text = "";
					break;
				case 2:
					//Angoli di eulero
					int length = sampwin.Count();
					double[,] instant = angoliDiEuleroContinua(sampwin, selectedSensor);
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
					//Deviazione standard
					myCurveList.Add(new Curve("Standard Deviation", deviazioneStandard(module(sampwin), smoothRange), Color.Blue));
					myPane.Title.Text = "Standard Deviation";
					break;
				case 4:
					//arcotangente(magnY/magnZ)
					myCurveList.Add(new Curve("arcotangente(magnY/magnZ)", arctanMyMzContinua(sampwin), Color.Blue));
					myPane.Title.Text = "arcotangente(magnY/magnZ)";
					myPane.YAxis.Title.Text = "Rad";
					break;
				case 5:
					//Dead Reckoning
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
					break;
				default:
					break;
			}

			if (checkBoxSegmentation.Checked && checkBoxSegmentation.Enabled) {
				double[] valore = module(sampwin);
				
				double max = 0;
				for (int i = 0; i < valore.Length; i++) {
					if (valore[i] > max)
						max = valore[i];
				}

				double standardValue = 10;
				double soglia1 = 0.3;
				int soglia2 = 30;

				switch (selectedSensorType) {
					case 0:
						standardValue = 9.81;
						soglia1 = max / 100;
						soglia2 = 5;
						break;
					case 1:
						standardValue = 9.81;
						soglia1 = 0.3;
						soglia2 = 21;
						break;
					case 2:
						standardValue = 9.81;
						soglia1 = 0.3;
						soglia2 = 21;
						break;
					default:
						break;
				}


				for (int i = 0; i < sampwin.Count(); i++) {
					if (Math.Abs(valore[i] - standardValue) > soglia1) {
						if (segAction != "Action" || segPossibleAction != "Action") {
							if (segPossibleAction != "Action") {
								segPossibleStart = i;
								segPossibleAction = "Action";
							} else if (i - segPossibleStart > soglia2) {
								if (segAction != null) {
									//stampa calm

									PointPairList tempSegPPL1 = new PointPairList();
									for (int j = segStart; j < segPossibleStart - 1; ++j) {
										tempSegPPL1.Add((double)j / frequence, valore[j]);
									}
									myPane.AddCurve(null, tempSegPPL1, (segAction == "Calm") ? Color.Blue : Color.Red, SymbolType.None);

								}
								//aggiungi qualcosa puo darsi che sia una curva
								//ovviamente qui avra' il colore BLUE perchè stiamo considerando la FINE della parte senza i picchi e senza LABEL (ne sono abbastanza sicuro)
								//l'inserimento si appoggia ai precedenti valori di 
								segAction = "Action";
								segStart = segPossibleStart;
							}
						}
					} else {
						if (segAction != "Calm" || segPossibleAction != "Calm") {
							if (segPossibleAction != "Calm") {
								segPossibleStart = i;
								segPossibleAction = "Calm";
							} else if (i - segPossibleStart > soglia2) {
								//stampa action
								if (segAction != null) {

									PointPairList tempSegPPL2 = new PointPairList();
									for (int j = segStart; j < segPossibleStart - 1; ++j) {
										tempSegPPL2.Add((double)j / frequence, valore[j]);
									}
									myPane.AddCurve(null, tempSegPPL2, (segAction == "Calm") ? Color.Blue : Color.Red, SymbolType.None);

								}
								//aggiungi qualcosa puo darsi che sia una curva
								//ovviamente qui avra' il colore BLUE perchè stiamo considerando la FINE della parte senza i picchi e senza LABEL (ne sono abbastanza sicuro)
								//l'inserimento si appoggia ai precedenti valori di 
								segAction = "Calm";
								segStart = segPossibleStart; 
							}
						}
					}
				}
				PointPairList tempSegPPL = new PointPairList();
				for (int j = segStart; j < segPossibleStart - 1; ++j) {
					tempSegPPL.Add((double)j / frequence, valore[j]);
				}
				myPane.AddCurve(null, tempSegPPL, (segAction == "Calm") ? Color.Blue : Color.Red, SymbolType.None);
				segPossibleStart = 0;
				segStart = 0;
				segAction = null;
				segPossibleAction = null;
			} else if (checkBoxSmoothing.Checked) {
				foreach (Curve c in myCurveList) {
					c.PointsValue = smoothing(c.PointsValue, smoothRange);
					c.Label += " smoothed";
				}
			}

			foreach (Curve c in myCurveList) {
				PointPairList ppl = new PointPairList();
				if (c.PointsValue != null) {
					if (checkBoxPlotDomain.Checked) {
						ppl = populate(c.PointsValue, c.PointsValue.Length - window * frequence, c.PointsValue.Length);
					} else {
						ppl = populate(c.PointsValue);
					}
				} else {
					ppl = c.PointsValueList;
				}
				LineItem myLine = myPane.AddCurve(c.Label, ppl, c.Color, c.SymbolType);
				myLineList.Add(myLine);
			}

			zedGraphControl1.AxisChange();
			zedGraphControl1.Refresh();
		}

		/// <summary>
		/// Operazione per il riconoscimento delle azioni compiute dal soggetto.
		/// </summary>
		/// <param name="sampwin">Samwpin campione.</param>
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

			//MOTO-STAZIONAMENTO
			if (startTime.Year == 1900) {
				startTime = DateTime.Now;
			}

			//Deviazione Standard modulo accelerometro
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
					//Moto-stazionamento
					if (stDevArray[i] < 0.01) {
						//possibile moto stazionario
						//finisce il moto
						if (action == "non-fermo") {
							//Fine del moto.
							//Stampa l'azione di moto appena terminata.
							outToFileStr += tempTime.AddSeconds(motoStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time).ToString("HH:mm:ss") + " non-fermo\n";
							//printToServerConsoleProtected(motoStart + " " + motoEnd + " non-fermo\n");
							//save start point moto stazionario
							fermoStart = time; //L'inizio del moto-stazionario coincide con la fine del moto.
						}
						//Viene impostata l'azione attuale. 
						action = "fermo";
					} else {
						//possibile moto
						//finisce il moto stazionario
						if (action == "fermo") {
							//il non moto è finito, mi salvo i dati che devo salvare
							//save end point non moto
							outToFileStr += tempTime.AddSeconds(fermoStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time).ToString("HH:mm:ss") + " fermo\n";
							//printToServerConsoleProtected(fermoStart + " " + fermoEnd + " fermo\n");
							//save start point moto stazionario
							motoStart = time;
						}
						action = "non-fermo";
					}

					//Lay-Stand-Sit
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

					//Girata
					if (Math.Abs(thetaMagnArray[i] - refAngolo) < 0.15) { // 10 gradi = 0.15 radianti
						if (turnAction != "prosegue") {
							if (turnPossibleAction != "prosegue") {
								turnPossibleStart = time;
								turnPossibleAction = "prosegue";
							}
							if (time - turnPossibleStart > 0.3) {
								if (turnAction != null)
									outToFileStr += tempTime.AddSeconds(turnStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time - 0.3).ToString("HH:mm:ss") + " " + turnAction + "\n";
								turnAction = turnPossibleAction;
								turnStart = time - 0.3;
								//turnStart = turnPossibleStart;
							}
						}
					} else {
						//segue if per determinare direzione della svolta
						if (thetaMagnArray[i] - refAngolo < 0) {
							//DX
							if (turnAction != "girata dx") {
								if (turnPossibleAction != "girata dx") {
									turnPossibleStart = time;
									turnPossibleAction = "girata dx";
								} else if (time - turnPossibleStart > 0.3) {
									if (turnAction != null)
										outToFileStr += tempTime.AddSeconds(turnStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time - 0.3).ToString("HH:mm:ss") + " " + turnAction + "\n";
									turnAction = turnPossibleAction;
									turnStart = time - 0.3;
									//turnStart = turnPossibleStart;
								}
							}
						} else {
							//SX
							if (turnAction != "girata sx") {
								if (turnPossibleAction != "girata sx") {
									turnPossibleStart = time;
									turnPossibleAction = "girata sx";
								} else if (time - turnPossibleStart > 0.3) {
									if (turnAction != null)
										outToFileStr += tempTime.AddSeconds(turnStart).ToString("HH:mm:ss") + " " + tempTime.AddSeconds(time - 0.3).ToString("HH:mm:ss") + " " + turnAction + "\n";
									turnAction = turnPossibleAction;
									turnStart = time - 0.3;
									//turnStart = turnPossibleStart;
								}
							}
						}
						//variazione avvenuta
						refAngolo = thetaMagnArray[i];
					}
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

		/// <summary>
		/// Operazione per il plotting del dead reckoning multi client.
		/// </summary>
		/// <param name="sampwin">Sampwin campione.</param>
		/// <param name="client_index">Indice del client che disegna il proprio path.</param>
		public void DrawSampwinMultiClient(List<double[,]> sampwin, int client_index) {
			myPane.CurveList.Clear();
			zedGraphControl1.Invalidate();
			myPane.Title.Text = "Dead Reckoning Multi Client";
			myPane.YAxis.Title.Text = "m";
			myPane.XAxis.Title.Text = "m";
			Curve tmpLine1 = new Curve("Path subject #" + client_index, computeDeadReckoning(sampwin), Color.BlueViolet, SymbolType.None);
			multiClientCurves[client_index] = tmpLine1;
			Color[] colors = { Color.Blue, Color.Green, Color.Orange, Color.LightCyan, Color.Magenta, Color.Salmon, Color.Brown, Color.Beige, Color.LavenderBlush, Color.LightGoldenrodYellow };
			for (int i = 0; i < clientsAmount; i++) {
				if (multiClientCurves[i] != null) {
					myPane.AddCurve(multiClientCurves[i].Label, multiClientCurves[i].PointsValueList, colors[i], multiClientCurves[i].SymbolType);
					PointPairList tempPPL = multiClientCurves[i].PointsValueList;
					PointPairList templist1 = new PointPairList();
					PointPairList templist2 = new PointPairList();
					templist1.Add(tempPPL.First().X, tempPPL.First().Y);
					LineItem tmpLine2 = myPane.AddCurve("Start #" + i, templist1, colors[i], SymbolType.Triangle);
					templist2.Add(tempPPL.Last().X, tempPPL.Last().Y);
					LineItem tmpLine3 = myPane.AddCurve("End #" + i, templist2, colors[i], SymbolType.Circle);
				}
			}

			zedGraphControl1.AxisChange();
			zedGraphControl1.Refresh();
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
			checkBoxSegmentation.Enabled = (selectedChart == 0 ? true : false);
			checkBoxSegmentation.Checked = false;
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
			if (checkBoxSmoothing.Checked == true) {
				checkBoxSegmentation.Checked = false;
			}
		}

		private void checkBoxSegmentation_CheckedChanged(object sender, EventArgs e) {
			if (parser.sampwin != null && parser.sampwin.Count > 0) {
				try {
					DrawSampwin(parser.sampwin);
				} catch (Exception ex) { }
			}
			if (checkBoxSegmentation.Checked == true) {
				checkBoxSmoothing.Checked = false;
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

		private void numericUpDownClientsAmount_ValueChanged(object sender, EventArgs e) {
			clientsAmount = (int)numericUpDownClientsAmount.Value;
		}

		private void checkBoxSaveCsv_CheckedChanged(object sender, EventArgs e) {
			printCSV = checkBoxSaveCsv.Checked;
		}
	}
}
