using System;
using System.Drawing;
using System.Windows.Forms; //using specifico per questo tipo di layout
using System.Threading; //sarà probabilmente necessario, non credo implementerò delle task
using Graph = System.Windows.Forms.DataVisualization.Charting; //è un modo per semplificare l'uso del namespace?


//FORSE STO COMMENTANDO UN PO' TROPPO MA MI SERVE ANCHE DA STUDIO E POI IL CODICE CHE CONTA C'E'
//TUTTI I COMMENTI LI POSSIAMO TOGLIERE UNA VOLTA CHE ABBIAMO STUDIATO ASSIEME LE PARTI CHE NON SIAMO STATI NOI A SCRIVERE

namespace FormProgetto {
	public partial class Form1 : Form {

		Chart chartprova = new Chart();
		Graph.Chart chart1;
		int frequence = 5;
		int window = 10;

		public Form1() //si tratta del costruttore del form, comune a tutte i windows form
		{
			InitializeComponent();
		}

		public void genericThreadCreation() //questo è un metodo che ho fatto io, se lo vorrò chiamare nel main farò form1.genericThreadCreation ecc
		{
			//se possibile passeremo ai pool di thread
			//Thread t = new Thread(riceviInput); //<prende un delegato, funzione
			//posso passargli una funzione che legga i dati dalla console app ecc 
			//t.Start;
			//Thread.Sleep(1000);

		}

		public void riceviInput() {
			Console.WriteLine("Supponiamo stia facendo qualcosa di figo");
			//qui ci potrebbe essere un delegato o altre cose bah
		}

		public int fakeInputGenerator() //qui a scopo di test per le fasi iniziali
		{
			Thread.Sleep(2000);
			return 5;
			//giusto per simulare un return, potrei mettere un random e invocare il metodo piu volte o lavorare con un array
		}

		//private void Button1_Click di sicuro lo utilizzerò a breve, 

