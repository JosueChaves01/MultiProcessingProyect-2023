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
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Concurrent;

static List<string> LeerArchivo(string ubicacionArchivo)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    List<string> lineasArchivo = new List<string>();

    string[] lineas = File.ReadAllLines(ubicacionArchivo);

    Parallel.Invoke(() =>
    {
        foreach (string linea in lineas)
        {
            lineasArchivo.Add(linea);
        }
    });
    Console.WriteLine("Tiempo de ejecución(lectura de arcivhos): " + temporizador.ElapsedMilliseconds + " milisegundos");
    return lineasArchivo;
}

static Dictionary<string, string> CrearListaCatones(List<string> miLista)
{
    StreamReader archivo = new StreamReader("C:\\Users\\josuc\\Documents\\U\\Arquitectura de Computadores\\padron_completo\\Distelec.txt");
    Dictionary<string, string> miDiccionario = new Dictionary<string, string>();

    while (!archivo.EndOfStream)
    {
        string linea = archivo.ReadLine();
        string[] campos = linea.Split(',');
        string primerElemento = campos[2];
        string segundoElemento = campos[0].Substring(0, 6);

        if (!miDiccionario.ContainsKey(primerElemento))
        {
            miDiccionario.Add(primerElemento, segundoElemento);
        }
    }
    return miDiccionario;
}
static Dictionary<string, string> CrearListaDistritos(List<string> miLista)
{
    StreamReader archivo = new StreamReader("C:\\Users\\josuc\\Documents\\U\\Arquitectura de Computadores\\padron_completo\\Distelec.txt");
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


static void CantidadDeVotantesPorProvincia(List<string> miLista)
{

    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    int degreeOfParallelism = Environment.ProcessorCount;
    object sync = new object();
    List<Dictionary<string, int>> listaCantidadPersonasPorProvncia = new List<Dictionary<string, int>>();
    string provincia = "SAN JOSE";
    Parallel.For(0, 8, workerId =>
    {
        lock (sync)
        {



            if (workerId == 0)
            {
                provincia = "SAN JOSE";
            }
            else if (workerId == 1)
            {
                provincia = "ALAJUELA";
            }
            else if (workerId == 2)
            {
                provincia = "HEREDIA";
            }
            else if (workerId == 3)
            {
                provincia = "CARTAGO";
            }
            else if (workerId == 4)
            {
                provincia = "GUANACASTE";
            }
            else if (workerId == 5)
            {
                provincia = "PUNTARENAS";
            }
            else if (workerId == 6)
            {
                provincia = "LIMON";
            }
            else if (workerId == 7)
            {
                provincia = "Consulado";
            }
            string indiceProv = (workerId + 1).ToString();

            int cont = 0;
            foreach (string linea in miLista)
            {
                if (linea.Substring(10, 1).Equals(indiceProv))
                {
                    cont++; ;
                }
            }


            Dictionary<string, int> listaFinal = new Dictionary<string, int>();
            listaFinal.Add(provincia, cont);
            listaCantidadPersonasPorProvncia.Add(listaFinal);
            listaCantidadPersonasPorProvncia.Sort((x, y) => y.Values.First().CompareTo(x.Values.First()));
        }
    });

    foreach (Dictionary<string, int> prov in listaCantidadPersonasPorProvncia)
    {
        Console.WriteLine("Hay " + prov.Values.First() + " votantes en la provincia " + prov.Keys.First());
    }

    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");


}



static List<Dictionary<string, int>> CantidadDeVotantesPorCanton(List<string> miLista, Dictionary<string, string> miDiccionario)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    object sync = new object();
    List<Dictionary<string, int>> listaCantidadPersonasPorCanton = new List<Dictionary<string, int>>();
    int degreeOfParallelism = Environment.ProcessorCount;


    Parallel.ForEach(miDiccionario, new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism }, (dato) =>
    {
        int cont = 0;
        foreach (string linea in miLista)
        {

            string indiceDistrito = dato.Value;
            lock (sync)
            {

                if (indiceDistrito.Substring(0, 3).Equals(linea.Substring(10, 3)))
                {
                    cont++;
                }
            }

        }

        Dictionary<string, int> listaFinal = new Dictionary<string, int>();
        listaFinal.Add(dato.Key, cont);
        listaCantidadPersonasPorCanton.Add(listaFinal);
    });


    foreach (Dictionary<string, int> canton in listaCantidadPersonasPorCanton)
    {
        Console.WriteLine("El canton " + canton.Keys.First() + " tiene " + canton.Values.First() + " votantes");
    }


    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
    return listaCantidadPersonasPorCanton;
}
static List<Dictionary<string, int>> CantidadDeVotantesPorDistrito(List<string> miLista, Dictionary<string, string> miDiccionario)
{

    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();

    List<Dictionary<string, int>> listaCantidadPersonasPorDistrito = new List<Dictionary<string, int>>();
    int degreeOfParallelism = Environment.ProcessorCount;


    Parallel.ForEach(miDiccionario, new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism }, (dato) =>
    {
        int cont = 0;
        foreach (string linea in miLista)
        {

            string indiceDistrito = dato.Value;

            if (indiceDistrito.Substring(0, 6).Equals(linea.Substring(10, 6)))
            {
                cont++;
            }

        }

        Dictionary<string, int> listaFinal = new Dictionary<string, int>();
        listaFinal.Add(dato.Key, cont);
        listaCantidadPersonasPorDistrito.Add(listaFinal);
    });


    foreach (Dictionary<string, int> canton in listaCantidadPersonasPorDistrito)
    {
        Console.WriteLine("El distrito " + canton.Keys.First() + " tiene " + canton.Values.First() + " votantes");
    }

    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
    return listaCantidadPersonasPorDistrito;
}
static void NApellidosMasCumunes(List<string[]> personas)
{
    Console.WriteLine("Ingrese la cantidad de datos que desea ver: ");
    int c = Console.Read()-40;
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew(); 
    Dictionary<string, int> Apellidos = new Dictionary<string, int>();
    Dictionary<string, int> Apellidos2 = new Dictionary<string, int>();
    Parallel.ForEach(personas, persona =>
    {
        lock (Apellidos)
        {
            if (!Apellidos.ContainsKey(persona[6]))
            {
                Apellidos[persona[6]] = 1;
            }
            else
            {
                Apellidos[persona[6]]++;
            }
        }
    });
    Parallel.ForEach(personas, persona =>
    {
        lock (Apellidos)
        {
            if (!Apellidos.ContainsKey(persona[7]))
            {
                Apellidos[persona[7]] = 1;
            }
            else
            {
                Apellidos[persona[7]]++;
            }
        }
    });
    var sortedApellidos = from pair in Apellidos
                          orderby pair.Value descending
                          select pair;
    int cont = 0;
    Console.WriteLine("N pimeros apellidos mas comunes");
    foreach (KeyValuePair<string, int> kp in sortedApellidos)
    {
        cont++;
        Console.WriteLine(kp.Key + " " + kp.Value);
        if (cont > c)
        {
            break;
        }
    }
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");

}

