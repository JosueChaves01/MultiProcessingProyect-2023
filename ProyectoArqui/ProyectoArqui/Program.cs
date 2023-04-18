using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Collections;
using System.Runtime.InteropServices;

static List<string> ConvertirArchivoALista(string ubicacionArchivo)
{
    List<string> lineasArchivo = new List<string>();

    try
    {
        using (StreamReader lector = new StreamReader(ubicacionArchivo))
        {
            string linea;
            int cont = Environment.ProcessorCount;
            
            
            while ((linea = lector.ReadLine()) != null)
            {
                lineasArchivo.Add(linea);
            }

            

        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Ocurrió un error al leer el archivo: " + ex.Message);
    }

    return lineasArchivo;
}

static void ImprimirLista(List<string> miLista)
{
    Parallel.ForEach(miLista, elemento =>
    {
        Console.WriteLine(elemento);
    });
}

static List<string> CantidadDeVotantesPorProvinciaLista(List<string> miLista)
{
    List<string> nuevaLista = new List<string>();
    int cont = 0;
    Parallel.ForEach(miLista, elemento =>
    {
        if (elemento.Substring(10, 1).Equals("1"))
        {
            nuevaLista.Add(elemento);
            cont++;

        }
    });
        Console.WriteLine("Hay " + cont + " votante en San Jose.");
     
    return nuevaLista;

}

static void CantidadDeVotantesPorProvincia(List<string> miLista)
{
    Console.WriteLine(" 1 -San Jose\n 2 -Alajuela\n 3 -Cartago\n 4 -Heredia\n 5 -Guanacaste\n 6 -Puntarenas\n 7 -Limon\n 8 -Cosulado");
    Console.WriteLine("ingrese el indice de la provincia para el que desea saber la cantidad de votantes:");
    string canton = Console.ReadLine();
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    int cont = 0;
    int degreeOfParallelism = Environment.ProcessorCount;
    object sync = new object();
    Parallel.For(0, degreeOfParallelism, workerId =>
    {
        int lim = miLista.Count();
        var max = lim * (workerId + 1) / degreeOfParallelism;
        for (int i = (int)lim * workerId / degreeOfParallelism; i < max; i++)

            if (miLista[i].Substring(10, 1).Equals(canton))
            {
                lock (sync)
                {
                    cont++;
                }
            }
    });
    Console.WriteLine("Hay " + cont + " votantes en la provincia.");
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");


}

static void ConsultarPersonasPorCanton(List<string> miLista, string canton)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    object sync = new object();
    int cont = 0;
    Parallel.ForEach(miLista, elemento =>
    {
        if (elemento.Substring(10, 4).Equals(canton))
        {
            lock(sync)
            {
                cont++;
            }
        }

    });
        Console.WriteLine("Hay " + cont + " votantes en el canton descrito.");
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos(Paralelismo, ForEach con Lock)");

}
static void ConsultarPersonasPorCantonSecuencial(List<string> miLista, string canton)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    int cont = 0;

    foreach(string element in miLista)
    {
        if (element.Substring(10, 4).Equals(canton))
        {

            cont++;
            
        }
    }
    Console.WriteLine("Hay " + cont + " votantes en el canton descrito. (Secuencial)");
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
}

static void ConsultarPersonasPorCantonForLimitado(List<string> miLista, string canton)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    int degreeOfParallelism = Environment.ProcessorCount;
    object sync = new object();
    int cont = 0;
    Parallel.For(0, degreeOfParallelism, workerId =>
    {
        int lim = miLista.Count(); 
        var max = lim * (workerId + 1) / degreeOfParallelism;
        for (int i = (int)lim * workerId / degreeOfParallelism; i < max; i++)
            
            if (miLista[i].Substring(10, 4).Equals(canton))
            {
                lock (sync)
                {
                    cont++;
                }
            }
    });
    Console.WriteLine("Hay " + cont + " votantes en el canton descrito. (For limitado)");
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
}

