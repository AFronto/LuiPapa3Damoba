using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace golyos_jatek
{
    public partial class golyo : Form
    {
        public golyo()
        {            InitializeComponent();         }
        Graphics g; Rectangle r; Pen p; SolidBrush ecset;
        static byte[] piros = new byte[65]; //1-64 helyek közül 0, ha üres és 1 ha van rajta piros golyó
        static byte[] kek = new byte[65];  //1-64 helyek közül 0, ha üres és 1 ha van rajta kék golyó
        static Int16[] rudx = new Int16[17];  //kijelzéshez szükséges x koordinátát tartalmazza
        static Int16[] rudy = new Int16[17];  //kijelzéshez szükséges y koordinátát tartalmazza
        static Int16[] hx = new Int16[65];
        static Int16[] hy = new Int16[65];
        static char[,] rudfelirat = new char[4, 17];
        static string[] kod = new string[65];//1,17,33,49:'A1-4'; 2,18,34,50:'B1-4'
        static byte[] kij = new byte[65]; // 0:üres; 1: piros; 2: kék;
        static byte[] t = new byte[65]; //1-64 poz értékek 0: ha üres; 1: ha piros; 2: ha kék
        static byte kijvez = 0; //0: kiinduló állapot; 1: j1; 2: j2; 3: j3;
        static byte[,] kijtolt = new byte[4, 65];//első index a kijvez; második index az adott hely elforgatva
        static byte[] piroslep = new byte[33];
        static byte[] keklep = new byte[33];
        static byte[,] rud = new byte[17, 2]; //1-16 rudak az; 0 az üres helyek száma; 1 a következő szabad
        static byte[,] kv = new byte[77, 3]; //kék 76 egyenes; 0:1 ha még működhet; 1: meglévő golyók száma; ha 3, akkor még hány kell
        static byte[,] pv = new byte[77, 3]; //piros 76 egyenes; 0:1 ha még működhet; 1: meglévő golyók száma; ha 3, akkor még hány kell
        static byte[,] rud1 = new byte[17, 2]; // másolat a gép lépés kiválasztásához
        static byte[,] pv1 = new byte[77, 3];  // másolat a gép lépés kiválasztásához
        static byte[,] kv1 = new byte[77, 3];  // másolat a gép lépés kiválasztásához
        static byte[,] szab =new byte [77,5]; // 76 egyenes szabálya
        static byte[,] pozszab = new byte[65, 8];//egy adott helyhez tartozó szabályok az
        static byte[] pozrud = new byte[65]; //megadja, hogy egy hely (1-64) hányas rúdon van
        static byte[] poz = new byte[65]; // adott helyhez hány egyenes tartozik 4 vagy 7
        static byte[] gyozegy = new byte[8]; // piros győzelem esetén a győztes egyenesek azonosítóit tartalmazza
        static byte[,] er1 = new byte[4, 8]; //elemzésnél infokat tartalmaz, a rúdra még egy golyó rakható
        static byte[,] er2 = new byte[16, 8]; //elemzésnél infokat tartalmaz, a rúdra még két golyó rakható
        static byte[,] er3 = new byte[64, 8]; //elemzésnél infokat tartalmaz, a rúdra még három golyó rakható
        static byte[,] er4 = new byte[256, 8]; //elemzésnél infokat tartalmaz, a rúdra még négy golyó rakható

        static Int16[] suly = new Int16[22];
        static byte hianylepsz = 0;
        static byte egy = 0;
        static byte p0 = 0; // egyenes vizsgálandó pontja
        static byte p1 = 0; // az egyenes 1. pontja
        static byte p2 = 0; // az egyenes 2. pontja
        static byte p3 = 0; // az egyenes 3. pontja
        static byte p4 = 0; // az egyenes 4. pontja
        static byte szabad = 0; // szabad lépések száma
        static byte kkenyszer = 0; // kék kényszer lépéseinek száma (0-16) rudanként max 1
        static byte pkenyszer = 0; // piros kényszer lépéseinek száma (0-16) rudanként max 1
        static byte knyer1 = 0; // kék nyerő lépései az elemzés során
        static byte pnyer1 = 0; // piros nyerő lépései az elemzés során
        static byte valasztsor = 0; // segéd változó az elemzés táblázatainak megfelelő sorának kiválasztásához
        static byte sk3o2 = 0; // kék kombináció két egyenesre, oldalra egyszerre 2-t kellene tenni (suly[2])
        static byte sp3o2 = 0; // piros kombináció két egyenesre, oldalra egyszerre 2-t kellene tenni (suly[3])
        static byte sk3m2 = 0; // kék kombináció két egyenesre, egymás fölé egyszerre 2-t kellene tenni (suly[4])
        static byte sp3m2 = 0; // piros kombináció két egyenesre, egymás fölé egyszerre 2-t kellene tenni (suly[5])
        static byte sk3h1 = 0; // kék lehetséges egyenes 3 golyóval, egy lépés hiányzik (suly[6])
        static byte sp3h1 = 0; // piros lehetséges egyenes 3 golyóval, egy lépés hiányzik (suly[7])
        static byte sk3h2 = 0; // kék lehetséges egyenes 3 golyóval, két lépés hiányzik (suly[8])
        static byte sp3h2 = 0; // piros lehetséges egyenes 3 golyóval, két lépés hiányzik (suly[9])
        static byte sk3h3 = 0; // kék lehetséges egyenes 3 golyóval, három lépés hiányzik (suly[10])
        static byte sp3h3 = 0; // piros lehetséges egyenes 3 golyóval, három lépés hiányzik (suly[11])
        static byte sk3h4 = 0; // kék lehetséges egyenes 3 golyóval, négy lépés hiányzik (suly[12])
        static byte sp3h4 = 0; // piros lehetséges egyenes 3 golyóval, két lépés hiányzik (suly[13])
        static byte sk2 = 0; // kék lehetséges egyenes 2 golyóval (suly[14])
        static byte sp2 = 0; // piros lehetséges egyenes 2 golyóval (suly[15])
        static byte sk1 = 0; // kék lehetséges egyenesek száma (suly[16])
        static byte sp1 = 0; // piros lehetséges egyenesek száma (suly[17])  
        static byte ktiltott = 0; // kék tiltott helyek száma (suly[18])
        static byte ptiltott = 0; // piros tiltott helyek száma (suly[19])
        static byte k1lep = 0;
        static byte p1lep = 0;
        static byte lepszamlp = 0;
        static byte lepszamlk = 0;
        static byte kiv=0; // azt tárolja, hogy melyik forgatásnélküli rúdra klikkeltek 
        static byte kivjo = 0; //a kiválasztott rúd OK (lehet választani)

        static Boolean valaszthat = false; // csak akkor igaz, ha a játékos választhat, hogy melyik rúdra tesz
        static Boolean pgyoz = false; // ha piros nyer, akkor true
        static Boolean kgyoz = false; // ha kek nyer, akkor true
        static Boolean jj = false; static Boolean jjb = false;
        static Boolean jsz = true; static Boolean jszb = true;
        static Boolean kekkezd = false; static Boolean kekkezdb = false;
        static Boolean kezdo = true; static Boolean kezdob = true;
        static Boolean halado = false; static Boolean haladob = false;
        static Boolean mester = false; static Boolean mesterb = false;
        static Boolean ujrakezd = false;
        static Boolean elemzesfileba = false;
        static Random ra1 = new Random();
        static String konyvtar = "F:\\golyo\\";
        static String jatekos1 = "Tudor"; static String jatekos1b= "";
        static String jatekos2 = "Computer"; static String jatekos2b = "";
        static String plepsor = "";
        static String klepsor = "";
        static String szintkijel = "Kezdő";
        static Int16 erpiros = 0;
        static Int16 erkek = 0;
        static Int16 vez = 3;
        static Int32 szor = 0;
        static Int32 ossz = 0;

        public void teglalap(Brush b, Int16 x1, Int16 y1, Int16 x2, Int16 y2, byte vv)
        { g = CreateGraphics(); p = new Pen(b); p.Width = vv; r = new Rectangle(x1, y1, x2, y2); g.DrawRectangle(p, r); }
        public void tegla(Color ecs, Int16 x1, Int16 y1, Int16 x2, Int16 y2)
        { g = CreateGraphics(); ecset = new SolidBrush(ecs); r = new Rectangle(x1, y1, x2, y2); g.FillRectangle(ecset, r); }
        public void kor(Color ecs, Int16 x1, Int16 y1, Int16 x2, Int16 y2)
        { g = CreateGraphics(); p = new Pen(ecs); g.DrawEllipse(p, x1, y1, x2, y2); }
        public void korlap(Color ecs, Int16 x1, Int16 y1, Int16 x2, Int16 y2)
        { g = CreateGraphics(); ecset = new SolidBrush(ecs); g.FillEllipse(ecset, x1, y1, x2, y2); }
        public void vonal(Brush b, Int16 x1, Int16 y1, Int16 x2, Int16 y2, byte vv)
        { g = CreateGraphics(); p = new Pen(b); p.Width = vv; g.DrawLine(p, x1, y1, x2, y2); }

        private void frissit()
        {
            byte ertek = 0; byte k0 = 0; byte k1 = 0; byte k2 = 0; byte k3 = 0; byte k4 = 0; byte db = 0;
            Int16 x3 = 0; Int16 x4 = 0; Int16 y3 = 0; Int16 y4 = 0;
            Color hatter = BackColor;
            tegla(hatter, 2, 28, 630, 602);
            label1.Text = Convert.ToString(rudfelirat[kijvez, 1]);   label2.Text = Convert.ToString(rudfelirat[kijvez, 2]);
            label3.Text = Convert.ToString(rudfelirat[kijvez, 3]);   label4.Text = Convert.ToString(rudfelirat[kijvez, 4]);
            label5.Text = Convert.ToString(rudfelirat[kijvez, 5]);   label6.Text = Convert.ToString(rudfelirat[kijvez, 6]);
            label7.Text = Convert.ToString(rudfelirat[kijvez, 7]);   label8.Text = Convert.ToString(rudfelirat[kijvez, 8]);
            label9.Text = Convert.ToString(rudfelirat[kijvez, 9]);   label10.Text = Convert.ToString(rudfelirat[kijvez, 10]);
            label11.Text = Convert.ToString(rudfelirat[kijvez, 11]); label12.Text = Convert.ToString(rudfelirat[kijvez, 12]);
            label13.Text = Convert.ToString(rudfelirat[kijvez, 13]); label14.Text = Convert.ToString(rudfelirat[kijvez, 14]);
            label15.Text = Convert.ToString(rudfelirat[kijvez, 15]); label16.Text = Convert.ToString(rudfelirat[kijvez, 16]);
            teglalap(Brushes.Green, 2, 28, 1100, 675, 2);
            vonal(Brushes.Brown, 10, 240, 459, 240, 2);   vonal(Brushes.Brown, 178, 560, 627, 560, 2);
            vonal(Brushes.Brown, 10, 240, 178, 560, 2);   vonal(Brushes.Brown, 459, 240, 627, 560, 2);
            vonal(Brushes.Brown, 10, 240, 10, 280, 2);    vonal(Brushes.Brown, 178, 560, 178, 600, 2);
            vonal(Brushes.Brown, 627, 560, 627, 600, 2);  vonal(Brushes.Brown, 10, 280, 178, 600, 2);
            vonal(Brushes.Brown, 178, 600, 627, 600, 2);
            for (byte i = 1; i < 17; i++)
            { x3 = rudx[i]; y3 = rudy[i]; x4 = x3; y4 = y3; y4 += 180; vonal(Brushes.Black, x3, y3, x4, y4, 5); }      
            for (byte i = 1; i < 65; i++)
            {
                ertek = 0; k1 = kijtolt[kijvez, i];
                if (piros[k1] == 1) { ertek = 1; };
                if (kek[k1] == 1) { ertek = 2; };
                kij[i] = ertek;
                if (kij[0] == k1) { k2 = i; }
            }
            for (byte i = 1; i < 65; i++)
            {
                if (kij[i] == 1) { korlap(Color.Red, hx[i], hy[i], 32, 40); };
                if (kij[i] == 2) { korlap(Color.Blue, hx[i], hy[i], 32, 40); };
            }
            if (kij[0] != 0) { x3 = hx[k2]; y3 = hy[k2]; x3--; y3--; kor(Color.Yellow, hx[k2], hy[k2], 32, 40); kor(Color.Yellow, x3, y3, 34, 42); x3--; y3--; kor(Color.Yellow, x3, y3, 36, 44); }
            if (jj) 
            { 
                label22.Text = "Piros: Játékos1 / " + jatekos1 + " - Kék: Játékos2 / " + jatekos2;
                label21.Text = "Állás: Piros Játékos1 / " + jatekos1 + " - Kék: Játékos2 / " + jatekos2 + " : " + Convert.ToString(erpiros) + " - " + Convert.ToString(erkek);
            }
            if (jsz) 
            { 
                label22.Text = "Piros: Játékos1 / " + jatekos1 + " - Kék: Sámítógép / Szint:"+szintkijel;
                label21.Text = "Állás: Piros Játékos1 / " + jatekos1 + " - Kék: Számítógép : " + Convert.ToString(erpiros) + " - " + Convert.ToString(erkek);
            }
            if (pgyoz | kgyoz)
            {
                for (byte i = 1; i <= 7; i++) { if (gyozegy[i] > 0) { db++; }; }
                for (byte j = 1; db >= j; j++)
                {
                    k1 = szab[gyozegy[j], 1]; k2 = szab[gyozegy[j], 4];
                    for (byte i = 1; i < 65; i++)
                    {
                        k0 = kijtolt[kijvez, i];
                        if (k0 == k1) { k3 = i; };
                        if (k0 == k2) { k4 = i; };
                    };
                    x3 = hx[k3]; y3 = hy[k3]; x3 += 16; y3 += 20; x4 = hx[k4]; y4 = hy[k4]; x4 += 16; y4 += 20;
                    vonal(Brushes.White, x3, y3, x4, y4, 2);
                }
            }       
        }

        private void kezd()
        {
            byte i1 = 0; label17.Text = "Győzelem 2 pont. Döntetlen 1 pont.";
            button5.Visible = false; button5.Enabled = false;
            kijvez = 0; kij[0] = 0;
            for (byte i = 1; i < 65; i++)  {  kij[i] = 0; kek[i] = 0; piros[i] = 0; }
            for (byte i = 1; i < 33; i++) { piroslep[i] = 0; keklep[i] = 0; }
            for (byte i = 1; i < 17; i++) { rud[i, 0] = 4; rud[i, 1] = i; pozrud[i] = i; i1 = i; i1 += 16; pozrud[i1] = i; i1 += 16; pozrud[i1] = i; i1 += 16; pozrud[i1] = i; }
            for (byte i = 1; i < 77; i++) { kv[i, 0] = 1; kv[i, 1] = 0; kv[i, 2] = 0; pv[i, 0] = 1; pv[i, 1] = 0; pv[i, 2] = 0; }
            button2.Enabled = true; button2.Visible = true; button3.Enabled = true; button3.Visible = true;
            label1.Visible = true; label2.Visible = true; label3.Visible = true; label4.Visible = true; label5.Visible = true;
            label6.Visible = true; label7.Visible = true; label8.Visible = true; label9.Visible = true; label10.Visible = true;
            label11.Visible = true; label12.Visible = true; label13.Visible = true; label14.Visible = true; label15.Visible = true;
            label16.Visible = true; label17.Visible = true; label18.Visible = true; label19.Visible = true; label20.Visible = true;
            label21.Visible = true; label22.Visible = true; label23.Visible = true; label24.Visible = true; label25.Visible = true;
            label26.Visible = true; label27.Visible = true; label26.Text = "Piros lépései: "; label27.Text = "Kék lépései:   ";
            lepszamlp = 0; lepszamlk = 0; valaszthat = true; label18.Text = "";
            plepsor = "Piros lépései:"; klepsor = "  Kék lépései:"; label26.Text = plepsor; label27.Text = klepsor;
            if (kekkezd)
            {
                plepsor += " -- ";
                if (jsz) 
                { 
                    label23.Text = "Kék kezd, a számítógép lép ...";
                    kek[1] = 1; lepszamlk++; keklep[lepszamlk] = 1; klepsor += " A1"; 
                    label27.Text = klepsor; label24.Text = ""; label25.Text = "Kék utolsó lépése: A1";
                    for (byte i = 1; i < 8; i++)
                    {
                        i1 = pozszab[1, i]; if (i1 > 0)  { kv[i1, 1]++; pv[i1, 0] = 0; }
                    }
                    rud[1, 0]--; rud[1, 1] += 16; kij[0] = 1; frissit();
                    vez = 3; label23.Text = "Piros játékos1 / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; valaszthat = true;
                }
                if (jj) { label23.Text = "Kék kezd, játékos2 / " + jatekos2 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; vez = 2; valaszthat = true; }
            }
            else
            {
                klepsor += " -- ";
                if (jsz) { label23.Text = "Piros kezd, játékos1 / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; vez = 3; valaszthat = true; }
                if (jj) { label23.Text = "Piros kezd, játékos1 / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; vez = 1; valaszthat = true; }
            }
            label24.Text = " "; label25.Text = " "; for (byte i = 1; i < 8; i++) { gyozegy[i] = 0; }  pgyoz = false; kgyoz = false; frissit();
        }

        private void valaszt()
        {
            kivjo = kijtolt[kijvez, kiv]; valaszthat = false;
            switch (vez)
            {
                case 1: leppiros(); break;
                case 2: lepkek(); break;
                case 3: leppiros(); break;
                case 4: szgepkezd(); break;
                default: break;
            }
        }

        private void leppiros() //vez=1 vagy vez=3 bemenő adat=kivjo
        {
            byte lep1 = 0; byte k1 = 0; byte k2 = 0;
            if (rud[kivjo, 0] > 0)
            {
                lep1 = rud[kivjo, 1]; rud[kivjo, 0]--; rud[kivjo, 1] += 16; piros[lep1] = 1; lepszamlp++; piroslep[lepszamlp] = lep1;
                plepsor += " " + kod[lep1]; label26.Text = plepsor;label18.Text = ""; label24.Text = ""; label25.Text = "Piros utolsó lépése: " + kod[lep1];
                for (byte i = 1; i <= poz[lep1]; i++)
                {
                    k1 = pozszab[lep1, i]; kv[k1, 0] = 0;
                    if (pv[k1, 0] == 1) { pv[k1, 1]++; if (pv[k1, 1] == 4) { pgyoz = true; k2++; gyozegy[k2] = k1; } }     
                }
                kij[0] = lep1; frissit();
                if (pgyoz)
                { label23.Text = "Piros Nyert!";label24.Text = ""; erpiros+=2; frissit(); vez = 6; button5.Enabled = true; button5.Visible = true; }
                else
                {
                    k2 = lepszamlp; k2 += lepszamlk;
                    if (k2 == 64) { label23.Text = "Döntetlen..."; erpiros++; erkek++; vez = 6; button5.Enabled = true; button5.Visible = true; };
                    if (vez == 1) { vez = 2; label23.Text = "Kék játékos2 / " + jatekos2 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; valaszthat = true; };
                    if (vez == 3) { vez = 4; label23.Text = "A számítógép lép kékkel..."; valaszt(); };
                }
            }
            else
            { label24.Text = "Több golyó nem helyezhető a(z) " + rudfelirat[kijvez, kiv] + " rúdra!"; valaszthat = true; }
        }

        private void lepkek() //vez=2 játékos2 lép kékkel...
        {
            byte lep1 = 0; byte k1 = 0; byte k2 = 0;
            if (rud[kivjo, 0] > 0)
            {
                lep1 = rud[kivjo, 1]; rud[kivjo, 0]--; rud[kivjo, 1] += 16; kek[lep1] = 1; lepszamlk++; keklep[lepszamlk] = lep1;
                klepsor += " " + kod[lep1]; label27.Text = klepsor; label18.Text = ""; label24.Text = ""; label25.Text = "Kék utolsó lépése: " + kod[lep1];
                for (byte i = 1; i <= poz[lep1]; i++)
                {
                    k1 = pozszab[lep1, i]; pv[k1, 0] = 0;
                    if (kv[k1, 0] == 1) { kv[k1, 1]++; if (kv[k1, 1] == 4) { kgyoz = true; k2++; gyozegy[k2] = k1; } }    
                }
                kij[0] = lep1; frissit();
                if (kgyoz)
                { label23.Text = "Kék Nyert!"; label24.Text = ""; erkek+=2; frissit(); vez = 6; button5.Enabled = true; button5.Visible = true; }
                else 
                {
                    k2 = lepszamlp; k2 += lepszamlk;
                    if (k2 == 64) { label23.Text = "Döntetlen..."; erpiros++; erkek++; vez = 6; button5.Enabled = true; button5.Visible = true; };
                    vez = 1; label23.Text = "Piros játékos1 / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; valaszthat = true;
                }
            }
            else
            { label24.Text = "Több golyó nem helyezhető a(z) " + rudfelirat[kijvez, kiv] + " rúdra!"; valaszthat = true; }
        }

        private void hiany() // bemenet: egy; kimenet: hianylepsz; (ha egy egyenesen már van három golyó!!!)
        {
            byte r0 = 0; hianylepsz = 0; p1 = szab[egy, 1]; p2 = szab[egy, 2]; p3 = szab[egy, 3]; p4 = szab[egy, 4];
            if (t[p1] == 0) { p0 = p1; }; if (t[p2] == 0) { p0 = p2; }; if (t[p3] == 0) { p0 = p3; }; if (t[p4] == 0) { p0 = p4; };
            r0 = pozrud[p0]; if ((p0 >= r0) && (t[r0] == 0)) { hianylepsz++; }
            r0 += 16; if ((p0 >= r0) && (t[r0] == 0)) { hianylepsz++; }
            r0 += 16; if ((p0 >= r0) && (t[r0] == 0)) { hianylepsz++; }
            r0 += 16; if ((p0 >= r0) && (t[r0] == 0)) { hianylepsz++; }
        }

        private void szgepkezd()
        {
            byte[,] gment = new byte[8, 3]; // gép lépése előtti értékekeket elmentjük
            byte[,] jment = new byte[8, 3]; // a játékos lépése előtti értékeket elmentjük
            byte[] g1lepesek = new byte[17]; // a számítógép 1. lehetséges lépéseit tartalmazza
            byte[] j1lepesek = new byte[17]; // a játékos 1. lehetséges lépései
            byte g1lep = 0; // gép első lépése
            byte g1lepnyer = 0; // gép nyerő lépése
            byte g1lepsz = 0; // gép 1. lépéseinek a db száma
            byte g1egy = 0; // gép egy egyenes azonosítóját tartalmazza
            byte g1r = 0; // gép erre a rúdra tesz
            Boolean g1nyer = false;
            Int32[] g1lepertek = new Int32[17]; // gép első lépéseinek a pontértéke
            Int32 g1lepmax = 0;
            byte j1lep = 0; // a játékos első lépése
            byte j1lepnyer = 0; // a játékos 1. nyerő lépése
            byte j1lepsz = 0; // a játékos lépéseinek a száma
            byte j1egy = 0; //egy egyenes azonosítóját tartalmazza
            byte j1r = 0; // a játékos erre a rúdra tesz
            byte szamlnyero = 0; // tartalmazza a lehetséges nyerő egyenesek számát
            byte k1 = 0;
            Boolean j1nyer = false;
            Int32[] j1lepertek = new Int32[17]; // játékos első lépéseinek a pontértéke
            Int32 j1lepmin = 0;

            int hossz = 1; String szov1 = ""; String szov2 = "";
            szov1 = Convert.ToString(DateTime.Now); hossz = szov1.Length;
            szov2 = konyvtar + "gj_teszt_" + szov1[0] + szov1[1] + szov1[2] + szov1[3] + szov1[5] + szov1[6] + szov1[8] + szov1[9] + "_" + szov1[12];
            for (byte i = 13; i < 18; i++) { if (szov1[i] != ':') { szov2 += szov1[i]; } } szov2 += ".txt";
            StreamWriter sw = new StreamWriter(szov2);
            sw.WriteLine("Golyós játék álláselemzés");
           
            Boolean tovabb = true;
            for (byte i = 1; 17 > i; i++) { rud1[i, 0] = rud[i, 0]; rud1[i, 1] = rud[i, 1]; g1lepertek[i] = 0; }
            for (byte i = 1; 77 > i; i++) { pv1[i, 0] = pv[i, 0]; pv1[i, 1] = pv[i, 1]; pv1[i, 2] = pv[i, 2]; kv1[i, 0] = kv[i, 0]; kv1[i, 1] = kv[i, 1]; kv1[i, 2] = kv[i, 2]; }
            lepszamlk++;
            for (byte i = 1; 17 > i; i++)
            {
                if (rud1[i, 0] > 0)
                {
                    g1lepsz++; g1lep = rud1[i, 1]; g1lepesek[g1lepsz] = g1lep;
                    for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; if ((kv1[g1egy, 0] == 1) && (kv1[g1egy, 1] == 3)) { g1lepnyer = g1lep; g1nyer = true; } }
                }
            }
            if (g1nyer)
            {
                for (byte j = 1; poz[g1lepnyer] >= j; j++) { g1egy = pozszab[g1lepnyer, j]; if ((kv1[g1egy, 0] == 1) && (kv1[g1egy, 1] == 3)) { k1++; gyozegy[k1] = g1egy; } }
                kek[g1lepnyer] = 1; keklep[lepszamlk] = g1lepnyer; kgyoz = true;
                klepsor += " " + kod[g1lepnyer]; label27.Text = klepsor; label18.Text = " számítógép nyert (g1nyer)";
                g1r = pozrud[g1lepnyer]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[g1lepnyer] >=j; j++) { g1egy = pozszab[g1lepnyer, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                label23.Text = "Kék Nyert!"; label24.Text = ""; label25.Text = "Kék utolsó lépése: " + kod[g1lepnyer];
                erkek += 2; kij[0] = g1lepnyer; frissit(); vez = 6; button5.Enabled = true; button5.Visible = true; tovabb = false;
            }
            k1 = lepszamlk; k1 += lepszamlp;
            if ((tovabb) && (k1 == 64))
            {
                g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep];
                label27.Text = klepsor; label18.Text = " Utolsó lépés"; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[g1lep] >=j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                label23.Text = "Döntetlen!"; label24.Text = ""; label25.Text = "Kék utolsó lépése: " + kod[g1lep];
                erkek++; erpiros++; kij[0] = g1lep; frissit(); vez = 6; button5.Enabled = true; button5.Visible = true; tovabb = false;
            }
            if ((g1lepsz == 1) && (tovabb))
            {
                g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep];
                label27.Text = klepsor; label18.Text = "Csak egy rúdra tehet"; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[g1lep]>=j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                label23.Text = "Piros játékos1 / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; label24.Text = ""; 
                label25.Text = "Kék utolsó lépése: " + kod[g1lep]; kij[0] = g1lep; frissit(); vez = 3; valaszthat = true; tovabb = false;
            }
            if (tovabb)
            {
                for (byte i = 1; 17 > i; i++)
                {
                    if (rud1[i, 0] > 0)
                    { j1lep = rud1[i, 1]; for (byte j = 1; poz[j1lep] >=j; j++) { j1egy = pozszab[j1lep, j]; if ((pv1[j1egy, 0] == 1) && (pv1[j1egy, 1] == 3)) { j1lepnyer = j1lep; j1nyer = true; } } }
                }
                
            }
            if ((tovabb) && (j1nyer))
            {
                keklep[lepszamlk] = j1lepnyer; kek[j1lepnyer] = 1; klepsor += " " + kod[j1lepnyer];
                label27.Text = klepsor; label18.Text = "Ellenfél nyerési lehetőségét szüntette meg"; g1r = pozrud[j1lepnyer]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[j1lepnyer] >=j; j++) { g1egy = pozszab[j1lepnyer, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                label23.Text = "Piros játékos1 / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; ;
                label24.Text = ""; label25.Text = "Kék utolsó lépése: " + kod[j1lepnyer]; kij[0] = j1lepnyer; frissit(); vez = 3; tovabb = false; valaszthat = true;
            }
            if (tovabb)
            {
                szamlnyero = 0; for (byte i = 1; 77 > i; i++) { if (pv1[i, 0] == 0) { szamlnyero++; }; if (kv1[i, 0] == 0) { szamlnyero++; } }
                if (szamlnyero == 0)
                {
                    g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep];
                    label27.Text = klepsor; label18.Text = "Egyik fél sem tud már nyerni, döntetlen"; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                    for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                    label23.Text = "Piros játékos1 / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; label24.Text = "";
                    label25.Text = "Kék utolsó lépése: " + kod[g1lep]; kij[0] = g1lep; frissit(); vez = 3; valaszthat = true; tovabb = false;
                }
            }

            if (tovabb)
            { //lépés keresés
                for (byte i = 1; i < 65; i++) { k1 = 0; if (piros[i] == 1) { k1 = 1; } if (kek[i] == 1) { k1 = 2; } t[i] = k1; }
                for (byte g1 = 1; g1lepsz >= g1; g1++)
                { //1.
                    g1lep = g1lepesek[g1]; g1r = pozrud[g1lep]; rud1[g1r, 0]--; rud1[g1r, 1] += 16; t[g1lep] = 2;
                    for (byte j = 1; poz[g1lep] >=j; j++)
                    { //2a1
                        g1egy = pozszab[g1lep, j]; gment[j, 0] = kv1[g1egy, 0]; gment[j, 1] = kv1[g1egy, 1]; gment[j, 2] = pv1[g1egy, 0];
                        pv1[g1egy, 0] = 0; if (kv1[g1egy, 0] == 1) { kv1[g1egy, 1]++; }
                    } //2a1
                    j1lepsz = 0; j1nyer = false;
                    for (byte i = 1; 17 > i; i++)
                    { // 3.
                        if (rud1[i,0] > 0)
                        { // 4.
                            j1lepsz++; j1lep = rud1[i, 1]; j1lepesek[j1lepsz] = j1lep;
                            for (byte j = 1; poz[j1lep] >=j; j++) { j1egy = pozszab[j1lep, j]; if ((pv1[j1egy, 0] == 1) && (pv1[j1egy, 1] == 3)) { j1nyer = true; } }
                        } // 4.
                    } // 3.
                    j1lepertek[1] = 0;
                    if (j1nyer) { j1lepertek[1] = -32767; }
                    else
                    { // 5.
                        for (byte j1 = 1; j1lepsz >= j1; j1++)
                        { // 6.
                            j1lep = j1lepesek[j1]; j1r = pozrud[j1lep]; rud1[j1r, 0]--; rud1[j1r, 1] += 16; t[j1lep] = 1;
                            for (byte j = 1; poz[j1lep] >=j; j++)
                            { // 7a1
                                j1egy = pozszab[j1lep, j]; jment[j, 0] = pv1[j1egy, 0]; jment[j, 1] = pv1[j1egy, 1]; jment[j, 2] = kv1[j1egy, 0];
                                kv1[j1egy, 0] = 0; if (pv1[j1egy, 0] == 1) { pv1[j1egy, 1]++; }
                            } // 7a1
                            k1lep = g1lep; p1lep = j1lep;
                            //if (elemzesfileba) { elemzeskiir(); }
                           // else { elemzes(); } 
                            sw.WriteLine();
                            sw.WriteLine("g1lep: "+Convert.ToString(g1lep)+" j1lep: "+Convert.ToString(j1lep));
                            sw.WriteLine("Tábla:");
                            szov1 = Convert.ToString(t[13]) + "," + Convert.ToString(t[14]) + "," + Convert.ToString(t[15]) + "," + Convert.ToString(t[16]) + "," + " ,";
                            szov1 += Convert.ToString(t[29]) + "," + Convert.ToString(t[30]) + "," + Convert.ToString(t[31]) + "," + Convert.ToString(t[32]) + "," + " ,";
                            szov1 += Convert.ToString(t[45]) + "," + Convert.ToString(t[46]) + "," + Convert.ToString(t[47]) + "," + Convert.ToString(t[48]) + "," + " ,";
                            szov1 += Convert.ToString(t[61]) + "," + Convert.ToString(t[62]) + "," + Convert.ToString(t[63]) + "," + Convert.ToString(t[64]);
                            sw.WriteLine(szov1);
                            szov1 = Convert.ToString(t[9]) + "," + Convert.ToString(t[10]) + "," + Convert.ToString(t[11]) + "," + Convert.ToString(t[12]) + "," + " ,";
                            szov1 += Convert.ToString(t[25]) + "," + Convert.ToString(t[26]) + "," + Convert.ToString(t[27]) + "," + Convert.ToString(t[28]) + "," + " ,";
                            szov1 += Convert.ToString(t[41]) + "," + Convert.ToString(t[42]) + "," + Convert.ToString(t[43]) + "," + Convert.ToString(t[44]) + "," + " ,";
                            szov1 += Convert.ToString(t[57]) + "," + Convert.ToString(t[58]) + "," + Convert.ToString(t[59]) + "," + Convert.ToString(t[60]);
                            sw.WriteLine(szov1);
                            szov1 = Convert.ToString(t[5]) + "," + Convert.ToString(t[6]) + "," + Convert.ToString(t[7]) + "," + Convert.ToString(t[8]) + "," + " ,";
                            szov1 += Convert.ToString(t[21]) + "," + Convert.ToString(t[22]) + "," + Convert.ToString(t[23]) + "," + Convert.ToString(t[24]) + "," + " ,";
                            szov1 += Convert.ToString(t[37]) + "," + Convert.ToString(t[38]) + "," + Convert.ToString(t[39]) + "," + Convert.ToString(t[40]) + "," + " ,";
                            szov1 += Convert.ToString(t[53]) + "," + Convert.ToString(t[54]) + "," + Convert.ToString(t[55]) + "," + Convert.ToString(t[56]);
                            sw.WriteLine(szov1);
                            szov1 = Convert.ToString(t[1]) + "," + Convert.ToString(t[2]) + "," + Convert.ToString(t[3]) + "," + Convert.ToString(t[4]) + "," + " ,";
                            szov1 += Convert.ToString(t[17]) + "," + Convert.ToString(t[18]) + "," + Convert.ToString(t[19]) + "," + Convert.ToString(t[20]) + "," + " ,";
                            szov1 += Convert.ToString(t[33]) + "," + Convert.ToString(t[34]) + "," + Convert.ToString(t[35]) + "," + Convert.ToString(t[36]) + "," + " ,";
                            szov1 += Convert.ToString(t[49]) + "," + Convert.ToString(t[50]) + "," + Convert.ToString(t[51]) + "," + Convert.ToString(t[52]);
                            sw.WriteLine(szov1);
                            sw.WriteLine();

                            byte x1lep = 0; byte x2lep = 0; byte x3lep = 0; byte x4lep = 0; byte xegy = 1;
                            byte x1k = 0; byte x2k = 0; byte x3k = 0; byte x4k = 0;
                            byte x1p = 0; byte x2p = 0; byte x3p = 0; byte x4p = 0;
                            szabad = 0; kkenyszer = 0; pkenyszer = 0; knyer1 = 0; pnyer1 = 0;
                            sk3o2 = 0; sk3m2 = 0; sk3h1 = 0; sk3h2 = 0; sk3h3 = 0; sk3h4 = 0; sk2 = 0; sk1 = 0; ktiltott = 0;
                            sp3o2 = 0; sp3m2 = 0; sp3h1 = 0; sp3h2 = 0; sp3h3 = 0; sp3h4 = 0; sp2 = 0; sp1 = 0; ptiltott = 0;
                            for (byte k = 1; 77 > k; k++)
                            { //1.
                                if (kv1[k, 0] == 1) // a kék egyenes még működhet
                                { //2.
                                    sk1++;
                                    if (kv1[k, 1] == 2) { sk2++; }
                                    if (kv1[k, 1] == 3) { egy = k; hiany(); if (hianylepsz == 1) { sk3h1++; }; if (hianylepsz == 2) { sk3h2++; }; if (hianylepsz == 3) { sk3h3++; }; if (hianylepsz == 4) { sk3h4++; }; }
                                } //2.
                                if (pv1[k, 0] == 1) // a piros egyenes még működhet
                                { //3.
                                    sp1++;
                                    if (pv1[k, 1] == 2) { sp2++; }
                                    if (pv1[k, 1] == 3) { egy = k; hiany(); if (hianylepsz == 1) { sp3h1++; }; if (hianylepsz == 2) { sp3h2++; }; if (hianylepsz == 3) { sp3h3++; }; if (hianylepsz == 4) { sp3h4++; }; }
                                } //3.
                            } //1.
                            for (byte e1 = 1; 17 > e1; e1++)
                            { //4.
                                x1k = 0; x2k = 0; x3k = 0; x4k = 0; x1p = 0; x2p = 0; x3p = 0; x4p = 0;
                                if (rud1[e1, 0] == 1) // a rúdon három golyó van
                                { //5.
                                    x1lep = rud1[e1, 1]; szabad++;
                                    for (byte j = 1; poz[x1lep] >= j; j++)
                                    {
                                        xegy = pozszab[x1lep, j];
                                        if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x1k = 1; }
                                        if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x1p = 1; }
                                    }
                                    knyer1 += x1k; pnyer1 += x1p;
                                    if ((x1k == 1) && (x1p == 0)) { pkenyszer++; }
                                    if ((x1k == 0) && (x1p == 1)) { kkenyszer++; }
                                    valasztsor = x1p; if (x1k == 1) { valasztsor += 2; }
                                    szov1 = Convert.ToString(e1) + " Rud egy   ," + Convert.ToString(x1k) + "," + Convert.ToString(x1p) + ", , , , , , ,";
                                    szov1 += " valasztsor ," + Convert.ToString(valasztsor) + "," + Convert.ToString(er1[valasztsor, 0]) + "," + Convert.ToString(er1[valasztsor, 1]) + "," + Convert.ToString(er1[valasztsor, 2]) + "," + Convert.ToString(er1[valasztsor, 3]) + ",";
                                    szov1 += Convert.ToString(er1[valasztsor, 4]) + "," + Convert.ToString(er1[valasztsor, 5]) + "," + Convert.ToString(er1[valasztsor, 6]) + "," + Convert.ToString(er1[valasztsor, 7]);
                                    sw.WriteLine(szov1);
                                } //5.
                                if (rud1[e1, 0] == 2) // a rúdon már két golyó van
                                { //6.
                                    x1lep = rud1[e1, 1]; x2lep = x1lep; x2lep += 16; szabad++;
                                    for (byte j = 1; poz[x1lep] >= j; j++)
                                    { //7.
                                        xegy = pozszab[x1lep, j];
                                        if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x1k = 1; }
                                        if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x1p = 1; }
                                    } //7.
                                    for (byte j = 1; poz[x2lep] >= j; j++)
                                    { //8.
                                        xegy = pozszab[x2lep, j];
                                        if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x2k = 1; }
                                        if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x2p = 1; }
                                    } //8.
                                    valasztsor = x2p; if (x2k == 1) { valasztsor += 2; }; if (x1p == 1) { valasztsor += 4; }; if (x1k == 1) { valasztsor += 8; };
                                    sk3m2 += er2[valasztsor, 0]; sp3m2 += er2[valasztsor, 1]; ktiltott += er2[valasztsor, 2]; ptiltott += er2[valasztsor, 3];
                                    kkenyszer += er2[valasztsor, 4]; pkenyszer += er2[valasztsor, 5]; knyer1 += er2[valasztsor, 6]; pnyer1 += er2[valasztsor, 7];
                                    szov1 = Convert.ToString(e1) + " Rud kettő ," + Convert.ToString(x1k) + "," + Convert.ToString(x1p) + "," + Convert.ToString(x2k) + "," + Convert.ToString(x2p) + ", , , , ,";
                                    szov1 += " valasztsor ," + Convert.ToString(valasztsor) + "," + Convert.ToString(er2[valasztsor, 0]) + "," + Convert.ToString(er2[valasztsor, 1]) + "," + Convert.ToString(er2[valasztsor, 2]) + "," + Convert.ToString(er2[valasztsor, 3]) + ",";
                                    szov1 += Convert.ToString(er2[valasztsor, 4]) + "," + Convert.ToString(er2[valasztsor, 5]) + "," + Convert.ToString(er2[valasztsor, 6]) + "," + Convert.ToString(er2[valasztsor, 7]);
                                    sw.WriteLine(szov1);
                                } //6.
                                if (rud1[e1, 0] == 3) // a rúdon még csak egy golyó van
                                { //9.
                                    x1lep = rud1[e1, 1]; x2lep = x1lep; x2lep += 16; x3lep = x2lep; x3lep += 16; szabad++;
                                    for (byte j = 1; poz[x1lep] >= j; j++)
                                    { //10.
                                        xegy = pozszab[x1lep, j];
                                        if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x1k = 1; }
                                        if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x1p = 1; }
                                    } //10.
                                    for (byte j = 1; poz[x2lep] >= j; j++)
                                    { //11.
                                        xegy = pozszab[x2lep, j];
                                        if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x2k = 1; }
                                        if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x2p = 1; }
                                    } //11.
                                    for (byte j = 1; poz[x3lep] >= j; j++)
                                    { //12.
                                        xegy = pozszab[x3lep, j];
                                        if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x3k = 1; }
                                        if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x3p = 1; }
                                    } //12.
                                    valasztsor = x3p; if (x3k == 1) { valasztsor += 2; }; if (x2p == 1) { valasztsor += 4; }; if (x2k == 1) { valasztsor += 8; };
                                    if (x1p == 1) { valasztsor += 16; }; if (x1k == 1) { valasztsor += 32; };
                                    sk3m2 += er3[valasztsor, 0]; sp3m2 += er3[valasztsor, 1]; ktiltott += er3[valasztsor, 2]; ptiltott += er3[valasztsor, 3];
                                    kkenyszer += er3[valasztsor, 4]; pkenyszer += er3[valasztsor, 5]; knyer1 += er3[valasztsor, 6]; pnyer1 += er3[valasztsor, 7];
                                    szov1 = Convert.ToString(e1) + " Rud három ," + Convert.ToString(x1k) + "," + Convert.ToString(x1p) + "," + Convert.ToString(x2k) + "," + Convert.ToString(x2p) + "," + Convert.ToString(x3k) + "," + Convert.ToString(x3p) + ", , ,";
                                    szov1 += " valasztsor ," + Convert.ToString(valasztsor) + "," + Convert.ToString(er3[valasztsor, 0]) + "," + Convert.ToString(er3[valasztsor, 1]) + "," + Convert.ToString(er3[valasztsor, 2]) + "," + Convert.ToString(er3[valasztsor, 3]) + ",";
                                    szov1 += Convert.ToString(er3[valasztsor, 4]) + "," + Convert.ToString(er3[valasztsor, 5]) + "," + Convert.ToString(er3[valasztsor, 6]) + "," + Convert.ToString(er3[valasztsor, 7]);
                                    sw.WriteLine(szov1);
                                } //9.
                                if (rud1[e1, 0] == 4) // a rúdon még nincs golyó
                                { //13.
                                    x1lep = rud1[e1, 1]; x2lep = x1lep; x2lep += 16; x3lep = x2lep; x3lep += 16; x4lep = x3lep; x4lep += 16; szabad++;
                                    for (byte j = 1; poz[x1lep] >= j; j++)
                                    { //14.
                                        xegy = pozszab[x1lep, j];
                                        if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x1k = 1; }
                                        if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x1p = 1; }
                                    } //14.
                                    for (byte j = 1; poz[x2lep] >= j; j++)
                                    { //15.
                                        xegy = pozszab[x2lep, j];
                                        if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x2k = 1; }
                                        if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x2p = 1; }
                                    } //15.
                                    for (byte j = 1; poz[x3lep] >= j; j++)
                                    { //16.
                                        xegy = pozszab[x3lep, j];
                                        if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x3k = 1; }
                                        if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x3p = 1; }
                                    } //16.
                                    for (byte j = 1; poz[x4lep] >= j; j++)
                                    { //17.
                                        xegy = pozszab[x4lep, j];
                                        if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x4k = 1; }
                                        if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x4p = 1; }
                                    } //17.
                                    valasztsor = x4p; if (x4k == 1) { valasztsor += 2; }; if (x3p == 1) { valasztsor += 4; }; if (x3k == 1) { valasztsor += 8; };
                                    if (x2p == 1) { valasztsor += 16; }; if (x2k == 1) { valasztsor += 32; }; if (x1p == 1) { valasztsor += 64; }; if (x1k == 1) { valasztsor += 128; };
                                    sk3m2 += er4[valasztsor, 0]; sp3m2 += er4[valasztsor, 1]; ktiltott += er4[valasztsor, 2]; ptiltott += er4[valasztsor, 3];
                                    kkenyszer += er4[valasztsor, 4]; pkenyszer += er4[valasztsor, 5]; knyer1 += er4[valasztsor, 6]; pnyer1 += er4[valasztsor, 7];
                                    szov1 = Convert.ToString(e1) + " Rud négy   ," + Convert.ToString(x1k) + "," + Convert.ToString(x1p) + "," + Convert.ToString(x2k) + "," + Convert.ToString(x2p) + "," + Convert.ToString(x3k) + "," + Convert.ToString(x3p) + "," + Convert.ToString(x4k) + "," + Convert.ToString(x4p);
                                    szov1 += " valasztsor ," + Convert.ToString(valasztsor) + "," + Convert.ToString(er4[valasztsor, 0]) + "," + Convert.ToString(er4[valasztsor, 1]) + "," + Convert.ToString(er4[valasztsor, 2]) + "," + Convert.ToString(er4[valasztsor, 3]) + ",";
                                    szov1 += Convert.ToString(er4[valasztsor, 4]) + "," + Convert.ToString(er4[valasztsor, 5]) + "," + Convert.ToString(er4[valasztsor, 6]) + "," + Convert.ToString(er4[valasztsor, 7]);
                                    sw.WriteLine(szov1);
                                } //13.
                            } //4.
                            if (kkenyszer > 1) { sk3o2 = 1; }
                            if (pkenyszer > 1) { sp3o2 = 1; }
                            if (szabad == ktiltott) { pnyer1 ++; } // hogy a korábbi értékek ne vesszenek el
                            if (szabad == ptiltott) { knyer1++; } // hogy a korábbi értékek ne vesszenek el
                            sw.WriteLine("Összegzés:");
                            sw.WriteLine("szabad ," + Convert.ToString(szabad));
                            sw.WriteLine("kkenyszer ," + Convert.ToString(kkenyszer) + "," + Convert.ToString(sk3o2));
                            sw.WriteLine("pkenyszer ," + Convert.ToString(pkenyszer) + "," + Convert.ToString(sp3o2));
                                                      
                            sw.WriteLine("knyer1 ," + Convert.ToString(knyer1));
                            sw.WriteLine("pnyer1 ," + Convert.ToString(pnyer1));
                            sw.WriteLine("sk3o2 ," + Convert.ToString(sk3o2));
                            sw.WriteLine("sp3o2 ," + Convert.ToString(sp3o2));
                            sw.WriteLine("sk3m2 ," + Convert.ToString(sk3m2));
                            sw.WriteLine("sp3m2 ," + Convert.ToString(sp3m2));
                            sw.WriteLine("sk3h1 ," + Convert.ToString(sk3h1));
                            sw.WriteLine("sp3h1 ," + Convert.ToString(sp3h1));
                            sw.WriteLine("sk3h2 ," + Convert.ToString(sk3h2));
                            sw.WriteLine("sp3h2 ," + Convert.ToString(sp3h2));
                            sw.WriteLine("sk3h3 ," + Convert.ToString(sk3h3));
                            sw.WriteLine("sp3h3 ," + Convert.ToString(sp3h3));
                            sw.WriteLine("sk3h4 ," + Convert.ToString(sk3h4));
                            sw.WriteLine("sp3h4 ," + Convert.ToString(sp3h4));
                            sw.WriteLine("sk2 ," + Convert.ToString(sk2));
                            sw.WriteLine("sp2 ," + Convert.ToString(sp2));
                            sw.WriteLine("sk1 ," + Convert.ToString(sk1));
                            sw.WriteLine("sp1 ," + Convert.ToString(sp1));
                            sw.WriteLine("ptiltott ," + Convert.ToString(ptiltott));
                            sw.WriteLine("ktiltott ," + Convert.ToString(ktiltott));
                            sw.WriteLine("poz[k1lep] ," + Convert.ToString(poz[k1lep]));
                            sw.WriteLine("poz[p1lep] ," + Convert.ToString(poz[p1lep]));
                                                      
                            ossz = 0;
                            szor = suly[0]; szor *= knyer1; ossz += szor;
                            szor = suly[1]; szor *= pnyer1; ossz += szor;
                            szor = suly[2]; szor *= sk3o2; ossz += szor;
                            szor = suly[3]; szor *= sp3o2; ossz += szor;
                            szor = suly[4]; szor *= sk3m2; ossz += szor;
                            szor = suly[5]; szor *= sp3m2; ossz += szor;
                            szor = suly[6]; szor *= sk3h1; ossz += szor;
                            szor = suly[7]; szor *= sp3h1; ossz += szor;
                            szor = suly[8]; szor *= sk3h2; ossz += szor;
                            szor = suly[9]; szor *= sp3h2; ossz += szor;
                            szor = suly[10]; szor *= sk3h3; ossz += szor;
                            szor = suly[11]; szor *= sp3h3; ossz += szor;
                            szor = suly[12]; szor *= sk3h4; ossz += szor;
                            szor = suly[13]; szor *= sp3h4; ossz += szor;
                            szor = suly[14]; szor *= sk2; ossz += szor;
                            szor = suly[15]; szor *= sp2; ossz += szor;
                            szor = suly[16]; szor *= sk1; ossz += szor;
                            szor = suly[17]; szor *= sp1; ossz += szor;
                            szor = suly[18]; szor *= ptiltott; ossz += szor;
                            szor = suly[19]; szor *= ktiltott; ossz += szor;
                            szor = suly[20]; szor *= poz[k1lep]; ossz += szor;
                            szor = suly[21]; szor *= poz[p1lep]; ossz += szor;
                            sw.WriteLine("ossz ," + Convert.ToString(ossz));
                     
                            j1lepertek[j1] = ossz;
                            for (byte j = 1; j <= poz[j1lep]; j++)
                            { // 7a2
                                j1egy = pozszab[j1lep, j]; pv1[j1egy, 0]= jment[j, 0]; pv1[j1egy, 1] = jment[j, 1]; kv1[j1egy, 0]= jment[j, 2];
                            } // 7a2
                            rud1[j1r, 0]++; rud1[j1r, 1] -= 16; t[j1lep] = 0;
                        } // 6.
                    } // 5.
                    if (j1lepsz == 0) { j1lepmin = 0; }
                    else
                    { // 15.
                        j1lepmin = j1lepertek[1]; for (byte j = 1; j1lepsz >= j; j++) { if (j1lepmin > j1lepertek[j]) { j1lepmin = j1lepertek[j]; } }
                    } // 15.
                    g1lepertek[g1] = j1lepmin;
                    for (byte j = 1; poz[g1lep] >=j ; j++)
                    { // 2a2
                        g1egy = pozszab[g1lep, j]; kv1[g1egy, 0] = gment[j, 0]; kv1[g1egy, 1] = gment[j, 1]; pv1[g1egy, 0] = gment[j, 2];
                    } // 2a2
                    rud1[g1r, 0]++; rud1[g1r, 1] -= 16; t[g1lep] = 0;
                } //1.

                sw.Close();

                g1lepmax = g1lepertek[1]; g1lep = g1lepesek[1];
                for (byte j = 1; g1lepsz >= j; j++) { if (g1lepmax < g1lepertek[j]) { g1lepmax = g1lepertek[j]; g1lep = g1lepesek[j]; } } 
                
                keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep];
                label27.Text = klepsor; label18.Text = "Értékelés alapján..."; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[g1lep] >=j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                label23.Text = "Piros játékos1 / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; ;
                label24.Text = ""; label25.Text = "Kék utolsó lépése: " + kod[g1lep]; kij[0] = g1lep; frissit(); vez = 3; tovabb = false; valaszthat = true;
            } //lépés keresés
        }
        
          private void elemzes()
          {
              byte x1lep = 0; byte x2lep = 0; byte x3lep = 0; byte x4lep = 0; byte xegy = 1;
              byte x1k = 0; byte x2k = 0; byte x3k = 0; byte x4k = 0;
              byte x1p = 0; byte x2p = 0; byte x3p = 0; byte x4p = 0;
              szabad = 0; kkenyszer = 0; pkenyszer = 0; knyer1 = 0; pnyer1 = 0;
              sk3o2 = 0; sk3m2 = 0; sk3h1 = 0; sk3h2 = 0; sk3h3 = 0; sk3h4 = 0; sk2 = 0; sk1 = 0; ktiltott = 0;
              sp3o2 = 0; sp3m2 = 0; sp3h1 = 0; sp3h2 = 0; sp3h3 = 0; sp3h4 = 0; sp2 = 0; sp1 = 0; ptiltott = 0;
              for (byte k = 1; 77 > k; k++)
              { //1.
                  if (kv1[k, 0] == 1) // a kék egyenes még működhet
                  { //2.
                      sk1++;
                      if (kv1[k, 1] == 2) { sk2++; }
                      if (kv1[k, 1] == 3) { egy = k; hiany(); if (hianylepsz == 1) { sk3h1++; }; if (hianylepsz == 2) { sk3h2++; }; if (hianylepsz == 3) { sk3h3++; }; if (hianylepsz == 4) { sk3h4++; }; }
                  } //2.
                  if (pv1[k, 0] == 1) // a piros egyenes még működhet
                  { //3.
                      sp1++;
                      if (pv1[k, 1] == 2) { sp2++; }
                      if (pv1[k, 1] == 3) { egy = k; hiany(); if (hianylepsz == 1) { sp3h1++; }; if (hianylepsz == 2) { sp3h2++; }; if (hianylepsz == 3) { sp3h3++; }; if (hianylepsz == 4) { sp3h4++; }; }
                  } //3.
              } //1.
              for (byte e1 = 1; 17 > e1; e1++)
              { //4.
                  x1k = 0; x2k = 0; x3k = 0; x4k = 0; x1p = 0; x2p = 0; x3p = 0; x4p = 0;
                  if (rud1[e1, 0] == 1) // a rúdon már három golyó van
                  { //5. 
                      x1lep = rud1[e1, 1]; szabad++;
                      for (byte j = 1; poz[x1lep] >= j; j++)
                      {
                          xegy = pozszab[x1lep, j];
                          if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x1k = 1; }
                          if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x1p = 1; }
                      }    
                          knyer1 += x1k; pnyer1 += x1p; if ((x1k == 1) && (x1p == 0)) { pkenyszer++; } if ((x1k == 0) && (x1p == 1)) { kkenyszer++; }
                  } //5.
                  if (rud1[e1, 0] == 2) //a rúdon már két golyó van
                  { //6.
                      x1lep = rud1[e1, 1]; x2lep = x1lep; x2lep += 16; szabad++;
                      for (byte j = 1; poz[x1lep] >= j; j++)
                      { //7.
                          xegy = pozszab[x1lep, j];
                          if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x1k = 1; }
                          if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x1p = 1; }
                      } //7.
                      for (byte j = 1; poz[x2lep] >= j; j++)
                      { //8.
                          xegy = pozszab[x2lep, j];
                          if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x2k = 1; }
                          if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x2p = 1; }
                      } //8.
                      valasztsor = x2p; if (x2k == 1) { valasztsor += 2; }; if (x1p == 1) { valasztsor += 4; }; if (x1k == 1) { valasztsor += 8; };
                      sk3m2 += er2[valasztsor, 0]; sp3m2 += er2[valasztsor, 1]; ktiltott += er2[valasztsor, 2]; ptiltott += er2[valasztsor, 3];
                      kkenyszer += er2[valasztsor, 4]; pkenyszer += er2[valasztsor, 5]; knyer1 += er2[valasztsor, 6]; pnyer1 += er2[valasztsor, 7];
                  } //6.
                  if (rud1[e1, 0] == 3) //a rúdon már egy golyó van
                  { //9.
                      x1lep = rud1[e1, 1]; x2lep = x1lep; x2lep += 16; x3lep = x2lep; x3lep += 16; szabad++;
                      for (byte j = 1; poz[x1lep] >= j; j++)
                      { //10.
                          xegy = pozszab[x1lep, j];
                          if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x1k = 1; }
                          if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x1p = 1; }
                      } //10.
                      for (byte j = 1; poz[x2lep] >= j; j++)
                      { //11.
                          xegy = pozszab[x2lep, j];
                          if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x2k = 1; }
                          if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x2p = 1; }
                      } //11.
                      for (byte j = 1; poz[x3lep] >= j; j++)
                      { //12.
                          xegy = pozszab[x3lep, j];
                          if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x3k = 1; }
                          if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x3p = 1; }
                      } //12.
                      valasztsor = x3p; if (x3k == 1) { valasztsor += 2; }; if (x2p == 1) { valasztsor += 4; }; if (x2k == 1) { valasztsor += 8; };
                      if (x1p == 1) { valasztsor += 16; }; if (x1k == 1) { valasztsor += 32; };
                      sk3m2 += er3[valasztsor, 0]; sp3m2 += er3[valasztsor, 1]; ktiltott += er3[valasztsor, 2]; ptiltott += er3[valasztsor, 3];
                      kkenyszer += er3[valasztsor, 4]; pkenyszer += er3[valasztsor, 5]; knyer1 += er3[valasztsor, 6]; pnyer1 += er3[valasztsor, 7];
                  } //9.
                  if (rud1[e1, 0] == 4) //a rúdon még nincs egy golyó sem
                  { //13.
                      x1lep = rud1[e1, 1]; x2lep = x1lep; x2lep += 16; x3lep = x2lep; x3lep += 16; x4lep = x3lep; x4lep += 16; szabad++;
                      for (byte j = 1; poz[x1lep] >= j; j++)
                      { //14.
                          xegy = pozszab[x1lep, j];
                          if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x1k = 1; }
                          if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x1p = 1; }
                      } //14.
                      for (byte j = 1; poz[x2lep] >= j; j++)
                      { //15.
                          xegy = pozszab[x2lep, j];
                          if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x2k = 1; }
                          if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x2p = 1; }
                      } //15.
                      for (byte j = 1; poz[x3lep] >= j; j++)
                      { //16.
                          xegy = pozszab[x3lep, j];
                          if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x3k = 1; }
                          if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x3p = 1; }
                      } //16.
                      for (byte j = 1; poz[x4lep] >= j; j++)
                      { //17.
                          xegy = pozszab[x4lep, j];
                          if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x4k = 1; }
                          if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x4p = 1; }
                      } //17.
                      valasztsor = x4p; if (x4k == 1) { valasztsor += 2; }; if (x3p == 1) { valasztsor += 4; }; if (x3k == 1) { valasztsor += 8; };
                      if (x2p == 1) { valasztsor += 16; }; if (x2k == 1) { valasztsor += 32; }; if (x1p == 1) { valasztsor += 64; }; if (x1k == 1) { valasztsor += 128; };
                      sk3m2 += er4[valasztsor, 0]; sp3m2 += er4[valasztsor, 1]; ktiltott += er4[valasztsor, 2]; ptiltott += er4[valasztsor, 3];
                      kkenyszer += er4[valasztsor, 4]; pkenyszer += er4[valasztsor, 5]; knyer1 += er4[valasztsor, 6]; pnyer1 += er4[valasztsor, 7];
                  } //13.
              } //4.
              if (kkenyszer > 1) { sk3o2 = 1; }; if (pkenyszer > 1) { sp3o2 = 1; }; if (szabad == ktiltott) { pnyer1 += 1; }; if (szabad == ptiltott) { knyer1 += 1; };
              ossz = 0;
              szor = suly[0]; szor *= knyer1; ossz += szor;
              szor = suly[1]; szor *= pnyer1; ossz += szor;
              szor = suly[2]; szor *= sk3o2; ossz += szor;
              szor = suly[3]; szor *= sp3o2; ossz += szor;
              szor = suly[4]; szor *= sk3m2; ossz += szor;
              szor = suly[5]; szor *= sp3m2; ossz += szor;
              szor = suly[6]; szor *= sk3h1; ossz += szor;
              szor = suly[7]; szor *= sp3h1; ossz += szor;
              szor = suly[8]; szor *= sk3h2; ossz += szor;
              szor = suly[9]; szor *= sp3h2; ossz += szor;
              szor = suly[10]; szor *= sk3h3; ossz += szor;
              szor = suly[11]; szor *= sp3h3; ossz += szor;
              szor = suly[12]; szor *= sk3h4; ossz += szor;
              szor = suly[13]; szor *= sp3h4; ossz += szor;
              szor = suly[14]; szor *= sk2; ossz += szor;
              szor = suly[15]; szor *= sp2; ossz += szor;
              szor = suly[16]; szor *= sk1; ossz += szor;
              szor = suly[17]; szor *= sp1; ossz += szor;
              szor = suly[18]; szor *= ptiltott; ossz += szor;
              szor = suly[19]; szor *= ktiltott; ossz += szor;
              szor = suly[20]; szor *= poz[k1lep]; ossz += szor;
              szor = suly[21]; szor *= poz[p1lep]; ossz += szor;
          }

        private void szgephal()
          {
              byte[,] g1ment = new byte[8, 3]; // gép 1. lépése előtti értékekeket elmentjük
              byte[,] j1ment = new byte[8, 3]; // a játékos 1. lépése előtti értékeket elmentjük
              byte[] g1lepesek = new byte[17]; // a számítógép 1. lehetséges lépéseit tartalmazza
              byte[] j1lepesek = new byte[17]; // a játékos 1. lehetséges lépései
              byte g1lep = 0; // gép első lépése
              byte g1lepnyer = 0; // gép nyerő lépése
              byte g1lepsz = 0; // gép 1. lépéseinek a db száma
              byte g1egy = 0; // gép egy egyenes azonosítóját tartalmazza
              byte g1r = 0; // gép erre a rúdra tesz
              Boolean g1nyer = false;
              Int32[] g1lepertek = new Int32[17]; // gép első lépéseinek a pontértéke
              Int32 g1lepmax = 0;
              byte j1lep = 0; // a játékos első lépése
              byte j1lepnyer = 0; // a játékos 1. nyerő lépése
              byte j1lepsz = 0; // a játékos lépéseinek a száma
              byte j1egy = 0; //egy egyenes azonosítóját tartalmazza
              byte j1r = 0; // a játékos erre a rúdra tesz
              byte szamlnyero = 0; // tartalmazza a lehetséges nyerő egyenesek számát
              byte k1 = 0;
              Boolean j1nyer = false;
              Int32[] j1lepertek = new Int32[17]; // játékos első lépéseinek a pontértéke
              Int32 j1lepmin = 0;

              Boolean tovabb = true;
              for (byte i = 1; 17 > i; i++) { rud1[i, 0] = rud[i, 0]; rud1[i, 1] = rud[i, 1]; g1lepertek[i] = 0; }
              for (byte i = 1; 77 > i; i++) { pv1[i, 0] = pv[i, 0]; pv1[i, 1] = pv[i, 1]; pv1[i, 2] = pv[i, 2]; kv1[i, 0] = kv[i, 0]; kv1[i, 1] = kv[i, 1]; kv1[i, 2] = kv[i, 2]; }
              lepszamlk++;
              for (byte i = 1; 17 > i; i++)
              {
                  if (rud1[i, 0] > 0)
                  {
                      g1lepsz++; g1lep = rud1[i, 1]; g1lepesek[g1lepsz] = g1lep;
                      for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; if ((kv1[g1egy, 0] == 1) && (kv1[g1egy, 1] == 3)) { g1lepnyer = g1lep; g1nyer = true; } }
                  }
              }
              if (g1nyer) //Ha a kék nyer és vége
              {
                  for (byte j = 1; poz[g1lepnyer] >= j; j++) { g1egy = pozszab[g1lepnyer, j]; if ((kv1[g1egy, 0] == 1) && (kv1[g1egy, 1] == 3)) { k1++; gyozegy[k1] = g1egy; } }
                  kek[g1lepnyer] = 1; keklep[lepszamlk] = g1lepnyer; kgyoz = true;
                  klepsor += " " + kod[g1lepnyer]; label27.Text = klepsor; label18.Text = " számítógép nyert (g1nyer)";
                  g1r = pozrud[g1lepnyer]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                  for (byte j = 1; poz[g1lepnyer] >= j; j++) { g1egy = pozszab[g1lepnyer, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                  label23.Text = "Kék Nyert!"; label24.Text = ""; label25.Text = "Kék utolsó lépése: " + kod[g1lepnyer];
                  erkek += 2; kij[0] = g1lepnyer; frissit(); vez = 6; button5.Enabled = true; button5.Visible = true; tovabb = false;
              }
              k1 = lepszamlk; k1 += lepszamlp;
              if ((tovabb) && (k1 == 64)) //Az utolsó lépés
              {
                  g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep];
                  label27.Text = klepsor; label18.Text = " Utolsó lépés"; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                  for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                  label23.Text = "Döntetlen!"; label24.Text = ""; label25.Text = "Kék utolsó lépése: " + kod[g1lep];
                  erkek++; erpiros++; kij[0] = g1lep; frissit(); vez = 6; button5.Enabled = true; button5.Visible = true; tovabb = false;
              }
              if ((g1lepsz == 1) && (tovabb)) //Csak egyetlen rúdra lehet már csak tenni
              {
                  g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep];
                  label27.Text = klepsor; label18.Text = "Csak egy rúdra tehet"; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                  for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                  label23.Text = "Piros játékos1 / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; label24.Text = "";
                  label25.Text = "Kék utolsó lépése: " + kod[g1lep]; kij[0] = g1lep; frissit(); vez = 3; valaszthat = true; tovabb = false;
              }
              if (tovabb) //ellenfél győztes lépés lehetőségének keresése
              {
                  for (byte i = 1; 17 > i; i++)
                  {
                      if (rud1[i, 0] > 0)
                      { j1lep = rud1[i, 1]; for (byte j = 1; poz[j1lep] >= j; j++) { j1egy = pozszab[j1lep, j]; if ((pv1[j1egy, 0] == 1) && (pv1[j1egy, 1] == 3)) { j1lepnyer = j1lep; j1nyer = true; } } }
                  }
              }
              if ((tovabb) && (j1nyer)) //ha az ellenfél tudna nyerni egy adott helyen, akkor azt megakadályozzuk
              {
                  keklep[lepszamlk] = j1lepnyer; kek[j1lepnyer] = 1; klepsor += " " + kod[j1lepnyer];
                  label27.Text = klepsor; label18.Text = "Ellenfél nyerési lehetőségét szüntette meg"; g1r = pozrud[j1lepnyer]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                  for (byte j = 1; poz[j1lepnyer] >= j; j++) { g1egy = pozszab[j1lepnyer, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                  label23.Text = "Piros játékos1 / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére...";
                  label24.Text = ""; label25.Text = "Kék utolsó lépése: " + kod[j1lepnyer]; kij[0] = j1lepnyer; frissit(); vez = 3; tovabb = false; valaszthat = true;
              }
              if (tovabb) // Keresés, hogy bármelyik játékosnak van még nyerő esélye
              {
                  szamlnyero = 0; for (byte i = 1; 77 > i; i++) { if (pv1[i, 0] == 0) { szamlnyero++; }; if (kv1[i, 0] == 0) { szamlnyero++; } }
                  if (szamlnyero == 0) // Ha már egyik fél sem tud nyerni, akkor elemzés nélkül az első lépést megteszi a számítógép (Fel kéne ajánlani a döntetlent!!!)
                  {
                      g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep];
                      label27.Text = klepsor; label18.Text = "Egyik fél sem tud már nyerni, döntetlen"; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                      for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                      label23.Text = "Piros játékos1 / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; label24.Text = "";
                      label25.Text = "Kék utolsó lépése: " + kod[g1lep]; kij[0] = g1lep; frissit(); vez = 3; valaszthat = true; tovabb = false;
                  }
              }

              if (tovabb)
              { //lépés keresés
                  for (byte i = 1; i < 65; i++) { k1 = 0; if (piros[i] == 1) { k1 = 1; } if (kek[i] == 1) { k1 = 2; } t[i] = k1; }
                  for (byte g1 = 1; g1lepsz >= g1; g1++)
                  { //1.
                      g1lep = g1lepesek[g1]; g1r = pozrud[g1lep]; rud1[g1r, 0]--; rud1[g1r, 1] += 16; t[g1lep] = 2;
                      for (byte j = 1; poz[g1lep] >= j; j++)
                      { //2a1
                          g1egy = pozszab[g1lep, j]; g1ment[j, 0] = kv1[g1egy, 0]; g1ment[j, 1] = kv1[g1egy, 1]; g1ment[j, 2] = pv1[g1egy, 0];
                          pv1[g1egy, 0] = 0; if (kv1[g1egy, 0] == 1) { kv1[g1egy, 1]++; }
                      } //2a1
                      j1lepsz = 0; j1nyer = false;
                      for (byte i = 1; 17 > i; i++)
                      { // 3.
                          if (rud1[i, 0] > 0)
                          { // 4.
                              j1lepsz++; j1lep = rud1[i, 1]; j1lepesek[j1lepsz] = j1lep;
                              for (byte j = 1; poz[j1lep] >= j; j++) { j1egy = pozszab[j1lep, j]; if ((pv1[j1egy, 0] == 1) && (pv1[j1egy, 1] == 3)) { j1nyer = true; } }
                          } // 4.
                      } // 3.
                      j1lepertek[1] = 0;
                      if (j1nyer) { j1lepertek[1] = -32767; }
                      else
                      { // 5.
                          for (byte j1 = 1; j1lepsz >= j1; j1++)
                          { // 6.
                              j1lep = j1lepesek[j1]; j1r = pozrud[j1lep]; rud1[j1r, 0]--; rud1[j1r, 1] += 16; t[j1lep] = 1;
                              for (byte j = 1; poz[j1lep] >= j; j++)
                              { // 7a1
                                  j1egy = pozszab[j1lep, j]; j1ment[j, 0] = pv1[j1egy, 0]; j1ment[j, 1] = pv1[j1egy, 1]; j1ment[j, 2] = kv1[j1egy, 0];
                                  kv1[j1egy, 0] = 0; if (pv1[j1egy, 0] == 1) { pv1[j1egy, 1]++; }
                              } // 7a1
                              k1lep = g1lep; p1lep = j1lep;
                             
                               elemzes();
                              j1lepertek[j1] = ossz;

                              for (byte j = 1; j <= poz[j1lep]; j++)
                              { // 7a2
                                  j1egy = pozszab[j1lep, j]; pv1[j1egy, 0] = j1ment[j, 0]; pv1[j1egy, 1] = j1ment[j, 1]; kv1[j1egy, 0] = j1ment[j, 2];
                              } // 7a2
                              rud1[j1r, 0]++; rud1[j1r, 1] -= 16; t[j1lep] = 0;
                          } // 6.
                      } // 5.
                      if (j1lepsz == 0) { j1lepmin = 0; }
                      else
                      { // 15.
                          j1lepmin = j1lepertek[1]; for (byte j = 1; j1lepsz >= j; j++) { if (j1lepmin > j1lepertek[j]) { j1lepmin = j1lepertek[j]; } }
                      } // 15.
                      g1lepertek[g1] = j1lepmin;
                      for (byte j = 1; poz[g1lep] >= j; j++)
                      { // 2a2
                          g1egy = pozszab[g1lep, j]; kv1[g1egy, 0] = g1ment[j, 0]; kv1[g1egy, 1] = g1ment[j, 1]; pv1[g1egy, 0] = g1ment[j, 2];
                      } // 2a2
                      rud1[g1r, 0]++; rud1[g1r, 1] -= 16; t[g1lep] = 0;
                  } //1.
                  g1lepmax = g1lepertek[1]; g1lep = g1lepesek[1];
                  for (byte j = 1; g1lepsz >= j; j++) { if (g1lepmax < g1lepertek[j]) { g1lepmax = g1lepertek[j]; g1lep = g1lepesek[j]; } }

                  keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep];
                  label27.Text = klepsor; label18.Text = "Értékelés alapján..."; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                  for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                  label23.Text = "Piros játékos1 / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; 
                  label24.Text = ""; label25.Text = "Kék utolsó lépése: " + kod[g1lep]; kij[0] = g1lep; frissit(); vez = 3; tovabb = false; valaszthat = true;
              } //lépés keresés
          }




        private void kilépésToolStripMenuItem_Click(object sender, EventArgs e) { this.Close(); }

        private void startToolStripMenuItem_Click(object sender, EventArgs e) { kezd0(); kezd(); } //teszt(); elemzeskiir();
        
        private void button2_Click(object sender, EventArgs e) //Forgatás balra
        {
            if (kijvez > 0) { kijvez--; }
            else { kijvez = 3; }
            frissit();
        }

        private void button3_Click(object sender, EventArgs e) //Forgatás jobbra
        {
            if (kijvez < 3) { kijvez++; }
            else { kijvez = 0; }
            frissit();
        }

        private void label1_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 1; valaszt(); } }

        private void label2_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 2; valaszt(); } }

        private void label3_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 3; valaszt(); } }

        private void label4_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 4; valaszt(); } }

        private void label5_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 5; valaszt(); } }

        private void label6_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 6; valaszt(); } }

        private void label7_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 7; valaszt(); } }

        private void label8_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 8; valaszt(); } }

        private void label9_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 9; valaszt(); } }

        private void label10_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 10; valaszt(); } }

        private void label11_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 11; valaszt(); } }

        private void label12_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 12; valaszt(); } }

        private void label13_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 13; valaszt(); } }

        private void label14_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 14; valaszt(); } }

        private void label15_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 15; valaszt(); } }

        private void label16_Click(object sender, EventArgs e)
        { if (valaszthat) { kiv = 16; valaszt(); } }

        private void mentésToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int hossz = 1; byte k = 1; String szov1 = ""; String szov2 = "";
            szov1 = Convert.ToString(DateTime.Now); hossz = szov1.Length;
            szov2 = konyvtar + "gj_parti_" + szov1[0] + szov1[1] + szov1[2] + szov1[3] + szov1[5] + szov1[6] + szov1[8] + szov1[9] + "_" + szov1[12];
            for (byte i = 13; i < 18; i++) { if (szov1[i] != ':') { szov2 += szov1[i]; } } szov2 += ".txt";
            StreamWriter sw = new StreamWriter(szov2);
            sw.WriteLine("Golyós játék"); sw.WriteLine(" ");
            sw.WriteLine("Tábla:"); k = 0; szov1 = "";
            for (byte i = 1; i < 65; i++)
            {
                k++; szov1 += Convert.ToString(i) + ":";
                if (kij[i] == 1) { szov1 += "P "; }
                if (kij[i] == 2) { szov1 += "K "; }
                if (kij[i] == 0) { szov1 += "  "; }
                if (k == 16) { sw.WriteLine(szov1); szov1 = ""; k = 0; }
            }
            sw.WriteLine(" "); sw.WriteLine(" "); sw.WriteLine("Parti mentése:");
            if (jj)
            {
                sw.WriteLine("Játékos1 - Játékos2 játszott:");
                szov1 = jatekos1 + " - " + jatekos2; sw.WriteLine(szov1);
                if (kekkezd) { sw.WriteLine(jatekos2 + " kezdett a kék golyókkal"); }
                else { sw.WriteLine(jatekos1 + " kezdett a piros golyókkal"); }
                // állás, nyert, döntetlen
            }
            if (jsz)
            {
                sw.WriteLine("Játékos1 / " + jatekos1 + " - Számítógép (" + szintkijel + " szint) játszott:");
                if (kekkezd) { sw.WriteLine("A számítógép kezdett a kék golyókkal"); }
                else { sw.WriteLine(jatekos1 + " kezdett a piros golyókkal"); }
                // állás: nyert, döntetlen, ...
            }
            sw.WriteLine("Megtett lépések:");
            if (lepszamlk > lepszamlp) { k = lepszamlk; }
            else { k = lepszamlp; }
            for (byte i = 1; i <= k; i++)
            {
                szov1 = Convert.ToString(i) + ".lépéspár: ";
                if (kekkezd) { szov1 += kod[keklep[i]] + " " + kod[piroslep[i]]; }
                else { szov1 += kod[piroslep[i]] + " " + kod[keklep[i]]; }
                sw.WriteLine(szov1);
            }
            
            sw.Close();
        }

        private void beállításToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = true; groupBox1.Visible = true;
            label19.Enabled = true; label19.Visible = true;
            textBox1.Enabled = true; textBox1.Visible = true;
            label20.Enabled = true; label20.Visible = true;
            textBox2.Enabled = true; textBox2.Visible = true;
            groupBox2.Enabled = true; groupBox2.Visible = true;
            groupBox3.Enabled = true; groupBox3.Visible = true;
            button1.Enabled = true; button1.Visible = true;
            button4.Enabled = true; button4.Visible = true;
            jatekos1b = ""; jatekos2b = "";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) // játékos1 - Játékos2
        { jjb = true; jszb = false; }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) // játékos1 - Számítógép
        { jjb = false; jszb = true; }

        private void textBox1_TextChanged(object sender, EventArgs e)
        { 
            jatekos1b = textBox1.Text; 
            radioButton1.Text = "Piros: Játékos1 / "+jatekos1b+" - Kék: Játékos2 / "+jatekos2b;
            radioButton2.Text= "Piros: Játékos1 / "+jatekos1b+" - Kék: Számítógép";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            jatekos2b = textBox2.Text;
            radioButton1.Text = "Piros: Játékos1 / " + jatekos1b + " - Kék: Játékos2 / " + jatekos2b;
            radioButton2.Text = "Piros: Játékos1 / " + jatekos1b + " - Kék: Számítógép";
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {        }
        
        private void groupBox2_Enter(object sender, EventArgs e)
        {        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e) //Piros kezd
        {        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e) //Kék kezd
        {        }

        private void button4_Click(object sender, EventArgs e) //Beállítás módosítások elvetése
        {
            radioButton1.Text = "Piros: Játékos1 / " + jatekos1 + " - Kék: Játékos2 / " + jatekos2;
            radioButton2.Text = "Piros: Játékos1 / " + jatekos1 + " - Kék: Számítógép";
            if (jj) { radioButton2.Checked = false; radioButton1.Checked = true; }
            else { radioButton1.Checked = false;radioButton2.Checked = true;  }
            label19.Enabled = false; label19.Visible = false;
            textBox1.Text = ""; textBox1.Enabled = false; textBox1.Visible = false;
            label20.Enabled = false; label20.Visible = false;
            textBox2.Text = ""; textBox2.Enabled = false; textBox2.Visible = false;
            groupBox1.Enabled = false; groupBox1.Visible = false;
            if (kekkezd) { radioButton3.Checked = false; radioButton4.Checked = true; }
            else { radioButton4.Checked = false;radioButton3.Checked = true;  }
            groupBox2.Enabled = false; groupBox2.Visible = false;
            if (kezdo) { radioButton6.Checked = false; radioButton7.Checked = false; radioButton5.Checked = true; };
            if (halado) {  radioButton7.Checked = false; radioButton5.Checked = false; radioButton6.Checked = true; };
            if (mester) {radioButton5.Checked = false; radioButton6.Checked = false; radioButton7.Checked = true;  };
            groupBox3.Enabled = false;
            if (jj) { groupBox3.Visible = false; }
            button1.Enabled = false; button1.Visible = false;
            button4.Enabled = false; button4.Visible = false;
            if (jj) { label21.Text = "Állás: Piros Játékos1 / " + jatekos1 + " - Kék: Játékos2 / " + jatekos2 + " : " + Convert.ToString(erpiros) + " - " + Convert.ToString(erkek); }
            if (jsz) { label21.Text = "Állás: Piros Játékos1 / " + jatekos1 + " - Kék: Számítógép : " + Convert.ToString(erpiros) + " - " + Convert.ToString(erkek); }

           
        }
      

        private void button1_Click(object sender, EventArgs e) // Beállítások rögzítése
        {
            String szov1 = " ";
            if (radioButton1.Checked) { jjb = true; jszb = false; szov1 = "jj "; }
            if (radioButton2.Checked) { jjb = false; jszb = true; szov1 = "jsz "; }
            groupBox1.Enabled = false; groupBox1.Visible = false;
            label19.Enabled = false; label19.Visible = false;
            textBox1.Enabled = false; textBox1.Visible = false; szov1 += jatekos1b+" ";
            label20.Enabled = false; label20.Visible = false;
            textBox2.Enabled = false; textBox2.Visible = false; szov1 += jatekos2b;
            if (radioButton3.Checked) { kekkezdb = false; szov1 += " Piros kezd "; }
            if (radioButton4.Checked) { kekkezdb = true; szov1 += " Kék kezd "; }
            groupBox2.Enabled = false; groupBox2.Visible = false;
            if (radioButton5.Checked) { kezdob = true; haladob = false; mesterb = false; szov1 += " Kezdő"; szintkijel = "Kezdő"; }
            if (radioButton6.Checked) { kezdob = false; haladob = true; mesterb = false; szov1 += " Haladó "; szintkijel = "Haladó"; }
            if (radioButton7.Checked) { kezdob = false; haladob = false; mesterb = true; szov1 += " Mester"; szintkijel = "Mester"; }
            groupBox3.Enabled = false; groupBox3.Visible = false;
            button1.Enabled = false; button1.Visible = false;
            button4.Enabled = false; button4.Visible = false;
            label18.Text = szov1;
            ujrakezd = false;
            if (jj!=jjb) {ujrakezd=true;}
            if (jszb && ((kezdo != kezdob) | (halado != haladob) | (mester != mesterb))) { ujrakezd = true; }
            if (kekkezd != kekkezdb) { ujrakezd = true; }
            if ((jj) & (jatekos1b.Length > 0) & (jatekos1 != jatekos1b)) { ujrakezd = true; }
            if ((jj) & (jatekos2b.Length > 0) & (jatekos2 != jatekos2b)) { ujrakezd = true; }
            if (jjb) { jj = true; jsz = false; kekkezd = kekkezdb; jatekos1 = jatekos1b; jatekos2 = jatekos2b; kezdo = kezdob; halado = haladob; mester = mesterb; }
            if (jszb) { jj = false; jsz = true; kekkezd = kekkezdb; jatekos1 = jatekos1b; jatekos2 = jatekos2b; kezdo = kezdob; halado = haladob; mester = mesterb; }
            if (ujrakezd) { erpiros=0;erkek=0; kezd();frissit();}
            if (jj) { label21.Text = "Állás: Piros Játékos1 / " + jatekos1 + " - Kék: Játékos2 / " + jatekos2 + " : " + Convert.ToString(erpiros) + " - " + Convert.ToString(erkek); }
            if (jsz) { label21.Text = "Állás: Piros Játékos1 / " + jatekos1 + " - Kék: Számítógép : " + Convert.ToString(erpiros) + " - " + Convert.ToString(erkek); }

            
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e) //Számítógép szint Kezdő
        { kezdob = true; haladob = false; mesterb = false; }

        private void radioButton6_CheckedChanged(object sender, EventArgs e) //Számítógép szint Haladó
        { kezdob = false; haladob = true; mesterb = false; }

        private void radioButton7_CheckedChanged(object sender, EventArgs e) //Számítógép szint Mester
        { kezdob = false; haladob = false; mesterb = true; }

        private void button5_Click(object sender, EventArgs e) //Új parti
        {
            if (kekkezd) { kekkezd = false; }
            else { kekkezd = true; }
            kezd();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!elemzesfileba) { elemzesfileba = true; button6.Text = "Elemzés Ki"; }
            else { elemzesfileba = false; button6.Text = "Elemzés Be"; }
        }

        private void kezd0()
        {
            byte sz1 = 0; byte sz2 = 0; byte i1 = 0; byte i2 = 0; this.Text = "Golyós játék   Verzió: 2.1";
            rudx[1] = 182; rudy[1] = 340; rudx[2] = 308; rudy[2] = 340; rudx[3] = 434; rudy[3] = 340; rudx[4] = 560; rudy[4] = 340; //rúdak x-y koordinátái
            rudx[5] = 140; rudy[5] = 260; rudx[6] = 266; rudy[6] = 260; rudx[7] = 392; rudy[7] = 260; rudx[8] = 518; rudy[8] = 260;
            rudx[9] = 98; rudy[9] = 180; rudx[10] = 224; rudy[10] = 180; rudx[11] = 350; rudy[11] = 180; rudx[12] = 476; rudy[12] = 180;
            rudx[13] = 56; rudy[13] = 100; rudx[14] = 182; rudy[14] = 100; rudx[15] = 308; rudy[15] = 100; rudx[16] = 434; rudy[16] = 100;
            suly[0] = 30000; //K+  győzelem egy lépésben
            suly[1] = -27000; // J-  győzelem egy lépésben
            suly[2] = 25000; //K+ nyerő kombináció (az ellenfélnek egyszerre kettőt kell tennie)
            suly[3] = -22500; //P- nyerő kombináció (a számítógépnek egyszerre kettőt kell tennie)
            suly[4] = 20000; //K+ nyerő kombináció (az ellenfélnek egymás fölé egyszerre kettőt kellene tennie)
            suly[5] = -18000; //P- nyerő kombináció (a számítógépnek egymás fölé egyszerre kettőt kellene tennie)
            suly[6] = 3000; //  K+ lehetséges nyerő vonalon már három golyó van és egy hiányzik
            suly[7] = -2700; // P- lehetséges nyerő vonalon már három golyó van és egy hiányzik
            suly[8] = 1000; //  K+ lehetséges nyerő vonalon már három golyó van és kettő hiányzik
            suly[9] = -900; //  P- lehetséges nyerő vonalon már három golyó van és kettő hiányzik
            suly[10] = 800; //  K+ lehetséges nyerő vonalon már három golyó van és három hiányzik
            suly[11] = -720; //  P- lehetséges nyerő vonalon már három golyó van és három hiányzik
            suly[12] = 600; //  K+ lehetséges nyerő vonalon már három golyó van és négy hiányzik
            suly[13] = -540; //  P- lehetséges nyerő vonalon már három golyó van és négy hiányzik
            suly[14] = 300; // K+  lehetséges nyerő vonalak száma (ahol már legalább két golyó van)
            suly[15] = -270; // P-  lehetséges nyerő vonalak száma (ahol már legalább két golyó van)
            suly[16] = 100; // K+ lehetséges nyerő vonalak száma
            suly[17] = -100; // P- lehetséges nyerő vonalak száma
            suly[18] = 50; // P+ tilt lépések száma
            suly[19] = -45; // K+ tilt lépések száma
            suly[20] = 15; // K+ lépés pozicíó
            suly[21] = -14; // P- lépés pozicíó
            hx[1] = rudx[1]; hx[1] -= 16; hy[1] = rudy[1]; hy[1] += 140; //hx[k] k. pozíció x koordinátája, hy[k] k. pozíció y koordinátája
            hx[17] = hx[1]; hy[17] = rudy[1]; hy[17] += 100;
            hx[33] = hx[1]; hy[33] = rudy[1]; hy[33] += 60;
            hx[49] = hx[1]; hy[49] = rudy[1]; hy[49] += 20;
            hx[2] = rudx[2]; hx[2] -= 16; hy[2] = rudy[2]; hy[2] += 140;
            hx[18] = hx[2]; hy[18] = rudy[2]; hy[18] += 100;
            hx[34] = hx[2]; hy[34] = rudy[2]; hy[34] += 60;
            hx[50] = hx[2]; hy[50] = rudy[2]; hy[50] += 20;
            hx[3] = rudx[3]; hx[3] -= 16; hy[3] = rudy[3]; hy[3] += 140;
            hx[19] = hx[3]; hy[19] = rudy[3]; hy[19] += 100;
            hx[35] = hx[3]; hy[35] = rudy[3]; hy[35] += 60;
            hx[51] = hx[3]; hy[51] = rudy[3]; hy[51] += 20;
            hx[4] = rudx[4]; hx[4] -= 16; hy[4] = rudy[4]; hy[4] += 140;
            hx[20] = hx[4]; hy[20] = rudy[4]; hy[20] += 100;
            hx[36] = hx[4]; hy[36] = rudy[4]; hy[36] += 60;
            hx[52] = hx[4]; hy[52] = rudy[4]; hy[52] += 20;
            hx[5] = rudx[5]; hx[5] -= 16; hy[5] = rudy[5]; hy[5] += 140;
            hx[21] = hx[5]; hy[21] = rudy[5]; hy[21] += 100;
            hx[37] = hx[5]; hy[37] = rudy[5]; hy[37] += 60;
            hx[53] = hx[5]; hy[53] = rudy[5]; hy[53] += 20;
            hx[6] = rudx[6]; hx[6] -= 16; hy[6] = rudy[6]; hy[6] += 140;
            hx[22] = hx[6]; hy[22] = rudy[6]; hy[22] += 100;
            hx[38] = hx[6]; hy[38] = rudy[6]; hy[38] += 60;
            hx[54] = hx[6]; hy[54] = rudy[6]; hy[54] += 20;
            hx[7] = rudx[7]; hx[7] -= 16; hy[7] = rudy[7]; hy[7] += 140;
            hx[23] = hx[7]; hy[23] = rudy[7]; hy[23] += 100;
            hx[39] = hx[7]; hy[39] = rudy[7]; hy[39] += 60;
            hx[55] = hx[7]; hy[55] = rudy[7]; hy[55] += 20;
            hx[8] = rudx[8]; hx[8] -= 16; hy[8] = rudy[8]; hy[8] += 140;
            hx[24] = hx[8]; hy[24] = rudy[8]; hy[24] += 100;
            hx[40] = hx[8]; hy[40] = rudy[8]; hy[40] += 60;
            hx[56] = hx[8]; hy[56] = rudy[8]; hy[56] += 20;
            hx[9] = rudx[9]; hx[9] -= 16; hy[9] = rudy[9]; hy[9] += 140;
            hx[25] = hx[9]; hy[25] = rudy[9]; hy[25] += 100;
            hx[41] = hx[9]; hy[41] = rudy[9]; hy[41] += 60;
            hx[57] = hx[9]; hy[57] = rudy[9]; hy[57] += 20;
            hx[10] = rudx[10]; hx[10] -= 16; hy[10] = rudy[10]; hy[10] += 140;
            hx[26] = hx[10]; hy[26] = rudy[10]; hy[26] += 100;
            hx[42] = hx[10]; hy[42] = rudy[10]; hy[42] += 60;
            hx[58] = hx[10]; hy[58] = rudy[10]; hy[58] += 20;
            hx[11] = rudx[11]; hx[11] -= 16; hy[11] = rudy[11]; hy[11] += 140;
            hx[27] = hx[11]; hy[27] = rudy[11]; hy[27] += 100;
            hx[43] = hx[11]; hy[43] = rudy[11]; hy[43] += 60;
            hx[59] = hx[11]; hy[59] = rudy[11]; hy[59] += 20;
            hx[12] = rudx[12]; hx[12] -= 16; hy[12] = rudy[12]; hy[12] += 140;
            hx[28] = hx[12]; hy[28] = rudy[12]; hy[28] += 100;
            hx[44] = hx[12]; hy[44] = rudy[12]; hy[44] += 60;
            hx[60] = hx[12]; hy[60] = rudy[12]; hy[60] += 20;
            hx[13] = rudx[13]; hx[13] -= 16; hy[13] = rudy[13]; hy[13] += 140;
            hx[29] = hx[13]; hy[29] = rudy[13]; hy[29] += 100;
            hx[45] = hx[13]; hy[45] = rudy[13]; hy[45] += 60;
            hx[61] = hx[13]; hy[61] = rudy[13]; hy[61] += 20;
            hx[14] = rudx[14]; hx[14] -= 16; hy[14] = rudy[14]; hy[14] += 140;
            hx[30] = hx[14]; hy[30] = rudy[14]; hy[30] += 100;
            hx[46] = hx[14]; hy[46] = rudy[14]; hy[46] += 60;
            hx[62] = hx[14]; hy[62] = rudy[14]; hy[62] += 20;
            hx[15] = rudx[15]; hx[15] -= 16; hy[15] = rudy[15]; hy[15] += 140;
            hx[31] = hx[15]; hy[31] = rudy[15]; hy[31] += 100;
            hx[47] = hx[15]; hy[47] = rudy[15]; hy[47] += 60;
            hx[63] = hx[15]; hy[63] = rudy[15]; hy[63] += 20;
            hx[16] = rudx[16]; hx[16] -= 16; hy[16] = rudy[16]; hy[16] += 140;
            hx[32] = hx[16]; hy[32] = rudy[16]; hy[32] += 100;
            hx[48] = hx[16]; hy[48] = rudy[16]; hy[48] += 60;
            hx[64] = hx[16]; hy[64] = rudy[16]; hy[64] += 20;
            rudfelirat[0, 1] = 'A'; rudfelirat[0, 2] = 'B'; rudfelirat[0, 3] = 'C'; rudfelirat[0, 4] = 'D';
            rudfelirat[0, 5] = 'E'; rudfelirat[0, 6] = 'F'; rudfelirat[0, 7] = 'G'; rudfelirat[0, 8] = 'H';
            rudfelirat[0, 9] = 'I'; rudfelirat[0, 10] = 'J'; rudfelirat[0, 11] = 'K'; rudfelirat[0, 12] = 'L';
            rudfelirat[0, 13] = 'M'; rudfelirat[0, 14] = 'N'; rudfelirat[0, 15] = 'O'; rudfelirat[0, 16] = 'P';
            rudfelirat[1, 1] = 'M'; rudfelirat[1, 2] = 'I'; rudfelirat[1, 3] = 'E'; rudfelirat[1, 4] = 'A';
            rudfelirat[1, 5] = 'N'; rudfelirat[1, 6] = 'J'; rudfelirat[1, 7] = 'F'; rudfelirat[1, 8] = 'B';
            rudfelirat[1, 9] = 'O'; rudfelirat[1, 10] = 'K'; rudfelirat[1, 11] = 'G'; rudfelirat[1, 12] = 'C';
            rudfelirat[1, 13] = 'P'; rudfelirat[1, 14] = 'L'; rudfelirat[1, 15] = 'H'; rudfelirat[1, 16] = 'D';
            rudfelirat[2, 1] = 'P'; rudfelirat[2, 2] = 'O'; rudfelirat[2, 3] = 'N'; rudfelirat[2, 4] = 'M';
            rudfelirat[2, 5] = 'L'; rudfelirat[2, 6] = 'K'; rudfelirat[2, 7] = 'J'; rudfelirat[2, 8] = 'I';
            rudfelirat[2, 9] = 'H'; rudfelirat[2, 10] = 'G'; rudfelirat[2, 11] = 'F'; rudfelirat[2, 12] = 'E';
            rudfelirat[2, 13] = 'D'; rudfelirat[2, 14] = 'C'; rudfelirat[2, 15] = 'B'; rudfelirat[2, 16] = 'A';
            rudfelirat[3, 1] = 'D'; rudfelirat[3, 2] = 'H'; rudfelirat[3, 3] = 'L'; rudfelirat[3, 4] = 'P';
            rudfelirat[3, 5] = 'C'; rudfelirat[3, 6] = 'G'; rudfelirat[3, 7] = 'K'; rudfelirat[3, 8] = 'O';
            rudfelirat[3, 9] = 'B'; rudfelirat[3, 10] = 'F'; rudfelirat[3, 11] = 'J'; rudfelirat[3, 12] = 'N';
            rudfelirat[3, 13] = 'A'; rudfelirat[3, 14] = 'E'; rudfelirat[3, 15] = 'I'; rudfelirat[3, 16] = 'M';
            for (byte i = 1; i < 65; i++) { kijtolt[0, i] = i; };
            kijtolt[1, 1] = 13; kijtolt[1, 2] = 9; kijtolt[1, 3] = 5; kijtolt[1, 4] = 1;
            kijtolt[1, 5] = 14; kijtolt[1, 6] = 10; kijtolt[1, 7] = 6; kijtolt[1, 8] = 2;
            kijtolt[1, 9] = 15; kijtolt[1, 10] = 11; kijtolt[1, 11] = 7; kijtolt[1, 12] = 3;
            kijtolt[1, 13] = 16; kijtolt[1, 14] = 12; kijtolt[1, 15] = 8; kijtolt[1, 16] = 4;
            kijtolt[2, 1] = 16; kijtolt[2, 2] = 15; kijtolt[2, 3] = 14; kijtolt[2, 4] = 13;
            kijtolt[2, 5] = 12; kijtolt[2, 6] = 11; kijtolt[2, 7] = 10; kijtolt[2, 8] = 9;
            kijtolt[2, 9] = 8; kijtolt[2, 10] = 7; kijtolt[2, 11] = 6; kijtolt[2, 12] = 5;
            kijtolt[2, 13] = 4; kijtolt[2, 14] = 3; kijtolt[2, 15] = 2; kijtolt[2, 16] = 1;
            kijtolt[3, 1] = 4; kijtolt[3, 2] = 8; kijtolt[3, 3] = 12; kijtolt[3, 4] = 16;
            kijtolt[3, 5] = 3; kijtolt[3, 6] = 7; kijtolt[3, 7] = 11; kijtolt[3, 8] = 15;
            kijtolt[3, 9] = 2; kijtolt[3, 10] = 6; kijtolt[3, 11] = 10; kijtolt[3, 12] = 14;
            kijtolt[3, 13] = 1; kijtolt[3, 14] = 5; kijtolt[3, 15] = 9; kijtolt[3, 16] = 13;
            for (byte i = 17; i < 65; i++)
            {
                sz1 = i; sz1 -= 16;
                sz2 = kijtolt[1, sz1]; sz2 += 16; kijtolt[1, i] = sz2;
                sz2 = kijtolt[2, sz1]; sz2 += 16; kijtolt[2, i] = sz2;
                sz2 = kijtolt[3, sz1]; sz2 += 16; kijtolt[3, i] = sz2;
            }
            kod[1] = "A"; kod[2] = "B"; kod[3] = "C"; kod[4] = "D"; kod[5] = "E"; kod[6] = "F"; kod[7] = "G"; kod[8] = "H";
            kod[9] = "I"; kod[10] = "J"; kod[11] = "K"; kod[12] = "L"; kod[13] = "M"; kod[14] = "N"; kod[15] = "O"; kod[16] = "P"; kod[0] = " ";
            i1 = 0; for (byte i = 2; 5 > i; i++)
            { i1 += 16; for (byte k = 1; 17 > k; k++) { i2 = k; i2 += i1; kod[i2] = kod[k] + Convert.ToString(i); } }
            for (byte i = 1; 17 > i; i++) { kod[i] += "1"; }
            szab[1, 1] = 1; szab[1, 2] = 2; szab[1, 3] = 3; szab[1, 4] = 4; // def.: hogy az egyes egeneseknek mik a pontjaik
            szab[2, 1] = 5; szab[2, 2] = 6; szab[2, 3] = 7; szab[2, 4] = 8;
            szab[3, 1] = 9; szab[3, 2] = 10; szab[3, 3] = 11; szab[3, 4] = 12;
            szab[4, 1] = 13; szab[4, 2] = 14; szab[4, 3] = 15; szab[4, 4] = 16;
            szab[5, 1] = 1; szab[5, 2] = 5; szab[5, 3] = 9; szab[5, 4] = 13;
            szab[6, 1] = 2; szab[6, 2] = 6; szab[6, 3] = 10; szab[6, 4] = 14;
            szab[7, 1] = 3; szab[7, 2] = 7; szab[7, 3] = 11; szab[7, 4] = 15;
            szab[8, 1] = 4; szab[8, 2] = 8; szab[8, 3] = 12; szab[8, 4] = 16;
            szab[9, 1] = 4; szab[9, 2] = 7; szab[9, 3] = 10; szab[9, 4] = 13;
            szab[10, 1] = 1; szab[10, 2] = 6; szab[10, 3] = 11; szab[10, 4] = 16;
            szab[11, 1] = 17; szab[11, 2] = 18; szab[11, 3] = 19; szab[11, 4] = 20;
            szab[12, 1] = 21; szab[12, 2] = 22; szab[12, 3] = 23; szab[12, 4] = 24;
            szab[13, 1] = 25; szab[13, 2] = 26; szab[13, 3] = 27; szab[13, 4] = 28;
            szab[14, 1] = 29; szab[14, 2] = 30; szab[14, 3] = 31; szab[14, 4] = 32;
            szab[15, 1] = 17; szab[15, 2] = 21; szab[15, 3] = 25; szab[15, 4] = 29;
            szab[16, 1] = 18; szab[16, 2] = 22; szab[16, 3] = 26; szab[16, 4] = 30;
            szab[17, 1] = 19; szab[17, 2] = 23; szab[17, 3] = 27; szab[17, 4] = 31;
            szab[18, 1] = 20; szab[18, 2] = 24; szab[18, 3] = 28; szab[18, 4] = 32;
            szab[19, 1] = 20; szab[19, 2] = 23; szab[19, 3] = 26; szab[19, 4] = 29;
            szab[20, 1] = 17; szab[20, 2] = 22; szab[20, 3] = 27; szab[20, 4] = 32;
            szab[21, 1] = 33; szab[21, 2] = 34; szab[21, 3] = 35; szab[21, 4] = 36;
            szab[22, 1] = 37; szab[22, 2] = 38; szab[22, 3] = 39; szab[22, 4] = 40;
            szab[23, 1] = 41; szab[23, 2] = 42; szab[23, 3] = 43; szab[23, 4] = 44;
            szab[24, 1] = 45; szab[24, 2] = 46; szab[24, 3] = 47; szab[24, 4] = 48;
            szab[25, 1] = 33; szab[25, 2] = 37; szab[25, 3] = 41; szab[25, 4] = 45;
            szab[26, 1] = 34; szab[26, 2] = 38; szab[26, 3] = 42; szab[26, 4] = 46;
            szab[27, 1] = 35; szab[27, 2] = 39; szab[27, 3] = 43; szab[27, 4] = 47;
            szab[28, 1] = 36; szab[28, 2] = 40; szab[28, 3] = 44; szab[28, 4] = 48;
            szab[29, 1] = 36; szab[29, 2] = 39; szab[29, 3] = 42; szab[29, 4] = 45;
            szab[30, 1] = 33; szab[30, 2] = 38; szab[30, 3] = 43; szab[30, 4] = 48;
            szab[31, 1] = 52; szab[31, 2] = 55; szab[31, 3] = 58; szab[31, 4] = 61;
            szab[32, 1] = 49; szab[32, 2] = 54; szab[32, 3] = 59; szab[32, 4] = 64;
            szab[33, 1] = 1; szab[33, 2] = 17; szab[33, 3] = 33; szab[33, 4] = 49;
            szab[34, 1] = 2; szab[34, 2] = 18; szab[34, 3] = 34; szab[34, 4] = 50;
            szab[35, 1] = 3; szab[35, 2] = 19; szab[35, 3] = 35; szab[35, 4] = 51;
            szab[36, 1] = 4; szab[36, 2] = 20; szab[36, 3] = 36; szab[36, 4] = 52;
            szab[37, 1] = 5; szab[37, 2] = 21; szab[37, 3] = 37; szab[37, 4] = 53;
            szab[38, 1] = 6; szab[38, 2] = 22; szab[38, 3] = 38; szab[38, 4] = 54;
            szab[39, 1] = 7; szab[39, 2] = 23; szab[39, 3] = 39; szab[39, 4] = 55;
            szab[40, 1] = 8; szab[40, 2] = 24; szab[40, 3] = 40; szab[40, 4] = 56;
            szab[41, 1] = 9; szab[41, 2] = 25; szab[41, 3] = 41; szab[41, 4] = 57;
            szab[42, 1] = 10; szab[42, 2] = 26; szab[42, 3] = 42; szab[42, 4] = 58;
            szab[43, 1] = 11; szab[43, 2] = 27; szab[43, 3] = 43; szab[43, 4] = 59;
            szab[44, 1] = 12; szab[44, 2] = 28; szab[44, 3] = 44; szab[44, 4] = 60;
            szab[45, 1] = 13; szab[45, 2] = 29; szab[45, 3] = 45; szab[45, 4] = 61;
            szab[46, 1] = 14; szab[46, 2] = 30; szab[46, 3] = 46; szab[46, 4] = 62;
            szab[47, 1] = 15; szab[47, 2] = 31; szab[47, 3] = 47; szab[47, 4] = 63;
            szab[48, 1] = 16; szab[48, 2] = 32; szab[48, 3] = 48; szab[48, 4] = 64;
            szab[49, 1] = 49; szab[49, 2] = 50; szab[49, 3] = 51; szab[49, 4] = 52;
            szab[50, 1] = 53; szab[50, 2] = 54; szab[50, 3] = 55; szab[50, 4] = 56;
            szab[51, 1] = 57; szab[51, 2] = 58; szab[51, 3] = 59; szab[51, 4] = 60;
            szab[52, 1] = 61; szab[52, 2] = 62; szab[52, 3] = 63; szab[52, 4] = 64;
            szab[53, 1] = 49; szab[53, 2] = 53; szab[53, 3] = 57; szab[53, 4] = 61;
            szab[54, 1] = 50; szab[54, 2] = 54; szab[54, 3] = 58; szab[54, 4] = 62;
            szab[55, 1] = 51; szab[55, 2] = 55; szab[55, 3] = 59; szab[55, 4] = 63;
            szab[56, 1] = 52; szab[56, 2] = 56; szab[56, 3] = 60; szab[56, 4] = 64;
            szab[57, 1] = 1; szab[57, 2] = 18; szab[57, 3] = 35; szab[57, 4] = 52;
            szab[58, 1] = 5; szab[58, 2] = 22; szab[58, 3] = 39; szab[58, 4] = 56;
            szab[59, 1] = 9; szab[59, 2] = 26; szab[59, 3] = 43; szab[59, 4] = 60;
            szab[60, 1] = 13; szab[60, 2] = 30; szab[60, 3] = 47; szab[60, 4] = 64;
            szab[61, 1] = 4; szab[61, 2] = 19; szab[61, 3] = 34; szab[61, 4] = 49;
            szab[62, 1] = 8; szab[62, 2] = 23; szab[62, 3] = 38; szab[62, 4] = 53;
            szab[63, 1] = 12; szab[63, 2] = 27; szab[63, 3] = 42; szab[63, 4] = 57;
            szab[64, 1] = 16; szab[64, 2] = 31; szab[64, 3] = 46; szab[64, 4] = 61;
            szab[65, 1] = 1; szab[65, 2] = 21; szab[65, 3] = 41; szab[65, 4] = 61;
            szab[66, 1] = 2; szab[66, 2] = 22; szab[66, 3] = 42; szab[66, 4] = 62;
            szab[67, 1] = 3; szab[67, 2] = 23; szab[67, 3] = 43; szab[67, 4] = 63;
            szab[68, 1] = 4; szab[68, 2] = 24; szab[68, 3] = 44; szab[68, 4] = 64;
            szab[69, 1] = 13; szab[69, 2] = 25; szab[69, 3] = 37; szab[69, 4] = 49;
            szab[70, 1] = 14; szab[70, 2] = 26; szab[70, 3] = 38; szab[70, 4] = 50;
            szab[71, 1] = 15; szab[71, 2] = 27; szab[71, 3] = 39; szab[71, 4] = 51;
            szab[72, 1] = 16; szab[72, 2] = 28; szab[72, 3] = 40; szab[72, 4] = 52;
            szab[73, 1] = 4; szab[73, 2] = 23; szab[73, 3] = 42; szab[73, 4] = 61;
            szab[74, 1] = 13; szab[74, 2] = 26; szab[74, 3] = 39; szab[74, 4] = 52;
            szab[75, 1] = 1; szab[75, 2] = 22; szab[75, 3] = 43; szab[75, 4] = 64;
            szab[76, 1] = 16; szab[76, 2] = 27; szab[76, 3] = 38; szab[76, 4] = 49;
            for (byte i = 1; i < 65; i++)
            {
                poz[i] = 4; i1 = 1;
                for (byte k = 1; k < 77; k++) { for (byte k1 = 1; k1 < 5; k1++) { if (szab[k, k1] == i) { pozszab[i, i1] = k; i1++; } } } //egy adott pozícióhoz, mely szabályok tartoznak 4 vagy hét
            }
            poz[1] = 7; poz[4] = 7; poz[13] = 7; poz[16] = 7; //egy adott pozíción hány egyenes megy át
            poz[22] = 7; poz[23] = 7; poz[26] = 7; poz[27] = 7;
            poz[38] = 7; poz[39] = 7; poz[42] = 7; poz[43] = 7;
            poz[49] = 7; poz[52] = 7; poz[61] = 7; poz[64] = 7;

            for (byte i = 0; 3 > i; i++) { for (byte j = 0; 8 > j; j++) { er1[i, j] = 0; } }
            er1[1, 4] = 1; er1[1, 7] = 1; er1[2, 5] = 1; er1[2, 6] = 1; er1[3, 6] = 1; er1[3, 7] = 1;

            for (byte i = 0; 16 > i; i++) { for (byte j = 0; 8 > j; j++) { er2[i, j] = 0; } }
            er2[10, 0] = 1; er2[11, 0] = 1; er2[5, 1] = 1; er2[7, 1] = 1; er2[1, 2] = 1; er2[3, 2] = 1; er2[2, 3] = 1; er2[3, 3] = 1;
            for (byte j = 4; 8 > j; j++) { er2[j, 4] = 1; er2[j, 7] = 1; }
            for (byte j = 8; 12 > j; j++) { er2[j, 5] = 1; er2[j, 6] = 1; }
            for (byte j = 12; 16 > j; j++) { er2[j, 6] = 1; er2[j, 7] = 1; }
            
            for (byte i = 0; 64 > i; i++) { for (byte j = 0; 8 > j; j++) { er3[i, j] = 0; } }
            er3[10, 0] = 1; er3[11, 0] = 1; er3[26, 0] = 1; er3[27, 0] = 1; for (byte i = 40; 48 > i; i++) { er3[i, 0] = 1; }
            er3[5, 1] = 1; er3[7, 1] = 1; for (byte i = 20; 24 > i; i++) { er3[i, 1] = 1; er3[(i + 8), 1] = 1; } er3[37, 1] = 1; er3[39, 1] = 1;
            for (byte i = 4; 8 > i; i++) { er3[i, 2] = 1; er3[(i + 8), 2] = 1; }
            for (byte i = 8; 16 > i; i++) { er3[i, 3] = 1; }
            for (byte i = 16; 32 > i; i++) { er3[i, 4] = 1; }
            for (byte i = 32; 48 > i; i++) { er3[i, 5] = 1; }
            for (byte i = 16; 32 > i; i++) { er3[i, 7] = 1; er3[(i + 16), 6] = 1; }
            for (byte i = 48; 64 > i; i++) { er3[i, 6] = 1; er3[i, 7] = 1; }

            for (Int16 i = 0; 255 >= i; i++) { for (byte j = 0; 8 > j; j++) { er4[i, j] = 0; } }
            er4[10, 0] = 1; er4[11, 0] = 1; er4[26, 0] = 1; er4[27, 0] = 1; er4[74, 0] = 1; er4[75, 0] = 1; 
            er4[138, 0] = 1; er4[139, 0] = 1; er4[154, 0] = 1; er4[155, 0] = 1;
            for (byte i = 40; 48 > i; i++) { er4[i, 0] = 1; er4[(i + 64), 0] = 1; } for (byte i = 160; 192 > i; i++) { er4[i, 0] = 1; }
            er4[5, 1] = 1; er4[7, 1] = 1; er4[37, 1] = 1; er4[39, 1] = 1; er4[69, 1] = 1; er4[71, 1] = 1; 
            er4[101, 1] = 1; er4[103, 1] = 1; er4[133, 1] = 1; er4[135, 1] = 1;
            for (byte i = 20; 24 > i; i++) { er4[i, 1] = 1; er4[(i + 8), 1] = 1; er4[(i + 128), 1] = 1; er4[(i + 136), 1] = 1; } 
            for (byte i = 80; 96 > i; i++) { er4[i, 1] = 1; er4[(i + 32), 1] = 1; }
            for (byte i = 16; 32 > i; i++) { er4[i, 2] = 1; er4[(i + 32), 2] = 1; er4[(i + 16), 3] = 1; er4[(i + 32), 3] = 1; }

            for (byte i = 64; 128 > i; i++) { er4[i, 4] = 1; er4[(i + 64), 5] = 1; er4[i, 7] = 1; er4[(i + 64), 6] = 1; er4[(i+128), 6] = 1; er4[(i + 128), 7] = 1; }
            
        }
        private void teszt()
        {
            byte kegy = 0; byte r1 = 0;
            int hossz = 1; String szov1 = ""; String szov2 = "";
            szov1 = Convert.ToString(DateTime.Now); hossz = szov1.Length;
            szov2 = konyvtar + "gj_teszt_" + szov1[0] + szov1[1] + szov1[2] + szov1[3] + szov1[5] + szov1[6] + szov1[8] + szov1[9] + "_" + szov1[12];
            for (byte i = 13; i < 18; i++) { if (szov1[i] != ':') { szov2 += szov1[i]; } } szov2 += ".txt";
            StreamWriter sw = new StreamWriter(szov2);
            sw.WriteLine("Golyós játék teszt állás"); sw.WriteLine(" ");

            kek[4] = 1; kek[5] = 1; kek[7] = 1; kek[8] = 1; kek[13] = 1;
            piros[2] = 1; piros[12] = 1; piros[14] = 1; piros[15] = 1;
            for (byte i = 1; 77 > i; i++) { kv1[i, 0] = 1; kv[i, 1] = 0; pv1[i, 0] = 1; pv1[i, 1] = 0; }
            for (byte i = 1; 17 > i; i++) { rud1[i, 0] = 4; rud1[i, 1] = i; }
            for (byte i = 1; 65 > i; i++)
            {
                if (kek[i] == 1)
                {
                    for (byte j = 1; poz[i] >= j; j++)
                    {
                        kegy = pozszab[i, j];
                        if (kv1[kegy, 0] == 1) { kv1[kegy, 1]++; }
                        pv1[kegy, 0] = 0;
                    }
                    r1 = pozrud[i]; rud1[r1, 0]--; rud1[r1, 1] += 16;
                }
                if (piros[i] == 1)
                {
                    for (byte j = 1; poz[i] >= j; j++)
                    {
                        kegy = pozszab[i, j];
                        if (pv1[kegy, 0] == 1) { pv1[kegy, 1]++; }
                        kv1[kegy, 0] = 0;
                    }
                    r1 = pozrud[i]; rud1[r1, 0]--; rud1[r1, 1] += 16;
                }
            }
            for (byte i = 1; i < 65; i++) { r1 = 0; if (piros[i] == 1) { r1 = 1; } if (kek[i] == 1) { r1 = 2; } t[i] = r1; }
            sw.WriteLine("Tábla:");
            szov1 = Convert.ToString(t[13]) + "," + Convert.ToString(t[14]) + "," + Convert.ToString(t[15]) + "," + Convert.ToString(t[16]) + "," + " ,";
            szov1 += Convert.ToString(t[29]) + "," + Convert.ToString(t[30]) + "," + Convert.ToString(t[31]) + "," + Convert.ToString(t[32]) + "," + " ,";
            szov1 += Convert.ToString(t[45]) + "," + Convert.ToString(t[46]) + "," + Convert.ToString(t[47]) + "," + Convert.ToString(t[48]) + "," + " ,";
            szov1 += Convert.ToString(t[61]) + "," + Convert.ToString(t[62]) + "," + Convert.ToString(t[63]) + "," + Convert.ToString(t[64]);
            sw.WriteLine(szov1);
            szov1 = Convert.ToString(t[9]) + "," + Convert.ToString(t[10]) + "," + Convert.ToString(t[11]) + "," + Convert.ToString(t[12]) + "," + " ,";
            szov1 += Convert.ToString(t[25]) + "," + Convert.ToString(t[26]) + "," + Convert.ToString(t[27]) + "," + Convert.ToString(t[28]) + "," + " ,";
            szov1 += Convert.ToString(t[41]) + "," + Convert.ToString(t[42]) + "," + Convert.ToString(t[43]) + "," + Convert.ToString(t[44]) + "," + " ,";
            szov1 += Convert.ToString(t[57]) + "," + Convert.ToString(t[58]) + "," + Convert.ToString(t[59]) + "," + Convert.ToString(t[60]);
            sw.WriteLine(szov1);
            szov1 = Convert.ToString(t[5]) + "," + Convert.ToString(t[6]) + "," + Convert.ToString(t[7]) + "," + Convert.ToString(t[8]) + "," + " ,";
            szov1 += Convert.ToString(t[21]) + "," + Convert.ToString(t[22]) + "," + Convert.ToString(t[23]) + "," + Convert.ToString(t[24]) + "," + " ,";
            szov1 += Convert.ToString(t[37]) + "," + Convert.ToString(t[38]) + "," + Convert.ToString(t[39]) + "," + Convert.ToString(t[40]) + "," + " ,";
            szov1 += Convert.ToString(t[53]) + "," + Convert.ToString(t[54]) + "," + Convert.ToString(t[55]) + "," + Convert.ToString(t[56]);
            sw.WriteLine(szov1);
            szov1 = Convert.ToString(t[1]) + "," + Convert.ToString(t[2]) + "," + Convert.ToString(t[3]) + "," + Convert.ToString(t[4]) + "," + " ,";
            szov1 += Convert.ToString(t[17]) + "," + Convert.ToString(t[18]) + "," + Convert.ToString(t[19]) + "," + Convert.ToString(t[20]) + "," + " ,";
            szov1 += Convert.ToString(t[33]) + "," + Convert.ToString(t[34]) + "," + Convert.ToString(t[35]) + "," + Convert.ToString(t[36]) + "," + " ,";
            szov1 += Convert.ToString(t[49]) + "," + Convert.ToString(t[50]) + "," + Convert.ToString(t[51]) + "," + Convert.ToString(t[52]);
            sw.WriteLine(szov1);
            sw.WriteLine();
            sw.WriteLine("Kék egyenesek:, '0','1', Piros egyenesek:,'0', '1' ");
            for (byte i = 1; 77 > i; i++) 
            {
                szov1 =Convert.ToString(i)+","+ Convert.ToString(kv1[i, 0]) + "," + Convert.ToString(kv1[i, 1]) + ", ," + Convert.ToString(pv1[i, 0]) + "," + Convert.ToString(pv1[i, 1]);
                sw.WriteLine(szov1);
            }
            sw.WriteLine();
            sw.WriteLine("Rúd pozíciók:, '0', '1',");
            for (byte i = 1; 17 > i; i++) { sw.WriteLine(Convert.ToString(i)+","+ Convert.ToString(rud1[i, 0]) + "," + Convert.ToString(rud1[i, 1])); }
            sw.Close();
        }

      

    }
}