static void cantidadDePersonasConApellidoParticularP(List<string[]> personas)
{
    Console.WriteLine("Ingrese la cantidad de datos que desea ver: ");
    string apellido = Console.ReadLine();
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    Dictionary<string, int> Apellidos = new Dictionary<string, int>();
    Dictionary<string, int> Apellidos2 = new Dictionary<string, int>();
    Parallel.ForEach(personas, persona =>
    {
        lock (Apellidos)
        {
            if (!Apellidos.ContainsKey(persona[6]))
            {
                Apellidos[persona[6]] = 1;
            }
            else
            {
                Apellidos[persona[6]]++;
            }
        }
    });
    Parallel.ForEach(personas, persona =>
    {
        lock (Apellidos)
        {
            if (!Apellidos.ContainsKey(persona[7]))
            {
                Apellidos[persona[7]] = 1;
            }
            else
            {
                Apellidos[persona[7]]++;
            }
        }
    });
    var sortedApellidos = from pair in Apellidos
                          orderby pair.Value descending
                          select pair;
    int cont = 0;
    
    foreach (KeyValuePair<string, int> kp in sortedApellidos)
    {
        apellido = apellido.ToUpper();
        string dato = kp.Key.Substring(0,apellido.Length);
        if (dato.Equals(apellido))
        {
            Console.WriteLine(kp.Key + " " + kp.Value);
        }
        
    }
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");

}




