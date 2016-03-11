using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace unaltrotentativo
{
    public partial class Form1 : Form
    {
        Random random = new Random();
        int frequence = 50;
        int window = 10;
        bool isAngoliDiEuleroPressed = false;
        bool isRollPressed = false;
        bool isPitchPressed = false;
        bool isYawPressed = false;
        bool isSmoothPressed = false;
        GraphPane myPane;
        double[,] sampwin;
        LineItem rollLine;
        LineItem rollSmooth;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //SAMPWIN ARRAY TRIDIMENSIONALE, SCRITTO CHIARAMENTE NELLA CONSEGNA, LA POSSIAMO SCRIVERE COME double[, ,] ANZICHE double[][][] SCRITTURA VAGAMENTE PIU BARBARICA
            //SI FA RIFERIMENTO A DUE SAMPWIN UNA CON LE INIZIALI MAIUSCOLE TRIDIMENSIONALE ED UNA TUTTA IN MINUSCOLO CON 

            this.Text = "GimmyCuloDoIt4All";
            this.Opacity = 1; //assolutamente inutile, ma in se l'istruzione mi piaceva, magari riesco a fare i grafici meno trasparenti
            this.Size = new Size(1280, 960); //non può essere utilizzato come una normale chiamata a metodo this.Size(x,y), verificato con errore a compilazione
            this.CenterToScreen();


            sampwin = generateSampwin();
            
            double[] arrayDiProva = multiToSingleArray(sampwin, 0);
            double[] arrayDiProva2 = smoothing(arrayDiProva, 10);
            
            //zoom da risistemare come opzioni nel designer o qui

            double[] rI = rapportoIncrementale(sampwin, 0);
            double[] sampwinSingleDim = multiToSingleArray(sampwin, 0);
            double[] dS = deviazioneStandard(sampwinSingleDim, 3);
            LineItem rILine = zedGraphControl1.GraphPane.AddCurve("SampwinSingleDim", populate(sampwinSingleDim), Color.Cyan, SymbolType.None);
            LineItem dSLine = zedGraphControl1.GraphPane.AddCurve("DS", populate(dS), Color.DarkCyan, SymbolType.None);
            //verificare con gimmy che effettivamente la divisione con la frequenza sia la cosa migliore da fare, fare ovviamente test con valori adatti può cambiare tutto
            zedGraphControl1.AxisChange();            
        }

        public double[] module(double[,] sampwin) //PRIMA OPERAZIONE: MODULO
        {
            double[] arrayModulo = new double[frequence * window];
            for (int i = 0; i < frequence * window; ++i)
                arrayModulo[i] = Math.Sqrt(Math.Pow(sampwin[0, i], 2) + Math.Pow(sampwin[1, i], 2) + Math.Pow(sampwin[2, i], 2));
            return arrayModulo;
        }
        //
        public double[] smoothing2(double[] popolazione, int range) //SECONDA OPERAZIONE: SMOOTHING
        {
            //la finestra (!= da window) <= 2*range+1 
            double media = 0;
            int size = popolazione.GetLength(0); //MODIFICATA RELATIVAMENTE DI RECENTE, VERIFICARE CHE VADA BENE soprattutto il paramatro passato 0
            double[] popolazione2 = new double[size];
            for (int i = 0; i < size; ++i)
            {
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
                }
                else if (i >= range && size - i <= range) //stretto a dx largo a sx
                {
                    for (int i2 = 0; i2 < size - i - 1; ++i2) //stretto
                        media += popolazione[i + i2];
                    for (int i2 = 0; i2 <= range; ++i2) //largo 
                        media += popolazione[i - i2];
                    media -= popolazione[i];
                    media /= (size - i + range);
                    popolazione2[i] = media;
                    media = 0;
                }
                else if (i < range && size - range <= i) //stretto a dx stretto sx
                {
                    for (int i2 = 0; i2 < size - i - 1; ++i2) //stretto
                        media += popolazione[i + i2];
                    for (int i2 = 0; i2 < i; ++i2) //stretto
                        media += popolazione[i - i2];
                    media -= popolazione[i];
                    media /= size;
                    popolazione2[i] = media;
                    media = 0;
                }
                else if (i >= range && size - range > i) //stretto a dx largo a sx
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

        public double[] rapportoIncrementale(double[,] sampwin, int firstDimension)//TERZA OPERAZIONE: DERIVATA
        {
            int dim = sampwin.GetLength(1);
            double[] rapportoIncrementale = new double[dim];
            for (int i = 0; i < dim -1; i++) //ci vorra di sicuro dim-1
            {
                rapportoIncrementale[i] = (sampwin[firstDimension,i+1]-sampwin[firstDimension,i])/((double)1/frequence);
            }
                return rapportoIncrementale;
        }
        
        //notare quale delle due sia effettivamente la migliore, questa seconda rende piu facile la lettura e la manutenzione
        public double[] smoothing(double[] popolazione, int range)//SECONDA OPERAZIONE: SMOOTHING
        {
            int size = popolazione.GetLength(0);
            double[] smooth = new double[size];
            int finestra = 0, dx = 0, sx = 0;
            double media = 0;
            for (int i = 0; i < size; ++i)
            {
                if (i < range) { sx = i; }
                else sx = range;
                if (size - range > i) { dx = range; }
                else dx = size - i - 1;
                finestra = dx + sx + 1;
                for (int j = i - sx; j <= i + dx; ++j)
                    media += popolazione[j];
                media /= finestra;
                smooth[i] = media;
                finestra = 0;
                dx = 0;
                sx = 0;
            }
            return smooth;
        }

        public double[] deviazioneStandard(double[] popolazione, int range)//QUARTA OPERAZIONE: DEVIAZIONE STANDARD
        {
            double[] smooth = smoothing(popolazione, range);
            int size = popolazione.GetLength(0);
            double[] deviazioneStandard = new double[size];
            int finestra = 0, dx = 0, sx = 0;
            for (int i = 0; i < size; ++i)
            {
                if (i < range) { sx = i; }
                else sx = range;
                if (size - range > i) { dx = range; }
                else dx = size - i - 1;
                finestra = dx + sx + 1;
                deviazioneStandard[i] = Math.Sqrt(Math.Pow((popolazione[i] - smooth[i]), 2)/(finestra));
                finestra = 0;
                dx = 0;
                sx = 0;
            }
                return deviazioneStandard;
        }

        public double[,] angoliDiEulero(double[,] sampwin) //QUINTA OPERAZIONE: ANGOLI DI EULERO
        {
            double q0, q1, q2, q3;
            int dim = sampwin.GetLength(1);
            double[,] arrayAngoli = new double[3, dim];
            for (int i = 0; i < dim; ++i)
            {
                //estrazione della quadrupla del campione iesimo
                q0 = sampwin[9, i];
                q1 = sampwin[10, i];
                q2 = sampwin[11, i];
                q3 = sampwin[12, i];
                //roll/phi
                arrayAngoli[0, i] = Math.Atan((2 * q2 * q3 + 2 * q0 * q1) / (2 * Math.Pow(q0, 2) * Math.Pow(q3, 2) - 1));
                //pitch/theta
                arrayAngoli[1, i] = -Math.Asin(2 * q1 * q3 - 2 * q0 * q2);
                //yaw/psi
                arrayAngoli[2, i] = Math.Atan((2 * q1 * q2 + 2 * q0 * q3) / (2 * Math.Pow(q0, 2) * Math.Pow(q1, 2) - 1));
            }
            return arrayAngoli;
        }

        private void createGraph(ZedGraph.ZedGraphControl zedGraphControl, int drawX, int drawY, int sizeX, int sizeY, string titolo, string x, string y)
        {
            zedGraphControl.Location = new Point(drawX, drawY);
            zedGraphControl.Size = new Size(sizeX, sizeY);
            myPane = zedGraphControl.GraphPane;
            myPane.Title.Text = titolo;
            myPane.XAxis.Title.Text = x;
            myPane.YAxis.Title.Text = y;
        }

        private PointPairList populate(double[] array) //selesnia - vecchio arrayToSeries
        {
            int length = array.Length;
            PointPairList list = new PointPairList();
            for (int i = 0; i < length; ++i)
                list.Add((double)i / frequence, array[i]);
            return list;
        }

        public double[,] generateSampwin() //generazione simulata di un sampwin semplificato
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

        public double[] multiToSingleArray(double[,] multiArray, int firstDimension)
        {
            //if 0 <= firstDimension <= 2 stiamo estraendo una delle coordinate per la simulazione di un primo generico sensore
            //if 9 <= firstDimension <= 12 stiamo estraendo uno dei quaternioni
            int dim = multiArray.GetLength(1);
            double[] singleArray = new double[dim];
            for (int i = 0; i < dim; ++i)
                singleArray[i] = multiArray[firstDimension, i];
            return singleArray;
        }

        protected override bool ProcessDialogKey(Keys keyData) //escape 
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);

        }
        
        //angoli di eulero
        private void button1_Click(object sender, EventArgs e)
        {
            if (isAngoliDiEuleroPressed == false)
            {
                isAngoliDiEuleroPressed = true;
                createGraph(zedGraphControl1, 10, 10, 800, 800, "Prova", "Secondi", "Grandezza fisica");
                //RAPPRESENTAZIONE DEGLI ANGOLI DI EULERO
                //trovare dei bei colori per differenziarli
                //da mettere dentro un sorta di selezione per quando ci saranno i bottoni, rappresenteremo tutto sullo stesso grafico
                //la smooth sarà sempre facoltativa soprattutto con valori come questi
                //finché non avremo degli input sensati altro che la mia generazione casuale con la tangente che punta a valori sempre piu alti ed il seno non compare mai per allora avremo modo di fare un grafo leggibile 
                //double[] rollPhi = multiToSingleArray(angoliDiEulero(sampwin), 0);
                //LineItem roll = zedGraphControl1.GraphPane.AddCurve("ROLL", populate(rollPhi), Color.LawnGreen, SymbolType.None);
                //LineItem rollSmooth = zedGraphControl1.GraphPane.AddCurve("ROLL-SMOOTH", populate(smoothing(rollPhi, frequence * window, 3)), Color.Green, SymbolType.None);
                //fare u metodo che ci pensi lui a tutto e che pulisca il codice
                //prenderò una decisione quando vedrò l'aspetto del grafo con valori reali
                //roll.Line.Style = System.Drawing.Drawing2D.DashStyle.Custom;
                //roll.Line.Width = 1;
                //roll.Line.DashOn = 5;
                //roll.Line.DashOff = 1;
                double[] pitchTheta = multiToSingleArray(angoliDiEulero(sampwin), 1);
                LineItem pitch = zedGraphControl1.GraphPane.AddCurve("PITCH", populate(pitchTheta), Color.Cyan, SymbolType.None);
                LineItem pitchSmooth = zedGraphControl1.GraphPane.AddCurve("PITCH-SMOOTH", populate(smoothing(pitchTheta, 3)), Color.DarkCyan, SymbolType.None);
                double[] yawPsi = multiToSingleArray(angoliDiEulero(sampwin), 2);
                LineItem yaw = zedGraphControl1.GraphPane.AddCurve("YAW", populate(yawPsi), Color.LightCoral, SymbolType.None);
                LineItem yawSmooth = zedGraphControl1.GraphPane.AddCurve("YAW-SMOOTH", populate(smoothing(yawPsi, 3)), Color.Coral, SymbolType.None);

                //PointPairList populateDiProva2 = populate(arrayDiProva2);
                //LineItem gimmysCurve = zedGraphControl1.GraphPane.AddCurve("Culo(MB)", populateDiProva2, Color.DarkCyan, SymbolType.None);
                //PointPairList populateDiProva = populate(arrayDiProva);
                //LineItem gimmysCurve2 = zedGraphControl1.GraphPane.AddCurve("Culo(MB)", populateDiProva, Color.Cyan, SymbolType.None);

                //gimmysCurve.Line.Style = DashStyle.Custom;
                //gimmysCurve.Line.Width = 2;
                //gimmysCurve.Line.DashOn = 1;
                //gimmysCurve.Line.DashOff = 1;
                //gimmysCurve2.Line.Width = 1.5F;
                //gimmysCurve2.Line.DashOn = 1;
                //gimmysCurve2.Line.DashOff = 1;


                zedGraphControl1.Refresh();
                zedGraphControl1.AxisChange();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (isAngoliDiEuleroPressed && !isRollPressed)
            {
                isRollPressed = true;
                double[] rollPhi = multiToSingleArray(angoliDiEulero(sampwin), 0);
                rollLine = zedGraphControl1.GraphPane.AddCurve("ROLL", populate(rollPhi), Color.LawnGreen, SymbolType.None);
                if (isSmoothPressed)
                {
                    rollSmooth = zedGraphControl1.GraphPane.AddCurve("ROLL-SMOOTH", populate(smoothing(rollPhi, 3)), Color.Green, SymbolType.None);
                }
                zedGraphControl1.Refresh();
                zedGraphControl1.AxisChange();
            }
            else if (isAngoliDiEuleroPressed && isRollPressed)
            {
                isRollPressed = false;
                zedGraphControl1.GraphPane.CurveList.Remove(rollLine);
                if (isSmoothPressed)
                {
                    zedGraphControl1.GraphPane.CurveList.Remove(rollSmooth);
                }
                zedGraphControl1.Refresh();
                zedGraphControl1.AxisChange();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void smooth_Click(object sender, EventArgs e)
        {
            if (!isSmoothPressed)
            {
                isSmoothPressed = true;
                zedGraphControl1.Refresh();
                zedGraphControl1.AxisChange();
            }
            else if (isSmoothPressed)
            {
                isSmoothPressed = false;
                zedGraphControl1.Refresh();
                zedGraphControl1.AxisChange();
            }
        }
    }
}
