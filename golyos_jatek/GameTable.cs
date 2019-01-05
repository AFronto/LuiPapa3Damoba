using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace golyos_jatek
{
    class GameTable
    {

        public Dictionary<String, List<Boolean>> Table
        { get; set; }

        private Player piros; //Players are entities so they are represented with a class
        private Player kek;  //Players are entities so they are represented with a class

        public GameTable(Player p1,Player p2)
        {
            Table = new Dictionary<string, List<bool>>();

            piros = p1;
            kek = p2;
        }


        private void kezd()
        {
            byte i1 = 0;
            szov17 = "Győzelem 2 pont. Döntetlen 1 pont.";
            dontetlen = false;
            pgyoz = false;
            kgyoz = false;
            button5.Visible = false;
            button5.Enabled = false;
            button6.Visible = false;
            button6.Enabled = false;
            kijvez = 0;
            kij[0] = 0;
            visszadb = 0;
            visszak = 0;
            visszap = 0;
            for (byte i = 1; i < 65; i++) { kij[i] = 0; kek[i] = 0; piros[i] = 0; t[i] = 0; }

            for (byte i = 1; i < 33; i++) { piroslep[i] = 0; keklep[i] = 0; }

            for (byte i = 1; i < 17; i++) { rud[i, 0] = 4; rud[i, 1] = i; pozrud[i] = i; i1 = i; i1 += 16; pozrud[i1] = i; i1 += 16; pozrud[i1] = i; i1 += 16; pozrud[i1] = i; }

            for (byte i = 1; i < 77; i++) { kv[i, 0] = 1; kv[i, 1] = 0; kv[i, 2] = 0; pv[i, 0] = 1; pv[i, 1] = 0; pv[i, 2] = 0; }

            button2.Enabled = true; button2.Visible = true; button3.Enabled = true; button3.Visible = true;
            label1.Visible = true; label2.Visible = true; label3.Visible = true; label4.Visible = true; label5.Visible = true;
            label6.Visible = true; label7.Visible = true; label8.Visible = true; label9.Visible = true; label10.Visible = true;
            label11.Visible = true; label12.Visible = true; label13.Visible = true; label14.Visible = true; label15.Visible = true;
            label16.Visible = true; label17.Visible = true; label18.Visible = true; label19.Visible = true; label20.Visible = true;
            label21.Visible = true; label22.Visible = true; label23.Visible = true; label24.Visible = true; label25.Visible = true;
            label26.Visible = true; label27.Visible = true; label28.Visible = true; label28.Visible = true;
            lepszamlp = 0; lepszamlk = 0; valaszthat = true; label18.Text = ""; szov18 = "";
            plepsor = "Piros lépései:"; klepsor = "  Kék lépései:"; szov26 = plepsor; szov27 = klepsor; label27.Text = klepsor;
            if (kekkezd)
            {
                plepsor += " -- ";
                if (jsz)
                {
                    kek[1] = 1; lepszamlk++; keklep[lepszamlk] = 1; klepsor += " A1";
                    label27.Text = klepsor; label24.Text = ""; label25.Text = "Kék utolsó lépése: A1";
                    for (byte i = 1; i < 8; i++)
                    {
                        i1 = pozszab[1, i]; if (i1 > 0) { kv[i1, 1]++; pv[i1, 0] = 0; }
                    }
                    rud[1, 0]--; rud[1, 1] += 16; kij[0] = 1; vez = 3;
                    szov23 = "Piros játékos " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; valaszthat = true;
                }
                if (jj) { szov23 = "Kék játékos kezd, " + jatekos2 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; vez = 2; valaszthat = true; }
            }
            else
            {
                klepsor += " -- ";
                if (jsz) { szov23 = "Piros játékos kezd, " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; vez = 3; valaszthat = true; }
                if (jj) { szov23 = "Piros játékos kezd, " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; vez = 1; valaszthat = true; }
            }
            szov24 = ""; szov25 = ""; for (byte i = 1; i < 8; i++) { gyozegy[i] = 0; }
            pgyoz = false; kgyoz = false; frissit();
        }

        private void valaszt()
        {
            kivjo = kijtolt[kijvez, kiv]; valaszthat = false;
            switch (vez)
            {
                case 1: leppiroskek(); break;
                case 2: lepkek(); break;
                case 3: leppiros(); break;
                default: break;
            }
        }

        private void leppiroskek() //vez=1 bemenő adat=kivjo piros a kék játékos lépéseire válaszol
        {
            byte lep1 = 0; byte k1 = 0; byte k2 = 0;
            if (rud[kivjo, 0] > 0)
            {
                lep1 = rud[kivjo, 1]; rud[kivjo, 0]--; rud[kivjo, 1] += 16; piros[lep1] = 1; lepszamlp++; piroslep[lepszamlp] = lep1; t[lep1] = 1;
                plepsor += " " + kod[lep1]; szov26 = plepsor; szov24 = ""; szov25 = "Piros utolsó lépése: " + kod[lep1];
                for (byte i = 1; i <= poz[lep1]; i++)
                {
                    k1 = pozszab[lep1, i]; kv[k1, 0] = 0;
                    if (pv[k1, 0] == 1) { pv[k1, 1]++; if (pv[k1, 1] == 4) { pgyoz = true; k2++; gyozegy[k2] = k1; } }
                }
                kij[0] = lep1; frissit();
                if (pgyoz)
                { szov23 = "Piros Nyert!"; erpiros += 2; vez = 6; button5.Enabled = true; button5.Visible = true; }
                else
                {
                    k2 = lepszamlp; k2 += lepszamlk;
                    if (k2 == 64) { szov23 = "Döntetlen..."; dontetlen = true; erpiros++; erkek++; vez = 6; button5.Enabled = true; button5.Visible = true; };
                    if (vez == 1) { vez = 2; szov23 = "Kék játékos / " + jatekos2 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; valaszthat = true; };
                }
            }
            else
            { szov24 = "Több golyó nem helyezhető a(z) " + rudfelirat[kijvez, kiv] + " rúdra!"; valaszthat = true; }
            frissit();
        }

        private void lepkek() //vez=2 játékos2 lép kékkel...
        {
            byte lep1 = 0; byte k1 = 0; byte k2 = 0;
            if (rud[kivjo, 0] > 0)
            {
                lep1 = rud[kivjo, 1];
                rud[kivjo, 0]--;
                rud[kivjo, 1] += 16;
                kek[lep1] = 1;
                lepszamlk++; keklep[lepszamlk] = lep1; t[lep1] = 2;
                klepsor += " " + kod[lep1]; szov27 = klepsor; szov24 = ""; szov25 = "Kék utolsó lépése: " + kod[lep1];
                for (byte i = 1; i <= poz[lep1]; i++)
                {
                    k1 = pozszab[lep1, i]; pv[k1, 0] = 0;
                    if (kv[k1, 0] == 1) { kv[k1, 1]++; if (kv[k1, 1] == 4) { kgyoz = true; k2++; gyozegy[k2] = k1; } }
                }
                kij[0] = lep1; p1 = 2; p2 = 2; p3 = Convert.ToByte(p2 / 16); // label18.Text = Convert.ToString(p3);
                if (kgyoz)
                { szov23 = "Kék Nyert!"; erkek += 2; vez = 6; button5.Enabled = true; button5.Visible = true; }
                else
                {
                    k2 = lepszamlp; k2 += lepszamlk;
                    if (k2 == 64) { szov23 = "Döntetlen..."; dontetlen = true; erpiros++; erkek++; vez = 6; button5.Enabled = true; button5.Visible = true; };
                    vez = 1; szov23 = "Piros játékos " + jatekos1 + " lép. Klikkelj a kiválasztott rúd betűjelére..."; valaszthat = true;
                }
            }
            else
            { szov24 = "Több golyó nem helyezhető a(z) " + rudfelirat[kijvez, kiv] + " rúdra!"; valaszthat = true; }
            frissit();
        }

        private void leppiros() // vagy vez=3 bemenő adat=kivjo
        {
            byte lep1 = 0; byte k1 = 0; byte k2 = 0; tovpir = true;
            if (rud[kivjo, 0] > 0)
            {
                lep1 = rud[kivjo, 1]; rud[kivjo, 0]--; rud[kivjo, 1] += 16; piros[lep1] = 1; lepszamlp++; piroslep[lepszamlp] = lep1; t[lep1] = 1;
                if (halado) { szov23 = "Számítógép keresi a választ, ..."; };
                if (mester) { szov23 = "Számítógép keresi a választ, 4-5 perc ..."; };

                plepsor += " " + kod[lep1]; szov26 = plepsor; szov24 = ""; szov25 = "Piros utolsó lépése: " + kod[lep1];
                for (byte i = 1; i <= poz[lep1]; i++)
                {
                    k1 = pozszab[lep1, i]; kv[k1, 0] = 0;
                    if (pv[k1, 0] == 1) { pv[k1, 1]++; if (pv[k1, 1] == 4) { pgyoz = true; k2++; gyozegy[k2] = k1; } }
                }
                kij[0] = lep1;
                if (pgyoz) { szov23 = "Piros Nyert!"; erpiros += 2; vez = 6; button5.Enabled = true; button5.Visible = true; tovpir = false; }
                if (tovpir)
                {
                    k2 = lepszamlp; k2 += lepszamlk;
                    if (k2 == 64) { szov23 = "Döntetlen..."; dontetlen = true; erpiros++; erkek++; vez = 6; button5.Enabled = true; button5.Visible = true; tovpir = false; }
                }
                if (tovpir) { timer2.Enabled = true; }
            }
            else
            { szov24 = "Több golyó nem helyezhető a(z) " + rudfelirat[kijvez, kiv] + " rúdra!"; valaszthat = true; }
            tovpir = false; frissit();
        }

        private int ConvertTable()
        {
            return 0;
        }
    }
}