static void NApellidosMasCumunesP(List<string[]> personas)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    Dictionary<string,int> Apellidos = new Dictionary<string,int>();    
    Dictionary<string,int> Apellidos2 = new Dictionary<string,int>();    
    foreach (string[] persona in personas)
    {
        if (Apellidos == null)
        {
            Apellidos.Add(persona[6],1);
        }
        else if (!Apellidos.ContainsKey(persona[6])) 
        {
            
            Apellidos.Add(persona[6], 1);
            
        }
        else if (Apellidos.ContainsKey(persona[6]))
        {
            Apellidos[persona[6]] = Apellidos[persona[6]] + 1;
        }
    }
    foreach (string[] persona in personas)
    {
        if (Apellidos == null)
        {
            Apellidos.Add(persona[7],1);
        }
        else if (!Apellidos.ContainsKey(persona[7])) 
        {
            
            Apellidos.Add(persona[7], 1);
            
        }
        else if (Apellidos.ContainsKey(persona[7]))
        {
            Apellidos[persona[7]] = Apellidos[persona[7]] + 1;
        }
    }
    

    var sortedApellidos = from pair in Apellidos
                          orderby pair.Value descending
                          select pair;

    int cont = 0;
    foreach(KeyValuePair<string,int> kp in sortedApellidos)
    {
        cont++;
        Console.WriteLine(kp.Key + " " + kp.Value);
        if(cont > 9)
        {
            break;
        }
    }
    
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");

}


