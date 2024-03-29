﻿
using System.Diagnostics;
using System.IO.Pipes;

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
            
        }
    });

    listaCantidadPersonasPorProvncia.Sort((x, y) => y.Values.First().CompareTo(x.Values.First()));
    foreach (Dictionary<string, int> prov in listaCantidadPersonasPorProvncia)
    {
        Console.WriteLine("Hay " + prov.Values.First() + " votantes en la provincia " + prov.Keys.First());
    }

    Console.WriteLine("Tiempo de ejecución(CANTIDAD DE VOTANTES POR PROVINCIA PARARELO): " + temporizador.ElapsedMilliseconds + " milisegundos");


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

    listaCantidadPersonasPorCanton.Sort((x, y) => y.Values.First().CompareTo(x.Values.First()));
    foreach (Dictionary<string, int> canton in listaCantidadPersonasPorCanton)
    {
        Console.WriteLine("El canton " + canton.Keys.First() + " tiene " + canton.Values.First() + " votantes");
    }


    Console.WriteLine("Tiempo de ejecución(CANTIDAD DE VOTANTES POR CANTON PARARELO): " + temporizador.ElapsedMilliseconds + " milisegundos");
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

    listaCantidadPersonasPorDistrito.Sort((x, y) => y.Values.First().CompareTo(x.Values.First()));
    foreach (Dictionary<string, int> canton in listaCantidadPersonasPorDistrito)
    {
        Console.WriteLine("El distrito " + canton.Keys.First() + " tiene " + canton.Values.First() + " votantes");
    }

    Console.WriteLine("Tiempo de ejecución(CANTIDAD DE VOTANTES POR DISTRITO PARARELO): " + temporizador.ElapsedMilliseconds + " milisegundos");
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
    Console.WriteLine("Tiempo de ejecución(N APELLIDOS MAS COMUNES PARARELO): " + temporizador.ElapsedMilliseconds + " milisegundos");

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
    Console.WriteLine("Tiempo de ejecución(CANTIDAD DE PERSONAS CON APELLIDO PARTICULAR): " + temporizador.ElapsedMilliseconds + " milisegundos");

}




static void NApellidosMasCumunesP(List<string[]> personas, int x)
{
    if(x == 0)
    {
        Console.WriteLine("Ingrese la cantidad de datos que desea ver: ");
        x = Console.Read();
    }
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
        if(cont > x)
        {
            break;
        }
    }
    
    Console.WriteLine("Tiempo de ejecución(N APELLIDO MAS COMUNES PARARELO): " + temporizador.ElapsedMilliseconds + " milisegundos");

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



    Console.WriteLine("Tiempo de ejecución(BUSCAR PERSONA PARARELO): " + temporizador.ElapsedMilliseconds + " milisegundos");
}

