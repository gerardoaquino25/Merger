using System;
using System.IO;
using System.Linq;

namespace Merger
{
    class Program
    {
        private const string RutaBackup = "C:\\Users\\Gerardo\\Desktop\\App_Main_Chef";
        private const string RutaReemplazo = "C:\\Users\\Gerardo\\Desktop\\App_Main_Chef_Tst";
        private const string RutaResultado = "C:\\Users\\Gerardo\\Desktop\\Resultado";
        private const string RutaResultadoMerges = "C:\\Users\\Gerardo\\Desktop\\ResultadoConMergeFiles2";
        private static readonly string[] NoReemplazar = { "emb3chef_init_control", "Connectors.py", "Constantes.py" };

        static void Main(string[] args)
        {
            while (true)
            {
                MostrarOpcionesGenerales();
                ElegirOpcionGeneral(LeerOpcion());
            }
        }

        private static int LeerOpcion()
        {
            try
            {
                return int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static void MostrarOpcionesGenerales()
        {
            Console.WriteLine("Elija la opcion:");
            Console.WriteLine("1-Copiar datos.");
            Console.WriteLine("2-Ver datos copiados.");
            Console.WriteLine("99-Salir.");
        }

        private static void ElegirOpcionGeneral(int opcion)
        {
            switch (opcion)
            {
                case 1:
                    CopyData();
                    break;
                case 2:
                    ViewCopiedData(RutaResultado);
                    break;
                case 99:
                    Environment.Exit(1);
                    break;
                default:
                    Console.WriteLine("No eligio una opcion correcta.");
                    break;
            }
        }

        private static void ViewCopiedData(string ruta, string space = "", int counter = 0)
        {
            var nombre = Path.GetFileName(ruta);
            var filesEntries = Directory.GetFiles(ruta);

            Console.WriteLine(space + "Carpeta: {0}", nombre);
            if (filesEntries.Length > 0)
                Console.WriteLine(space + "Archivos:");
            space = space + "    ";
            foreach (var filesEntry in filesEntries)
            {
                Console.WriteLine(space + Path.GetFileName(filesEntry));
                counter++;
                if (counter <= 20) continue;
                Console.Read();
                counter = 0;
            }

            var directoryEntries = Directory.GetDirectories(ruta);
            foreach (var directoryEntry in directoryEntries)
            {
                ViewCopiedData(directoryEntry, space, counter);
            }
        }

        private static void CopyData()
        {
            var copyFiles = false;
            var copyMergeFiles = false;
            var path = RutaResultado;
            try
            {
                Console.WriteLine("Desea copiar los archivos? (S/N)");
                var comando = Console.ReadLine();
                while (comando.ToUpper() != "S" && comando.ToUpper() != "N")
                {
                    Console.WriteLine("Ingreso una opcion invalida.");
                    comando = Console.ReadLine();
                }

                if (comando.ToUpper() == "S")
                {
                    copyFiles = true;

                    Console.WriteLine("Indique archivos desea copiar:");
                    Console.WriteLine("1. Archivos comunes.");
                    Console.WriteLine("2. Archivos a no mergear.");
                    var comando2 = LeerOpcion();
                    while (comando2 != 1 && comando2 != 2)
                    {
                        Console.WriteLine("Ingreso una opcion invalida.");
                        comando2 = LeerOpcion();
                    }

                    if (comando2 == 2)
                    {
                        Console.WriteLine("Solo se copiaran los siguientes archivos:");
                        Console.WriteLine(string.Join("\n", NoReemplazar));
                        copyMergeFiles = true;
                        path = RutaResultadoMerges;
                    }
                    else
                    {
                        Console.WriteLine("Los siguientes archivos seran exceptuados de la copia:");
                        Console.WriteLine(string.Join("\n", NoReemplazar));
                    }
                }

                var directoryEntries = Directory.GetDirectories(RutaBackup);
                foreach (var directoryEntry in directoryEntries)
                {
                    var nombre = Path.GetFileName(directoryEntry);
                    var directoryName = path + "\\" + nombre;
                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    var directoryEntries2 = Directory.GetDirectories(directoryEntry);
                    foreach (var directoryEntry2 in directoryEntries2)
                    {
                        var nombre2 = Path.GetFileName(directoryEntry2);
                        var directoryName2 = directoryName + "\\" + nombre2;
                        if (!Directory.Exists(directoryName2))
                            Directory.CreateDirectory(directoryName2);

                        var replaceFiles = Directory.GetFiles(RutaReemplazo);
                        foreach (var replaceFile in replaceFiles)
                        {
                            var fileName = Path.GetFileName(replaceFile);
                            if (copyFiles && ((NoReemplazar.All(x => x != fileName) && !copyMergeFiles) || (NoReemplazar.Any(x => x == fileName) && copyMergeFiles)))
                                File.Copy(replaceFile, directoryName2 + "\\" + fileName, false);
                        }

                        var directoryEntries3 = Directory.GetDirectories(RutaReemplazo);
                        foreach (var directoryEntry3 in directoryEntries3)
                        {
                            var nombre3 = Path.GetFileName(directoryEntry3);
                            var directoryName3 = directoryName2 + "\\" + nombre3;
                            if (!Directory.Exists(directoryName3))
                                Directory.CreateDirectory(directoryName3);

                            var replaceFiles2 = Directory.GetFiles(RutaReemplazo + "\\" + nombre3);
                            foreach (var replaceFile2 in replaceFiles2)
                            {
                                var fileName2 = Path.GetFileName(replaceFile2);
                                if (copyFiles && ((NoReemplazar.All(x => x != fileName2) && !copyMergeFiles) || (NoReemplazar.Any(x => x == fileName2) && copyMergeFiles)))
                                    File.Copy(replaceFile2, directoryName3 + "\\" + fileName2, false);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Ha ocurrido un error.");
            }
        }
    }
}