static List<string[]> organizarPersonas(List<string> miLista)
{
    //      Retorna un pernsona(List<>) que continene listas de strings(string[])
    //      peronsa[0] =  CEDULA      peronsa[1] =    CODELEC       peronsa[2] =    Relleno     peronsa[3] =    Fecha de vencimiento de la cedula
    //      peronsa[4] =    Junta receptora de votos        peronsa[5] =    NOMBRE      peronsa[6] =    APELLIDO1       peronsa[7] =    APELLIDO2
    //
    //


    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    int degreeOfParallelism = Environment.ProcessorCount;
    object sync = new object();

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
    object sync = new object();

    Parallel.For(0, degreeOfParallelism, workerId =>
    {
        int lim = personas.Count();
        var max = lim * (workerId + 1) / degreeOfParallelism;
        for (int i = (int)lim * workerId / degreeOfParallelism; i < max; i++)
        {
            string tipoIdentificacion = personas[i][2].ToUpper();

            lock (sync)
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

static void NCantonesConMasVotantesRegristrados(List<string> listaP, Dictionary<string, string> miDiccionario, int n)
{
    int c;
    if (n == 0)
    {
        Console.WriteLine("Ingrese la cantidad de datos que desea ver: ");
        c = int.Parse(Console.ReadLine());
    }
    else
    {
        c = n;
    }
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();

    List<Dictionary<string, int>> listaCantidadPersonasPorCanton = new List<Dictionary<string, int>>();
    int degreeOfParallelism = Environment.ProcessorCount;

    Parallel.ForEach(miDiccionario, new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism }, (dato) =>
    {
        int cont = 0;
        foreach (string linea in listaP)
        {

            string indiceCanton = dato.Value;

            if (indiceCanton.Substring(0, 3).Equals(linea.Substring(10, 3)))
            {
                cont++;
            }

        }

        Dictionary<string, int> listaFinal = new Dictionary<string, int>();
        listaFinal.Add(dato.Key, cont);
        listaCantidadPersonasPorCanton.Add(listaFinal);


    });

    listaCantidadPersonasPorCanton.Sort((x, y) => y.Values.First().CompareTo(x.Values.First()));
    for (int i = 0; i < c; i++)
    {
        Console.WriteLine("El canton " + listaCantidadPersonasPorCanton[i].Keys.First() + " tiene " + listaCantidadPersonasPorCanton[i].Values.First() + " votantes");
    }

    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");

}
static void NDistritosConMasVotantesRegristrados(List<string> listaP, Dictionary<string, string> miDiccionario, int n)
{
    int c;
    if (n == 0)
    {
        Console.WriteLine("Ingrese la cantidad de datos que desea ver: ");
        c = int.Parse(Console.ReadLine());
    }
    else
    {
        c = n;
    }
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();

    List<Dictionary<string, int>> listaCantidadPersonasPorDistrito = new List<Dictionary<string, int>>();
    int degreeOfParallelism = Environment.ProcessorCount;


    Parallel.ForEach(miDiccionario, new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism }, (dato) =>
    {
        int cont = 0;
        foreach (string linea in listaP)
        {

            string indiceDistrito = dato.Value;

            if (indiceDistrito.Substring(0, 6).Equals(linea.Substring(10, 6)))
            {
                cont++;
            }

        }

        Dictionary<string, int> listaFinal = new Dictionary<string, int>();
        listaFinal.Add(dato.Key, cont);
        listaCantidadPersonasPorDistrito.Add(listaFinal);
    });

    listaCantidadPersonasPorDistrito.Sort((x, y) => y.Values.First().CompareTo(x.Values.First()));
    for (int i = 0; i < c; i++)
    {
        Console.WriteLine("El distrito " + listaCantidadPersonasPorDistrito[i].Keys.First() + " tiene " + listaCantidadPersonasPorDistrito[i].Values.First() + " votantes");
    }

    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");

}

static void puntoN(List<string> listaPersonas, Dictionary<string, string> listaCantones, Dictionary<string, string> listaDistritos)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    int degreeOfParallelism = Environment.ProcessorCount;
    object sync = new object();

    Parallel.For(0, 5, workerId =>
    {
        if (workerId == 0)
        {
            CantidadDeVotantesPorProvincia(listaPersonas);
            Console.WriteLine("done!0");
        }

        if (workerId == 1)
        {
            CantidadDeVotantesPorCanton(listaPersonas, listaCantones);
            Console.WriteLine("done!1");
        }

        if (workerId == 2)
        {
            CantidadDeVotantesPorDistrito(listaPersonas, listaDistritos);
            Console.WriteLine("done!2");
        }

        if (workerId == 3)
        {
            NCantonesConMasVotantesRegristrados(listaPersonas, listaCantones, 10);
            Console.WriteLine("done!3");
        }

        if (workerId == 4)
        {
            NDistritosConMasVotantesRegristrados(listaPersonas, listaDistritos, 10);
            Console.WriteLine("done!4");
        }

    });
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");


}

//======================MENU PRINCIPAL
static void Menu()
{

    List<string> listaPersonas = LeerArchivo("C:\\Users\\josuc\\Documents\\U\\Arquitectura de Computadores\\padron_completo\\PADRON_COMPLETO.txt");
    List<string> listaDatos = LeerArchivo("C:\\Users\\josuc\\Documents\\U\\Arquitectura de Computadores\\padron_completo\\Distelec.txt");
    List<string[]> listaDatosOrdenados = organizarPersonas(listaPersonas);
    Dictionary<string, string> listaCantones = new Dictionary<string, string>();
    Dictionary<string, string> listaDistritos = new Dictionary<string, string>();
    object sync = new object();
    Parallel.For(0, 2, workeId =>
    {
        lock (sync)
        {

            if (workeId == 0)
            {
                listaDistritos = CrearListaDistritos(listaDatos);
            }
            else if (workeId == 1)
            {
                listaCantones = CrearListaCatones(listaDatos);
            }

        }
    });

    bool valid = true;
    while (valid == true)
    {
        Console.WriteLine("==========Menu de Consultas==========" +
        "\n1.  Buscar persona a partir de cualquier dato" + //Consulta a
        "\n2.  Mostrar cantidad de votantes por provincia" + //Consulta b
        "\n3.  Mostrar cantidad de votantes por canton" + //Consulta c
        "\n4.  Mostrar cantidad de votantes por distrito" + //Consulta d
        "\n5.  Mostrar los N cantones con mas votantes registrados" + //Consulta e
        "\n6.  Mostrar los N distritos con mas votantes registrados" + //Consulta f
        "\n7.  Mostrar cantidad de personas por tipo de identificacion" + //Consulta g
        "\n8.  Mostrar personas con cedula expirada en fecha dada" + //Consulta h
        "\n9.  Mostrar cantidad de personas con un nombre en particular" + //Consulta i
        "\n10. Mostrar cantidad de personas con un apellido en particular" + //Consulta j
        "\n11. Mostrar los N nombres mas comunes" + //Consulta k
        "\n12  Mostrar los N apellidos mas comunes" + //Consulta l
        "\n13. Mostrar los N nombres menos comunes" + //Consulta m
        "\n14. Ejecutar las consultas 2, 3, 4, 5, 6, 7, 8, 11, 12 y 13" + //Consulta n
        "\nElige que consulta deseas revisar: ");
        int op = Convert.ToInt16(Console.ReadLine());

        switch (op)
        {
            case 1:
                buscarPersona(listaPersonas, listaDatosOrdenados);
                break;
            case 2:
                CantidadDeVotantesPorProvincia(listaPersonas);
                break;
            case 3:
                CantidadDeVotantesPorCanton(listaPersonas, listaCantones);
                break;
            case 4:
                CantidadDeVotantesPorDistrito(listaPersonas, listaDistritos);
                break;
            case 5:
                NCantonesConMasVotantesRegristrados(listaPersonas, listaCantones, 0);
                break;
            case 6:
                NDistritosConMasVotantesRegristrados(listaPersonas, listaDistritos, 0);
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                cantidadDePersonasConApellidoParticularP(listaDatosOrdenados);
                break;
            case 11:
                break;
            case 12:
                NApellidosMasCumunes(listaDatosOrdenados);
                break;
            case 13:
                break;
            case 14:
                puntoN(listaPersonas, listaCantones, listaDistritos);
                break;
            default:
                Console.WriteLine("--------------------OPCION INVALIDA--------------------");
                valid = false;
                break;
        }
    }
}

//=================================FUNCIONES DE FORMA SECUENCIAL=================================================
static void CantidadDeVotantesPorProvinciaSec(List<string> miLista)
{

    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    int degreeOfParallelism = Environment.ProcessorCount;
    object sync = new object();
    List<Dictionary<string, int>> listaCantidadPersonasPorProvncia = new List<Dictionary<string, int>>();
    string provincia = "SAN JOSE";
    for (int workerId = 0; workerId <= 7; workerId++)
    {


        if (workerId == 0)
        {
            provincia = "SAN JOSE";
        }
        else if (workerId == 1)
        {
            provincia = "ALAJUELA";
        }
        else if (workerId == 2)
        {
            provincia = "HEREDIA";
        }
        else if (workerId == 3)
        {
            provincia = "CARTAGO";
        }
        else if (workerId == 4)
        {
            provincia = "GUANACASTE";
        }
        else if (workerId == 5)
        {
            provincia = "PUNTARENAS";
        }
        else if (workerId == 6)
        {
            provincia = "LIMON";
        }
        else if (workerId == 7)
        {
            provincia = "Consulado";
        }
        string indiceProv = (workerId + 1).ToString();

        int cont = 0;
        foreach (string linea in miLista)
        {
            if (linea.Substring(10, 1).Equals(indiceProv))
            {
                cont++; ;
            }


        Dictionary<string, int> listaFinal = new Dictionary<string, int>();
        listaFinal.Add(provincia, cont);
        listaCantidadPersonasPorProvncia.Add(listaFinal);
        listaCantidadPersonasPorProvncia.Sort((x, y) => y.Values.First().CompareTo(x.Values.First()));
        }
    }

    foreach (Dictionary<string, int> prov in listaCantidadPersonasPorProvncia)
    {
        Console.WriteLine("Hay " + prov.Values.First() + " votantes en la provincia " + prov.Keys.First());
    }

    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");


}


static void ConsultarPersonasPorCantonSecuencial(List<string> miLista, Dictionary<string, string> miDiccionario)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    int cont = 0;
    List<Dictionary<string, int>> listaCantidadPersonasPorCanton = new List<Dictionary<string, int>>();

    foreach (KeyValuePair<string, string> datoCanton in miDiccionario)
    {
        foreach (string element in miLista)
        {
            if (element.Substring(10, 3).Equals(datoCanton.Value.Substring(0, 3)))
            {

                cont++;

            }
            Dictionary<string, int> listaFinal = new Dictionary<string, int>();
            listaFinal.Add(datoCanton.Key, cont);
            listaCantidadPersonasPorCanton.Add(listaFinal);
        }
    }
    Console.WriteLine("Hay " + cont + " votantes en el canton descrito. (Secuencial)");
    Console.WriteLine("Tiempo de ejecución(Cantidad de votantes por canton): " + temporizador.ElapsedMilliseconds + " milisegundos");
}