static void NNombreMasComunesSec(List<string[]> lineas, int n)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    
    Dictionary<string, int> primerosNombres = new Dictionary<string, int>();
    Dictionary<string, int> segundosNombres = new Dictionary<string, int>();

    foreach (string[] linea in lineas)
    {
        string[] partesNombre = linea[5].Split(' ');

        if (partesNombre.Length > 0)
        {
            string primerNombre = partesNombre[0].Trim();

            if (!string.IsNullOrEmpty(primerNombre))
            {
                if (!primerosNombres.ContainsKey(primerNombre))
                {
                    primerosNombres.Add(primerNombre, 1);
                }
                else
                {
                    primerosNombres[primerNombre]++;
                }
            }
        }

        if (partesNombre.Length > 1)
        {
            string segundoNombre = partesNombre[1].Trim();

            if (!string.IsNullOrEmpty(segundoNombre))
            {
                if (!segundosNombres.ContainsKey(segundoNombre))
                {
                    segundosNombres.Add(segundoNombre, 1);
                }
                else
                {
                    segundosNombres[segundoNombre]++;
                }
            }
        }
    }

    Console.WriteLine($"Los {n} primeros nombres más comunes son:");
    var sortedPrimerosNombres = from pair in primerosNombres
                                orderby pair.Value descending
                                select pair;
    segundosNombres.Remove("DEL");
    segundosNombres.Remove("DE");
    int cont = 0;
    foreach (KeyValuePair<string, int> nombre in sortedPrimerosNombres)
    {
        cont++;

        Console.WriteLine($"{nombre.Key}: {nombre.Value}");

        if (cont >= n)
        {
            break;
        }
    }

    Console.WriteLine();

    Console.WriteLine($"Los {n} segundos nombres más comunes son:");
    var sortedSegundosNombres = from pair in segundosNombres
                                orderby pair.Value descending
                                select pair;

    cont = 0;
    foreach (KeyValuePair<string, int> nombre in sortedSegundosNombres)
    {
        cont++;

        Console.WriteLine($"{nombre.Key}: {nombre.Value}");

        if (cont >= n)
        {
            break;
        }
    }
    

    Console.WriteLine("Tiempo de ejecución(CANTIDAD DE PERSONAS CON UN NOMBRE EN PARTICULAR SECUANCIAL): " + temporizador.ElapsedMilliseconds + " milisegundos");
}
static void NNombreMasComunes(List<string[]> lineas, int n)
{
    if(n == 0)
    {
        Console.WriteLine("Ingrese la cantidad de datos que desea ver: ");
        n = Console.Read() - 39;
    }

    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();

    Dictionary<string, int> primerosNombres = new Dictionary<string, int>();
    Dictionary<string, int> segundosNombres = new Dictionary<string, int>();

    object sync = new object();

    Parallel.ForEach(lineas, linea =>
    {
        lock (sync)
        {
            string[] partesNombre = linea[5].Split(' ');

            if (partesNombre.Length > 0)
            {
                string primerNombre = partesNombre[0].Trim();

                if (!string.IsNullOrEmpty(primerNombre))
                {
                    if (!primerosNombres.ContainsKey(primerNombre))
                    {
                        primerosNombres.Add(primerNombre, 1);
                    }
                    else
                    {
                        primerosNombres[primerNombre]++;
                    }
                }
            }

            if (partesNombre.Length > 1)
            {
                string segundoNombre = partesNombre[1].Trim();

                if (!string.IsNullOrEmpty(segundoNombre))
                {
                    if (!segundosNombres.ContainsKey(segundoNombre))
                    {
                        segundosNombres.Add(segundoNombre, 1);
                    }
                    else
                    {
                        segundosNombres[segundoNombre]++;
                    }
                }
            }
        }
    });

    Console.WriteLine($"Los {n} primeros nombres más comunes son:");
    var sortedPrimerosNombres = from pair in primerosNombres
                                orderby pair.Value descending
                                select pair;
    segundosNombres.Remove("DEL");
    segundosNombres.Remove("DE");
    int cont = 0;
    foreach (KeyValuePair<string, int> nombre in sortedPrimerosNombres)
    {
        cont++;

        Console.WriteLine($"{nombre.Key}: {nombre.Value}");

        if (cont >= n)
        {
            break;
        }
    }

    Console.WriteLine();

    Console.WriteLine($"Los {n} segundos nombres más comunes son:");
    var sortedSegundosNombres = from pair in segundosNombres
                                orderby pair.Value descending
                                select pair;

    cont = 0;
    foreach (KeyValuePair<string, int> nombre in sortedSegundosNombres)
    {
        cont++;

        Console.WriteLine($"{nombre.Key}: {nombre.Value}");

        if (cont >= n)
        {
            break;
        }
    }


    Console.WriteLine("Tiempo de ejecución(CANTIDAD DE PERSONAS CON UN NOMBRE EN PARTICULAR PARALELO): " + temporizador.ElapsedMilliseconds + " milisegundos");
}static void NNombreMenosComunes(List<string[]> lineas, int n)
{
    if(n == 0)
    {
        Console.WriteLine("Ingrese la cantidad de datos que desea ver: ");
        n = Console.Read() - 39;
    }

    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();

    Dictionary<string, int> primerosNombres = new Dictionary<string, int>();
    Dictionary<string, int> segundosNombres = new Dictionary<string, int>();

    object sync = new object();

    Parallel.ForEach(lineas, linea =>
    {
        lock (sync)
        {
            string[] partesNombre = linea[5].Split(' ');

            if (partesNombre.Length > 0)
            {
                string primerNombre = partesNombre[0].Trim();

                if (!string.IsNullOrEmpty(primerNombre))
                {
                    if (!primerosNombres.ContainsKey(primerNombre))
                    {
                        primerosNombres.Add(primerNombre, 1);
                    }
                    else
                    {
                        primerosNombres[primerNombre]++;
                    }
                }
            }

            if (partesNombre.Length > 1)
            {
                string segundoNombre = partesNombre[1].Trim();

                if (!string.IsNullOrEmpty(segundoNombre))
                {
                    if (!segundosNombres.ContainsKey(segundoNombre))
                    {
                        segundosNombres.Add(segundoNombre, 1);
                    }
                    else
                    {
                        segundosNombres[segundoNombre]++;
                    }
                }
            }
        }
    });

    Console.WriteLine($"Los {n} primeros nombres más comunes son:");
    var sortedPrimerosNombres = from pair in primerosNombres
                                orderby pair.Value ascending
                                select pair;
    segundosNombres.Remove("DEL");
    segundosNombres.Remove("DE");
    int cont = 0;
    foreach (KeyValuePair<string, int> nombre in sortedPrimerosNombres)
    {
        cont++;

        Console.WriteLine($"{nombre.Key}: {nombre.Value}");

        if (cont >= n)
        {
            break;
        }
    }

    Console.WriteLine();

    Console.WriteLine($"Los {n} segundos nombres más comunes son:");
    var sortedSegundosNombres = from pair in segundosNombres
                                orderby pair.Value ascending
                                select pair;

    cont = 0;
    foreach (KeyValuePair<string, int> nombre in sortedSegundosNombres)
    {
        cont++;

        Console.WriteLine($"{nombre.Key}: {nombre.Value}");

        if (cont >= n)
        {
            break;
        }
    }


    Console.WriteLine("Tiempo de ejecución(CANTIDAD DE PERSONAS CON UN NOMBRE EN PARTICULAR PARALELO): " + temporizador.ElapsedMilliseconds + " milisegundos");
}