static Dictionary<string, string> CrearListaDatos(List<string> miLista)
{
    StreamReader archivo = new StreamReader("C:/Users/josuc/Desktop/padron_completo/Distelec.txt");
    Dictionary<string, string> miDiccionario = new Dictionary<string, string>();
    
    while (!archivo.EndOfStream)
    {
        string linea = archivo.ReadLine();
        string[] campos = linea.Split(',');
        string primerElemento = campos[2];
        string segundoElemento = campos[0].Substring(0,6);

        if (!miDiccionario.ContainsKey(primerElemento))
        {
            miDiccionario.Add(primerElemento, segundoElemento);
        }
    }
    return miDiccionario;
}
static Dictionary<string, string> CrearListaDistritos(List<string> miLista)
{
    StreamReader archivo = new StreamReader("C:/Users/josuc/Desktop/padron_completo/Distelec.txt");
    Dictionary<string, string> miDiccionario = new Dictionary<string, string>();

    while (!archivo.EndOfStream)
    {
        string linea = archivo.ReadLine();
        string[] campos = linea.Split(',');
        string primerElemento = campos[3];
        string segundoElemento = campos[0].Substring(0, 6);

        if (!miDiccionario.ContainsKey(primerElemento))
        {
            miDiccionario.Add(primerElemento, segundoElemento);
        }
    }
    return miDiccionario;
}
static void imprimirDiccionario(Dictionary<string, string> miDiccionario)
{
    foreach (KeyValuePair<string, string> par in miDiccionario)
    {
        Console.WriteLine("Canton: " + par.Key + " , Codigo: " + par.Value);
    }
}

static void CantidadDeVotantesPorCanton(List<string> miLista, Dictionary<string, string> miDiccionario)
{
    Console.WriteLine("Ingrese el nombre del canton:");
    string canton = Console.ReadLine().ToUpper();

    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    int degreeOfParallelism = Environment.ProcessorCount;
    object sync = new object();
    int cont = 0;
    canton = miDiccionario[canton].Substring(0,3);
    Console.WriteLine(canton);
    Parallel.For(0, degreeOfParallelism, workerId =>
    {
        int lim = miLista.Count();
        var max = lim * (workerId + 1) / degreeOfParallelism;
        for (int i = (int)lim * workerId / degreeOfParallelism; i < max; i++)

            if (miLista[i].Substring(10, 3).Equals(canton))
            {
                lock (sync)
                {
                    cont++;
                }
            }
    });
    Console.WriteLine("Hay " + cont + " votantes en el canton descrito. (For limitado)");
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");

}
static List<string[]> organizarPersonas(List<string> miLista)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    int degreeOfParallelism = Environment.ProcessorCount;
    object sync = new object();
    int cont = 0;
    List<string[]> personas = new List<string[]>();
    Parallel.For(0, degreeOfParallelism, workerId =>
    {
        int lim = miLista.Count();
        var max = lim * (workerId + 1) / degreeOfParallelism;
        for (int i = (int)lim * workerId / degreeOfParallelism; i < max; i++)
        {

            lock (sync)
            {
                string linea = miLista[i];
                string[] persona = linea.Split(',');
                personas.Add(persona);
            }
        }
    });
    return personas;
}
static void buscarPersona(List<string> miLista, List<string[]> personas)
{
    Console.WriteLine("Ingrese el dato que desea buscar:");
    string datoBuscado = Console.ReadLine().ToUpper();
    Console.WriteLine("Ingrese el tipo de dato por el que desea buscar, de ingresar un tipo de dato incorrecto se buscara por cedula");
    Console.WriteLine("- 0 Cedula\n- 1 Codigo Electoral\n- 2 fecha de vencimiento de la cedula(01012030)\n- 3 Nombre\n- 4 Primer Apellido\n- 5 Segundo Apellido");
    string tipoDatoS = Console.ReadLine();
    int tipoDato = 5;
    if (tipoDatoS.Equals('0'))
    {
        tipoDato = 0;
    }
    else if (tipoDatoS.Equals("1"))
    {
        tipoDato = 1;
    }
    else if (tipoDatoS.Equals("2"))
    {
        tipoDato = 3;
    }
    else if (tipoDatoS.Equals("3"))
    {
        tipoDato = 5;
    }
    else if (tipoDatoS.Equals("4"))
    {
        tipoDato = 6;
    }
    else if (tipoDatoS.Equals("5"))
    {
        tipoDato = 7;
    }
    else
    {
        tipoDato = 0;
    }

    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    int degreeOfParallelism = Environment.ProcessorCount;
    object sync = new object();
    int cont = 0;
    
    Parallel.For(0, degreeOfParallelism, workerId =>
    {

        int lim = miLista.Count();
        var max = lim * (workerId + 1) / degreeOfParallelism;
        for (int i = (int)lim * workerId / degreeOfParallelism; i < max; i++)
        {
            string datoPersona = personas[i][tipoDato].Substring(0, datoBuscado.Length);

            
                if (datoPersona.Equals(datoBuscado))
                {
                    Console.WriteLine(personas[i][0] + " | " + personas[i][1] + " | " + personas[i][3] + " | " + personas[i][5] + " | " + personas[i][6] + " | " + personas[i][7]);
                }
            
            
        }
    });



    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
}