static List<Dictionary<string, int>> CantidadDeVotantesPorCantonSec(List<string> miLista, Dictionary<string, string> miDiccionario)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    object sync = new object();
    List<Dictionary<string, int>> listaCantidadPersonasPorCanton = new List<Dictionary<string, int>>();

    foreach (KeyValuePair<string, string> dato in miDiccionario)
    {
        int cont = 0;
        foreach (string linea in miLista)
        {
            string indiceDistrito = dato.Value;
            if (indiceDistrito.Substring(0, 3).Equals(linea.Substring(10, 3)))
            {
                cont++;
            }
        }

        Dictionary<string, int> listaFinal = new Dictionary<string, int>();
        listaFinal.Add(dato.Key, cont);
        listaCantidadPersonasPorCanton.Add(listaFinal);
    }

    foreach (Dictionary<string, int> canton in listaCantidadPersonasPorCanton)
    {
        Console.WriteLine("El canton " + canton.Keys.First() + " tiene " + canton.Values.First() + " votantes");
    }

    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
    return listaCantidadPersonasPorCanton;
}
static List<Dictionary<string, int>> CantidadDeVotantesPorDistritoSec(List<string> miLista, Dictionary<string, string> miDiccionario)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();

    List<Dictionary<string, int>> listaCantidadPersonasPorDistrito = new List<Dictionary<string, int>>();

    foreach (KeyValuePair<string, string> dato in miDiccionario)
    {
        int cont = 0;
        foreach (string linea in miLista)
        {
            string indiceDistrito = dato.Value;

            if (indiceDistrito.Substring(0, 6).Equals(linea.Substring(10, 6)))
            {
                cont++;
            }
        }

        Dictionary<string, int> listaFinal = new Dictionary<string, int>();
        listaFinal.Add(dato.Key, cont);
        listaCantidadPersonasPorDistrito.Add(listaFinal);
    }

    foreach (Dictionary<string, int> distrito in listaCantidadPersonasPorDistrito)
    {
        Console.WriteLine("El distrito " + distrito.Keys.First() + " tiene " + distrito.Values.First() + " votantes");
    }

    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
    return listaCantidadPersonasPorDistrito;
}
static void NApellidosMasCumunesSec(List<string[]> personas)
{
    Console.WriteLine("Ingrese la cantidad de datos que desea ver: ");
    int c = Console.Read() - 40;
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    Dictionary<string, int> Apellidos = new Dictionary<string, int>();

    foreach (string[] persona in personas)
    {
        if (!Apellidos.ContainsKey(persona[6]))
        {
            Apellidos[persona[6]] = 1;
        }
        else
        {
            Apellidos[persona[6]]++;
        }

        if (!Apellidos.ContainsKey(persona[7]))
        {
            Apellidos[persona[7]] = 1;
        }
        else
        {
            Apellidos[persona[7]]++;
        }
    }

    var sortedApellidos = from pair in Apellidos
                          orderby pair.Value descending
                          select pair;
    int cont = 0;
    Console.WriteLine("N pimeros apellidos mas comunes");
    foreach (KeyValuePair<string, int> kp in sortedApellidos)
    {
        cont++;
        Console.WriteLine(kp.Key + " " + kp.Value);
        if (cont > c)
        {
            break;
        }
    }
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
}