static void CantidadDePersonasConUnNombreParticular(List<string[]> lineas)
{
    Dictionary<string, int> nombres = new Dictionary<string, int>();

    foreach (string[] linea in lineas)
    {
        string nombre = linea[5].Trim();

        if (!string.IsNullOrEmpty(nombre))
        {
            string[] partesNombre = nombre.Split(' ');

            if (partesNombre.Length >= 1)
            {
                string primerNombre = partesNombre[0];

                if (!nombres.ContainsKey(primerNombre))
                {
                    nombres.Add(primerNombre, 1);
                }
                else
                {
                    nombres[primerNombre]++;
                }
            }

            if (partesNombre.Length >= 2)
            {
                string segundoNombre = partesNombre[1];

                if (!nombres.ContainsKey(segundoNombre))
                {
                    nombres.Add(segundoNombre, 1);
                }
                else
                {
                    nombres[segundoNombre]++;
                }
            }
        }
    }
    var sortedNombre = from pair in nombres
                       orderby pair.Value ascending
                       select pair;

    foreach (KeyValuePair<string, int> nombre in sortedNombre)
    {
        
        Console.WriteLine($"{nombre.Key}: {nombre.Value}");
        
    }
}


//=============================CANTIDAD DE PERSONAS POR IDENTIFICACION======================
static void PersonasPorIdentificacion(List<string[]> personas)
{
    int provincia = 0;
    int extranjero = 0;
    int nacimiento = 0;
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();

    Parallel.ForEach(personas, persona =>
    {
        if (persona[0].Substring(0,1).Equals("1") || persona[0].Substring(0, 1).Equals("2") || persona[0].Substring(0, 1).Equals("3") || persona[0].Substring(0, 1).Equals("4") || persona[0].Substring(0, 1).Equals("5") || persona[0].Substring(0, 1).Equals("6") || persona[0].Substring(0, 1).Equals("7"))
        {
            provincia++;
        }

        else if (persona[0].Substring(0, 1).Equals("8"))
        {
            extranjero++;
        }

        else if (persona[0].Substring(0, 1).Equals("9"))
        {
            nacimiento++;
        }
    });

    Console.WriteLine("Personas con tipo de identificacion asociadas a una provincia: " + provincia);
    Console.WriteLine("Personas con tipo de identificacion de extranjero nacionalizado: " + extranjero);
    Console.WriteLine("Personas con tipo de identificacion sin lugar de nacimiento reportado: " + nacimiento);
    Console.WriteLine("Tiempo de ejecución(CANTIDAD DE PERSONAS POR IDENTIFICACION PARARELO): " + temporizador.ElapsedMilliseconds + " milisegundos");
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

    Console.WriteLine("Tiempo de ejecución(N CANTONES CON MAS VOTANTES REGISTRADOS PARARELO): " + temporizador.ElapsedMilliseconds + " milisegundos");

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

    Console.WriteLine("Tiempo de ejecución(N DISTRITOS CON MAS VOTANTES REGISTRADOS PARARELO): " + temporizador.ElapsedMilliseconds + " milisegundos");

}

