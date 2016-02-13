using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace Zedgraph_Test {
	public partial class Form1 : Form {
		Random random = new Random();
		int frequence = 50;
		int window = 10;

		public Form1() {
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e) {
			double[,] sampwin = generateSampwin(frequence, window, random);
			createGraph(zedGraphControl1, 10, 10, 500, 500, "Prova", "Secondi", "Grandezza fisica");
			double[] arrayDiProva = sampwinToSingleArray(sampwin, 0);
			PointPairList populateDiProva = populate(arrayDiProva, frequence);
			LineItem gimmysCurve2 = zedGraphControl1.GraphPane.AddCurve("Culo(MB)", populateDiProva, Color.Cyan, SymbolType.None);
			arrayDiProva = smoothing(arrayDiProva, frequence * window, 10);
			populateDiProva = populate(arrayDiProva, frequence);
			LineItem gimmysCurve = zedGraphControl1.GraphPane.AddCurve("Culo(MB)", populateDiProva, Color.DarkCyan, SymbolType.None);
			zedGraphControl1.AxisChange();
			//zoom da risistemare come opzioni nel designer o qui




		}

		public double[] module(double[,] sampwin, int frequence, int window) //PRIMA OPERAZIONE: MODULO
		{
			double[] arrayModulo = new double[frequence * window];
			for (int i = 0; i < frequence * window; ++i)
				arrayModulo[i] = Math.Sqrt(Math.Pow(sampwin[0, i], 2) + Math.Pow(sampwin[1, i], 2) + Math.Pow(sampwin[2, i], 2));
			return arrayModulo;
		}

		public double[] smoothing(double[] popolazione, int size, int range) //SECONDA OPERAZIONE: SMOOTHING
		{
			//la finestra (!= da window) <= 2*range+1 
			double media = 0;
			double[] popolazione2 = new double[size];
			for (int i = 0; i < size; ++i) {
				if (i < range && (size - range) > i) //stretto a sx largo a dx
				{
					for (int i2 = 0; i2 <= i; ++i2) //stretto
						media += popolazione[i - i2];
					for (int i2 = 0; i2 <= range; ++i2) //largo 
						media += popolazione[i + i2];
					media -= popolazione[i];
					media /= (i + 1 + range);
					popolazione2[i] = media;
					media = 0;
				} else if (i >= range && size - i <= range) //stretto a dx largo a sx
				  {
					for (int i2 = 0; i2 < size - i - 1; ++i2) //stretto
						media += popolazione[i + i2];
					for (int i2 = 0; i2 <= range; ++i2) //largo 
						media += popolazione[i - i2];
					media -= popolazione[i];
					media /= (size - i + range);
					popolazione2[i] = media;
					media = 0;
				} else if (i < range && size - range <= i) //stretto a dx stretto sx
				  {
					for (int i2 = 0; i2 < size - i - 1; ++i2) //stretto
						media += popolazione[i + i2];
					for (int i2 = 0; i2 < i; ++i2) //stretto
						media += popolazione[i - i2];
					media -= popolazione[i];
					media /= size;
					popolazione2[i] = media;
					media = 0;
				} else if (i >= range && size - range > i) //stretto a dx largo a sx
				  {
					for (int i2 = 0; i2 <= range; ++i2) //largo 
						media += popolazione[i + i2];
					for (int i2 = 0; i2 <= range; ++i2) //largo 
						media += popolazione[i - i2];
					media -= popolazione[i];
					media /= (2 * range + 1);
					popolazione2[i] = media;
					media = 0;
				}
			}
			return popolazione2;
		}

		private void createGraph(ZedGraph.ZedGraphControl zedGraphControl, int drawX, int drawY, int sizeX, int sizeY, string titolo, string x, string y) {
			zedGraphControl.Location = new Point(drawX, drawY);
			zedGraphControl.Size = new Size(sizeX, sizeY);
			GraphPane myPane = zedGraphControl.GraphPane;
			myPane.Title.Text = titolo;
			myPane.XAxis.Title.Text = x;
			myPane.YAxis.Title.Text = y;
		}

		private PointPairList populate(double[] array, int frequence) //selesnia - vecchio arrayToSeries
		{
			int length = array.Length;
			PointPairList list = new PointPairList();
			for (int i = 0; i < length; ++i)
				list.Add((double)i / frequence, array[i]);
			//for (int i = 1; i <= length; ++i)
			//    list.Add((double)i / frequence, array[i-1]);
			return list;
		}

		public double[,] generateSampwin(int frequence, int window, Random random) //generazione simulata di un sampwin semplificato
		{
			int firstDimension = 13;
			double[,] sampwin = new double[firstDimension, frequence * window];
			for (int i = 0; i < firstDimension; ++i)
				sampwin[i, 0] = random.Next(-100, 100);
			for (int i = 0; i < firstDimension; ++i)
				for (int j = 1; j < frequence * window; ++j)
					sampwin[i, j] = sampwin[i, j - 1] + (random.Next(-100, 100));
			return sampwin;
		}

		public double[] sampwinToSingleArray(double[,] sampwin, int firstDimension) {
			//if 0 <= firstDimension <= 2 stiamo estraendo una delle coordinate per la simulazione di un primo generico sensore
			//if 9 <= firstDimension <= 12 stiamo estraendo uno dei quaternioni
			int dim = sampwin.GetLength(1);
			double[] singleArray = new double[dim];
			for (int i = 0; i < dim; ++i)
				singleArray[i] = sampwin[firstDimension, i];
			return singleArray;
		}

		protected override bool ProcessDialogKey(Keys keyData) //escape 
		{
			if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape) {
				this.Close();
				return true;
			}
			return base.ProcessDialogKey(keyData);

		}

		private void InitializeComponent() {
			this.SuspendLayout();
			// 
			// Form1
			// 
			this.ClientSize = new System.Drawing.Size(885, 527);
			this.Name = "Form1";
			this.ResumeLayout(false);

		}
	}
}