static void cantidadDePersonasConApellidoParticularSec(List<string[]> personas)
{
    Console.WriteLine("Ingrese el apellido que desea buscar: ");
    string apellido = Console.ReadLine();
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    Dictionary<string, int> Apellidos = new Dictionary<string, int>();
    Dictionary<string, int> Apellidos2 = new Dictionary<string, int>();
    foreach (string[] persona in personas)
    {
        if (!Apellidos.ContainsKey(persona[6]))
        {
            Apellidos[persona[6]] = 1;
        }
        else
        {
            Apellidos[persona[6]]++;
        }
    }
    foreach (string[] persona in personas)
    {
        if (!Apellidos.ContainsKey(persona[7]))
        {
            Apellidos[persona[7]] = 1;
        }
        else
        {
            Apellidos[persona[7]]++;
        }
    }
    var sortedApellidos = from pair in Apellidos
                          orderby pair.Value descending
                          select pair;
    int cont = 0;
    apellido = apellido.ToUpper();
    foreach (KeyValuePair<string, int> kp in sortedApellidos)
    {
        string dato = kp.Key.Substring(0, apellido.Length);
        if (dato.Equals(apellido))
        {
            Console.WriteLine(kp.Key + " " + kp.Value);
            cont++;
            if (cont >= 10)
            {
                break;
            }
        }
    }
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
}


