using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;

namespace sorozatok
{
    class Program
    {
        public static string hetnapja(int ev,int ho, int nap)
        {
            string[] napok = { "v", "h", "k", "sze", "cs", "p", "szo" };
            int[] honapok = {0,3,2,5,0,3,5,1,4,6,2,4 };
            if (ho<3)
            {
                ev = ev - 1;
            }
            return napok[(ev + ev / 4 - ev / 100 + ev / 400 + honapok[ho - 1] + nap)%7];
        }
        class Adat
        {
            public string datum { get; set; }
            public string cim { get; set; }
            public int evad { get; set; }
            public string epizod { get; set; }
            public int hossz { get; set; } //perc

            public int megnezve { get; set; }

        }
        static void Main(string[] args)
        {
            string filename = "lista.txt";
            int db = File.ReadAllLines(filename).Count();
            FileStream fs = new FileStream(filename, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            List<Adat> Sorozat = new List<Adat>();
            List<string> sor = new List<string>();
            string[] e_h = new string[2];
            for (int i = 0; i < db/5; i++)
            {
                sor.Clear();
                Adat beolv = new Adat();
                for (int n = 1; n <=5 ; n++)
                {
                    sor.Add(sr.ReadLine());
                }
                if (sor[0][0]=='2')
                {
                    beolv.datum = sor[0];
                }
                else { beolv.datum = "NI"; }
                beolv.cim = sor[1];
                e_h = sor[2].Split('x');
                beolv.evad = int.Parse(e_h[0]);
                beolv.epizod = e_h[1];
                beolv.hossz = int.Parse(sor[3]);
                beolv.megnezve = int.Parse(sor[4]);
                Sorozat.Add(beolv);
            }
            //foreach (var item in Sorozat)
            //{
            //    Console.WriteLine(item.datum+" "+item.cim+" epizod: "+item.epizod+" h: "+item.hossz+" v: "+item.megnezve);
            //}
            int d = 0;
            foreach (var item in Sorozat)
            {
                if (item.datum!="NI")
                {
                    d++;
                }
            }
            Console.WriteLine("2. feladat");
            Console.WriteLine("A listában {0} db vetítési dátummal rendelkező epizód van.\n",d);
            int v = 0;
            foreach (var item in Sorozat)
            {
                if (item.megnezve==1)
                {
                    v++;
                }
            }
            
            Console.WriteLine("3.feladat\nA listában levő epizódok {0:00.00}%-át látta.",(double)v/(db/5)*100);
            Console.WriteLine("4. feladat");
            int nap = 0;
            int ora = 0;
            int perc = 0;
            int sum = 0;
            int maradek_perc = 0;
            int maradek_ora = 0;
            foreach (var item in Sorozat)
            {
                sum += item.hossz;
                if (sum>=60)
                {                
                    maradek_perc = sum - 60;
                    ++ora;
                    sum = maradek_perc;
                }
                if (ora>=24)
                {
                    maradek_ora = ora - 24;
                    ++nap;
                    ora = maradek_ora;
                }
                

            }

            Console.WriteLine("Sorozatnézéssel {0} napot {1} órát és {2} percet töltött.",nap,ora,sum);
            Console.WriteLine("\n5. feladat");
            Console.Write("Adjon meg egy dátumot! Dátum= ");
            string input_datum = Console.ReadLine();
            int[] datum = new int[3];
            string[] inputdate = new string[3];
            inputdate = input_datum.Split('.');
            datum[0] = int.Parse(inputdate[0]); //év
            datum[1] = int.Parse(inputdate[1]); //hónap
            datum[2] = int.Parse(inputdate[2]); //nap

            string[] vizsg_date = new string[3];
            foreach (var item in Sorozat)
            {
                if (item.datum!="NI")
                {
                    vizsg_date = item.datum.Split('.');
                    if (int.Parse(vizsg_date[0])<=datum[0] && int.Parse(vizsg_date[1]) <= datum[1] && int.Parse(vizsg_date[2]) <= datum[2] && item.megnezve==0)
                    {
                        Console.WriteLine(item.evad+"x"+item.epizod+"\t"+item.cim);
                    }

                }
            }
            List<string> Cimek = Sorozat.Select(x => x.cim).Distinct().ToList();
            Console.WriteLine("7. feladat");
            Console.Write("Adja meg egy hét napját (például cs)! Nap= ");
            string input_nap = Console.ReadLine();
            string elozo = "";
            int adas_cnt = 0;
            foreach (var item in Cimek)
            {
                for (int i = 0; i < Sorozat.Count(); i++)
                {
                    if (Sorozat[i].datum != "NI")
                    {
                        vizsg_date = Sorozat[i].datum.Split('.');
                        if (input_nap == hetnapja(int.Parse(vizsg_date[0]), int.Parse(vizsg_date[1]), int.Parse(vizsg_date[2])))
                        {
                            if (Sorozat[i].cim == item)
                            {
                                
                                if (Sorozat[i].cim!=elozo)
                                {
                                    adas_cnt++;
                                    elozo = item;
                                    Console.WriteLine(item);
                                }
                                
                                
                            }
                        }
                    }
                }
            }
            if (adas_cnt==0)
            {
                Console.WriteLine("Az adott napon nem kerül adásba sorozat.");
            }

            string outfilename = "summa.txt";
            int minute_sum = 0;
            string output = "";
            int evad = 0;
            foreach (var item in Cimek)
            {
                evad = 0;
                minute_sum = 0;
                for (int i = 0; i < Sorozat.Count(); i++)
                {
                    if (item==Sorozat[i].cim)
                    {
                        minute_sum += Sorozat[i].hossz;
                        evad = Sorozat[i].evad;
                        output = item + " " + minute_sum + " " + evad + "\n";

                    }
                    

                }
                
                File.AppendAllText(outfilename,output);
            }

        }
    }
}
