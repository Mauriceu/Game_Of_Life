﻿using System;
using System.Collections.Generic;
using System.Timers;

namespace Game_Of_Life
{
    
    public class GameBoard
    {

        /**
         * 2D-Liste, enthält alle existierenden Zellen
         */
        public List<List<Cell>> Board;
        private int _width;
        private int _height;

        /**
         * Erstellt die 2D-Liste,
         * Anhand der Übergabeparameter wird die Größe (Höhe, Breite) festgelegt
         */
        public void FillBoard(int x, int y)
        {
            _width = x;
            _height = y;
            Board = new List<List<Cell>>();

            for (int posY = 0; posY < _height; posY++)
            {
                Board.Add(new List<Cell>());
                for (int posX = 0; posX < _width; posX++)
                {
                    Board[posY].Add(new Cell(posY.ToString() + posX));
                }
                SetNeighbourCells(posY);
            }
        }

        /**
         * Iteriert die aktuelle Board-Reihe ein zweites mal und setzt die Zelle der vorherigen Reihe (falls vorhanden)
         * und die Zellen der aktuellen Reihe als Nachbarzellen
         */
        private void SetNeighbourCells(int posY)
        {
            for (int posX = 0; posX < _width; posX++)
            {
                IterateRowHelper(posY, posX,-1);
                IterateRowHelper(posY, posX,0);
            }
        }
        
        /**
         * Iteriert die aktuelle Board-Reihe durch und setzt gefundene Zellen als Nachbarzellen,
         * ignoriert OutOfBounds-Error:
         * Diese bedeuten nämlich einfach, dass an diesem Platz keine Nachbarzelle existiert, da außerhalb des Spielfeldes
         */
        private void IterateRowHelper(int posY, int posX,  int offsetY)
        {
            Cell currentCell = Board[posY][posX];
            for (var offsetX = -1; offsetX <= 1; offsetX++)
            {
                Cell neighbour = null;
                try
                {
                    neighbour = Board[posY + offsetY][posX + offsetX];
                }
                catch (Exception e)
                {
                    // ignored
                }

                if (neighbour != null &&
                    neighbour.ID != Board[posY][posX].ID &&
                    !currentCell.HasNeighbour(neighbour))
                {
                    currentCell.AddNeighbour(neighbour);
                    neighbour.AddNeighbour(currentCell);
                }
            }
        }

        /**
         * Board wird von oben links nach unten rechts iteriert. Jede Zelle durchläuft die "lebt"/"stirbt"-Logik
         */
        public void NextGeneration()
        {
            foreach (var row in Board)
            {
                foreach (Cell cell in row)
                {
                    cell.CheckIfCellLivesAfterGenerationChange();
                }
            }
            FinishGenerationChange();
        }

        /**
         * Das gespeicherte Ergebnis des Generationswechsel wird nun für jede Zelle übernommen
         */
        private void FinishGenerationChange()
        {
            foreach (var row in Board)
            {
                foreach (Cell cell in row)
                {
                    cell.EvolveCell();
                }
            }
        }

        /**
         * Interval in ms
         */
        public void NextGenerationOnTimer(int interval = 5000)
        {
            var timer = new Timer {Interval = interval};

            timer.Elapsed += GenerationChangeOnTimerElapse;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void GenerationChangeOnTimerElapse(Object source, System.Timers.ElapsedEventArgs e)
        {
            NextGeneration();

            //PrintBoard();
        }
        
        
        public void PrintBoard()
        {
            Console.WriteLine();
            Console.WriteLine("---Board---");

            for (int posY = 0; posY < Board.Count; posY++)
            {
                for (int posX = 0; posX < Board[posY].Count; posX++)
                {
                    var cell = Board[posY][posX];
                    string txt = "0";
                    if (cell.IsAlive())
                    {
                        txt = "1";
                    }
                    
                    Console.Write(txt + " ");
                }
                Console.WriteLine();
            }
        }
    }
}