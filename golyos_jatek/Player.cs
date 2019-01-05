using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace golyos_jatek
{
    class Player
    {
        public byte[] Positions //
        { get; set; }

        public List<String> Moves
        { get; set; }

        public int Points
        { get; set; }

        private Boolean usedUndo;
        private String name;
        private String color;

        public Player(String name, String color)
        {
            Positions = new byte[64];
            Moves = new List<string>();
            usedUndo = false;
            Points = 0;
            this.name = name;
            this.color = color;
        }


        public string Win()
        {
            if (usedUndo)
            {
                usedUndo = false;
                Points += 1;
                return "Visszalépés miatt a győzelemért csak 1 pont jár!";
            }

            return "";
        }
    }
}