static void CantidadDeVotantesPorDistrito(List<string> miLista, Dictionary<string, string> miDiccionario)
{
    Console.WriteLine("Ingrese el nombre del distrito:");
    string distrito = Console.ReadLine().ToUpper();

    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    int degreeOfParallelism = Environment.ProcessorCount;
    object sync = new object();
    int cont = 0;
    for (int i = 0; i < 52; i++)
    {
        if (distrito.Length < 53)
        {
            distrito = distrito + " ";

        }

    }
    try
    {
        distrito = miDiccionario[distrito];
    }
    catch (Exception ex)
    {
            
            Console.WriteLine("El distrito ingresado no existe!");
            return;
    }



    int cont2 = 0; 
    Parallel.For(0, degreeOfParallelism, workerId =>
    {
        int lim = miLista.Count();
        var max = lim * (workerId + 1) / degreeOfParallelism;
        for (int i = (int)lim * workerId / degreeOfParallelism; i < max; i++)

            if (miLista[i].Substring(10, 6).Equals(distrito))
            {
                lock (sync)
                {
                    cont++;
                }
            }
            else
            {
                lock(sync)
                { 
                    cont2++;
                }
            }
    });

    Console.WriteLine("Hay " + cont + " votantes en el distrito descrito.");
    Console.WriteLine((cont + cont2)-miLista.Count());
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");

}

    //=============================CANTIDAD DE PERSONAS POR IDENTIFICACION======================
    static void PersonasPorIdentificacion(List<string[]> personas)
    {
        Dictionary<string, int> personasPorIdentificacion = new Dictionary<string, int>()
    {
        { "CEDULA", 0 },
        { "CODIGO ELECTORAL", 0 },
        { "FECHA DE VENCIMIENTO", 0 }
    };

        Stopwatch temporizador;
        temporizador = Stopwatch.StartNew();
        int degreeOfParallelism = Environment.ProcessorCount;

        Parallel.For(0, degreeOfParallelism, workerId =>
        {
            int lim = personas.Count();
            var max = lim * (workerId + 1) / degreeOfParallelism;
            for (int i = (int)lim * workerId / degreeOfParallelism; i < max; i++)
            {
                string tipoIdentificacion = personas[i][2].ToUpper();

                lock (personasPorIdentificacion)
                {
                    if (personasPorIdentificacion.ContainsKey(tipoIdentificacion))
                    {
                        personasPorIdentificacion[tipoIdentificacion]++;
                    }
                }
            }
        });

        foreach (KeyValuePair<string, int> kvp in personasPorIdentificacion)
        {
            Console.WriteLine("Tipo de identificación: " + kvp.Key + " | Cantidad de personas: " + kvp.Value);
        }

        Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
    };


List<string> listaPersonas = ConvertirArchivoALista("C:/Users/josuc/Desktop/padron_completo/PADRON_COMPLETO.txt");
List<string> listaDatos = ConvertirArchivoALista("C:/Users/josuc/Desktop/padron_completo/Distelec.txt");
List<string[]> listaDatosOrdenados = organizarPersonas(listaPersonas);
//ImprimirLista(CantidadDeVotantesPorProvinciaLista(miLista));
//CantidadDeVotantesPorProvincia(listaPersonas);
//CantidadDeVotantesPorCanton(listaPersonas, CrearListaDatos(listaDatos));
CantidadDeVotantesPorDistrito(listaPersonas, CrearListaDistritos(listaDatos));
//buscarPersona(listaPersonas,listaDatosOrdenados);