		private void Form1_Load(object sender, EventArgs e) //load event handler
		{
			// You can set properties in the Load event handler.
			//link alle proprietà della form https://msdn.microsoft.com/en-us/library/system.windows.forms.form_properties%28v=vs.110%29.aspx
			//qui dentro è dove si consiglia di mettere la maggior parte del codice con cui inizializziamo qualsiasi cosa
			//gira prima che venga mostrata a schermo la form
			//i suoi fratelli sono FormClosing, FormClosed, non sono sicuro della sintassi

			//se utilizzao il tutto in un metodo non quale la Load posso chiaramente inizializzare via costruttore e poi passare tutto come form1.method(), classico
			this.Text = "GimmyDoesIt4All";
			this.Opacity = .85; //assolutamente inutile, ma in se l'istruzione mi piaceva, magari riesco a fare i grafici meno trasparenti
			this.Size = new Size(1280, 720); //non può essere utilizzato come una normale chiamata a metodo this.Size(x,y), verificato con errore a compilazione
			this.CenterToScreen(); //se lo scrivo prima del size non viene centrato, veniva centrata solo la versione con la dimensione di default

			// Create new Graph
			//nb chart viene inizializzato fuori da tutti questi vari metodi, proprio all'inizio della classe, se lo si vuole modificare sta la
			chart1 = new Graph.Chart(); //chart forse è un nome un po' troppo banale, provvedere a migliorare in seguito
			chart1.Location = new System.Drawing.Point(10, 10); //posizione 
			chart1.Size = new System.Drawing.Size(500, 500);


			int[] saveThePopulation = generateSimulatedData(frequence, window);

			chart1.ChartAreas.Add("NewChartArea");
			//chart1.ChartAreas.Position(10,10);

			chart1.Series.Add("PrimoSensore"); //https://msdn.microsoft.com/it-it/library/dd456769.aspx per tracciare piu serie in un unica area grafico, forse lo renderemo opzionale
											   //ho rilevato un problema, ovvero con questo tipo di x, continua a scrivere tutti i valori finchè non ne ha per fare un numero intero quindi nel primo tentaivo 50
			chart1.Series["PrimoSensore"].ChartType = SeriesChartType.Line;
			for (int i = 0; i < frequence * window; ++i) {
				chart1.Series["PrimoSensore"].Points.AddXY(i / frequence, saveThePopulation[i]); //cosi dovrei avere efficacemente popolato le data series, ora meglio disegnarle + smoothing
			}
			//si tratta di una selezione di una porzione del grafico, mi pare una feature interessante, implementare anche lato 
			chart1.ChartAreas["NewChartArea"].CursorX.IsUserEnabled = true;
			chart1.ChartAreas["NewChartArea"].CursorX.IsUserSelectionEnabled = true;
			chart1.ChartAreas["NewChartArea"].AxisX.ScaleView.Zoomable = true;
			chart1.ChartAreas["NewChartArea"].CursorX.Interval = 0.1;
			//chart.palette al più presto
			//ChartArea chartAreaPrimoSensore = new ChartArea();
			chart1.Series["PrimoSensore"].ChartArea = "NewChartArea";


			//non sarebbe affatto male mettere in luce un asse verticale ogni 100 valori per dire, da rivedere appena funziona il resto
			//chart1.ChartAreas[0].AxisY.StripLines.Add(new StripLine());
			//chart1.ChartAreas[0].AxisY.StripLines[0].BackColor = Color.FromArgb(80, 252, 180, 65);
			//chart1.ChartAreas[0].AxisY.StripLines[0].StripWidth = 40;
			//chart1.ChartAreas[0].AxisY.StripLines[0].Interval = 10000;
			//chart1.ChartAreas[0].AxisY.StripLines[0].IntervalOffset = 20;

			//interessante per quando potrebbe essere necessario disegnare piu grafici in una stessa area
			//http://stackoverflow.com/questions/15423080/how-to-modify-c-sharp-chart-control-chartarea-percentages
			//chart1.ChartAreas[0].Position.Y = 10;
			//chart1.ChartAreas[0].Position.Height = 60;
			//chart1.ChartAreas[1].Position.Y = 70; / chart1.ChartAreas[1].Position.Y = chart1.ChartAreas[0].Position.Bottom; la seconda mi sembra molto interessante
			//chart1.ChartAreas[1].Position.Height = 20;

			Controls.Add(this.chart1); //finchè non scrivo questo non compare assolutamente nulla, quindi va il più in fondo possibili

			//double yValue = 50.0;
			//Random random = new Random();
			//for (int pointIndex = 0; pointIndex < 20000; pointIndex++)
			//{
			//    yValue = yValue + (random.NextDouble() * 10.0 - 5.0);
			//    //chart1.Series["Default"].Points.AddY(yValue); //dovrebbe servirmi a riempi le data series ma ancora una che funzioni non l'ho messa su
			//}

			//// Data arrays.
			//string[] seriesArray = { "Cats", "Dogs" };
			//int[] pointsArray = { 1, 2 };

			//// Set palette.
			//this.chart1.Palette = ChartColorPalette.SeaGreen;

			//// Set title.
			//this.chart1.Titles.Add("Pets");

			//// Add series.
			//for (int i = 0; i < seriesArray.Length; i++)
			//{
			//    // Add series.
			//    Series series = this.chart1.Series.Add(seriesArray[i]);

			//    // Add point.
			//    series.Points.Add(pointsArray[i]);
			//}

			//tutta sta parte avendo poi cura dei this ecc potrà essere tranquillamente spostata in metodi accessori,
			//per ora la ragione per cui faccio tutto qui ed ignoro il main è la comodità di stare in un unica finestra
			//comunque sia 

			// Display the form in the center of the screen.


			// Display the form as a modal dialog box.

			//il codice qui di seguito l'ho commentato in gruppo così: Keyboard:  CTRL + K, CTRL + C (comment); CTRL + K, CTRL + U (uncomment)
			//qui di seguito un esempio per capire cosa può rappresentare il sender, di fatto è
			//Button someButton = sender as Button;
			//    if(someButton != null
			//    {
			//        someButton.Text = "I was clicked!";
			//    }
			//EventArgs sono argomenti utili alla tipologia di evento, Form1_Load (quindi Load) può non averne bisogno, closing ho visto ne fa uso

			// You can set properties in the Load event handler.
			this.Text = "GimmyCuloDoIt4All";
		}

		//da eliminare poi una volta a disposizione i veri dati funzionanti, per ora va benissimo
		public int[] generateSimulatedData(int frequence, int window) {
			int size = frequence * window;
			int[] array = new int[size];
			Random random = new Random();
			if (size > 1) {
				array[0] = random.Next(-100, 100);
				for (int i = 1; i < size; ++i) {
					array[i] = array[i - 1] + (random.Next(-100, 100));
				}

			}
			return array;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
			// You can cancel the Form from closing.
			// Un esempio di cosa si può fare qui dentro, se non lo popolo non da comunque problemi
			//if ((DateTime.Now.Minute % 2) == 1)
			//{
			//    this.Text = "Can't close on odd minute";
			//    e.Cancel = true;
			//}
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
			// You can write to a file to store settings here.
		}

		//mi scocciava ricevere il messaggio di errore quando scrivevo codice che c'era il programma ancora aperto, quindi con escape faccio in un attivo a chiuderlo
		//un pelo complesso, io sarei stato in grado di scrivere solo this.Close ecc, l'ho trovato online, funziona bene
		protected override bool ProcessDialogKey(Keys keyData) {
			if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape) {
				this.Close();
				return true;
			}
			return base.ProcessDialogKey(keyData);

		}
	}
}