static void personasCuyaIdentificacionVenceEnX(List<string[]> personas, int x)
{
    string fechaVencimiento;
    if (x == 0)
    {
        Console.WriteLine("ingrese la fecha de vencimiento en el formato 'aaaammdd' ( a = anio , m = mes , d = dia )");
         fechaVencimiento = Console.ReadLine();
    }

    else
    {
        fechaVencimiento = DateTime.Now.ToString("yyyyMMdd");
    }
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    object sync = new object();
    int degre = Environment.ProcessorCount;
    List <string[]> personasConIdentificacionVencida = new List<string[]>();
    Parallel.ForEach(personas, new ParallelOptions { MaxDegreeOfParallelism = degre }, persona => 
    {
        lock (sync)
        {
            int fecha = int.Parse(persona[3]);
            if ( fecha == int.Parse(fechaVencimiento))
            {
                personasConIdentificacionVencida.Add(persona);
            }
        }
    });

    foreach(string[] persona in personasConIdentificacionVencida)
    {
        Console.WriteLine(persona[0] + " | " + persona[1] + " | " + persona[3] + " | " + persona[5] + " | " + persona[6] + " | " + persona[7]);
    }
    Console.WriteLine("Tiempo de ejecución(PERSONAS CUYA FECHA DE VENCIMIENTO VENCE EN X FECHA PARALELO): " + temporizador.ElapsedMilliseconds + " milisegundos");
}


static void puntoN(List<string> listaPersonas, Dictionary<string, string> listaCantones, Dictionary<string, string> listaDistritos, List<string[]> listaDatosOrdenados)
{
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    int degreeOfParallelism = Environment.ProcessorCount;
    object sync = new object();

    Parallel.For(0, 9, workerId =>
    {
        if (workerId == 0)
        {
            CantidadDeVotantesPorProvincia(listaPersonas);
        }

        if (workerId == 1)
        {
            CantidadDeVotantesPorCanton(listaPersonas, listaCantones);
        }

        if (workerId == 2)
        {
            CantidadDeVotantesPorDistrito(listaPersonas, listaDistritos);
        }

        if (workerId == 3)
        {
            NCantonesConMasVotantesRegristrados(listaPersonas, listaCantones, 10);
        }

        if (workerId == 4)
        {
            NDistritosConMasVotantesRegristrados(listaPersonas, listaDistritos, 10);
        }

        if (workerId == 5)
        {
            personasCuyaIdentificacionVenceEnX(listaDatosOrdenados,1);
        }

        if (workerId == 6)
        {
            NApellidosMasCumunesP(listaDatosOrdenados, 10);
        }
        
        if (workerId == 7)
        {
            NNombreMasComunes(listaDatosOrdenados, 10);
        }

        if(workerId == 8)
        {
            NNombreMenosComunes(listaDatosOrdenados, 10);
        }
        
        if(workerId == 8)
        {
            PersonasPorIdentificacion(listaDatosOrdenados);
        }



    });
    Console.WriteLine("Tiempo de ejecución(PUNTO N): " + temporizador.ElapsedMilliseconds + " milisegundos");


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
                PersonasPorIdentificacion(listaDatosOrdenados);
                break;
            case 8:
                personasCuyaIdentificacionVenceEnX(listaDatosOrdenados, 0);
                break;
            case 9:
                CantidadDePersonasConUnNombreParticular(listaDatosOrdenados);
                break;
            case 10:
                cantidadDePersonasConApellidoParticularP(listaDatosOrdenados);
                break;
            case 11:
                NNombreMasComunes(listaDatosOrdenados,0);
                break;
            case 12:
                NApellidosMasCumunes(listaDatosOrdenados);
                break;
            case 13:
                NNombreMenosComunes(listaDatosOrdenados, 0);
                break;
            case 14:
                puntoN(listaPersonas, listaCantones, listaDistritos, listaDatosOrdenados);
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

    Console.WriteLine("Tiempo de ejecución(CANTIDAD DE VOTANTES POR PROVINCIA SECUENCIAL): " + temporizador.ElapsedMilliseconds + " milisegundos");


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

    Console.WriteLine("Tiempo de ejecución(CATNIDAD DE VOTANTES POR CANTON SECUENCIAL): " + temporizador.ElapsedMilliseconds + " milisegundos");
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

    Console.WriteLine("Tiempo de ejecución(CANTIDAD DE VOTANTES POR DISTRITO SECUENCIAL): " + temporizador.ElapsedMilliseconds + " milisegundos");
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
    Console.WriteLine("Tiempo de ejecución(N APELLIDOS MAS COMUNES SECUENCIAL): " + temporizador.ElapsedMilliseconds + " milisegundos");
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
    Console.WriteLine("Tiempo de ejecución(CANTIDAD DE PERSONAS CON APELIIDO PARTICULAR): " + temporizador.ElapsedMilliseconds + " milisegundos");
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

    Console.WriteLine("Tiempo de ejecución(N APELLIDO MAS COMUNES SECUENCIAL): " + temporizador.ElapsedMilliseconds + " milisegundos");
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

    Console.WriteLine("Tiempo de ejecución(BUSCAR PERSONAS SECUENCIAL): " + temporizador.ElapsedMilliseconds + " milisegundos");
}