static void NApellidosMasCumunesPSec(List<string[]> personas)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    Dictionary<string, int> Apellidos = new Dictionary<string, int>();
    Dictionary<string, int> Apellidos2 = new Dictionary<string, int>();
    foreach (string[] persona in personas)
    {
        if (Apellidos == null)
        {
            Apellidos.Add(persona[6], 1);
        }
        else if (!Apellidos.ContainsKey(persona[6]))
        {

            Apellidos.Add(persona[6], 1);

        }
        else if (Apellidos.ContainsKey(persona[6]))
        {
            Apellidos[persona[6]] = Apellidos[persona[6]] + 1;
        }
    }
    foreach (string[] persona in personas)
    {
        if (Apellidos == null)
        {
            Apellidos.Add(persona[7], 1);
        }
        else if (!Apellidos.ContainsKey(persona[7]))
        {

            Apellidos.Add(persona[7], 1);

        }
        else if (Apellidos.ContainsKey(persona[7]))
        {
            Apellidos[persona[7]] = Apellidos[persona[7]] + 1;
        }
    }


    var sortedApellidos = from pair in Apellidos
                          orderby pair.Value descending
                          select pair;

    int cont = 0;
    Console.WriteLine("N pimeros apellidos mas comunes");
    foreach (KeyValuePair<string, int> kp in sortedApellidos)
    {
        cont++;
        Console.WriteLine(kp.Key + " " + kp.Value);
        if (cont > 9)
        {
            break;
        }
    }

    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
}

static void buscarPersonaSec(List<string> miLista, List<string[]> personas)
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

    for (int i = 0; i < personas.Count; i++)
    {
        string datoPersona = personas[i][tipoDato].Substring(0, datoBuscado.Length);

        if (datoPersona.Equals(datoBuscado))
        {
            Console.WriteLine(personas[i][0] + " | " + personas[i][1] + " | " + personas[i][3] + " | " + personas[i][5] + " | " + personas[i][6] + " | " + personas[i][7]);
        }
    }

    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
}



