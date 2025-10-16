using System;
using System.Runtime.InteropServices;

#region INTERFACCIA

#endregion

#region SINGLETON
///<summary> Singleton: garantisce che una classe abbia una sola istanzae fornisce un punto di accesso globale a essa.</summary>
/// <typeparam name="T">Tipo della classe singleton</typeparam>
public sealed class SocialNetWork
{
    List<Utenti> utenti = new List<Utenti>();
    List<Post> post = new List<Post>();
    private static SocialNetWork _instance;
    private SocialNetWork() { }
    public static SocialNetWork Instance => _instance ??= new SocialNetWork();
    public void AggiungiUtente(Utenti u) => utenti.Add(u);
    public void AggiungiPost(Post p) => post.Add(p);
    public List<Utenti> GetUtenti() => utenti;
    public List<Post> GetPost() => post;
}
#endregion

#region CLASSI CONCRETE
// 2. ConcreteComponent: oggetto base senza decorazioni
public class Pizza : IPiatto
{
    public string Descrizione() => "Pizza Margherita:\n";
    public string Prepara() => "Preparazione della pizza in corso...\n";
}

public class Hamburger : IPiatto
{
    public string Descrizione() => "Hamburger:\n";
    public string Prepara() => "Preparazione dell'hamburger in corso...\n";
}

public class Insalata : IPiatto
{
    public string Descrizione() => "Insalata Mista:\n";
    public string Prepara() => "Preparazione dell'insalata in corso...\n";
}

public class Chef
{
    private IPreparazioneStrategia _strategia;
    public Chef(IPreparazioneStrategia strategia)
    {
        _strategia = strategia;
    }
    public void PreparaPiatto(IPiatto piatto)
    {
        string descrizione = piatto.Descrizione();
        string preparazione = _strategia.Prepara(descrizione);
        Console.WriteLine(preparazione);
    }
}
#endregion

#region DECORATORE ASTRATTO
// 3. Decorator: classe astratta che implementa IComponent
//    e incapsula un IComponent interno
public abstract class IngredientiExtra : IPiatto
{
    // Riferimento al componente "decorato"
    protected IPiatto _piatto;

    // Costruttore: richiede un componente da decorare
    protected IngredientiExtra(IPiatto piatto)
    {
        _piatto = piatto;
    }

    // Delegazione dell'operazione al componente interno
    public virtual string Descrizione() => _piatto.Descrizione();
    public virtual string Prepara() => _piatto.Prepara();
}
#endregion

#region DECORATORE CONCRETO
// 4. ConcreteDecoratorA: aggiunge comportamento prima e dopo la chiamata
public class ConFormaggio : IngredientiExtra
{
    public ConFormaggio(IPiatto piatto)
        : base(piatto) { }

    public override string Descrizione() => base.Descrizione() + "\n+ Formaggio";
}

public class ConBacon : IngredientiExtra
{
    public ConBacon(IPiatto piatto)
        : base(piatto) { }

    public override string Descrizione() => base.Descrizione() + "\n+ Bacon";
}

public class ConSalsa : IngredientiExtra
{
    public ConSalsa(IPiatto piatto)
        : base(piatto) { }

    public override string Descrizione() => base.Descrizione() + "\n+ Salsa";
}
#endregion

#region FACTORY
public class PiattoFactory
{
    private static PiattoFactory _instance;

    private PiattoFactory() { }

    public static PiattoFactory Instance => _instance ??= new PiattoFactory();

    public IPiatto CreaPiatto(string tipo) => tipo.ToLower().Trim() switch
    {
        "pizza" => new Pizza(),
        "hamburger" => new Hamburger(),
        "insalata" => new Insalata(),
        _ => null
    };
}
#endregion

#region STRATEGY
public class Fritto : IPreparazioneStrategia
{
    public string Prepara(string descrizione) => descrizione + " - Metodo di preparazione: Fritto\n";
}

public class AlForno : IPreparazioneStrategia
{
    public string Prepara(string descrizione) => descrizione + " - Metodo di preparazione: Al Forno\n";
}

public class AllaGriglia : IPreparazioneStrategia
{
    public string Prepara(string descrizione) => descrizione + " - Metodo di preparazione: Alla Griglia\n";
}
#endregion

#region MAIN
// Esempio di utilizzo (Client)
class Program
{
    static void Main()
    {
        Console.Clear();
        bool continua = true;

        while (continua)
        {
            Console.WriteLine("=== Benvenuto nella Cucina dello Chef ===");
            Console.WriteLine("Scegli il piatto:");
            Console.WriteLine("1 - Pizza");
            Console.WriteLine("2 - Hamburger");
            Console.WriteLine("3 - Insalata");
            Console.WriteLine("0 - Esci");

            string sceltaBase = Console.ReadLine();
            IPiatto piatto = sceltaBase switch
            {
                "1" => PiattoFactory.Instance.CreaPiatto("pizza"),
                "2" => PiattoFactory.Instance.CreaPiatto("hamburger"),
                "3" => PiattoFactory.Instance.CreaPiatto("insalata"),
                "0" => null,
                _ => null
            };

            if (piatto == null)
            {
                if (sceltaBase == "0")
                {
                    Console.WriteLine("Arrivederci!");
                    break;
                }
                Console.WriteLine("Scelta non valida.\n");
                continue;
            }

            bool piattoTerminato = false;
            while (!piattoTerminato)
            {
                Console.WriteLine("\nAggiungi ingredienti extra:");
                Console.WriteLine("1 - Formaggio");
                Console.WriteLine("2 - Bacon");
                Console.WriteLine("3 - Salsa");
                Console.WriteLine("0 - Nessuna/Finito");

                string sceltaDeco = Console.ReadLine();
                switch (sceltaDeco)
                {
                    case "1":
                        piatto = new ConFormaggio(piatto);
                        break;
                    case "2":
                        piatto = new ConBacon(piatto);
                        break;
                    case "3":
                        piatto = new ConSalsa(piatto);
                        break;
                    case "0":
                        piattoTerminato = true;
                        break;
                    default:
                        Console.WriteLine("Scelta decorazione non valida.");
                        break;
                }
            }

            bool cotturaTerminata = false;
            while (!cotturaTerminata)
            {
                Console.WriteLine("\nAggiungi ingredienti extra:");
                Console.WriteLine("1 - Al Forno");
                Console.WriteLine("2 - Alla Griglia");
                Console.WriteLine("3 - Fritto");
                Console.WriteLine("0 - Nessuna/Finito");

                string sceltaCottura = Console.ReadLine();
                switch (sceltaCottura)
                {
                    case "1":
                        new Chef(new AlForno()).PreparaPiatto(piatto);
                        cotturaTerminata = true;
                        break;
                    case "2":
                        new Chef(new AllaGriglia()).PreparaPiatto(piatto);
                        cotturaTerminata = true;
                        break;
                    case "3":
                        new Chef(new Fritto()).PreparaPiatto(piatto);
                        cotturaTerminata = true;
                        break;
                    case "0":
                        cotturaTerminata = true;
                        break;
                    default:
                        Console.WriteLine($"Scelta non valida");
                        break;
                }
            }

            Console.WriteLine("\nEcco il tuo piatto:");
            Console.WriteLine(piatto.Descrizione());
            Console.WriteLine("\n------------------------------\n");
        }
    }
}
#endregion