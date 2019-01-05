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
    public partial class UI : Form
    {
        Graphics g;
        Rectangle r;
        Pen p;
        SolidBrush ecset;
        private String verzio = " Golyós játék   Verzió: otello_1.2 "; // verzió szám
        private GameTable gameTable;
        List<char> readingOrder;

        private int[] rudx = new int[16];  //kijelzéshez szükséges x koordinátát tartalmazza
        private int[] rudy = new int[16];  //kijelzéshez szükséges y koordinátát tartalmazza

        private Player piros; //Players are entities so they are represented with a class
        private Player kek;  //Players are entities so they are represented with a class


        // static String konyvtar = "F:\\golyo\\"; // ebbe a könyvtárba menti a partikat és az elemzéseket


        static Int16[] hx = new Int16[65];
        static Int16[] hy = new Int16[65];
        static char[,] rudfelirat = new char[4, 17];

        static string[] kod = new string[65];//1,17,33,49:'A1-4'; 2,18,34,50:'B1-4'

        static byte[] kij = new byte[65]; // 0:üres; 1: piros; 2: kék;
        static byte[] t = new byte[65]; //1-64 poz értékek 0: ha üres; 1: ha piros; 2: ha kék

        static byte[,] hianydb = new byte[65, 5]; //hiany db szám kiszámításában segéd tábla 
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
        static byte[,] szab = new byte[77, 5]; // 76 egyenes szabálya
        static byte[,] pozszab = new byte[65, 8];//egy adott helyhez tartozó szabályok az
        static byte[] pozrud = new byte[65]; //megadja, hogy egy hely (1-64) hányas rúdon van
        static byte[] poz = new byte[65]; // adott helyhez hány egyenes tartozik 4 vagy 7
        static byte[] gyozegy = new byte[8]; // piros győzelem esetén a győztes egyenesek azonosítóit tartalmazza
        static byte[,] er1 = new byte[4, 8]; //elemzésnél infokat tartalmaz, a rúdra még egy golyó rakható
        static byte[,] er2 = new byte[16, 8]; //elemzésnél infokat tartalmaz, a rúdra még két golyó rakható
        static byte[,] er3 = new byte[64, 8]; //elemzésnél infokat tartalmaz, a rúdra még három golyó rakható
        static byte[,] er4 = new byte[256, 8]; //elemzésnél infokat tartalmaz, a rúdra még négy golyó rakható

        static Int16[] suly = new Int16[22];

        static Int16 visszap = 0; //piros visszavon lépést
        static Int16 visszak = 0; // kék visszavon lépést
        static Int16 visszadb = 0; //  visszavonás után számláló

        static byte hianylepsz = 0;
        static byte egy = 0;
        static byte p0 = 0; // egyenes vizsgálandó pontja
        static byte p1 = 0; // az egyenes 1. pontja
        static byte p2 = 0; // az egyenes 2. pontja
        static byte p3 = 0; // az egyenes 3. pontja
        static byte p4 = 0; // az egyenes 4. pontja
        static byte r1 = 0; // az egyenes 1. pontjának első szabad pontja
        static byte r2 = 0; // az egyenes 2. pontjának első szabad pontja
        static byte r3 = 0; // az egyenes 3. pontjának első szabad pontja
        static byte r4 = 0; // az egyenes 4. pontjának első szabad pontja
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
        static byte k1lep = 0; // elemzésnél a gép első virtuélis lépése
        static byte k2lep = 0; // elemzésnél a gép második virtuélis lépése
        static byte k3lep = 0; // elemzésnél a gép harmadik virtuélis lépése
        static byte p1lep = 0; // elemzésnél a játékos első virtuélis lépése
        static byte p2lep = 0; // elemzésnél a játékos második virtuélis lépése
        static byte p3lep = 0; // elemzésnél a játékos harmadik virtuélis lépése
        static byte lepszamlp = 0;
        static byte lepszamlk = 0;
        static byte kiv = 0; // azt tárolja, hogy melyik forgatásnélküli rúdra klikkeltek 
        static byte kivjo = 0; //a kiválasztott rúd OK (lehet választani)

        static Boolean valaszthat = false; // csak akkor igaz, ha a játékos választhat, hogy melyik rúdra tesz
        static Boolean pgyoz = false; // ha piros nyer, akkor true
        static Boolean kgyoz = false; // ha kek nyer, akkor true
        static Boolean dontetlen = false;
        static Boolean jj = false; static Boolean jjb = false;
        static Boolean jsz = true; static Boolean jszb = true;
        static Boolean kekkezd = false; static Boolean kekkezdb = false;
        static Boolean kezdo = true; static Boolean kezdob = true;
        static Boolean halado = false; static Boolean haladob = false;
        static Boolean mester = false; static Boolean mesterb = false;
        static Boolean ujrakezd = false;
        static Boolean tovpir = false;
    
        static String jatekos1 = "Pandúr"; static String jatekos1b = jatekos1;
        static String jatekos2 = "Betyár"; static String jatekos2b = jatekos2;
        static String plepsor = "";
        static String klepsor = "";
        static String szintkijel = "Kezdő";
        static String szintkijelb = "Kezdő";
        static String szov18 = "";
        static String szov17 = "";
        static String szov22 = "";
        static String szov23 = "";
        static String szov24 = "";
        static String szov25 = "";
        static String szov26 = "";
        static String szov27 = "";
        static String szov28 = "";
        static Int16 erpiros = 0;
        static Int16 erkek = 0;
        static Int16 vez = 3;
        static Int32 szor = 0;
        static Int32 ossz = 0;

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
        Int32 g1lepmax = 0; //  mindig a számítógép addig megvizsgált 1. lépései közül a legnagyobb pontértéket tartalmazza
        byte j1lep = 0; // a játékos első lépése
        byte j1lepnyer = 0; // a játékos 1. nyerő lépése
        byte j1lepsz = 0; // a játékos lépéseinek a száma
        byte j1egy = 0; //egy egyenes azonosítóját tartalmazza
        byte j1r = 0; // a játékos erre a rúdra tesz
        byte szamlnyero = 0; // tartalmazza a lehetséges nyerő egyenesek számát
        Boolean j1nyer = false;
        Int32 j1lepmin = 0; //mindig a játékos addig megvizsgált 1. lépései közül a legkisebb pontértéket tartalmazza
        Boolean j1tov = true;

        byte[,] g2ment = new byte[8, 3]; // gép 2. lépése előtti értékekeket elmentjük
        byte[,] j2ment = new byte[8, 3]; // a játékos 2. lépése előtti értékeket elmentjük
        byte[] g2lepesek = new byte[17]; // a számítógép 2. lehetséges lépéseit tartalmazza
        byte[] j2lepesek = new byte[17]; // a játékos 2. lehetséges lépései
        byte g2lep = 0; // gép második lépése
        byte g2lepsz = 0; // gép 2. lépéseinek a db száma
        byte g2egy = 0; // gép egy egyenes azonosítóját tartalmazza 2. lépés során
        byte g2r = 0; // gép erre a rúdra tesz a második lépésben
        Boolean g2nyer = false;
        Int32 g2lepmax = 0; // mindig a számítógép addig megvizsgált 2. lépései közül a legnagyobb pontértéket tartalmazza
        byte j2lep = 0; // a játékos második lépése
        byte j2lepsz = 0; // a játékos második lépéseinek a száma
        byte j2egy = 0; //egy egyenes azonosítóját tartalmazza 2. lépés során
        byte j2r = 0; // a játékos erre a rúdra tesz a második lépésben
        Boolean j2nyer = false;
        Int32 j2lepmin = 0; //mindig a játékos addig megvizsgált 2. lépései közül a legkisebb pontértéket tartalmazza
        Boolean g2tov = true;
        Boolean j2tov = true;

        byte[,] g3ment = new byte[8, 3]; // gép 3. lépése előtti értékekeket elmentjük
        byte[,] j3ment = new byte[8, 3]; // a játékos 3. lépése előtti értékeket elmentjük
        byte[] g3lepesek = new byte[17]; // a számítógép 3. lehetséges lépéseit tartalmazza
        byte[] j3lepesek = new byte[17]; // a játékos 3. lehetséges lépései
        byte g3lep = 0; // gép 3. lépése
        byte g3lepsz = 0; // gép 3. lépéseinek a db száma
        byte g3egy = 0; // gép egy egyenes azonosítóját tartalmazza 3. lépés során
        byte g3r = 0; // gép erre a rúdra tesz a 3. lépésben
        Boolean g3nyer = false;
        Int32 g3lepmax = 0; // mindig a számítógép addig megvizsgált 3. lépései közül a legnagyobb pontértéket tartalmazza
        byte j3lep = 0; // a játékos 3. lépése
        byte j3lepsz = 0; // a játékos 3. lépéseinek a száma
        byte j3egy = 0; //egy egyenes azonosítóját tartalmazza 3. lépés során
        byte j3r = 0; // a játékos erre a rúdra tesz a 3. lépésben
        Boolean j3nyer = false;
        Int32 j3lepmin = 0; //mindig a játékos addig megvizsgált 3. lépései közül a legkisebb pontértéket tartalmazza
        Boolean g3tov = true;
        Boolean j3tov = true;



        public UI() {

            piros = new Player("Alma", "Piros");
            kek = new Player("Korte", "Kek");

            gameTable = new GameTable(piros,kek);
            readingOrder = new List<char>();
            for (int i= 0;i < 16; i++)
            {
                readingOrder.Add((char)('A'+i));
                Console.WriteLine((char)('A' + i));         //TODO: check if works!!
            }

            InitializeComponent();
        }

        public void teglalap(Brush b, Int16 x1, Int16 y1, Int16 x2, Int16 y2, byte vv)
        { g = CreateGraphics(); p = new Pen(b); p.Width = vv; r = new Rectangle(x1, y1, x2, y2); g.DrawRectangle(p, r); }

        public void tegla(Color ecs, Int16 x1, Int16 y1, Int16 x2, Int16 y2)
        { g = CreateGraphics(); ecset = new SolidBrush(ecs); r = new Rectangle(x1, y1, x2, y2); g.FillRectangle(ecset, r); }

        public void kor(Color ecs, Int16 x1, Int16 y1, Int16 x2, Int16 y2)
        { g = CreateGraphics(); p = new Pen(ecs); g.DrawEllipse(p, x1, y1, x2, y2); }

        public void korlap(Color ecs, int x1, int y1, int x2, int y2)
        { g = CreateGraphics(); ecset = new SolidBrush(ecs); g.FillEllipse(ecset, x1, y1, x2, y2); }

        public void vonal(Brush b, int x1, int y1, int x2, int y2, byte vv)
        { g = CreateGraphics(); p = new Pen(b); p.Width = vv; g.DrawLine(p, x1, y1, x2, y2); }

        public void Refresh()
        {
            Color hatter = BackColor;
            szov28 = Convert.ToString(DateTime.Now);

            tegla(hatter, 2, 28, 630, 602);

            label1.Text = Convert.ToString(rudfelirat[kijvez, 1]); label2.Text = Convert.ToString(rudfelirat[kijvez, 2]);
            label3.Text = Convert.ToString(rudfelirat[kijvez, 3]); label4.Text = Convert.ToString(rudfelirat[kijvez, 4]);
            label5.Text = Convert.ToString(rudfelirat[kijvez, 5]); label6.Text = Convert.ToString(rudfelirat[kijvez, 6]);
            label7.Text = Convert.ToString(rudfelirat[kijvez, 7]); label8.Text = Convert.ToString(rudfelirat[kijvez, 8]);
            label9.Text = Convert.ToString(rudfelirat[kijvez, 9]); label10.Text = Convert.ToString(rudfelirat[kijvez, 10]);
            label11.Text = Convert.ToString(rudfelirat[kijvez, 11]); label12.Text = Convert.ToString(rudfelirat[kijvez, 12]);
            label13.Text = Convert.ToString(rudfelirat[kijvez, 13]); label14.Text = Convert.ToString(rudfelirat[kijvez, 14]);
            label15.Text = Convert.ToString(rudfelirat[kijvez, 15]); label16.Text = Convert.ToString(rudfelirat[kijvez, 16]);
            teglalap(Brushes.Green, 2, 28, 1100, 675, 2);
            vonal(Brushes.Brown, 10, 240, 459, 240, 2); vonal(Brushes.Brown, 178, 560, 627, 560, 2);
            vonal(Brushes.Brown, 10, 240, 178, 560, 2); vonal(Brushes.Brown, 459, 240, 627, 560, 2);
            vonal(Brushes.Brown, 10, 240, 10, 280, 2); vonal(Brushes.Brown, 178, 560, 178, 600, 2);
            vonal(Brushes.Brown, 627, 560, 627, 600, 2); vonal(Brushes.Brown, 10, 280, 178, 600, 2);
            vonal(Brushes.Brown, 178, 600, 627, 600, 2);

            for (int i = 0; i < 16; i++)
            {
                vonal(Brushes.Black, rudx[i], rudy[i], rudx[i], (rudy[i] + 180), 5);
            }

            int poleCounter = 0;
            foreach (char key in readingOrder)
            {
                for (int i = 0; i<gameTable.Table[""+key].Count; i++)
                {
                    if (gameTable.Table["" + key][i])
                    {
                        korlap(Color.Blue, rudx[poleCounter]-16, rudy[poleCounter]+140-i*40, 32, 40);
                    }
                    else {
                        korlap(Color.Red, rudx[poleCounter] - 16, rudy[poleCounter] + 140 - i * 40, 32, 40);
                    }
                    
                }
                poleCounter++;
            }

            //TODO: Make the lastmove yellow

            if (piros.Moves.Count+kek.Moves.Count >= 4) {
                button6.Enabled = true;
                button6.Visible = true;
            }
            else
            {
                button6.Enabled = false;
                button6.Visible = false;
            }


            /*if (pgyoz | kgyoz)
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
                    vonal(Brushes.White, x3, y3, x4, y4, 2); button6.Enabled = false; button6.Visible = false;

                }
            }*/ //TODO: white line

            //TODO: refresh stats for winning should be done elsewhere maybe white line too 

            //TODO: delete warning messages after some moves



        }
      
        private void hiany() // bemenet: egy; kimenet: hianylepsz; (ha egy egyenesen már van három golyó!!!)
        {
            hianylepsz = 0; p1 = szab[egy, 1]; p2 = szab[egy, 2]; p3 = szab[egy, 3]; p4 = szab[egy, 4];
            r1 = rud1[pozrud[p1], 0]; r2 = rud1[pozrud[p2], 0]; r3 = rud1[pozrud[p3], 0]; r4 = rud1[pozrud[p4], 0];
            hianylepsz += hianydb[p1, r1]; hianylepsz += hianydb[p2, r2]; hianylepsz += hianydb[p3, r3]; hianylepsz += hianydb[p4, r4];
         }

        private void szgepkezd()
        {
            timer2.Enabled = false; button6.Visible = false; button6.Enabled = false;
            Boolean tovabb = true; g1lepsz = 0; g1nyer = false; byte k1 = 0; j1nyer = false; lepszamlk++; 
            for (byte i = 1; 17 > i; i++) { rud1[i, 0] = rud[i, 0]; rud1[i, 1] = rud[i, 1]; }
            for (byte i = 1; 77 > i; i++) { pv1[i, 0] = pv[i, 0]; pv1[i, 1] = pv[i, 1]; pv1[i, 2] = pv[i, 2]; kv1[i, 0] = kv[i, 0]; kv1[i, 1] = kv[i, 1]; kv1[i, 2] = kv[i, 2]; }
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
                kek[g1lepnyer] = 1; keklep[lepszamlk] = g1lepnyer; kgyoz = true; klepsor += " " + kod[g1lepnyer]; szov27 = klepsor; t[g1lepnyer] = 2;
                g1r = pozrud[g1lepnyer]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[g1lepnyer] >= j; j++) { g1egy = pozszab[g1lepnyer, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Kék Nyert!"; szov24 = ""; szov25 = "Kék utolsó lépése: " + kod[g1lepnyer];
                erkek += 2; kij[0] = g1lepnyer; frissit(); vez = 6; button5.Enabled = true; button5.Visible = true; tovabb = false;
            }
            k1 = lepszamlk; k1 += lepszamlp;
            if ((tovabb) && (k1 == 64))
            {
                g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep]; t[g1lep] = 2;
                szov27 = klepsor; szov18 = " Utolsó lépés"; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Döntetlen!"; dontetlen = true; szov24 = ""; szov25 = "Kék utolsó lépése: " + kod[g1lep];
                erkek++; erpiros++; kij[0] = g1lep; frissit(); vez = 6; button5.Enabled = true; button5.Visible = true; tovabb = false;
            }
            if ((g1lepsz == 1) && (tovabb))
            {
                g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep]; t[g1lep] = 2;
                szov27 = klepsor; szov18 = "Csak egy rúdra tehet"; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Piros játékos / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; szov24 = "";
                szov25 = "Kék utolsó lépése: " + kod[g1lep]; kij[0] = g1lep; frissit(); vez = 3; valaszthat = true; tovabb = false;
            }
            if (tovabb)
            {
                for (byte i = 1; 17 > i; i++)
                {
                    if (rud1[i, 0] > 0)
                    { j1lep = rud1[i, 1]; for (byte j = 1; poz[j1lep] >= j; j++) { j1egy = pozszab[j1lep, j]; if ((pv1[j1egy, 0] == 1) && (pv1[j1egy, 1] == 3)) { j1lepnyer = j1lep; j1nyer = true; } } }
                }
            }
            if ((tovabb) && (j1nyer))
            {
                keklep[lepszamlk] = j1lepnyer; kek[j1lepnyer] = 1; klepsor += " " + kod[j1lepnyer]; szov27 = klepsor; g1r = pozrud[j1lepnyer]; rud[g1r, 0]--; rud[g1r, 1] += 16; t[j1lepnyer] = 2;
                for (byte j = 1; poz[j1lepnyer] >= j; j++) { g1egy = pozszab[j1lepnyer, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Piros játékos / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; ;
                szov24 = ""; szov25 = "Kék utolsó lépése: " + kod[j1lepnyer]; kij[0] = j1lepnyer; frissit(); vez = 3; tovabb = false; valaszthat = true;
            }
            if (tovabb)
            {
                szamlnyero = 0; for (byte i = 1; 77 > i; i++) { if (pv1[i, 0] == 0) { szamlnyero++; }; if (kv1[i, 0] == 0) { szamlnyero++; } }
                if (szamlnyero == 0)
                {
                    g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep]; t[g1lep] = 2;
                    szov27 = klepsor; szov18 = "Egyik fél sem tud már nyerni, döntetlen"; dontetlen = true; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                    for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                    szov23 = "Piros játékos / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; szov24 = "";
                    szov25 = "Kék utolsó lépése: " + kod[g1lep]; kij[0] = g1lep; frissit(); vez = 3; valaszthat = true; tovabb = false;
                }
            }       
            if (tovabb)
            { //lépés keresés
                for (byte g1 = 1; g1lepsz >= g1; g1++)
                { //1.
                    g1lep = g1lepesek[g1]; g1r = pozrud[g1lep]; rud1[g1r, 0]--; rud1[g1r, 1] += 16;
                    for (byte j = 1; poz[g1lep] >= j; j++)
                    { //2a1
                        g1egy = pozszab[g1lep, j]; g1ment[j, 0] = kv1[g1egy, 0]; g1ment[j, 1] = kv1[g1egy, 1]; g1ment[j, 2] = pv1[g1egy, 0];
                        pv1[g1egy, 0] = 0; if (kv1[g1egy, 0] == 1) { kv1[g1egy, 1]++; }
                    } //2a1
                    j1lepsz = 0; j1nyer = false; j1tov = true;
                    for (byte i = 1; 17 > i; i++)
                    { // 3.
                        if (rud1[i, 0] > 0)
                        { // 4.
                            j1lepsz++; j1lep = rud1[i, 1]; j1lepesek[j1lepsz] = j1lep;
                            for (byte j = 1; poz[j1lep] >= j; j++) { j1egy = pozszab[j1lep, j]; if ((pv1[j1egy, 0] == 1) && (pv1[j1egy, 1] == 3)) { j1nyer = true; } }
                        } // 4.
                    } // 3.
                    if (j1nyer) { j1lepmin = -90000; j1tov = false;  }
                    if (j1lepsz == 0) { j1lepmin = 0; j1tov = false; }
                    if (j1tov)
                    { // 5.
                        for (byte j1 = 1; j1lepsz >= j1; j1++)
                        { // 6.
                            j1lep = j1lepesek[j1]; j1r = pozrud[j1lep]; rud1[j1r, 0]--; rud1[j1r, 1] += 16;
                            for (byte j = 1; poz[j1lep] >= j; j++)
                            { // 7a1
                                j1egy = pozszab[j1lep, j]; j1ment[j, 0] = pv1[j1egy, 0]; j1ment[j, 1] = pv1[j1egy, 1]; j1ment[j, 2] = kv1[j1egy, 0];
                                kv1[j1egy, 0] = 0; if (pv1[j1egy, 0] == 1) { pv1[j1egy, 1]++; }
                            } // 7a1
                            k1lep = g1lep; p1lep = j1lep; elemzes();
                            if (j1 == 1) { j1lepmin = ossz; };
                            if (j1lepmin > ossz) { j1lepmin = ossz; } // a legkisebb értéket tartjuk meg
                            for (byte j = 1; j <= poz[j1lep]; j++)
                            { // 7a2
                                j1egy = pozszab[j1lep, j]; pv1[j1egy, 0] = j1ment[j, 0]; pv1[j1egy, 1] = j1ment[j, 1]; kv1[j1egy, 0] = j1ment[j, 2];
                            } // 7a2
                            rud1[j1r, 0]++; rud1[j1r, 1] -= 16;
                        } // 6.
                    } // 5.
                    for (byte j = 1; poz[g1lep] >= j; j++)
                    { // 2a2
                        g1egy = pozszab[g1lep, j]; kv1[g1egy, 0] = g1ment[j, 0]; kv1[g1egy, 1] = g1ment[j, 1]; pv1[g1egy, 0] = g1ment[j, 2];
                    } // 2a2
                    rud1[g1r, 0]++; rud1[g1r, 1] -= 16;
                    if (g1 == 1) { g1lepmax = j1lepmin; g1lepnyer = g1lep; };
                    if (g1lepmax < j1lepmin) { g1lepmax = j1lepmin; g1lepnyer = g1lep; };
                } //1.
                keklep[lepszamlk] = g1lepnyer; kek[g1lepnyer] = 1; klepsor += " " + kod[g1lepnyer]; t[g1lepnyer] = 2;
                szov27 = klepsor; g1r = pozrud[g1lepnyer]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[g1lepnyer] >= j; j++) { g1egy = pozszab[g1lepnyer, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Piros játékos / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; ;
                szov24 = ""; szov25 = "Kék utolsó lépése: " + kod[g1lepnyer]; kij[0] = g1lepnyer; frissit(); vez = 3; tovabb = false; valaszthat = true;
                p0 = lepszamlp; p0 += lepszamlk;
                if (p0 > 4) { button6.Enabled = true; button6.Visible = true; }
                else { button6.Enabled = false; button6.Visible = false; }
            } //lépés keresés
        }

        private void elemzes() //eredmény az ossz változóban
        {
            byte x1lep = 0; byte x2lep = 0; byte x3lep = 0; byte x4lep = 0; byte xegy = 1; 
            byte x1k = 0; byte x2k = 0; byte x3k = 0; byte x4k = 0;  byte x1p = 0; byte x2p = 0; byte x3p = 0; byte x4p = 0;
            szabad = 0; kkenyszer = 0; pkenyszer = 0; knyer1 = 0; pnyer1 = 0;
            sk3o2 = 0; sk3m2 = 0; sk3h1 = 0; sk3h2 = 0; sk3h3 = 0; sk3h4 = 0; sk2 = 0; sk1 = 0; ktiltott = 0;
            sp3o2 = 0; sp3m2 = 0; sp3h1 = 0; sp3h2 = 0; sp3h3 = 0; sp3h4 = 0; sp2 = 0; sp1 = 0; ptiltott = 0;
            for (byte k = 1; 77 > k; k++)
            { //1.
                if (kv1[k, 0] == 1) // a kék egyenes még működhet
                { //2.
                    if (kv1[k, 1] == 1) { sk1++; }
                    if (kv1[k, 1] == 2) { sk2++; }
                    if (kv1[k, 1] == 3) { egy = k; hiany(); if (hianylepsz == 1) { sk3h1++; }; if (hianylepsz == 2) { sk3h2++; }; if (hianylepsz == 3) { sk3h3++; }; if (hianylepsz == 4) { sk3h4++; }; }
                } //2.
                if (pv1[k, 0] == 1) // a piros egyenes még működhet
                { //3.
                    if (pv1[k, 1] == 2) { sp1++; }
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
                        if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x1k = 1; };
                        if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x1p = 1; };
                    }
                    knyer1 += x1k; pnyer1 += x1p;
                    if ((x1k == 1) && (x1p == 0)) { pkenyszer++; };
                    if ((x1k == 0) && (x1p == 1)) { kkenyszer++; };
                    valasztsor = x1p; if (x1k == 1) { valasztsor += 2; };
                } //5.
                if (rud1[e1, 0] == 2) // a rúdon már két golyó van
                { //6.
                    x1lep = rud1[e1, 1]; x2lep = x1lep; x2lep += 16; szabad++;
                    for (byte j = 1; poz[x1lep] >= j; j++)
                    { //7.
                        xegy = pozszab[x1lep, j];
                        if ((kv1[xegy, 0] == 1) && (kv1[xegy, 1] == 3)) { x1k = 1; };
                        if ((pv1[xegy, 0] == 1) && (pv1[xegy, 1] == 3)) { x1p = 1; };
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
                } //13.
            } //14.
            if (kkenyszer > 1) { sk3o2 = 1; };
            if (pkenyszer > 1) { sp3o2 = 1; }
            if (szabad == ktiltott) { pnyer1++; ;}; // hogy a korábbi értékek ne vesszenek el
            if (szabad == ptiltott) { knyer1++; }; // hogy a korábbi értékek ne vesszenek el                           
            ossz = 0;
            szor = suly[0]; szor *= knyer1; ossz += szor; szor = suly[1]; szor *= pnyer1; ossz += szor; szor = suly[2]; szor *= sk3o2; ossz += szor;
            szor = suly[3]; szor *= sp3o2; ossz += szor; szor = suly[4]; szor *= sk3m2; ossz += szor; szor = suly[5]; szor *= sp3m2; ossz += szor;
            szor = suly[6]; szor *= sk3h1; ossz += szor; szor = suly[7]; szor *= sp3h1; ossz += szor; szor = suly[8]; szor *= sk3h2; ossz += szor;
            szor = suly[9]; szor *= sp3h2; ossz += szor; szor = suly[10]; szor *= sk3h3; ossz += szor; szor = suly[11]; szor *= sp3h3; ossz += szor;
            szor = suly[12]; szor *= sk3h4; ossz += szor; szor = suly[13]; szor *= sp3h4; ossz += szor; szor = suly[14]; szor *= sk2; ossz += szor;
            szor = suly[15]; szor *= sp2; ossz += szor; szor = suly[16]; szor *= sk1; ossz += szor; szor = suly[17]; szor *= sp1; ossz += szor;
            szor = suly[18]; szor *= ptiltott; ossz += szor; szor = suly[19]; szor *= ktiltott; ossz += szor;
            if (poz[k1lep] == 7) { ossz += 350; }; // 7* suly[20]=7*50=350
            if (poz[p1lep] == 7) { ossz -= 315; }; // 7* suly[21]=7*(-45)=-315
            if (halado) { if (poz[k2lep] == 7) { ossz += 350; };  if (poz[p2lep] == 7) { ossz -= 315; }; };
            if (mester) { if (poz[k3lep] == 7) { ossz += 350; };  if (poz[p3lep] == 7) { ossz -= 315; }; };
        }

        private void szgephal()
        {
            timer2.Enabled = false; button6.Enabled = false; button6.Visible = false; byte k1 = 0; Boolean tovabb = true; 
            for (byte i = 1; 17 > i; i++) { rud1[i, 0] = rud[i, 0]; rud1[i, 1] = rud[i, 1]; }
            for (byte i = 1; 77 > i; i++) { pv1[i, 0] = pv[i, 0]; pv1[i, 1] = pv[i, 1]; pv1[i, 2] = pv[i, 2]; kv1[i, 0] = kv[i, 0]; kv1[i, 1] = kv[i, 1]; kv1[i, 2] = kv[i, 2]; }
            lepszamlk++; g1lepsz = 0; g1nyer = false; j1nyer = false;
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
                kek[g1lepnyer] = 1; keklep[lepszamlk] = g1lepnyer; kgyoz = true; t[g1lepnyer] = 2;
                klepsor += " " + kod[g1lepnyer]; szov27 = klepsor; g1r = pozrud[g1lepnyer]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[g1lepnyer] >= j; j++) { g1egy = pozszab[g1lepnyer, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Kék Nyert!"; szov24 = ""; szov25 = "Kék utolsó lépése: " + kod[g1lepnyer];
                erkek += 2; kij[0] = g1lepnyer; frissit(); vez = 6; button5.Enabled = true; button5.Visible = true; tovabb = false;
            }
            k1 = lepszamlk; k1 += lepszamlp;
            if ((tovabb) && (k1 == 64)) //Az utolsó lépés
            {
                g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep]; t[g1lep] = 2;
                szov27 = klepsor; szov18 = " Utolsó lépés"; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Döntetlen!"; dontetlen = true; szov24 = ""; szov25 = "Kék utolsó lépése: " + kod[g1lep];
                erkek++; erpiros++; kij[0] = g1lep; frissit(); vez = 6; button5.Enabled = true; button5.Visible = true; tovabb = false;
            }
            if ((g1lepsz == 1) && (tovabb)) //Csak egyetlen rúdra lehet már csak tenni
            {
                g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep]; t[g1lep] = 2;
                szov27 = klepsor; szov18 = "Csak egy rúdra tehet"; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Piros játékos / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; szov24 = "";
                szov25 = "Kék utolsó lépése: " + kod[g1lep]; kij[0] = g1lep; frissit(); vez = 3; valaszthat = true; tovabb = false;
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
                keklep[lepszamlk] = j1lepnyer; kek[j1lepnyer] = 1; klepsor += " " + kod[j1lepnyer]; szov27 = klepsor; g1r = pozrud[j1lepnyer]; rud[g1r, 0]--; rud[g1r, 1] += 16; t[j1lepnyer] = 2;
                for (byte j = 1; poz[j1lepnyer] >= j; j++) { g1egy = pozszab[j1lepnyer, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Piros játékos / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; szov24 = ""; 
                szov25 = "Kék utolsó lépése: " + kod[j1lepnyer]; kij[0] = j1lepnyer; frissit(); vez = 3; tovabb = false; valaszthat = true;
            }
            if (tovabb) // Keresés, hogy bármelyik játékosnak van még nyerő esélye
            {
                szamlnyero = 0; for (byte i = 1; 77 > i; i++) { if (pv1[i, 0] == 0) { szamlnyero++; }; if (kv1[i, 0] == 0) { szamlnyero++; } }
                if (szamlnyero == 0) // Ha már egyik fél sem tud nyerni, akkor elemzés nélkül az első lépést megteszi a számítógép (Fel kéne ajánlani a döntetlent!!!)
                {
                    g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep]; t[g1lep] = 2;
                    szov27 = klepsor; szov18 = "Egyik fél sem tud már nyerni, döntetlen"; dontetlen = true; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                    for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                    szov23 = "Piros játékos / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; szov24 = "";
                    szov25 = "Kék utolsó lépése: " + kod[g1lep]; kij[0] = g1lep; frissit(); vez = 3; valaszthat = true; tovabb = false;
                }
            }

            if (tovabb)
            { //lépés keresés
                for (byte g1 = 1; g1lepsz >= g1; g1++)
                { //1.
                    g1lep = g1lepesek[g1]; g1r = pozrud[g1lep]; rud1[g1r, 0]--; rud1[g1r, 1] += 16;
                    for (byte j = 1; poz[g1lep] >= j; j++)
                    { //2a1
                        g1egy = pozszab[g1lep, j]; g1ment[j, 0] = kv1[g1egy, 0]; g1ment[j, 1] = kv1[g1egy, 1]; g1ment[j, 2] = pv1[g1egy, 0];
                        pv1[g1egy, 0] = 0; if (kv1[g1egy, 0] == 1) { kv1[g1egy, 1]++; }
                    } //2a1
                    j1lepsz = 0; j1nyer = false; j1tov = true; 
                    for (byte i = 1; 17 > i; i++)
                    { // 3.
                        if (rud1[i, 0] > 0)
                        { // 4.
                            j1lepsz++; j1lep = rud1[i, 1]; j1lepesek[j1lepsz] = j1lep;
                            for (byte j = 1; poz[j1lep] >= j; j++) { j1egy = pozszab[j1lep, j]; if ((pv1[j1egy, 0] == 1) && (pv1[j1egy, 1] == 3)) { j1nyer = true; } }
                        } // 4.
                    } // 3.
                    if (j1nyer) { j1lepmin = -90000; j1tov = false; }
                    if (j1lepsz == 0) { j1lepmin = 0; j1tov = false; };
                    if (j1tov)
                    {  // 5.
                        for (byte j1 = 1; j1lepsz >= j1; j1++)
                        { // 6.
                            j1lep = j1lepesek[j1]; j1r = pozrud[j1lep]; rud1[j1r, 0]--; rud1[j1r, 1] += 16;
                            for (byte j = 1; poz[j1lep] >= j; j++)
                            { // 7a1
                                j1egy = pozszab[j1lep, j]; j1ment[j, 0] = pv1[j1egy, 0]; j1ment[j, 1] = pv1[j1egy, 1]; j1ment[j, 2] = kv1[j1egy, 0];
                                kv1[j1egy, 0] = 0; if (pv1[j1egy, 0] == 1) { pv1[j1egy, 1]++; }
                            } // 7a1
                            k1lep = g1lep; p1lep = j1lep; g2lepsz = 0; g2nyer = false; g2tov = true;
                            for (byte i = 1; 17 > i; i++)
                            { //8.
                                if (rud1[i, 0] > 0)
                                { //9.
                                    g2lepsz++; g2lep = rud1[i, 1]; g2lepesek[g2lepsz] = g2lep; 
                                    for (byte j = 1; poz[g2lep] >= j; j++) { g2egy = pozszab[g2lep, j]; if ((kv1[g2egy, 0] == 1) && (kv1[g2egy, 1] == 3)) { g2nyer = true; } }
                                } //9.
                            } // 8.
                            if (g2nyer) { g2lepmax = 80000; g2tov = false; };
                            if (g2lepsz == 0) { g2lepmax = 0; g2tov = false; };
                            if (g2tov)
                            { // 10.
                                for (byte g2 = 1; g2lepsz >= g2; g2++)
                                { //11.
                                    g2lep = g2lepesek[g2]; g2r = pozrud[g2lep]; rud1[g2r, 0]--; rud1[g2r, 1] += 16;
                                    for (byte j = 1; poz[g2lep] >= j; j++)
                                    { //12a1
                                        g2egy = pozszab[g2lep, j]; g2ment[j, 0] = kv1[g2egy, 0]; g2ment[j, 1] = kv1[g2egy, 1]; g2ment[j, 2] = pv1[g2egy, 0];
                                        pv1[g2egy, 0] = 0; if (kv1[g2egy, 0] == 1) { kv1[g2egy, 1]++; }
                                    } //12a1
                                    j2lepsz = 0; j2nyer = false; j2tov = true; 
                                    for (byte i = 1; 17 > i; i++)
                                    { // 13.
                                        if (rud1[i, 0] > 0)
                                        { // 14.
                                            j2lepsz++; j2lep = rud1[i, 1]; j2lepesek[j2lepsz] = j2lep;
                                            for (byte j = 1; poz[j2lep] >= j; j++) { j2egy = pozszab[j2lep, j]; if ((pv1[j2egy, 0] == 1) && (pv1[j2egy, 1] == 3)) { j2nyer = true; } }
                                        } // 14.
                                    } // 13.
                                    if (j2nyer) { j2lepmin = -70000; j2tov = false; }
                                    if (j2lepsz == 0) { j2lepmin = 0; j2tov = false; }
                                    if (j2tov)
                                      { // 15.
                                          for (byte j2 = 1; j2lepsz >= j2; j2++)
                                          { // 16. játékos 2. lépéseit teszi meg
                                              j2lep = j2lepesek[j2]; j2r = pozrud[j2lep]; rud1[j2r, 0]--; rud1[j2r, 1] += 16;
                                              for (byte j = 1; poz[j2lep] >= j; j++)
                                              { // 17a1
                                                  j2egy = pozszab[j2lep, j]; j2ment[j, 0] = pv1[j2egy, 0]; j2ment[j, 1] = pv1[j2egy, 1]; j2ment[j, 2] = kv1[j2egy, 0];
                                                  kv1[j2egy, 0] = 0; if (pv1[j2egy, 0] == 1) { pv1[j2egy, 1]++; }
                                              } // 17a1
                                              k2lep = g2lep; p2lep = j2lep; elemzes();
                                              if (j2 == 1) { j2lepmin = ossz; } // az első játékos 2. lépéséhez tartozó álláspontértéket elmentjük, mint a minimum
                                              if (j2lepmin > ossz) { j2lepmin = ossz; } // mindig a legalacsonyabb állásértéket tartalmazza a j2lepmin
                                              for (byte j = 1; j <= poz[j2lep]; j++)
                                              { // 17a1 vissza
                                                  j2egy = pozszab[j2lep, j]; pv1[j2egy, 0] = j2ment[j, 0]; pv1[j2egy, 1] = j2ment[j, 1]; kv1[j2egy, 0] = j2ment[j, 2];
                                              } // 17a1 vissza
                                              rud1[j2r, 0]++; rud1[j2r, 1] -= 16;
                                          } // 16.
                                      } // 15.
                                    for (byte j = 1; poz[g2lep] >= j; j++)
                                    { // 12a1 vissza
                                        g2egy = pozszab[g2lep, j]; kv1[g2egy, 0] = g2ment[j, 0]; kv1[g2egy, 1] = g2ment[j, 1]; pv1[g2egy, 0] = g2ment[j, 2];
                                    } // 12a1 vissza
                                    rud1[g2r, 0]++; rud1[g2r, 1] -= 16;
                                    if (g2 == 1) { g2lepmax = j2lepmin; }; // g2lepmax felveszi az első j2min értéket
                                    if (g2lepmax < j2lepmin) { g2lepmax = j2lepmin; };
                                } //11.
                            } //10.                                           
                            for (byte j = 1; j <= poz[j1lep]; j++)
                            { // 7a1 vissza
                                j1egy = pozszab[j1lep, j]; pv1[j1egy, 0] = j1ment[j, 0]; pv1[j1egy, 1] = j1ment[j, 1]; kv1[j1egy, 0] = j1ment[j, 2];
                            } // 7a1 vissza
                            rud1[j1r, 0]++; rud1[j1r, 1] -= 16;
                            if (j1 == 1) { j1lepmin = g2lepmax; };
                            if (j1lepmin > g2lepmax) { j1lepmin = g2lepmax; }; // a játékos szemponjából az a kedvező, ha utánna a gép által kiválasztott legjobb lépés a legkisebb
                        } // 6.
                    }  // 5.
                    for (byte j = 1; poz[g1lep] >= j; j++)
                    { // 2a1 vissza
                        g1egy = pozszab[g1lep, j]; kv1[g1egy, 0] = g1ment[j, 0]; kv1[g1egy, 1] = g1ment[j, 1]; pv1[g1egy, 0] = g1ment[j, 2];
                    } // 2a1 vissza
                    rud1[g1r, 0]++; rud1[g1r, 1] -= 16;
                    if (g1 == 1) { g1lepmax = j1lepmin; g1lepnyer = g1lep; }; // az első lépéshez tartozó legjobb állás étéket elmentjük
                    if (g1lepmax < j1lepmin) { g1lepmax = j1lepmin; g1lepnyer = g1lep; }; // a magasabb állásértéket tároljuk és megőrizzük
                } //1.
                keklep[lepszamlk] = g1lepnyer; kek[g1lepnyer] = 1; klepsor += " " + kod[g1lepnyer]; szov27 = klepsor; g1r = pozrud[g1lepnyer]; rud[g1r, 0]--; rud[g1r, 1] += 16; t[g1lep] = 2;
                for (byte j = 1; poz[g1lepnyer] >= j; j++) { g1egy = pozszab[g1lepnyer, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Piros játékos " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; 
                szov25 = "Kék utolsó lépése: " + kod[g1lepnyer]; kij[0] = g1lepnyer; frissit(); vez = 3; tovabb = false; valaszthat = true;
                p0 = lepszamlp; p0 += lepszamlk;
                if (p0 > 4) { button6.Enabled = true; button6.Visible = true; }
                else { button6.Enabled = false; button6.Visible = false; }
            } //lépés keresés
        }

        private void szgepmester() //számítógép mester szint
        {
            timer2.Enabled = false; button6.Enabled = false; button6.Visible = false; byte k1 = 0; Boolean tovabb = true;
            for (byte i = 1; 17 > i; i++) { rud1[i, 0] = rud[i, 0]; rud1[i, 1] = rud[i, 1]; }
            for (byte i = 1; 77 > i; i++) { pv1[i, 0] = pv[i, 0]; pv1[i, 1] = pv[i, 1]; pv1[i, 2] = pv[i, 2]; kv1[i, 0] = kv[i, 0]; kv1[i, 1] = kv[i, 1]; kv1[i, 2] = kv[i, 2]; }
            lepszamlk++; g1lepsz = 0; g1nyer = false; j1nyer = false;
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
                kek[g1lepnyer] = 1; keklep[lepszamlk] = g1lepnyer; kgyoz = true; klepsor += " " + kod[g1lepnyer]; szov27 = klepsor; g1r = pozrud[g1lepnyer]; rud[g1r, 0]--; rud[g1r, 1] += 16; t[g1lepnyer] = 2;
                for (byte j = 1; poz[g1lepnyer] >= j; j++) { g1egy = pozszab[g1lepnyer, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Kék Nyert!"; szov24 = ""; szov25 = "Kék utolsó lépése: " + kod[g1lepnyer];
                erkek += 2; kij[0] = g1lepnyer; frissit(); vez = 6; button5.Enabled = true; button5.Visible = true; tovabb = false;
            }
            k1 = lepszamlk; k1 += lepszamlp;
            if ((tovabb) && (k1 == 64)) //Az utolsó lépés
            {
                g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep]; t[g1lep] = 2;
                szov27 = klepsor; szov18 = " Utolsó lépés"; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Döntetlen!"; dontetlen = true; szov24 = ""; szov25 = "Kék utolsó lépése: " + kod[g1lep];
                erkek++; erpiros++; kij[0] = g1lep; frissit(); vez = 6; button5.Enabled = true; button5.Visible = true; tovabb = false;
            }
            if ((g1lepsz == 1) && (tovabb)) //Csak egyetlen rúdra lehet már csak tenni
            {
                g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep]; t[g1lep] = 2;
                szov27 = klepsor; szov18 = "Csak egy rúdra tehet"; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Piros játékos / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; szov24 = "";
                szov25 = "Kék utolsó lépése: " + kod[g1lep]; kij[0] = g1lep; frissit(); vez = 3; valaszthat = true; tovabb = false;
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
                keklep[lepszamlk] = j1lepnyer; kek[j1lepnyer] = 1; klepsor += " " + kod[j1lepnyer]; szov27 = klepsor; g1r = pozrud[j1lepnyer]; rud[g1r, 0]--; rud[g1r, 1] += 16; t[j1lepnyer] = 2;
                for (byte j = 1; poz[j1lepnyer] >= j; j++) { g1egy = pozszab[j1lepnyer, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Piros játékos / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; szov24 = ""; 
                szov25 = "Kék utolsó lépése: " + kod[j1lepnyer]; kij[0] = j1lepnyer; frissit(); vez = 3; tovabb = false; valaszthat = true;
            }
            if (tovabb) // Keresés, hogy bármelyik játékosnak van még nyerő esélye
            {
                szamlnyero = 0; for (byte i = 1; 77 > i; i++) { if (pv1[i, 0] == 0) { szamlnyero++; }; if (kv1[i, 0] == 0) { szamlnyero++; } }
                if (szamlnyero == 0) // Ha már egyik fél sem tud nyerni, akkor elemzés nélkül az első lépést megteszi a számítógép (Fel kéne ajánlani a döntetlent!!!)
                {
                    g1lep = g1lepesek[1]; keklep[lepszamlk] = g1lep; kek[g1lep] = 1; klepsor += " " + kod[g1lep]; t[g1lep] = 2;
                    szov27 = klepsor; szov18 = "Egyik fél sem tud már nyerni, döntetlen"; dontetlen = true; g1r = pozrud[g1lep]; rud[g1r, 0]--; rud[g1r, 1] += 16;
                    for (byte j = 1; poz[g1lep] >= j; j++) { g1egy = pozszab[g1lep, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                    szov23 = "Piros játékos / " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; szov24 = "";
                    szov25 = "Kék utolsó lépése: " + kod[g1lep]; kij[0] = g1lep; frissit(); vez = 3; valaszthat = true; tovabb = false;
                }
            }

            if (tovabb)
            { //lépés keresés
                for (byte g1 = 1; g1lepsz >= g1; g1++)
                { //1.
                    g1lep = g1lepesek[g1]; g1r = pozrud[g1lep]; rud1[g1r, 0]--; rud1[g1r, 1] += 16;
                    for (byte j = 1; poz[g1lep] >= j; j++)
                    { //2a1
                        g1egy = pozszab[g1lep, j]; g1ment[j, 0] = kv1[g1egy, 0]; g1ment[j, 1] = kv1[g1egy, 1]; g1ment[j, 2] = pv1[g1egy, 0];
                        pv1[g1egy, 0] = 0; if (kv1[g1egy, 0] == 1) { kv1[g1egy, 1]++; }
                    } //2a1
                    j1lepsz = 0; j1nyer = false; j1tov = true;
                    for (byte i = 1; 17 > i; i++)
                    { // 3.
                        if (rud1[i, 0] > 0)
                        { // 4.
                            j1lepsz++; j1lep = rud1[i, 1]; j1lepesek[j1lepsz] = j1lep;
                            for (byte j = 1; poz[j1lep] >= j; j++) { j1egy = pozszab[j1lep, j]; if ((pv1[j1egy, 0] == 1) && (pv1[j1egy, 1] == 3)) { j1nyer = true; } }
                        } // 4.
                    } // 3.
                    if (j1nyer) { j1lepmin = -90000; j1tov = false; }
                    if (j1lepsz == 0) { j1lepmin = 0; j1tov = false; };
                    if (j1tov)
                    {  // 5.
                        for (byte j1 = 1; j1lepsz >= j1; j1++)
                        { // 6.
                            j1lep = j1lepesek[j1]; j1r = pozrud[j1lep]; rud1[j1r, 0]--; rud1[j1r, 1] += 16;
                            for (byte j = 1; poz[j1lep] >= j; j++)
                            { // 7a1
                                j1egy = pozszab[j1lep, j]; j1ment[j, 0] = pv1[j1egy, 0]; j1ment[j, 1] = pv1[j1egy, 1]; j1ment[j, 2] = kv1[j1egy, 0];
                                kv1[j1egy, 0] = 0; if (pv1[j1egy, 0] == 1) { pv1[j1egy, 1]++; }
                            } // 7a1
                            k1lep = g1lep; p1lep = j1lep; g2lepsz = 0; g2nyer = false; g2tov = true;
                            for (byte i = 1; 17 > i; i++)
                            { //8.
                                if (rud1[i, 0] > 0)
                                { //9.
                                    g2lepsz++; g2lep = rud1[i, 1]; g2lepesek[g2lepsz] = g2lep;
                                    for (byte j = 1; poz[g2lep] >= j; j++) { g2egy = pozszab[g2lep, j]; if ((kv1[g2egy, 0] == 1) && (kv1[g2egy, 1] == 3)) { g2nyer = true; } }
                                } //9.
                            } // 8.
                            if (g2nyer) { g2lepmax = 80000; g2tov = false; };
                            if (g2lepsz == 0) { g2lepmax = 0; g2tov = false; };
                            if (g2tov)
                            { // 10.
                                for (byte g2 = 1; g2lepsz >= g2; g2++)
                                { //11.
                                    g2lep = g2lepesek[g2]; g2r = pozrud[g2lep]; rud1[g2r, 0]--; rud1[g2r, 1] += 16;
                                    for (byte j = 1; poz[g2lep] >= j; j++)
                                    { //12a1
                                        g2egy = pozszab[g2lep, j]; g2ment[j, 0] = kv1[g2egy, 0]; g2ment[j, 1] = kv1[g2egy, 1]; g2ment[j, 2] = pv1[g2egy, 0];
                                        pv1[g2egy, 0] = 0; if (kv1[g2egy, 0] == 1) { kv1[g2egy, 1]++; }
                                    } //12a1
                                    j2lepsz = 0; j2nyer = false; j2tov = true;
                                    for (byte i = 1; 17 > i; i++)
                                    { // 13.
                                        if (rud1[i, 0] > 0)
                                        { // 14.
                                            j2lepsz++; j2lep = rud1[i, 1]; j2lepesek[j2lepsz] = j2lep;
                                            for (byte j = 1; poz[j2lep] >= j; j++) { j2egy = pozszab[j2lep, j]; if ((pv1[j2egy, 0] == 1) && (pv1[j2egy, 1] == 3)) { j2nyer = true; } }
                                        } // 14.
                                    } // 13.
                                    if (j2nyer) { j2lepmin = -70000; j2tov = false; }
                                    if (j2lepsz == 0) { j2lepmin = 0; j2tov = false; }
                                    if (j2tov)
                                    { // 15.
                                        for (byte j2 = 1; j2lepsz >= j2; j2++)
                                        { // 16. játékos 2. lépéseit teszi meg
                                            j2lep = j2lepesek[j2]; j2r = pozrud[j2lep]; rud1[j2r, 0]--; rud1[j2r, 1] += 16;
                                            for (byte j = 1; poz[j2lep] >= j; j++)
                                            { // 17a1
                                                j2egy = pozszab[j2lep, j]; j2ment[j, 0] = pv1[j2egy, 0]; j2ment[j, 1] = pv1[j2egy, 1]; j2ment[j, 2] = kv1[j2egy, 0];
                                                kv1[j2egy, 0] = 0; if (pv1[j2egy, 0] == 1) { pv1[j2egy, 1]++; }
                                            } // 17a1
                                            k2lep = g2lep; p2lep = j2lep; g3lepsz = 0; g3nyer = false; g3tov = true;
                                            for (byte i = 1; 17 > i; i++)
                                            { //18.
                                                if (rud1[i, 0] > 0)
                                                { //19.
                                                    g3lepsz++; g3lep = rud1[i, 1]; g3lepesek[g3lepsz] = g3lep;
                                                    for (byte j = 1; poz[g3lep] >= j; j++) { g3egy = pozszab[g3lep, j]; if ((kv1[g3egy, 0] == 1) && (kv1[g3egy, 1] == 3)) { g3nyer = true; } }
                                                } //19.
                                            } // 18.
                                            if (g3nyer) { g3lepmax = 70000; g3tov = false; };
                                            if (g3lepsz == 0) { g3lepmax = 0; g3tov = false; };
                                            if (g3tov)
                                            { // 20.
                                                for (byte g3 = 1; g3lepsz >= g3; g3++)
                                                { // 21.
                                                    g3lep = g3lepesek[g3]; g3r = pozrud[g3lep]; rud1[g3r, 0]--; rud1[g3r, 1] += 16;
                                                    for (byte j = 1; poz[g3lep] >= j; j++)
                                                    { //22a1
                                                        g3egy = pozszab[g3lep, j]; g3ment[j, 0] = kv1[g3egy, 0]; g3ment[j, 1] = kv1[g3egy, 1]; g3ment[j, 2] = pv1[g3egy, 0];
                                                        pv1[g3egy, 0] = 0; if (kv1[g3egy, 0] == 1) { kv1[g3egy, 1]++; }
                                                    } //22a1
                                                    j3lepsz = 0; j3nyer = false; j3tov = true;
                                                    for (byte i = 1; 17 > i; i++)
                                                    { // 23.
                                                        if (rud1[i, 0] > 0)
                                                        { // 24.
                                                            j3lepsz++; j3lep = rud1[i, 1]; j3lepesek[j3lepsz] = j3lep;
                                                            for (byte j = 1; poz[j3lep] >= j; j++) { j3egy = pozszab[j3lep, j]; if ((pv1[j3egy, 0] == 1) && (pv1[j3egy, 1] == 3)) { j3nyer = true; } }
                                                        } // 24.
                                                    } // 23.
                                                    if (j3nyer) { j3lepmin = -60000; j3tov = false; }
                                                    if (j3lepsz == 0) { j3lepmin = 0; j3tov = false; }
                                                    if (j3tov)
                                                    { // 25.
                                                        for (byte j3 = 1; j3lepsz >= j3; j3++)
                                                        { //  26. játékos 3. lépéseit teszi meg
                                                            j3lep = j3lepesek[j3]; j3r = pozrud[j3lep]; rud1[j3r, 0]--; rud1[j3r, 1] += 16;
                                                            for (byte j = 1; poz[j3lep] >= j; j++)
                                                            { // 27a1
                                                                j3egy = pozszab[j3lep, j]; j3ment[j, 0] = pv1[j3egy, 0]; j3ment[j, 1] = pv1[j3egy, 1]; j3ment[j, 2] = kv1[j3egy, 0];
                                                                kv1[j3egy, 0] = 0; if (pv1[j3egy, 0] == 1) { pv1[j3egy, 1]++; }
                                                            } // 27a1
                                                            k3lep = g3lep; p3lep = j3lep; elemzes();
                                                            if (j3 == 1) { j3lepmin = ossz; } // az első játékos 3. lépéséhez tartozó j3lepmin értéket elmentjük, mint a minimum
                                                            if (j3lepmin > ossz) { j3lepmin = ossz; } // mindig a legalacsonyabb állásértéket tartalmazza a j3lepmin
                                                            for (byte j = 1; j <= poz[j3lep]; j++)
                                                            { // 27a1 vissza
                                                                j3egy = pozszab[j3lep, j]; pv1[j3egy, 0] = j3ment[j, 0]; pv1[j3egy, 1] = j3ment[j, 1]; kv1[j3egy, 0] = j3ment[j, 2];
                                                            } // 27a1 vissza
                                                            rud1[j3r, 0]++; rud1[j3r, 1] -= 16;
                                                        } // 26.
                                                    } // 25.
                                                    for (byte j = 1; poz[g3lep] >= j; j++)
                                                    { // 22a1 vissza
                                                        g3egy = pozszab[g3lep, j]; kv1[g3egy, 0] = g3ment[j, 0]; kv1[g3egy, 1] = g3ment[j, 1]; pv1[g3egy, 0] = g3ment[j, 2];
                                                    } // 12a1 vissza
                                                    rud1[g3r, 0]++; rud1[g3r, 1] -= 16;
                                                    if (g3 == 1) { g3lepmax = j3lepmin; }; // g3lepmax felveszi az első j3min értéket
                                                    if (g3lepmax < j3lepmin) { g3lepmax = j3lepmin; };
                                                } // 21.
                                            } // 20.
                                            if (j2 == 1) { j2lepmin = g3lepmax; } // az első játékos 2. lépéséhez tartozó g3lepmax értéket elmentjük, mint a minimum
                                            if (j2lepmin > g3lepmax) { j2lepmin = g3lepmax; } // mindig a legalacsonyabb állásértéket tartalmazza a j2lepmin
                                            for (byte j = 1; j <= poz[j2lep]; j++)
                                            { // 17a1 vissza
                                                j2egy = pozszab[j2lep, j]; pv1[j2egy, 0] = j2ment[j, 0]; pv1[j2egy, 1] = j2ment[j, 1]; kv1[j2egy, 0] = j2ment[j, 2];
                                            } // 17a1 vissza
                                            rud1[j2r, 0]++; rud1[j2r, 1] -= 16;
                                        } // 16.
                                    } // 15.
                                    for (byte j = 1; poz[g2lep] >= j; j++)
                                    { // 12a1 vissza
                                        g2egy = pozszab[g2lep, j]; kv1[g2egy, 0] = g2ment[j, 0]; kv1[g2egy, 1] = g2ment[j, 1]; pv1[g2egy, 0] = g2ment[j, 2];
                                    } // 12a1 vissza
                                    rud1[g2r, 0]++; rud1[g2r, 1] -= 16;
                                    if (g2 == 1) { g2lepmax = j2lepmin; }; // g2lepmax felveszi az első j2min értéket
                                    if (g2lepmax < j2lepmin) { g2lepmax = j2lepmin; };
                                } //11.
                            } //10.                                           
                            for (byte j = 1; j <= poz[j1lep]; j++)
                            { // 7a1 vissza
                                j1egy = pozszab[j1lep, j]; pv1[j1egy, 0] = j1ment[j, 0]; pv1[j1egy, 1] = j1ment[j, 1]; kv1[j1egy, 0] = j1ment[j, 2];
                            } // 7a1 vissza
                            rud1[j1r, 0]++; rud1[j1r, 1] -= 16;
                            if (j1 == 1) { j1lepmin = g2lepmax; };
                            if (j1lepmin > g2lepmax) { j1lepmin = g2lepmax; }; // a játékos szemponjából az a kedvező, ha utánna a gép által kiválasztott legjobb lépés a legkisebb
                        } // 6.
                    }  // 5.
                    for (byte j = 1; poz[g1lep] >= j; j++)
                    { // 2a1 vissza
                        g1egy = pozszab[g1lep, j]; kv1[g1egy, 0] = g1ment[j, 0]; kv1[g1egy, 1] = g1ment[j, 1]; pv1[g1egy, 0] = g1ment[j, 2];
                    } // 2a1 vissza
                    rud1[g1r, 0]++; rud1[g1r, 1] -= 16;
                    if (g1 == 1) { g1lepmax = j1lepmin; g1lepnyer = g1lep; }; // az első lépéshez tartozó legjobb állás étéket elmentjük
                    if (g1lepmax < j1lepmin) { g1lepmax = j1lepmin; g1lepnyer = g1lep; }; // a magasabb állásértéket tároljuk és megőrizzük
                } //1.
                keklep[lepszamlk] = g1lepnyer; kek[g1lepnyer] = 1; klepsor += " " + kod[g1lepnyer]; szov27 = klepsor; g1r = pozrud[g1lepnyer]; rud[g1r, 0]--; rud[g1r, 1] += 16; t[g1lepnyer] = 2;
                for (byte j = 1; poz[g1lepnyer] >= j; j++) { g1egy = pozszab[g1lepnyer, j]; pv[g1egy, 0] = 0; if (kv[g1egy, 0] == 1) { kv[g1egy, 1]++; } }
                szov23 = "Piros játékos " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; szov24 = "";
                szov25 = "Kék utolsó lépése: " + kod[g1lepnyer]; kij[0] = g1lepnyer; frissit(); vez = 3; tovabb = false; valaszthat = true;
                p0 = lepszamlp; p0 += lepszamlk;
                if (p0 > 4) { button6.Enabled = true; button6.Visible = true; }
                else { button6.Enabled = false; button6.Visible = false; }
            } //lépés keresés
        }

        private void kilépésToolStripMenuItem_Click(object sender, EventArgs e) { this.Close(); }
        private void startToolStripMenuItem_Click(object sender, EventArgs e) { kezd0(); kezd(); frissit(); }
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
            byte k = 1; String szov1 = ""; String szov3 = ""; Boolean jomentnev = false;
            szov1 = Convert.ToString(DateTime.Now);
            saveFileDialog1.Title = "Golyos játék mentés"; saveFileDialog1.Filter = "Text Files (*.txt)|*,txt"; saveFileDialog1.FilterIndex = 1;
            if (saveFileDialog1.ShowDialog() != DialogResult.Cancel)
            { szov3 = saveFileDialog1.FileName; szov3 += ".txt"; jomentnev = true; }
            if (jomentnev)
            {
                label24.Text = "Elmentve: " + szov3 + " néven"; StreamWriter sw = new StreamWriter(szov3);
                sw.WriteLine("File név: " + szov3); sw.WriteLine(verzio+" Dátum: "+szov1);
                if (jj)
                {
                    if (pgyoz) { szov1 = " Piros nyert!"; }
                    if (kgyoz) { szov1 = " Kék nyert!"; }
                    if (dontetlen) { szov1 = " Döntetlen!"; }
                    sw.WriteLine(jatekos1 + " - " + jatekos2 + " játszott."+szov1);
                    if (kekkezd) { sw.WriteLine(jatekos2 + " kezdett a kék golyókkal"); }
                    else { sw.WriteLine(jatekos1 + " kezdett a piros golyókkal"); }
                    
                }
                if (jsz)
                {
                    if (pgyoz) { szov1 = " Piros nyert!"; }
                    if (kgyoz) { szov1 = " Számítógép nyert!"; }
                    if (dontetlen) { szov1 = " Döntetlen!"; }
                    sw.WriteLine("Játékos / " + jatekos1 + " - Számítógép (" + szintkijel + ")"+szov1);
                    if (kekkezd) { sw.WriteLine("A számítógép kezdett a kék golyókkal"); }
                    else { sw.WriteLine(jatekos1 + " kezdett a piros golyókkal"); }
                }
                sw.WriteLine("Megtett lépések:");
                if (lepszamlk > lepszamlp) { k = lepszamlk; }
                else { k = lepszamlp; }
                for (byte i = 1; i <= k; i++)
                {
                    szov1 = Convert.ToString(i) + ". lépéspár: ";
                    if (kekkezd) { szov1 += kod[keklep[i]] + " " + kod[piroslep[i]]; }
                    else { szov1 += kod[piroslep[i]] + " " + kod[keklep[i]]; }
                    sw.WriteLine(szov1);
                }
               
                sw.Close();
            }
        }

        private void beállításToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label23.Text = "Válaszd ki a megfelelő beállításokat, add meg a játékos(ok) nevét!";
            groupBox1.Enabled = true; groupBox1.Visible = true;
            label19.Enabled = true; label19.Visible = true;
            textBox1.Enabled = true; textBox1.Visible = true;
            label20.Enabled = true; label20.Visible = true;
            textBox2.Enabled = true; textBox2.Visible = true;
            groupBox2.Enabled = true; groupBox2.Visible = true;
            groupBox3.Enabled = true; groupBox3.Visible = true;
            button1.Enabled = true; button1.Visible = true;
            button4.Enabled = true; button4.Visible = true;
            valaszthat = false; jjb = jj; jszb = jsz; jatekos1b = jatekos1; jatekos2b = jatekos2; kekkezdb = kekkezd; kezdob = kezdo; haladob = halado; mesterb = mester;
            if (jj) { radioButton1.Checked = true; radioButton2.Checked = false; };
            if (jsz) { radioButton1.Checked = false; radioButton2.Checked = true; };
            textBox1.Text = jatekos1; textBox2.Text = jatekos2;
            if (kekkezd) { radioButton3.Checked = false; radioButton4.Checked = true; }
            else { radioButton3.Checked = true; radioButton4.Checked = false; };
            if (kezdo) { radioButton5.Checked = true; radioButton6.Checked = false; radioButton7.Checked = false; };
            if (halado) { radioButton5.Checked = false; radioButton6.Checked = true; radioButton7.Checked = false; };
            if (mester) { radioButton5.Checked = false; radioButton6.Checked = false; radioButton7.Checked = true; };
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) // játékos1 - Játékos2
        { jjb = true; jszb = false; }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) // játékos1 - Számítógép
        { jjb = false; jszb = true; }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            jatekos1b = textBox1.Text;
            radioButton1.Text = "Piros: Játékos / " + jatekos1b + " - Kék: Játékos / " + jatekos2b;
            radioButton2.Text = "Piros: Játékos / " + jatekos1b + " - Kék: Számítógép";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            jatekos2b = textBox2.Text;
            radioButton1.Text = "Piros: Játékos / " + jatekos1b + " - Kék: Játékos / " + jatekos2b;
            radioButton2.Text = "Piros: Játékos / " + jatekos1b + " - Kék: Számítógép";
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        { }

        private void groupBox2_Enter(object sender, EventArgs e)
        { }

        private void groupBox3_Enter(object sender, EventArgs e)
        { }

        private void radioButton3_CheckedChanged(object sender, EventArgs e) //Piros kezd
        { kekkezdb = false; }

        private void radioButton4_CheckedChanged(object sender, EventArgs e) //Kék kezd
        { kekkezdb = true; }

        private void button4_Click(object sender, EventArgs e) //Beállítás módosítások elvetése
        {
            radioButton1.Text = "Piros: Játékos / " + jatekos1 + " - Kék: Játékos / " + jatekos2;
            radioButton2.Text = "Piros: Játékos / " + jatekos1 + " - Kék: Számítógép";
            if (jj) { radioButton2.Checked = false; radioButton1.Checked = true; }
            else { radioButton1.Checked = false; radioButton2.Checked = true; }
            label19.Enabled = false; label19.Visible = false;
            textBox1.Text = ""; textBox1.Enabled = false; textBox1.Visible = false;
            label20.Enabled = false; label20.Visible = false;
            textBox2.Text = ""; textBox2.Enabled = false; textBox2.Visible = false;
            groupBox1.Enabled = false; groupBox1.Visible = false;
            if (kekkezd) { radioButton3.Checked = false; radioButton4.Checked = true; }
            else { radioButton4.Checked = false; radioButton3.Checked = true; }
            groupBox2.Enabled = false; groupBox2.Visible = false;
            if (kezdo) { radioButton6.Checked = false; radioButton7.Checked = false; radioButton5.Checked = true; };
            if (halado) { radioButton7.Checked = false; radioButton5.Checked = false; radioButton6.Checked = true; };
            if (mester) { radioButton5.Checked = false; radioButton6.Checked = false; radioButton7.Checked = true; };
            groupBox3.Enabled = false; groupBox3.Visible = false;
            button1.Enabled = false; button1.Visible = false;
            button4.Enabled = false; button4.Visible = false;
            if (jj) { label21.Text = "Állás: Piros Játékos " + jatekos1 + " - Kék: Játékos " + jatekos2 + " : " + Convert.ToString(erpiros) + " - " + Convert.ToString(erkek); };
            if (jsz) { label21.Text = "Állás: Piros Játékos " + jatekos1 + " - Kék: Számítógép : " + Convert.ToString(erpiros) + " - " + Convert.ToString(erkek); };
            valaszthat = true;
        }

        private void button1_Click(object sender, EventArgs e) // Beállítások rögzítése
        {
            groupBox1.Enabled = false; groupBox1.Visible = false; label19.Enabled = false; label19.Visible = false; textBox1.Enabled = false; textBox1.Visible = false; 
            label20.Enabled = false; label20.Visible = false; textBox2.Enabled = false; textBox2.Visible = false; groupBox2.Enabled = false; groupBox2.Visible = false;
            groupBox3.Enabled = false; groupBox3.Visible = false; button1.Enabled = false; button1.Visible = false; button4.Enabled = false; button4.Visible = false;
            ujrakezd = false;
            if (jj != jjb) { ujrakezd = true; };
            if (jszb && ((kezdo != kezdob) | (halado != haladob) | (mester != mesterb))) { ujrakezd = true; };
            if (kekkezd != kekkezdb) { ujrakezd = true; };
            if ((jj) & (jatekos1b.Length > 0) & (jatekos1 != jatekos1b)) { ujrakezd = true; };
            if ((jj) & (jatekos2b.Length > 0) & (jatekos2 != jatekos2b)) { ujrakezd = true; };
            jj = jjb; jsz = jszb; kekkezd = kekkezdb; jatekos1 = jatekos1b; jatekos2 = jatekos2b; kezdo = kezdob; halado = haladob; mester = mesterb; szintkijel = szintkijelb;
            if (ujrakezd) { erpiros = 0; erkek = 0; kezd(); frissit(); }
            if (jj) { label21.Text = "Állás: Piros Játékos " + jatekos1 + " - Kék: Játékos " + jatekos2 + " : " + Convert.ToString(erpiros) + " - " + Convert.ToString(erkek); }
            if (jsz) { label21.Text = "Állás: Piros Játékos " + jatekos1 + " - Kék: Számítógép : " + Convert.ToString(erpiros) + " - " + Convert.ToString(erkek); }
            valaszthat = true; 
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e) //Számítógép szint Kezdő
        { kezdob = true; haladob = false; mesterb = false; szintkijelb = "Kezdő"; }

        private void radioButton6_CheckedChanged(object sender, EventArgs e) //Számítógép szint Haladó
        { kezdob = false; haladob = true; mesterb = false; szintkijelb = "Haladó"; }

        private void radioButton7_CheckedChanged(object sender, EventArgs e) //Számítógép szint Mester
        { kezdob = false; haladob = false; mesterb = true; szintkijelb = "Mester"; }

        private void button5_Click(object sender, EventArgs e) //Új parti
        {
            if (kekkezd) { kekkezd = false; }
            else { kekkezd = true; }
            kezd();
        }

        private void button6_Click(object sender, EventArgs e) // bemenet: 
        {
            byte lep1 = 0; byte k1 = 0; byte kekdb = 0; byte pirosdb = 0; string sor = ""; Int32 phossz = 0;
            valaszthat = false;
           if (jj)
            {
                if (vez == 2) // két játékos játszik, piros lépett és kék van lépésen és piros lépését visszavonjuk
                {
                    lep1 = piroslep[lepszamlp]; piroslep[lepszamlp] = 0; lepszamlp--; piros[lep1] = 0; k1 = pozrud[lep1]; rud[k1, 0]++; rud[k1, 1] -= 16; t[lep1] = 0;
                    phossz = plepsor.Length; sor = ""; phossz -= 4; for (Int32 i = 0; i <= phossz; i++) { sor += plepsor[i]; }; plepsor = sor; szov26 = plepsor;
                    szov18 = " Piros utolsó lépése visszavonva!";
                    for (byte i = 1; i <= poz[lep1]; i++)
                    {
                        k1 = pozszab[lep1, i]; pirosdb = 0; kekdb = 0;
                        if (t[szab[k1, 1]] == 1) { pirosdb++; } if (t[szab[k1, 2]] == 1) { pirosdb++; } if (t[szab[k1, 3]] == 1) { pirosdb++; } if (t[szab[k1, 4]] == 1) { pirosdb++; }
                        if (t[szab[k1, 1]] == 2) { kekdb++; } if (t[szab[k1, 2]] == 2) { kekdb++; } if (t[szab[k1, 3]] == 2) { kekdb++; } if (t[szab[k1, 1]] == 4) { kekdb++; }
                        if (pirosdb == 0) { kv[k1, 0] = 1; kv[k1, 1] = kekdb; }
                        if (kekdb == 0) { pv[k1, 0] = 1; pv[k1, 1] = pirosdb; }
                    }
                    k1 = keklep[lepszamlk]; szov25 = "Kék utolsó lépése: lepszamlk:" + kod[k1]; visszap++; visszadb = 1; kij[0] = k1;
                    vez = 1; szov23 = "Piros játékos " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére...";
                }
                else
                {
                    if (vez == 1) // két játékos játszik, kék lépett és piros van lépésen és kék lépését visszavonjuk
                    {
                        lep1 = keklep[lepszamlk]; keklep[lepszamlk] = 0; lepszamlk--; kek[lep1] = 0; k1 = pozrud[lep1]; rud[k1, 0]++; rud[k1, 1] -= 16; t[lep1] = 0;
                        phossz = klepsor.Length; sor = ""; phossz -= 4; for (Int32 i = 0; i <= phossz; i++) { sor += klepsor[i]; }; klepsor = sor; szov27 = klepsor;
                        szov18 = " Kék utolsó lépése visszavonva!";
                        for (byte i = 1; i <= poz[lep1]; i++)
                        {
                            k1 = pozszab[lep1, i]; pirosdb = 0; kekdb = 0;
                            if (t[szab[k1, 1]] == 1) { pirosdb++; } if (t[szab[k1, 2]] == 1) { pirosdb++; } if (t[szab[k1, 3]] == 1) { pirosdb++; } if (t[szab[k1, 4]] == 1) { pirosdb++; }
                            if (t[szab[k1, 1]] == 2) { kekdb++; } if (t[szab[k1, 2]] == 2) { kekdb++; } if (t[szab[k1, 3]] == 2) { kekdb++; } if (t[szab[k1, 1]] == 4) { kekdb++; }
                            if (pirosdb == 0) { kv[k1, 0] = 1; kv[k1, 1] = kekdb; }
                            if (kekdb == 0) { pv[k1, 0] = 1; pv[k1, 1] = pirosdb; }
                        }
                        k1 = piroslep[lepszamlp]; szov25 = "Piros utolsó lépése: " + kod[k1]; visszak++; visszadb = 1; kij[0] = k1;
                       vez = 2; szov23 = "Kék játékos " + jatekos2 + " lép. Klikkelj a kiválasztott rúd betűjelére...";
                    }
                }               
            }
           if (jsz)
           {
               lep1 = keklep[lepszamlk]; keklep[lepszamlk] = 0; lepszamlk--; kek[lep1] = 0; k1 = pozrud[lep1]; rud[k1, 0]++; rud[k1, 1] -= 16; t[lep1] = 0;
               phossz = klepsor.Length; sor = ""; phossz -= 4; for (Int32 i = 0; i <= phossz; i++) { sor += klepsor[i]; }; klepsor = sor; szov27 = klepsor;
               for (byte i = 1; i <= poz[lep1]; i++)
               {
                   k1 = pozszab[lep1, i]; pirosdb = 0; kekdb = 0;
                   if (t[szab[k1, 1]] == 1) { pirosdb++; } if (t[szab[k1, 2]] == 1) { pirosdb++; } if (t[szab[k1, 3]] == 1) { pirosdb++; } if (t[szab[k1, 4]] == 1) { pirosdb++; }
                   if (t[szab[k1, 1]] == 2) { kekdb++; } if (t[szab[k1, 2]] == 2) { kekdb++; } if (t[szab[k1, 3]] == 2) { kekdb++; } if (t[szab[k1, 1]] == 4) { kekdb++; }
                   if (pirosdb == 0) { kv[k1, 0] = 1; kv[k1, 1] = kekdb; }
                   if (kekdb == 0) { pv[k1, 0] = 1; pv[k1, 1] = pirosdb; }
               }
               lep1 = piroslep[lepszamlp]; piroslep[lepszamlp] = 0; lepszamlp--; piros[lep1] = 0; k1 = pozrud[lep1]; rud[k1, 0]++; rud[k1, 1] -= 16; t[lep1] = 0;
               phossz = plepsor.Length; sor = ""; phossz -= 4; for (Int32 i = 0; i <= phossz; i++) { sor += plepsor[i]; }; plepsor = sor; szov26 = plepsor;
               szov18 = " Kék és Piros utolsó lépése visszavonva!";
               for (byte i = 1; i <= poz[lep1]; i++)
               {
                   k1 = pozszab[lep1, i]; pirosdb = 0; kekdb = 0;
                   if (t[szab[k1, 1]] == 1) { pirosdb++; } if (t[szab[k1, 2]] == 1) { pirosdb++; } if (t[szab[k1, 3]] == 1) { pirosdb++; } if (t[szab[k1, 4]] == 1) { pirosdb++; }
                   if (t[szab[k1, 1]] == 2) { kekdb++; } if (t[szab[k1, 2]] == 2) { kekdb++; } if (t[szab[k1, 3]] == 2) { kekdb++; } if (t[szab[k1, 1]] == 4) { kekdb++; }
                   if (pirosdb == 0) { kv[k1, 0] = 1; kv[k1, 1] = kekdb; }
                   if (kekdb == 0) { pv[k1, 0] = 1; pv[k1, 1] = pirosdb; }
               }
               k1 = keklep[lepszamlk]; szov25 = "Kék utolsó lépése: lepszamlk:" + kod[k1]; visszap++; visszadb = 1; kij[0] = k1;
               vez = 3; szov23 = "Piros játékos " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére...";
           }
            frissit(); valaszthat = true;
        }

        private void kezd0()
        {
            timer1.Enabled = false;
            byte sz1 = 0; byte sz2 = 0; byte i1 = 0; byte i2 = 0; this.Text = verzio;

            
            int yValue = 340;
            for (int i=0; i<4; i++)
            {
                int xValue = 182-42*i;
                for (int j=0; j<4; j++)
                {
                    rudx[i*4+j] = xValue;
                    rudy[i*4+j] = yValue;

                    xValue += 126;
                }
                yValue -= 80;                
            }


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
            suly[20] = 50; // K+ lépés pozicíó
            suly[21] = -45; // P- lépés pozicíó

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

            for (byte i = 64; 128 > i; i++) { er4[i, 4] = 1; er4[(i + 64), 5] = 1; er4[i, 7] = 1; er4[(i + 64), 6] = 1; er4[(i + 128), 6] = 1; er4[(i + 128), 7] = 1; }
            for (byte i = 0; 65 > i; i++) { for (byte j = 0; 4 > j; j++) { hianydb[i, j] = 0; } }
            for (byte i = 1; 17 > i; i++) { hianydb[i, 4] = 1; p1 = 16; p2 = 32; p3 = 48; p1 += i; p2 += i; p3 += i; hianydb[p1, 3] = 1; hianydb[p1, 4] = 2;
                 hianydb[p2, 2] = 1; hianydb[p2, 3] = 2; hianydb[p2, 4] = 3; hianydb[p3, 1] = 1; hianydb[p3, 2] = 2; hianydb[p3, 3] = 3; hianydb[p3, 4] = 4;}
        }

        private void timer1_Tick(object sender, EventArgs e) { kezd0(); kezd(); frissit(); }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (kezdo) { szgepkezd(); }
            if (halado) { szgephal(); }
            if (mester) { szgepmester(); }
        }
      
    }
}