//=============================CANTIDAD DE PERSONAS POR IDENTIFICACION======================
static void PersonasPorIdentificacionSec(List<string[]> personas)
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
    object sync = new object();

    Parallel.For(0, degreeOfParallelism, workerId =>
    {
        int lim = personas.Count();
        var max = lim * (workerId + 1) / degreeOfParallelism;
        for (int i = (int)lim * workerId / degreeOfParallelism; i < max; i++)
        {
            string tipoIdentificacion = personas[i][2].ToUpper();

            lock (sync)
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

static void NCantonesConMasVotantesRegistradosSec(List<string> listaP, Dictionary<string, string> miDiccionario, int n)
{
    int c;
    if (n == 0)
    {
        Console.WriteLine("Ingrese la cantidad de datos que desea ver: ");
        c = int.Parse(Console.ReadLine());
    }
    else
    {
        c = n;
    }
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();

    List<Dictionary<string, int>> listaCantidadPersonasPorCanton = new List<Dictionary<string, int>>();

    foreach (KeyValuePair<string, string> dato in miDiccionario)
    {
        int cont = 0;
        foreach (string linea in listaP)
        {
            string indiceCanton = dato.Value;

            if (indiceCanton.Substring(0, 3).Equals(linea.Substring(10, 3)))
            {
                cont++;
            }
        }

        Dictionary<string, int> listaFinal = new Dictionary<string, int>();
        listaFinal.Add(dato.Key, cont);
        listaCantidadPersonasPorCanton.Add(listaFinal);
    }

    listaCantidadPersonasPorCanton.Sort((x, y) => y.Values.First().CompareTo(x.Values.First()));
    for (int i = 0; i < c; i++)
    {
        Console.WriteLine("El canton " + listaCantidadPersonasPorCanton[i].Keys.First() + " tiene " + listaCantidadPersonasPorCanton[i].Values.First() + " votantes");
    }

    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
}
static void NDistritosConMasVotantesRegristradosSec(List<string> listaP, Dictionary<string, string> miDiccionario, int n)
{
    int c;
    if (n == 0)
    {
        Console.WriteLine("Ingrese la cantidad de datos que desea ver: ");
        c = int.Parse(Console.ReadLine());
    }
    else
    {
        c = n;
    }
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();

    List<Dictionary<string, int>> listaCantidadPersonasPorDistrito = new List<Dictionary<string, int>>();

    foreach (KeyValuePair<string, string> dato in miDiccionario)
    {
        int cont = 0;
        foreach (string linea in listaP)
        {
            string indiceDistrito = dato.Value;

            if (indiceDistrito.Substring(0, 6).Equals(linea.Substring(10, 6)))
            {
                cont++;
            }
        }

        Dictionary<string, int> listaFinal = new Dictionary<string, int>();
        listaFinal.Add(dato.Key, cont);
        listaCantidadPersonasPorDistrito.Add(listaFinal);
    }

    listaCantidadPersonasPorDistrito.Sort((x, y) => y.Values.First().CompareTo(x.Values.First()));
    for (int i = 0; i < c; i++)
    {
        Console.WriteLine("El distrito " + listaCantidadPersonasPorDistrito[i].Keys.First() + " tiene " + listaCantidadPersonasPorDistrito[i].Values.First() + " votantes");
    }

    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
}
static void puntoNSec(List<string> listaPersonas, Dictionary<string, string> listaCantones, Dictionary<string, string> listaDistritos)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    object sync = new object();

    for(int workerId = 0; workerId < 5; workerId++)
    {
        if (workerId == 0)
        {
            CantidadDeVotantesPorProvinciaSec(listaPersonas);
            Console.WriteLine("done!0");
        }

        if (workerId == 1)
        {
            CantidadDeVotantesPorCantonSec(listaPersonas, listaCantones);
            Console.WriteLine("done!1");
        }

        if (workerId == 2)
        {
            CantidadDeVotantesPorDistritoSec(listaPersonas, listaDistritos);
            Console.WriteLine("done!2");
        }

        if (workerId == 3)
        {
            NCantonesConMasVotantesRegistradosSec(listaPersonas, listaCantones, 10);
            Console.WriteLine("done!3");
        }

        if (workerId == 4)
        {
            NDistritosConMasVotantesRegristradosSec(listaPersonas, listaDistritos, 10);
            Console.WriteLine("done!4");
        }

    }
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");


}

Menu();
//
//CantidadDeVotantesPorCanton(listaPersonas, listaCantones);
//CantidadDeVotantesPorDistrito(listaPersonas, listaDistritos);

//PersonasPorIdentificacion(listaDatosOrdenados);
//
//