//=============================CANTIDAD DE PERSONAS POR IDENTIFICACION======================
static void PersonasPorIdentificacionSec(List<string[]> personas)
{
    int provincia = 0;
    int extranjero = 0;
    int nacimiento = 0;
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();

    foreach (string[] persona in personas)
    {
        if (persona[0].Substring(0, 1).Equals("1") || persona[0].Substring(0, 1).Equals("2") || persona[0].Substring(0, 1).Equals("3") || persona[0].Substring(0, 1).Equals("4") || persona[0].Substring(0, 1).Equals("5") || persona[0].Substring(0, 1).Equals("6") || persona[0].Substring(0, 1).Equals("7"))
        {
            provincia++;
        }

        else if (persona[0].Substring(0, 1).Equals("8"))
        {
            extranjero++;
        }

        else if (persona[0].Substring(0, 1).Equals("9"))
        {
            nacimiento++;
        }
    }

    Console.WriteLine("Personas con tipo de identificacion asociadas a una provincia: " + provincia);
    Console.WriteLine("Personas con tipo de identificacion de extranjero nacionalizado: " + extranjero);
    Console.WriteLine("Personas con tipo de identificacion sin lugar de nacimiento reportado: " + nacimiento);
    Console.WriteLine("Tiempo de ejecución: " + temporizador.ElapsedMilliseconds + " milisegundos");
}


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
static void personasCuyaIdentificacionVenceEnXSec(List<string[]> personas, int x)
{
    string fechaVencimiento;
    if (x == 0)
    {
        Console.WriteLine("ingrese la fecha de vencimiento en el formato 'aaaammdd' ( a = anio , m = mes , d = dia )");
        fechaVencimiento = Console.ReadLine();
    }
    else
    {
        fechaVencimiento = DateTime.Now.ToString("yyyyMMdd");
    }
    
    Stopwatch temporizador;
    temporizador = Stopwatch.StartNew();
    object sync = new object();
    int degre = Environment.ProcessorCount;
    List<string[]> personasConIdentificacionVencida = new List<string[]>();
    foreach(string[] persona in personas)
    {
        
        int fecha = int.Parse(persona[3]);
        if (fecha == int.Parse(fechaVencimiento))
        {
            personasConIdentificacionVencida.Add(persona);
            
        }
    }

    foreach (string[] persona in personasConIdentificacionVencida)
    {
        Console.WriteLine(persona[0] + " | " + persona[1] + " | " + persona[3] + " | " + persona[5] + " | " + persona[6] + " | " + persona[7]);
    }
    Console.WriteLine("Tiempo de ejecución(PERSONAS CUYA FECHA DE VENCIMIENTO VENCE EN X FECHA SECUENCIAL): " + temporizador.ElapsedMilliseconds + " milisegundos");
}
Menu();
